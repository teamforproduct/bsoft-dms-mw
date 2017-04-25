namespace DMS_WebAPI.Models
{
    public class EmployeeCreationResult : UserCreationResult
    {
        public EmployeeCreationResult() { }

        public EmployeeCreationResult(UserCreationResult user) : base (user) { }

        public int EmployeeId { set; get; }
    }
}