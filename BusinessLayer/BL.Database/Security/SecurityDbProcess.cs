
using System.Collections.Generic;
using BL.Model.Users;

namespace BL.Database.Security
{
    public class SecurityDbProcess : CoreDb.CoreDb
    {
        public Employee GetEmployee(string userLogin)
        {
            //var dbContext = GetUserDmsContext(context);
            return null;
        }

        public IEnumerable<Position> GetPositionsByUser(Employee employee)
        {
            return null;
        }


    }
}