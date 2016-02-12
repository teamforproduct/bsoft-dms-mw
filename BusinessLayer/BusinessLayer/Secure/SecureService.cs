using System.Collections.Generic;
using BL.CrossCutting.DependencyInjection;
using BL.Model.Users;
using BL.CrossCutting.Interfaces;
using BL.Database.Security;

namespace BL.Logic.Secure
{
    public class SecureService : ISecureService
    {
        public Employee GetEmployee(IContext context, int id)
        {
            var db = DmsResolver.Current.Get<ISecurityDbProcess>();
            return db.GetEmployee(context, id);
        }

        public IEnumerable<CurrentPosition> GetPositionsByUser(Employee employee)
        {
            var db = DmsResolver.Current.Get<ISecurityDbProcess>();
            return db.GetPositionsByUser(employee);
        }
    }
}