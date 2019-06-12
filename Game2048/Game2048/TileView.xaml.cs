using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Game2048
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class TileView : ContentView
    {
        private TileVm _bindingContext;

		public TileView()
		{
			InitializeComponent();
		}

        private void OnBindingContextChanged(object sender, EventArgs e)
        {
            if (_bindingContext != null)
            {
                _bindingContext.AnimateInitial -= AnimateInitial;
                _bindingContext.Animate -= Animate;
            }
            _bindingContext = BindingContext as TileVm;
            if (_bindingContext != null)
            {
                _bindingContext.Animate += Animate;
                _bindingContext.AnimateInitial += AnimateInitial;
            }
        }

        private void AnimateInitial()
        {
            Fill.Scale = 0;
            Fill.ScaleTo(1, 250U, Easing.SinInOut);
        }

        private bool _isAnimating;

        private async void Animate(int x, int y)
        {
            var dx = x * (Sizes.Instance.Unit + Sizes.Instance.Margin);
            var dy = y * (Sizes.Instance.Unit + Sizes.Instance.Margin);

            if (_isAnimating)
            {
                Fill2.TranslationX = dx;
                Fill2.TranslationY = dy;
                Fill2.IsVisible = true;
                await Fill2.TranslateTo(0, 0, 250U, Easing.SinInOut);
                Fill2.IsVisible = false;
            }
            else
            {
                _isAnimating = true;
                Fill.TranslationX = dx;
                Fill.TranslationY = dy;
                await Fill.TranslateTo(0, 0, 250U, Easing.SinInOut);
                _isAnimating = false;
            }
        }
    }
}