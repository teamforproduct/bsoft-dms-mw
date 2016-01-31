using System.Collections.Generic;
using BL.Model.Users;
using BL.CrossCutting.Interfaces;
using System.Linq;

namespace BL.Database.Security
{
    public class SecurityDbProcess : CoreDb.CoreDb, ISecurityDbProcess
    {
        public Employee GetEmployee(IContext context, int id)
        {
            var dbContext = GetUserDmsContext(context);
            return dbContext.DictionaryAgentsSet.Where(x => x.Id == id).Select(x => new Employee
            {
                AgentId = x.Id,
                Name = x.Name
            }).FirstOrDefault();
        }

        public IEnumerable<Position> GetPositionsByUser(Employee employee)
        {
            return null;
        }


    }
}