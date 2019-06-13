using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using FFImageLoading;
using FFImageLoading.Work;
using Game2048.Mvvm;
using PCLStorage;
using Xamarin.Forms;

namespace Game2048
{
    public class MainVm : ViewModelBase
    {
        private readonly Random _random;

        public Sizes Sizes => Sizes.Instance;

        public TileVm[,] Tiles { get; }

        private readonly Dictionary<SwipeDirection, List<List<TileVm>>> _navigator;

        public event Action<string> Alert;

        private int _score;

        /// <summary>
        /// Gets or sets Score value.
        /// </summary>
        public int Score
        {
            get => _score;
            set => SetValue(ref _score, value, ScoreChanged);
        }

        private void ScoreChanged(int newValue)
        {
            if (newValue > HighScore)
                HighScore = newValue;
        }

        private int _highScore;

        /// <summary>
        /// Gets or sets HighScore value.
        /// </summary>
        public int HighScore
        {
            get => _highScore;
            set => SetValue(ref _highScore, value);
        }

        private int _topTile;

        /// <summary>
        /// Gets or sets TopTile value.
        /// </summary>
        public int TopTile
        {
            get => _topTile;
            set => SetValue(ref _topTile, value);
        }

        public MainVm()
        {
            _random = new Random();
            _navigator = new Dictionary<SwipeDirection, List<List<TileVm>>>
            {
                { SwipeDirection.Left, new List<List<TileVm>>() },
                { SwipeDirection.Right, new List<List<TileVm>>() },
                { SwipeDirection.Up, new List<List<TileVm>>() },
                { SwipeDirection.Down, new List<List<TileVm>>() },
            };
            for (var x = 0; x < 4; x++)
            {
                _navigator[SwipeDirection.Left].Add(new List<TileVm>());
                _navigator[SwipeDirection.Right].Add(new List<TileVm>());
                _navigator[SwipeDirection.Up].Add(new List<TileVm>());
                _navigator[SwipeDirection.Down].Add(new List<TileVm>());
            }
            Tiles = new TileVm[4, 4];
            for (var x = 0; x < 4; x++)
            {
                for (var y = 0; y < 4; y++)
                {
                    var tile = new TileVm(x, y);
                    Tiles[x, y] = tile;
                    _navigator[SwipeDirection.Left][x].Add(tile);
                    _navigator[SwipeDirection.Right][x].Insert(0, tile);
                    _navigator[SwipeDirection.Up][y].Add(tile);
                    _navigator[SwipeDirection.Down][y].Insert(0, tile);
                }
            }

            SwipeLeft = new Command(SwipeLeftCommand);
            SwipeRight = new Command(SwipeRightCommand);
            SwipeUp = new Command(SwipeUpCommand);
            SwipeDown = new Command(SwipeDownCommand);
            Reset = new Command(ResetCommand);
        }

        private string _progress;

        /// <summary>
        /// Gets or sets Progress value.
        /// </summary>
        public string Progress
        {
            get => _progress;
            set => SetValue(ref _progress, value);
        }

        private double _progressWidth;

        /// <summary>
        /// Gets or sets ProgressWidth value.
        /// </summary>
        public double ProgressWidth
        {
            get => _progressWidth;
            set => SetValue(ref _progressWidth, value);
        }

        public async Task LoadImages()
        {
            var service = ImageService.Instance;
            Progress = "Progress: 0%";
            for (int x = 2, progress = 1; x < 8192; x *= 2, progress++)
            {
                await service.LoadFile($"/storage/emulated/0/DCIM/Download/{x}.gif").Preload().RunAsync();
                var pct = Math.Min(100, progress * 100 / 12);
                Progress = $"Progress: {pct}%";
                ((MainPage)Application.Current.MainPage).SetProgress(pct);
                //ProgressWidth = (Sizes.Width70Percent - 6) * pct / 100;
            }
        }

        private bool AddTile()
        {
            var emptyTiles = new List<TileVm>();
            for (var x = 0; x < 4; x++)
            {
                for (var y = 0; y < 4; y++)
                {
                    if (!Tiles[x, y].HasValue)
                        emptyTiles.Add(Tiles[x, y]);
                }
            }

            if (!emptyTiles.Any())
                return false;

            var value = _random.Next(10) == 0 ? 4 : 2;
            var index = _random.Next(emptyTiles.Count);
            emptyTiles[index].SetValue(value);
            emptyTiles[index].SetInitialAnimation();

            return true;
        }

        public ICommand SwipeLeft { get; }

        private void SwipeLeftCommand()
        {
            Swipe(SwipeDirection.Left);
        }

        public ICommand SwipeRight { get; }

        private void SwipeRightCommand()
        {
            Swipe(SwipeDirection.Right);
        }

        public ICommand SwipeUp { get; }

        private void SwipeUpCommand()
        {
            Swipe(SwipeDirection.Up);
        }

        public ICommand SwipeDown { get; }

        private void SwipeDownCommand()
        {
            Swipe(SwipeDirection.Down);
        }

        public ICommand Reset { get; }

