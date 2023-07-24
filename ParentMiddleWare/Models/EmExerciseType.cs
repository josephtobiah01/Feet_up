
using System.Drawing;

namespace ParentMiddleWare.Models
{
    [Serializable]
    public class EmExerciseType
    {
        public long Id { get; set; }

        public string Name { get; set; }

        public string ExplainerVideoFr { get; set; }

        public string ExplainerTextFr { get; set; }

        public string ImageUrl { get; set; }

        public bool HasSetDefaultTemplate { get; set; }

        public bool IsSetCollapsed { get; set; }

        public bool IsDeleted { get; set; }

        public  EmEquipment EmEquipment { get; set; }

        public  EmExerciseClass EmExerciseClass { get; set; }

        public  EmForce EmForce { get; set; }

        public  EmLevel EmLevel { get; set; }

        public  EmMainMuscleWorked EmMainMuscleWorked { get; set; }

        public  EmMechanicsType EmMechanicsType { get; set; }

        public  EmOtherMuscleWorked EmOtherMuscleWorked { get; set; }

        public  EmSport EmSport { get; set; }
    }
}