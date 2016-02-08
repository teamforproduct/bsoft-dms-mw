using System.Collections.Generic;
using BL.CrossCutting.Interfaces;
using BL.Model.Users;

namespace BL.Database.Security
{
    public interface ISecurityDbProcess
    {
        Employee GetEmployee(IContext ctx, int id);
        IEnumerable<Position> GetPositionsByUser(Employee employee);
    }
}