        private async void ResetCommand()
        {
            for (var x = 0; x < 4; x++)
            {
                for (var y = 0; y < 4; y++)
                {
                    Tiles[x, y].Clear();
                }
            }

            Score = 0;
            TopTile = 2;

            AddTile();
            AddTile();

            await SaveState();
        }

        private async void Swipe(SwipeDirection direction)
        {
            if (TrySwipe(direction))
            {
                AddTile();
                await SaveState();
            }
            CheckEmpty();
        }

        private async void CheckEmpty()
        {
            for (var x = 0; x < 4; x++)
            {
                for (var y = 0; y < 4; y++)
                {
                    if (!Tiles[x, y].HasValue)
                        return;
                }
            }
            for (var x = 0; x < 4; x++)
            {
                for (int y = 0, y1 = 1; y1 < 4; y++, y1++)
                {
                    if (Tiles[x, y].Value == Tiles[x, y1].Value)
                        return;
                    if (Tiles[y, x].Value == Tiles[y1, x].Value)
                        return;
                }
            }

            await Task.Delay(300);
            RaiseAlert("You lost!");
        }

        private bool TrySwipe(SwipeDirection direction)
        {
            var list = _navigator[direction];
            var success = false;
            for (var x = 0; x < 4; x++)
            {
                for (var y = 0; y < 4; y++)
                {
                    Tiles[x, y].IsMoving = false;
                }
            }

            foreach (var sublist in list)
            {
                if (TrySwipe(sublist))
                    success = true;
            }
            return success;
        }

        private bool TrySwipe(List<TileVm> row)
        {
            var newIndex = 0;
            var success = false;
            for (var oldIndex = 1; oldIndex < row.Count; oldIndex++)
            {
                var tile = row[oldIndex];
                if (!tile.HasValue)
                    continue;
                if (newIndex < oldIndex)
                {
                    var prevTile = row[newIndex];
                    if (!prevTile.HasValue)
                    {
                        prevTile.SetValue(tile.Value);
                        tile.Clear();
                        success = true;
                        prevTile.IsMoving = true;
                        prevTile.SetAnimation(tile);
                    }
                    else if (prevTile.Value == tile.Value)
                    {
                        var twiceValue = 2 * tile.Value;
                        prevTile.SetValue(twiceValue);
                        Score += twiceValue;
                        if (twiceValue > TopTile)
                            TopTile = twiceValue;
                        tile.Clear();
                        success = true;
                        newIndex++;
                        if (!prevTile.IsMoving)
                            prevTile.SetAnimation(prevTile);
                        prevTile.SetAnimation(tile);
                    }
                    else
                    {
                        newIndex++;
                        prevTile = row[newIndex];
                        if (newIndex < oldIndex && !prevTile.HasValue)
                        {
                            prevTile.SetValue(tile.Value);
                            tile.Clear();
                            success = true;
                            prevTile.IsMoving = true;
                            prevTile.SetAnimation(tile);
                        }
                    }
                }
            }

            return success;
        }

        private void RaiseAlert(string message)
        {
            Alert?.Invoke(message);
        }

        public async Task SaveState()
        {
            var storage = FileSystem.Current.LocalStorage;
            var file = await storage.CreateFileAsync("state", CreationCollisionOption.ReplaceExisting);
            await file.WriteAllTextAsync(GetState());
        }

        public async Task LoadState()
        {
            var storage = FileSystem.Current.LocalStorage;
            var file = await storage.CreateFileAsync("state", CreationCollisionOption.OpenIfExists);
            SetState(await file.ReadAllTextAsync());
        }

        private bool _isBusy;

        /// <summary>
        /// Gets or sets IsBusy value.
        /// </summary>
        public bool IsBusy
        {
            get => _isBusy;
            set => SetValue(ref _isBusy, value);
        }

        public async Task Init()
        {
            try
            {
                IsBusy = true;
                await LoadImages();
                await LoadState();
            }
            finally
            {
                IsBusy = false;
            }
        }

        private string GetState()
        {
            var sb = new StringBuilder();
            for (var x = 0; x < 4; x++)
            {
                for (var y = 0; y < 4; y++)
                {
                    sb.Append(Tiles[x, y].HasValue ? Tiles[x, y].Value : 0);
                    sb.Append(",");
                }
            }
            sb.Append(Score);
            sb.Append(",");
            sb.Append(HighScore);
            return sb.ToString();
        }

        private void SetState(string state)
        {
            if (string.IsNullOrEmpty(state))
            {
                ResetCommand();
                return;
            }
            var numbers = state.Split(',');
            if (numbers.Length != 18)
                throw new FormatException();
            var index = 0;
            var maxVal = 2;
            for (var x = 0; x < 4; x++)
            {
                for (var y = 0; y < 4; y++)
                {
                    var value = int.Parse(numbers[index++]);
                    if (value > maxVal)
                        maxVal = value;
                    if (value > 0)
                        Tiles[x, y].SetValue(value);
                    else
                        Tiles[x, y].Clear();
                }
            }
            Score = int.Parse(numbers[index++]);
            HighScore = int.Parse(numbers[index]);
            TopTile = maxVal;
        }
    }
}
