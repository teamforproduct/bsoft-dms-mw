using System.Collections.Generic;
using BL.Model.Database;
using BL.Model.SystemCore;
using BL.Model.Users;
using System;

namespace BL.CrossCutting.Interfaces
{
    public interface IContext
    {
        Employee CurrentEmployee { get; set; }
        List<int> CurrentPositionsIdList { get; set; }
        Dictionary<int,int> CurrentPositionsAccessLevel { get; set; }
        DatabaseModel CurrentDB { get; set; }
        int CurrentPositionId { get; }
        int CurrentAgentId { get; }
        void SetCurrentPosition(int? position);
        bool IsAdmin { get; }
        LicenceInfo ClientLicence { get; set; }
        int CurrentClientId{ get; set; }
        DateTime CreateDate { get; }
    }
}