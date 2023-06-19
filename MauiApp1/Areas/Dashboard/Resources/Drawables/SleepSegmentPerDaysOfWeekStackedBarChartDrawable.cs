using Microsoft.Maui.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Font = Microsoft.Maui.Graphics.Font;
using static MauiApp1.Common.DeviceIntegration.DeviceIntegrationEnumerations;
using DeviceIntegration.Common.Data;

namespace MauiApp1.Areas.Dashboard.Resources.Drawables
{
    public class SleepSegmentPerDaysOfWeekStackedBarChartDrawable : IDrawable
    {
        #region [Fields]

        //private List<StepsCountDataPointViewItem> _stepsCountDataPointViewItems { get; set; }

        private int _xAxisNumberOfLines = 6;
        private int _yAxisNumberOfLines = 8;  // need to add plus 1 to original number of lines for spacing

        public double _xAxisMaxValue = 60;
        private List<DateTimeOffset> _categoryAxes = new List<DateTimeOffset>();

        private double _canvasLeftMargin = 16;
        private double _canvasRightMargin = 16;

        private double _chartTopMargin = 16;
        private double _chartBottomMargin = 64;

        private Point _yAxisEndPoint = new Point(0, 0);
        private Point _origin = new Point(0, 0);
        private Point _xAxisEndPoint = new Point(0, 0);

        private double _yAxisLongestTextBlockWidth = 50;

        private int _targetLineValue = 8;
        
        private float _fontSize = 12;

        #endregion


        #region [Public Properties]

        public List<SleepSegmentDataPointViewItem> SleepSegmentDataPointViewItems { get; set; }
        public DateTimeOffset StartDateTimeOffset { get; set; }
        //public DateTimeOffset EndDateTimeOffset { get; set; }

        #endregion


        #region [Public Methods :: Refresh Methods]

        public void Draw(ICanvas canvas, RectF dirtyRect)
        {
            //throw new NotImplementedException();

            _yAxisEndPoint = new Point(_canvasLeftMargin + _yAxisLongestTextBlockWidth, _chartTopMargin); // top-left point  //chart x value for left margin
            _origin = new Point(_canvasLeftMargin + _yAxisLongestTextBlockWidth, dirtyRect.Height - _chartBottomMargin);
            _xAxisEndPoint = new Point(dirtyRect.Width - _canvasRightMargin, dirtyRect.Height - _chartBottomMargin);

            _xAxisMaxValue = GetXAxisMaxValue();
            _categoryAxes = new List<DateTimeOffset>();

            GenerateChartYAxes(canvas, dirtyRect);
            GenerateBars(canvas, dirtyRect);
            GenerateChartXAxes(canvas, dirtyRect);
            GenerateLineAnnotation(canvas, dirtyRect);
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

            if (this.SleepSegmentDataPointViewItems != null && this.SleepSegmentDataPointViewItems.Count > 0)
            {
                //barItemHighestValue = this.SleepSegmentDataPointViewItems.OrderByDescending(t => t.SleepSegment).FirstOrDefault().SleepSegment.Value;

                var groupedBarItems = this.SleepSegmentDataPointViewItems
                                        .GroupBy(t => t.LocalDateTimeOffset.Date)
                                        .Select(t => new { Label = t.Key, Value = t.Sum(t => t.Duration) })
                                        .ToList();

                barItemHighestValue = groupedBarItems.OrderByDescending(t => t.Value).FirstOrDefault().Value;
            }

            if (barItemHighestValue < _targetLineValue)
            {
                barItemHighestValue = _targetLineValue;
                xAxisMaxValue = (int)(Math.Ceiling((decimal)barItemHighestValue / _xAxisNumberOfLines) * _xAxisNumberOfLines);

                // This is to ensure that the target line will be drawn 1 line before the last line and will not be place at the edge / border of the canvas, resulting in cropped texts
                xAxisMaxValue = xAxisMaxValue + (xAxisMaxValue / _xAxisNumberOfLines);
            }
            else
            {
                xAxisMaxValue = (int)(Math.Ceiling((decimal)barItemHighestValue / _xAxisNumberOfLines) * _xAxisNumberOfLines);
            }

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
                if (i != 0 && i != (_xAxisNumberOfLines + 1) && xValue != _targetLineValue)
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
                    double textBlockHeight = GetTextBlockSize(canvas, yValue.ToString("dd/MM"), _fontSize).Height;

                    canvas.FontSize = _fontSize;

                    canvas.DrawString(
                            value: yValue.ToString("dd/MM"),
                            x: (float)_canvasLeftMargin,
                            y: (float)(yAxisValue - 8),
                            width: (float)_yAxisLongestTextBlockWidth,
                            height: (float)textBlockHeight,
                            horizontalAlignment: HorizontalAlignment.Left,
                            verticalAlignment: VerticalAlignment.Center,
                            textFlow: TextFlow.OverflowBounds,
                            lineSpacingAdjustment: 0);

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
            double previousBarBlockWidth = 0;
            double stackedBarLeftMargin = 4;

            double percentageMaxValue = xAxisTotalInterval + xAxisInterval;

            List<int> customSortedSleepStageTypes = GetCustomSortedSleepStageTypes();

            yAxisValue -= yAxisInterval;

            foreach (DateTimeOffset category in _categoryAxes)
            {
                previousBarBlockWidth = 0;
                stackedBarLeftMargin = 0;

                Color trackerBarFillColor = Color.FromArgb("#EEF0F2");
                Color barFillColor = Colors.Transparent;

                // Tracker or the gray line
                canvas.FillColor = trackerBarFillColor;
                canvas.FillRoundedRectangle(
                   x: (float)_origin.X,
                   y: (float)(yAxisValue - (barHeight / 2)),
                   width: (float)percentageMaxValue,
                   height: (float)barHeight,
                   cornerRadius: (float)cornerRadius
                );



                foreach (int sleepStageTypeId in customSortedSleepStageTypes)
                {
                    SleepSegmentDataPointViewItem sleepSegment = this.SleepSegmentDataPointViewItems
                        .Where(t => t.LocalDateTimeOffset.Date == category.Date && t.SleepStageTypeId == sleepStageTypeId)
                        .FirstOrDefault();

                    if (sleepSegment != null)
                    {
                        double percentageValue = xAxisTotalInterval / _xAxisMaxValue * sleepSegment.Duration;
                        double blockWidth = percentageValue - stackedBarLeftMargin;

                        if ((percentageValue - stackedBarLeftMargin) > 0)
                        {
                            if (sleepStageTypeId == (int)SleepStageTypes.DeepSleep)
                            {
                                barFillColor = Color.FromArgb("#1C4599");
                            }
                            else if (sleepStageTypeId == (int)SleepStageTypes.LightSleep)
                            {
                                barFillColor = Color.FromArgb("#3275CF");
                            }
                            else if (sleepStageTypeId == (int)SleepStageTypes.Rem)
                            {
                                barFillColor = Color.FromArgb("#AD5FC0");
                            }
                            else if (sleepStageTypeId == (int)SleepStageTypes.AwakeDuringSleepCycle)
                            {
                                barFillColor = Color.FromArgb("#EC77D4");
                            }
                            else
                            {
                                barFillColor = Colors.Transparent;
                            }

                            canvas.FillColor = barFillColor;
                            canvas.FillRoundedRectangle(
                               x: (float)(_origin.X + previousBarBlockWidth + stackedBarLeftMargin),
                               y: (float)(yAxisValue - (barHeight / 2)),
                               width: (float)(percentageValue - stackedBarLeftMargin),
                               height: (float)barHeight,
                               cornerRadius: (float)cornerRadius
                           );

                            previousBarBlockWidth = previousBarBlockWidth + blockWidth + stackedBarLeftMargin;
                        }

                        if (percentageValue > 0)
                        {
                            stackedBarLeftMargin = 4;
                        }
                    }
                }

                yAxisValue -= yAxisInterval;
            }
        }

