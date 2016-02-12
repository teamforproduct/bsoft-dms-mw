using BL.Model.Database;
using BL.Model.Users;
using System.Collections.Generic;

namespace BL.CrossCutting.Interfaces
{
    public interface IContext
    {
        Employee CurrentEmployee { get; set; }
        List<CurrentPosition> CurrentPosition { get; set; }
        DatabaseModel CurrentDB { get; set; }
        int CurrentPositionId { get; }
        int CurrentAgentId { get; }
    }
}