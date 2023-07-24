using FitnessData.Common;
using FitnessData.Common.Data;
using Font = Microsoft.Maui.Graphics.Font;

namespace MauiApp1.Areas.Dashboard.Resources.Drawables
{
    public class CaloriesBurnedPerDayBarChartDrawable : IDrawable
    {
        #region [Fields]

        private int _xAxisNumberOfLines = 6;
        private int _yAxisNumberOfLines = 2; // need to add plus 1 to original number of lines for spacing

        public double _xAxisMaxValue = 60;
        private List<DateTimeOffset> _categoryAxes = new List<DateTimeOffset>();

        private double _canvasLeftMargin = 16;
        private double _canvasRightMargin = 16;

        private double _chartTopMargin = 16;
        private double _chartBottomMargin = 64;

        private Point _yAxisEndPoint = new Point(0, 0);
        private Point _origin = new Point(0, 0);
        private Point _xAxisEndPoint = new Point(0, 0);

        private double _yAxisLongestTextBlockWidth = 0;

        private float _fontSize = 12;

        #endregion


        #region [Public Properties]

        public CaloriesBurnedDataPointViewItem CaloriesBurnedDataPoint { get; set; }
        public DateTimeOffset StartDateTimeOffset { get; set; }

        #endregion


        #region [Public Methods :: Refresh Methods]

        public void Draw(ICanvas canvas, RectF dirtyRect)
        {

            _yAxisEndPoint = new Point(_canvasLeftMargin + _yAxisLongestTextBlockWidth, _chartTopMargin); // top-left point  //chart x value for left margin
            _origin = new Point(_canvasLeftMargin + _yAxisLongestTextBlockWidth, dirtyRect.Height - _chartBottomMargin);
            _xAxisEndPoint = new Point(dirtyRect.Width - _canvasRightMargin, dirtyRect.Height - _chartBottomMargin);

            _xAxisMaxValue = GetXAxisMaxValue();
            _categoryAxes = new List<DateTimeOffset>();

            GenerateChartYAxes(canvas, dirtyRect);
            GenerateBars(canvas, dirtyRect);
            GenerateChartXAxes(canvas, dirtyRect);
        }

        #endregion


        #region [Public Methods :: Refresh Methods]

        #endregion


        #region Private Method Tasks

