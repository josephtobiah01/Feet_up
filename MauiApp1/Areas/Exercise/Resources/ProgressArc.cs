using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MauiApp1.Areas.Exercise.Resources
{
    class ProgressArc : IDrawable
    {
        public int Angle = 0;
        public int StrokeSize = 4;

        public void Draw(ICanvas canvas,RectF dirtyrectangle)
        {
            canvas.StrokeColor = new Color(238, 240, 242);
            canvas.StrokeLineJoin = LineJoin.Round;
            canvas.StrokeSize = StrokeSize;
            canvas.StrokeLineCap = LineCap.Round;
            canvas.DrawArc(StrokeSize / 2, StrokeSize / 2, dirtyrectangle.Width - 4, dirtyrectangle.Height - 4, 210, 330, true, false);
            canvas.ResetStroke();
            canvas.StrokeColor = new Color(0, 98, 114);
            canvas.DrawArc(StrokeSize / 2, StrokeSize / 2, dirtyrectangle.Width - 4, dirtyrectangle.Height - 4, 210, 210 + (360 - Angle), true, false);
        }
        public void SetAngle(int newAngle)
        {
            //1-240
            Angle = Math.Max(1,Math.Min(240, newAngle));
        }
        public void setPercentage(double percent)
        {
            SetAngle((int)Math.Round(percent * 2.4));
        }
    }
}
