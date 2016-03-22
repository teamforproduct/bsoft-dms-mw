namespace BL.Model.Users
{
    public class Employee
    {
        public string Token { get; set; }
        public string UserId { get; set; }
        public int? AgentId { get; set; }
        public string Name { get; set; }
        public int LanguageId { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }
    }
}