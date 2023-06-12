namespace FitappAdminWeb.Net7.Classes.DTO
{
    public class AutoCompleteChoiceDTO
    {
        public string value { get; set; } = String.Empty;
        public string label { get; set; } = String.Empty;
        public bool selected { get; set; }
        public bool disabled { get; set; }
    }
}
