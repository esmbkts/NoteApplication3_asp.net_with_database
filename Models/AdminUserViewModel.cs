namespace asp.net.Models
{
    public class AdminUserViewModel
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string Firstname { get; set; }
        public string Surname { get; set; }
        public List<string> Usernames { get; set; }
    }                                                                      
}
