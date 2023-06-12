namespace ParentMiddleWare.Models
{
    [Serializable]
    public class EmSetMetrics
    {
        public EmSetMetrics()
        {

        }
        public long Id { get; set; }
        public double? TargetCustomMetric { get; set; }
        public double? ActualCustomMetric { get; set; }

        public EmSetMetricTypes EmSetMetricTypes { get; set;}

    }
}
