//using Microsoft.Maui.Graphics.Platform;

namespace MauiApp1.Areas.Supplement.Resources.Drawables
{
    public class MultiColorFillEllipse : IDrawable
    {
        public void Draw(ICanvas canvas, RectF dirtyRect)
        {
            DrawMultipleFillEllipse(canvas);
            DrawCheckmarkImage(canvas);            
        }


        private void DrawMultipleFillEllipse(ICanvas canvas)
        {
            canvas.FillColor = Color.FromArgb("#076878");
            canvas.FillEllipse(20, 0, 330, 330);

            canvas.FillColor = Color.FromArgb("#0D6674");
            canvas.FillEllipse(52, 32, 266, 266);

            canvas.FillColor = Color.FromArgb("#136471");
            canvas.FillEllipse(102, 82, 166, 166);
        }

        private void DrawCheckmarkImage(ICanvas canvas)
        {
            //Microsoft.Maui.Graphics.IImage image;
            //Assembly assembly = GetType().GetTypeInfo().Assembly;
            //using (Stream stream = assembly.GetManifestResourceStream("MyMauiApp.Resources.Images.checkmark.png"))
            //{
            //    image = PlatformImage.FromStream(stream);
            //}
            //if (image != null)
            //{
            //    canvas.DrawImage(image, 144, 134, image.Width, image.Height);
            //}
        }
    }
}
