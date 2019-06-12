using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Game2048.Mvvm;
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
            set => SetValue(ref _score, value);
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

            AddTile();
            AddTile();

            SwipeLeft = new Command(SwipeLeftCommand);
            SwipeRight = new Command(SwipeRightCommand);
            SwipeUp = new Command(SwipeUpCommand);
            SwipeDown = new Command(SwipeDownCommand);
            Reset = new Command(ResetCommand);
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

        private void ResetCommand()
        {
            for (var x = 0; x < 4; x++)
            {
                for (var y = 0; y < 4; y++)
                {
                    Tiles[x, y].Clear();
                }
            }

            Score = 0;

            AddTile();
            AddTile();
        }

        private void Swipe(SwipeDirection direction)
        {
            if (TrySwipe(direction))
                AddTile();
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
                        prevTile.SetValue(2 * tile.Value);
                        Score += 2 * tile.Value;
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
    }
}