        private void GenerateLineAnnotation(ICanvas canvas, RectF dirtyRect)
        {
            if (_targetLineValue > 0)
            {
                double xAxisInterval = (dirtyRect.Width - _canvasLeftMargin - _canvasRightMargin - _yAxisLongestTextBlockWidth) / (_xAxisNumberOfLines + 1); // add 1 so that we make the last line blank
                double xAxisTotalInterval = xAxisInterval * _xAxisNumberOfLines;

                double xAxisValue = _origin.X + (xAxisTotalInterval / _xAxisMaxValue * _targetLineValue);

                string lineAnnotationText = "Target 8 hours";

                canvas.StrokeColor = Colors.Black;
                canvas.StrokeSize = (float)0.5;
                canvas.StrokeDashPattern = new float[] { 16, 8 };
                canvas.DrawLine(
                    x1: (float)xAxisValue,
                    y1: (float)_yAxisEndPoint.Y,
                    x2: (float)xAxisValue,
                    y2: (float)(_origin.Y + 32)
                );

                double textBlockWidth = GetTextBlockSize(canvas, lineAnnotationText, (float)_fontSize).Width;
                double textBlockHeight = GetTextBlockSize(canvas, lineAnnotationText, (float)_fontSize).Height;

                canvas.DrawString(
                    value: lineAnnotationText,
                    x: (float)(xAxisValue - (textBlockWidth / 2)),
                    y: (float)(_origin.Y + 32),
                    width: (float)textBlockWidth,
                    height: (float)textBlockHeight,
                    horizontalAlignment: HorizontalAlignment.Center,
                    verticalAlignment: VerticalAlignment.Center,
                    textFlow: TextFlow.OverflowBounds,
                    lineSpacingAdjustment: 0);
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

            //Size maxSize = new Size(double.PositiveInfinity, double.PositiveInfinity);
            //textBlock.Measure(maxSize);

            //return new Size(textBlock.DesiredSize.Width, textBlock.DesiredSize.Height);
        }

        private List<int> GetCustomSortedSleepStageTypes()
        {
            List<int> sortedSleepStageTypes = new List<int>();

            sortedSleepStageTypes.Add((int)SleepStageTypes.DeepSleep);
            sortedSleepStageTypes.Add((int)SleepStageTypes.LightSleep);
            sortedSleepStageTypes.Add((int)SleepStageTypes.Rem);
            sortedSleepStageTypes.Add((int)SleepStageTypes.AwakeDuringSleepCycle);
            //sortedSleepStageTypes.Add((int)SleepStageTypes.Sleep);
            //sortedSleepStageTypes.Add((int)SleepStageTypes.OutOfBed);

            return sortedSleepStageTypes;
        }

        #endregion
    }
}
