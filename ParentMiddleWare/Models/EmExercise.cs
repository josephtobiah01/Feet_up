using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParentMiddleWare.Models
{
    [Serializable]
    public class EmExercise
    {
        //public EmExercise() 
        //{
        //    EmSet = new List<EmSet> { };
        //}

        public long Id { get; set; }
        public long FkExerciseTypeId { get; set; }
        public long FkTrainingId { get; set; }
        public bool IsSkipped { get; set; }        
        public bool IsComplete { get; set; }
        public DateTime? EndTimeStamp { get; set; }
        public bool IsCustomerAddedExercise { get; set; }
        public DateTimeOffset? TimeOffset { get; set; }
        public EmExerciseType EmExerciseType { get; set; }
        public List<EmSet> EmSet { get; set; }
    }
}
