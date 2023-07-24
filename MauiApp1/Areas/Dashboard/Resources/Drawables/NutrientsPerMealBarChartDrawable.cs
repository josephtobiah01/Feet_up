using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MauiApp1.Areas.Dashboard.Resources.Drawables
{
    public class NutrientsPerMealBarChartDrawable : IDrawable
    {
        public bool ShowTargetLine { get;set; }

        public double ProteinToDisplay { get; set; }
        public double CarbohydratesToDisplay { get; set; }
        public double FatToDisplay { get; set; }

        public long TargetCalories { get; set; }


        private double? _chartTopMargin = 16;
        private double? _chartBottomMargin = 32;
        private double? _chartLeftMargin = 15;
        private double? _chartRightMargin = 15;


        public void Draw(ICanvas canvas, RectF dirtyRect)
        {
            double? chartWidth = dirtyRect.Width - (_chartLeftMargin + _chartRightMargin);
            double? chartWidthDraw = dirtyRect.Width;
            double? totalBarLength = TargetCalories * 1.2;
            double? pixelPerCalorie = chartWidthDraw / totalBarLength;


            double? fullBarHeight = 15;
            double? nutrientBarHeight = 8;
            double? startX = _chartLeftMargin;
            double? startYFullBar = (dirtyRect.Height - fullBarHeight) / 2;
            double? startYNutrientBar = (dirtyRect.Height - nutrientBarHeight) / 2;

            // Draw the full bar

            canvas.FillColor = Color.FromArgb("#f8f8f8");
            canvas.FillRoundedRectangle(
                x: (float)startX,
                y: (float)(startYFullBar),
                width: (float)(chartWidth),
                height: (float)fullBarHeight,
                cornerRadius: 8
            );

            // Draw protein bar

            canvas.FillColor = Color.FromArgb("#00A01A");
            canvas.FillRoundedRectangle(
                x: (float)startX + 2,
                y: (float)(startYNutrientBar),
                width: (float)(ProteinToDisplay * pixelPerCalorie) - 1,
                height: (float)nutrientBarHeight,
                cornerRadius: 8
            );

            // Draw carbohydrate bar
            startX += (ProteinToDisplay * pixelPerCalorie);
            canvas.FillColor = Color.FromArgb("#F3C522");
            canvas.FillRoundedRectangle(
                x: (float)startX + 1,
                y: (float)(startYNutrientBar),
                width: (float)(CarbohydratesToDisplay * pixelPerCalorie) - 1,
                height: (float)nutrientBarHeight,
                cornerRadius: 8

            );

            // Draw fat bar
            startX += (CarbohydratesToDisplay * pixelPerCalorie);
            canvas.FillColor = Color.FromArgb("#0000FF");
            canvas.FillRoundedRectangle(
                x: (float)(startX) + 1,
                y: (float)(startYNutrientBar),
                width: (float)(FatToDisplay * pixelPerCalorie) + 1,
                height: (float)nutrientBarHeight,
                cornerRadius: 8
            );

            switch (ShowTargetLine)
            {
                case true:

                    // Draw target line

                    startX = pixelPerCalorie * (TargetCalories);
                    canvas.StrokeColor = Color.FromArgb("#5b5b5b");
                    canvas.DrawLine(
                        x1: (float)(startX),
                        x2: (float)(startX),
                        y1: (float)(startYNutrientBar - 5),
                        y2: (float)(startYNutrientBar + 20)
                    );

                    //startX = (pixelPerCalorie * (TargetCalories*1.2));
                    //canvas.StrokeColor = Color.FromArgb("#ff0000");
                    //canvas.DrawLine(
                    //    x1: (float)(startX),
                    //    x2: (float)(startX),
                    //    y1: (float)(startYNutrientBar - 5),
                    //    y2: (float)(startYNutrientBar + 20)
                    //);

                    break;

                case false:
                    break;
            }

        }
    }
}
