using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MauiApp1.Areas.Exercise.Resources
{
    class ProgressNeedle : IDrawable
    {
        public int GreaterHeight = 8;
        public int LesserHeight = 4;

        public void Draw(ICanvas canvas,RectF dirtyrectangle)
        {
            PathF path = new PathF();
            path.MoveTo(dirtyrectangle.Center.X + GreaterHeight / 2, dirtyrectangle.Center.Y + (GreaterHeight / 2));
            path.LineTo(dirtyrectangle.Center.X + GreaterHeight / 2, dirtyrectangle.Center.Y + (-GreaterHeight / 2));
            path.LineTo(dirtyrectangle.X, dirtyrectangle.Center.Y + (-LesserHeight / 2));
            path.LineTo(dirtyrectangle.X, dirtyrectangle.Center.Y + (LesserHeight / 2));
            canvas.FillColor = new Color(0, 98, 114);
            canvas.FillPath(path);
        }
    }
}
