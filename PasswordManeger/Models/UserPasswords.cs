namespace PasswordManeger.Models
{
    public class UserPasswords
    {
        public int Id { get; set; }
        public string ServiceName { get; set; }
        public string Password { get; set; }
        public string Link { get; set; }
        public Users users { get; set; }
        public string UserId { get; set; }
    }
}
