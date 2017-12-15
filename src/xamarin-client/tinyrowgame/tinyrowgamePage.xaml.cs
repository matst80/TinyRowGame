using System;
using SkiaSharp;
using SkiaSharp.Views.Forms;
using tinyrowgame.SocketCommands;
using TinyWebSockets;
using TinyWebSockets.Interfaces;
using Xamarin.Forms;

namespace tinyrowgame
{
    [PropertyChanged.AddINotifyPropertyChangedInterface]
    public class tinyrowgamePageContext 
    {
        internal void Appear()
        {
            Service.MessageHandler.PopulateActions(this);
        }
    }

    public partial class tinyrowgamePage : ContentPage
    {
        private tinyrowgamePageContext ContextModel = new tinyrowgamePageContext();

        public tinyrowgamePage()
        {
            BindingContext = ContextModel;
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            ContextModel.Appear();
        }

        private void OnCanvasPaint(object sender, SKPaintSurfaceEventArgs e)
        {
            var view = sender as SKCanvasView;
            e.Surface.Canvas.ResetMatrix();

            using (var paint = new SKPaint())
			{
                paint.Color = SKColor.Parse("#DDDDDD");
                e.Surface.Canvas.DrawRect(new SKRect(0,0, view.CanvasSize.Width, view.CanvasSize.Height), paint);
            }
        }
    }
}
