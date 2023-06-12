namespace ParentMiddleWare.Models
{
    [Serializable]
    public class EmSetMetricTypes
    {
        public EmSetMetricTypes()
        {
             
        }

        public long Id { get; set; }
        public string Name { get; set; }
        public bool isTime { get; set; }
        public bool isWeight { get; set; }
        public bool isDistance { get; set; }
        public bool isRepetition { get; set; }
        public bool isResistance { get; set; }

    }
}
