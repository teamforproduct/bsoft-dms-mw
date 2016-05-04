using System.Collections.Generic;
using BL.Model.Database;
using BL.Model.SystemCore;
using BL.Model.Users;

namespace BL.CrossCutting.Interfaces
{
    public interface IContext
    {
        Employee CurrentEmployee { get; set; }
        List<int> CurrentPositionsIdList { get; set; }
        DatabaseModel CurrentDB { get; set; }
        int CurrentPositionId { get; }
        int CurrentAgentId { get; }
        void SetCurrentPosition(int? position);
        bool IsAdmin { get; }
        LicenceInfo ClientLicence { get; set; }
    }
}