using BL.Model.Database;
using BL.Model.Users;
using System.Collections.Generic;

namespace BL.CrossCutting.Interfaces
{
    public interface IContext
    {
        Employee CurrentEmployee { get; set; }
        List<int> CurrentPositionsIdList { get; set; }
        DatabaseModel CurrentDB { get; set; }
        int? CurrentPositionId { get; set; }
        int CurrentAgentId { get; }
    }
}