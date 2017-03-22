using System.Collections.Generic;
using BL.Model.Database;
using BL.Model.SystemCore;
using BL.Model.Users;
using System;

namespace BL.CrossCutting.Interfaces
{
    public interface IContext
    {
        bool IsFormed { get; set; }
        Employee CurrentEmployee { get; set; }
        List<int> CurrentPositionsIdList { get; set; }
        Dictionary<int,int> CurrentPositionsAccessLevel { get; set; }
        List<string> GetAccessFilterForFullText(string addFilter);
        DatabaseModel CurrentDB { get; set; }
        int CurrentPositionId { get; }
        int CurrentAgentId { get; }
        void SetCurrentPosition(int? position);
        bool IsAdmin { get; }
        LicenceInfo ClientLicence { get; set; }
        int CurrentClientId{ get; set; }
        DateTime CreateDate { get; }
        bool IsChangePasswordRequired { get; set; }
        int? LoginLogId { get; set; }
        string LoginLogInfo { get; set; }
    }
}