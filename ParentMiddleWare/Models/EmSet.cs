using System.Reflection.Metadata.Ecma335;
using System.Linq;

namespace ParentMiddleWare.Models
{
    [Serializable]
    public class EmSet
    {
        //public EmSet()
        //{
        //    EmSetMetrics = new List<EmSetMetrics> { };
        //}

        public long Id { get; set; }
        public long ExerciseId { get; set; }
        public short SetSequenceNumber { get; set; }
        public bool IsComplete { get; set; }
        public bool IsSkipped { get; set; }
        public bool IsCustomerAddedSet { get; set; }
        public DateTime? EndTimeStamp { get; set; }
        public DateTimeOffset? TimeOffset { get; set; }
        public string PreviousHistory { get; set; }

        public List<EmSetMetrics> EmSetMetrics { get; set; }

        public double GetRestTime()
        {
           foreach(var x in EmSetMetrics)
            {

                if(x.EmSetMetricTypes.Name == "Rest" && x.EmSetMetricTypes.isTime) 
                {
                    return x.TargetCustomMetric ?? 0;
                }   
            }
            return 0;
        }

        //public async Task<string> GetTextAsync()
        //{
        //   await AirMemoryCache.GetUserHistory().Wh


        //    string r = string.Empty;
        //    foreach (var x in EmSetMetrics)
        //    {
        //        if (x.EmSetMetricTypes.Name != "Rest")
        //        {
        //            r += x.TargetCustomMetric + x.EmSetMetricTypes.Name + "x";
        //        }
        //    }
        //    return r.Trim(new char[] { 'x' });
        //}

        public int GetNumberOfMetrics()
        {
            return EmSetMetrics.Count;
        }

        public string[] GetLabels()
        {
            List<string> labels = new List<string>();
            foreach (var x in EmSetMetrics)
            {
               if(x.EmSetMetricTypes.isDistance)
                {
                    labels.Add("Distance");
                }
                else if (x.EmSetMetricTypes.isResistance)
                {
                    labels.Add("Res.");
                }
                else if (x.EmSetMetricTypes.isRepetition)
                {
                    labels.Add("Reps");
                }
                else if (x.EmSetMetricTypes.isWeight)
                {
                    labels.Add("Weight");
                }
                else if (x.EmSetMetricTypes.isTime && x.EmSetMetricTypes.Name != "Rest")
                {
                    labels.Add("Dur");
                }

            }
            return labels.ToArray();
        }


    }
}
