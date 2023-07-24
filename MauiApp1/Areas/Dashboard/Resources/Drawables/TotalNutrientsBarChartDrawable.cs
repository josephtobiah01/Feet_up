using MauiApp1.Areas.Dashboard.ViewModel;
using Microsoft.Maui.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace MauiApp1.Areas.Dashboard.Resources.Drawables
{
    public class TotalNutrientsBarChartDrawable : IDrawable
    {

        #region Fields
        #endregion Fields

        #region Properties
        #endregion Properties

        public List<NutrientsIntakeViewItem> NutrientsIntakeViewItems { get; set; }

        public bool ShowTargetLine { get; set; }

        public double ProteinToDisplay { get; set; }
        public double CarbohydratesToDisplay { get; set; }
        public double  FatToDisplay { get; set; }

        public int TargetCalories { get; set; }


        private double _chartTopMargin = 16;
        private double _chartBottomMargin = 32;
        private double _chartLeftMargin = 10;
        private double _chartRightMargin = 10;

        


        public void Draw(ICanvas canvas, RectF dirtyRect)
        {
            double chartWidthDraw = dirtyRect.Width;
            double availableWidth = chartWidthDraw - _chartLeftMargin - _chartRightMargin;
            double totalBarLength = (double)(TargetCalories * 1.2);
            double targetLine = (double)(TargetCalories);
            double pixelPerCalorie = availableWidth / totalBarLength;
            

            double fullBarHeight = 15;
            double nutrientBarHeight = 8;
            double startX = _chartLeftMargin;
            double startYFullBar = (dirtyRect.Height - fullBarHeight) / 2;
            double startYNutrientBar = (dirtyRect.Height - nutrientBarHeight) / 2;

            // Draw the full bar
            
            canvas.FillColor = Color.FromArgb("#f8f8f8");
            canvas.FillRectangle(
                x: (float)startX,
                y: (float)(startYFullBar),
                width: (float)(totalBarLength * pixelPerCalorie),
                height: (float)fullBarHeight
                
            );

            // Draw protein bar

            canvas.FillColor = Color.FromArgb("#00A01A");
            canvas.FillRoundedRectangle(
                x: (float)(startX + 0.2),
                y: (float)(startYNutrientBar),
                width: (float)(ProteinToDisplay * pixelPerCalorie),
                height: (float)nutrientBarHeight,
                cornerRadius: 8
            );

            // Draw carbohydrate bar
            startX += (ProteinToDisplay * pixelPerCalorie);
            canvas.FillColor = Color.FromArgb("#F3C522");
            canvas.FillRoundedRectangle(
                x: (float)(startX + 0.2),
                y: (float)(startYNutrientBar),
                width: (float)(CarbohydratesToDisplay * pixelPerCalorie),
                height: (float)nutrientBarHeight,
                cornerRadius: 8

            );

            // Draw fat bar
            startX += (CarbohydratesToDisplay * pixelPerCalorie);
            canvas.FillColor = Color.FromArgb("#0000FF");
            canvas.FillRoundedRectangle(
                x: (float)(startX + 0.2),
                y: (float)(startYNutrientBar),
                width: (float)(FatToDisplay * pixelPerCalorie),
                height: (float)nutrientBarHeight,
                cornerRadius: 8
            );

            // Draw target line 

            startX = (pixelPerCalorie * targetLine) + _chartLeftMargin;
            canvas.StrokeColor = Color.FromArgb("#5b5b5b");
            canvas.DrawLine(
                x1: (float)(startX),
                x2: (float)(startX),
                y1: (float)(startYNutrientBar - 5),
                y2: (float)(startYNutrientBar + 20)
            );


            switch (ShowTargetLine)
            {
                case true:

                    

                    //startX = (pixelPerCalorie * (TargetCalories*1.2));
                    //canvas.StrokeColor = Color.FromArgb("#ff0000");
                    //canvas.DrawLine(
                    //    x1: (float)(startX),
                    //    x2: (float)(startX),
                    //    y1: (float)(startYNutrientBar - 5),
                    //    y2: (float)(startYNutrientBar + 20)
                    //);
                    //comment

                    break;

                case false:
                    break;
            }
            
        }
    }
}
