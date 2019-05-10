namespace Manager.SaveManagement
{
    public class SaveOption
    {
        public string Title { get; set; }
        public string Value { get; set; }

        public SaveOption(string title, string value)
        {
            Title = title;
            Value = value;
        }
    }
}