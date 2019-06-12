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
	public partial class TileBackView : ContentView
	{
		public TileBackView ()
		{
			InitializeComponent ();
		}
	}
}