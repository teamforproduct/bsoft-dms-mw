using System.Collections.Generic;
using BL.CrossCutting.Interfaces;
using BL.Model.Users;

namespace BL.Logic.Secure
{
    public interface ISecureService
    {
        Employee GetEmployee(IContext context, int id);
        IEnumerable<Position> GetPositionsByUser(Employee employee);
    }
}