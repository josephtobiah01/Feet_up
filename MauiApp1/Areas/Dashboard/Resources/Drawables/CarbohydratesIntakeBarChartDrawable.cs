using MauiApp1.Areas.Dashboard.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MauiApp1.Areas.Dashboard.Resources.Drawables
{
    public class CarbohydratesIntakeBarChartDrawable : IDrawable
    {
        public List<CarbohydratesIntakeViewItem> CarbohydratesIntakeViewItems { get; set; }

        private int _xAxisNumberOfLines = 6;
        private int _yAxisNumberOfLines = 8;

        public double _xAxisMaxValue = 60;

        private double _canvasLeftMargin = 16;
        private double _canvasRightMargin = 16;

        private double _chartTopMargin = 16;
        private double _chartBottomMargin = 32;

        Point _yAxisEndPoint = new Point(0, 0);
        Point _origin = new Point(0, 0);
        Point _xAxisEndPoint = new Point(0, 0);

        private double _yAxisLongestTextBlockWidth = 50;

        public void Draw(ICanvas canvas, RectF dirtyRect)
        {
            ////throw new NotImplementedException();

            _yAxisEndPoint = new Point(_canvasLeftMargin + _yAxisLongestTextBlockWidth, _chartTopMargin); // top-left point  //chart x value for left margin
            _origin = new Point(_canvasLeftMargin + _yAxisLongestTextBlockWidth, dirtyRect.Height - _chartBottomMargin);
            _xAxisEndPoint = new Point(dirtyRect.Width - _canvasRightMargin, dirtyRect.Height - _chartBottomMargin);

            GenerateChartYAxes(canvas, dirtyRect);
            GenerateBars(canvas, dirtyRect);
            GenerateChartXAxes(canvas, dirtyRect);

        }

        private void GenerateChartXAxes(ICanvas canvas, RectF dirtyRect)
        {
            double xValue = 0;
            double xValueInterval = _xAxisMaxValue / _xAxisNumberOfLines;
            double xAxisInterval = (dirtyRect.Width - _canvasLeftMargin - _canvasRightMargin - _yAxisLongestTextBlockWidth) / (_xAxisNumberOfLines + 1); // add 1 so that we make the last line blank
            double xAxisValue = _origin.X + xAxisInterval;


            for (int i = 0; i <= _xAxisNumberOfLines; i++)
            {
                if (i != 0 && i != (_xAxisNumberOfLines + 1))
                {
                    canvas.StrokeColor = Color.FromArgb("#85929B");
                    canvas.StrokeSize = (float)2.5;
                    canvas.StrokeDashPattern = new float[] { 2, 1 };
                    canvas.DrawLine(
                        x1: (float)xAxisValue,
                        y1: (float)_yAxisEndPoint.Y,
                        x2: (float)xAxisValue,
                        y2: (float)_origin.Y);


                    canvas.DrawString(
                                value: xValue.ToString("N0"),
                                x: (float)(xAxisValue - xAxisInterval / 2),
                                y: (float)(_origin.Y + 8),
                                width: (float)xAxisInterval,
                                height: (float)30,
                                horizontalAlignment: HorizontalAlignment.Center,
                                verticalAlignment: VerticalAlignment.Center,
                                textFlow: TextFlow.OverflowBounds,
                                lineSpacingAdjustment: 0);
                }

                if (i > 0)
                {
                    xAxisValue += xAxisInterval;
                }

                xValue += xValueInterval;
            }
        }

        private void GenerateChartYAxes(ICanvas canvas, RectF dirtyRect)
        {
            double yAxisValue = _origin.Y;
            double yAxisInterval = (dirtyRect.Height - _chartTopMargin - _chartBottomMargin) / (_yAxisNumberOfLines);

            for (int i = 0; i <= _yAxisNumberOfLines; i++)
            {
                if (i == 1)
                {
                    canvas.StrokeColor = Colors.Black;
                    canvas.StrokeSize = (float)0.5;
                    canvas.StrokeDashPattern = new float[] { 1, 1 };
                    canvas.DrawLine(
                        x1: (float)_origin.X - 50,
                        y1: (float)yAxisValue + 16,
                        x2: (float)_xAxisEndPoint.X,
                        y2: (float)yAxisValue + 16);
                }
                else
                {
                    canvas.StrokeColor = Colors.White;
                    canvas.StrokeSize = (float)0.5;
                    canvas.StrokeDashPattern = new float[] { 1, 1 };
                    canvas.DrawLine(
                        x1: (float)_origin.X,
                        y1: (float)yAxisValue,
                        x2: (float)_xAxisEndPoint.X,
                        y2: (float)yAxisValue);
                }

                //canvas.StrokeSize = (float)0.5;
                //canvas.StrokeDashPattern = new float[] { 1, 1 };
                //canvas.DrawLine(
                //    x1: (float)_origin.X,
                //    y1: (float)yAxisValue,
                //    x2: (float)_xAxisEndPoint.X,
                //    y2: (float)yAxisValue);

                if (i != 0 && i != _yAxisNumberOfLines)
                {
                    string yValue = $"{this.CarbohydratesIntakeViewItems[i - 1].TransactionDate.ToString("dd/MM")}";

                    canvas.DrawString(
                            value: yValue,
                            x: (float)_canvasLeftMargin,
                            y: (float)(yAxisValue - (30 / 2)),
                            width: (float)_yAxisLongestTextBlockWidth,
                            height: (float)30,
                            horizontalAlignment: HorizontalAlignment.Left,
                            verticalAlignment: VerticalAlignment.Center,
                            textFlow: TextFlow.OverflowBounds,
                            lineSpacingAdjustment: 0);
                }

                yAxisValue -= yAxisInterval;
            }
        }

        private void GenerateBars(ICanvas canvas, RectF dirtyRect)
        {
            double yAxisValue = _origin.Y;
            double yAxisInterval = (dirtyRect.Height - _chartTopMargin - _chartBottomMargin) / (_yAxisNumberOfLines);
            double xAxisInterval = (dirtyRect.Width - _canvasLeftMargin - _canvasRightMargin - _yAxisLongestTextBlockWidth) / (_xAxisNumberOfLines + 1); // add 1 so that we make the last line blank
            double xAxisTotalInterval = xAxisInterval * _xAxisNumberOfLines;
            double barHeight = 8;

            yAxisValue -= yAxisInterval;

            foreach (CarbohydratesIntakeViewItem caloriesIntakeData in CarbohydratesIntakeViewItems)
            {
                Color barFillColor = Color.FromArgb("#F3C522");
                Color barUnfillColor = Color.FromArgb("#EEF0F2");

                double percentageValue = xAxisTotalInterval / _xAxisMaxValue * caloriesIntakeData.CarbohydratesIntakeCount;
                double percentageMaxValue = xAxisTotalInterval + xAxisInterval;

                canvas.FillColor = barUnfillColor;
                canvas.FillRoundedRectangle(
                   x: (float)_origin.X,
                   y: (float)(yAxisValue - (barHeight / 2)),
                   width: (float)percentageMaxValue,
                   height: 8,
                   cornerRadius: 4
               );

                canvas.FillColor = barFillColor;
                canvas.FillRoundedRectangle(
                   x: (float)_origin.X,
                   y: (float)(yAxisValue - (barHeight / 2)),
                   width: (float)percentageValue,
                   height: 8,
                   cornerRadius: 4
               );

                yAxisValue -= yAxisInterval;
            }
        }
    }
}