        private double GetXAxisMaxValue()
        {
            double barItemHighestValue = 0;
            double xAxisMaxValue = 0;
            double xValueInterval = 0;

            if (this.CaloriesBurnedDataPoint != null && this.CaloriesBurnedDataPoint.TotalCalories.HasValue && this.CaloriesBurnedDataPoint.TotalCalories.Value > 0)
            {
                barItemHighestValue = this.CaloriesBurnedDataPoint.TotalCalories.Value;
            }
            else
            {
                barItemHighestValue = 2100;
            }

            xAxisMaxValue = (int)(Math.Ceiling((decimal)barItemHighestValue / _xAxisNumberOfLines) * _xAxisNumberOfLines);

            xValueInterval = xAxisMaxValue / _xAxisNumberOfLines;

            if (xValueInterval >= 1000000)
            {
                xValueInterval = (int)(Math.Ceiling((decimal)xValueInterval / 100000) * 100000);
            }
            else if (xValueInterval >= 100000)
            {
                xValueInterval = (int)(Math.Ceiling((decimal)xValueInterval / 10000) * 10000);
            }
            else if (xValueInterval >= 10000)
            {
                xValueInterval = (int)(Math.Ceiling((decimal)xValueInterval / 1000) * 1000);
            }
            else if (xValueInterval >= 1000)
            {
                xValueInterval = (int)(Math.Ceiling((decimal)xValueInterval / 100) * 100);
            }
            else if (xValueInterval >= 100)
            {
                xValueInterval = (int)(Math.Ceiling((decimal)xValueInterval / 10) * 10);
            }
            else if (xValueInterval >= 10)
            {
                xValueInterval = (int)(Math.Ceiling((decimal)xValueInterval / 2) * 2);
            }
            else
            {
                xValueInterval = (int)(Math.Round((decimal)xValueInterval, 2));
            }

            xAxisMaxValue = xValueInterval * _xAxisNumberOfLines;

            return xAxisMaxValue;
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
                    canvas.StrokeSize = (float)0.5;
                    canvas.StrokeDashPattern = new float[] { 16, 8 };
                    canvas.DrawLine(
                        x1: (float)xAxisValue,
                        y1: (float)_yAxisEndPoint.Y,
                        x2: (float)xAxisValue,
                        y2: (float)_origin.Y);

                    double textBlockHeight = GetTextBlockSize(canvas, xValue.ToString("N0"), _fontSize).Height;

                    canvas.FontSize = _fontSize;
                    canvas.DrawString(
                                value: xValue.ToString("N0"),
                                x: (float)(xAxisValue - xAxisInterval / 2),
                                y: (float)(_origin.Y + 8),
                                width: (float)xAxisInterval,
                                height: (float)textBlockHeight,
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
                    canvas.StrokeColor = Color.FromArgb("#EFEFEF");
                    canvas.StrokeSize = (float)1;
                    canvas.StrokeDashPattern = new float[] { 4, 4 };
                    canvas.DrawLine(
                        x1: (float)_origin.X,
                        y1: (float)yAxisValue,
                        x2: (float)_xAxisEndPoint.X,
                        y2: (float)yAxisValue);
                }
                else
                {
                    canvas.StrokeColor = Color.FromArgb("#EFEFEF");
                    canvas.StrokeSize = (float)0.5;
                    canvas.StrokeDashPattern = new float[] { 4, 4 };
                    canvas.DrawLine(
                        x1: (float)_origin.X,
                        y1: (float)yAxisValue,
                        x2: (float)_xAxisEndPoint.X,
                        y2: (float)yAxisValue);
                }

                if (i != 0 && i != _yAxisNumberOfLines)
                {
                    DateTimeOffset yValue = this.StartDateTimeOffset.AddDays(i - 1);
 
                    _categoryAxes.Add(yValue);
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
            double cornerRadius = barHeight / 2;

            double percentageMaxValue = xAxisTotalInterval + xAxisInterval;

            yAxisValue -= yAxisInterval;

            foreach (DateTimeOffset category in _categoryAxes)
            {
                Color trackerBarFillColor = Color.FromArgb("#EEF0F2");
                Color barFillColor = Color.FromArgb("#0072DB");

                // Tracker or the gray line
                canvas.FillColor = trackerBarFillColor;
                canvas.FillRoundedRectangle(
                   x: (float)_origin.X,
                   y: (float)(yAxisValue - (barHeight / 2)),
                   width: (float)percentageMaxValue,
                   height: (float)barHeight,
                   cornerRadius: (float)cornerRadius
                );

                if (this.CaloriesBurnedDataPoint == null)
                {
                    continue;
                }

                if (this.CaloriesBurnedDataPoint.TotalCalories.HasValue == false)
                {
                    continue;
                }

                if (this.CaloriesBurnedDataPoint.LocalStartDateTimeOffset.Date == category.Date)
                {
                    double percentageValue = xAxisTotalInterval / _xAxisMaxValue * this.CaloriesBurnedDataPoint.TotalCalories.Value;

                    // Actual bar
                    canvas.FillColor = barFillColor;
                    canvas.StrokeColor = Colors.Transparent;
                    canvas.StrokeSize = 0;
                    canvas.FillRoundedRectangle(
                       x: (float)_origin.X,
                       y: (float)(yAxisValue - (barHeight / 2)),
                       width: (float)percentageValue,
                       height: (float)barHeight,
                       cornerRadius: (float)cornerRadius
                    );

                    /* 
                     * As stated by the Microsoft docs,
                     * Draw fill first then draw only (e.g draw fill rectangle then call draw rectangle) to make the stroke work
                    */
                    canvas.DrawRoundedRectangle(
                       x: (float)_origin.X,
                       y: (float)(yAxisValue - (barHeight / 2)),
                       width: (float)percentageValue,
                       height: (float)barHeight,
                       cornerRadius: (float)cornerRadius
                   );
                }

                yAxisValue -= yAxisInterval;
            }
        }

        private SizeF GetTextBlockSize(ICanvas canvas, string text, float fontSize) //(TextBlock textBlock)
        {
            Font font = Font.Default;

            /*
             * GetStringSize is inaccurate
             * Need to add 2 to font size to be accurate
             * https://swharden.com/blog/2022-05-25-maui-graphics/
             * https://github.com/dotnet/Microsoft.Maui.Graphics/issues/279
             */
            fontSize = fontSize + 3;

            return canvas.GetStringSize(text, font, fontSize);       
        }

        #endregion
    }
}
