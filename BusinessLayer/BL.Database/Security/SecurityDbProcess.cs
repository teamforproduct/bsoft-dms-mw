using System.Collections.Generic;
using BL.Model.Users;
using BL.CrossCutting.Interfaces;
using System.Linq;
using BL.CrossCutting.Helpers;
using BL.Database.DatabaseContext;

namespace BL.Database.Security
{
    public class SecurityDbProcess : CoreDb.CoreDb, ISecurityDbProcess
    {

        private readonly IConnectionStringHelper _helper;

        public SecurityDbProcess(IConnectionStringHelper helper)
        {
            _helper = helper;
        }

        public Employee GetEmployee(IContext ctx, int id)
        {
            using (var dbContext = new DmsContext(_helper.GetConnectionString(ctx)))
            {
                return dbContext.DictionaryAgentsSet.Where(x => x.Id == id).Select(x => new Employee
                {
                    AgentId = x.Id,
                    Name = x.Name
                }).FirstOrDefault();
            }
        }

        public IEnumerable<CurrentPosition> GetPositionsByUser(Employee employee)
        {
            return null;
        }


    }
}