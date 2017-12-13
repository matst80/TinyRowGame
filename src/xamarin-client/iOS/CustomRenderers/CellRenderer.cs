using System;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

//[assembly: ExportRenderer(typeof(tinyrowgame.Controls.Cell),typeof(CellRenderer))]

namespace tinyrowgame.iOS.CustomRenderers
{
    public class CellRenderer : VisualElementRenderer<tinyrowgame.Controls.Cell>
    {
        protected override void OnElementPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);
            Layer.BorderWidth = 1;
            Layer.BorderColor = Color.Gray.ToCGColor();
        }
    }
}
