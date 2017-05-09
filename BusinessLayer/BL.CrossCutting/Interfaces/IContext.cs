using BL.Model.Context;
using BL.Model.SystemCore;
using System;
using System.Collections.Generic;

namespace BL.CrossCutting.Interfaces
{
    public interface IContext
    {
        string Token { get; set; }
        bool IsFormed { get; set; }

        Client Client { get; set; }
        Employee Employee { get; set; }
        User User { get; set; }
        bool CurrentPositionsIdListDefined { get; }
        List<int> CurrentPositionsIdList { get; set; }
        bool CurrentPositionsAccessLevelDefined { get; }
        Dictionary<int,int> CurrentPositionsAccessLevel { get; set; }
        List<string> GetAccessFilterForFullText(string addFilter);
        DatabaseModel CurrentDB { get; set; }
        bool CurrentPositionDefined { get; }
        int CurrentPositionId { get; }
        int CurrentAgentId { get; }
        void SetCurrentPosition(int? position);
        bool IsAdmin { get; }
        LicenceInfo ClientLicence { get; set; }
        DateTime CreateDate { get; }
        DateTime LastChangeDate { get; set; }
        int? LoginLogId { get; set; }
        string LoginLogInfo { get; set; }
        IDmsDatabaseContext DbContext { get; set; }

    }
}