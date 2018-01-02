using System;
using System.Threading.Tasks;
using SkiaSharp;
using SkiaSharp.Views.Forms;
using TinyWebSockets;
using TinyWebSockets.Interfaces;
using Xamarin.Forms;

namespace tinyrowgame
{
    [PropertyChanged.AddINotifyPropertyChangedInterface]
    public class tinyrowgamePageContext : IMessageReceiver
    {
        internal void Appear()
        {
            Service.MessageHandler.RegisterActionReceiver(this);
        }

        public void SetStateService(MessageHandler stateService)
        {
            //throw new NotImplementedException();
        }

        public void HandleAction(IMessage action)
        {
            Status = action.Type;
        }

        public string Status { get; set; }

        public bool IsActive => true;
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
