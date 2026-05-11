namespace WinFormsApp1
{
    public class DataModelLogin
    {
        public Guid id { get; set; }
        public string account { get; set; } = null!;
        public string role { get; set; } = null!;
        public string token { get; set; } = null!;
    }
}
