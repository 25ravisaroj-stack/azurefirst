namespace product.Models
{
    public class UserListResponse
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public string? Photo { get; set; }
        public List<string> Roles { get; set; } = new();
    }
}