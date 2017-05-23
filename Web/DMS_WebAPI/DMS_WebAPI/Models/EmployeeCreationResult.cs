using System;

namespace DMS_WebAPI.Models
{
    public class EmployeeCreationResult : UserCreationResult
    {
        public EmployeeCreationResult() { }

        public EmployeeCreationResult(UserCreationResult user) : base ( user) { IsNew = user.IsNew; }

        public int EmployeeId { set; get; }

        [Obsolete( "Команда не возвращает полное имя созданного сотрудника", true)]
        public int EmployeeFullName { set; get; }
    }
}