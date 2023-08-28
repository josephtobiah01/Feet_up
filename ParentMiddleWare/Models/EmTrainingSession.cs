using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParentMiddleWare.Models
{
    [Serializable]
    public class EmTrainingSession
    {
        //public EmTrainingSession() 
        //{
        //    emExercises = new List<EmExercise> {  };
        //}

        public long Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public long FkEdsDailyPlan { get; set; }
        public DateTime? StartDateTime { get; set; }
        public DateTime? EndDateTime { get; set; }
        public bool IsMoved { get; set; }
        public bool IsSkipped { get; set; }
        public DateTime? EndTimeStamp { get; set; }
        public bool IsCustomerAddedTrainingSession { get; set; }
        public long? ReasonForReschedule { get; set; }
        public long? ReadonForSkipping { get; set; }
        public int? ExerciseDuration { get; set; }
        public DateTime? StartTimestamp { get; set; }
        public DateTimeOffset? TimeOffset { get; set; }
        public string CustomerFeedback { get; set; }
        public double FloatFeedback { get; set; }
        public List<EmExercise> emExercises;
    }
}
