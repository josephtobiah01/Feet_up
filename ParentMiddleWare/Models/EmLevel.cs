namespace ParentMiddleWare.Models
{
    [Serializable]
    public class EmLevel
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public bool IsDeleted { get; set; }
    }
}