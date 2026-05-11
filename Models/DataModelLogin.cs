namespace NetFavorite.Models
{
    public class DataModelLogin
    {
        public Guid Id { get; set; }
        public string Account { get; set; } = null!;
        public string Role { get; set; } = null!;
        public string Token { get; set; } = null!;
    }
}
