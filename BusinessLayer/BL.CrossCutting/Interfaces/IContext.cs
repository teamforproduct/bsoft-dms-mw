using BL.Model.Context;
using BL.Model.SystemCore;
using System;
using System.Collections.Generic;

namespace BL.CrossCutting.Interfaces
{
    public interface IContext
    { 
        string Key { get; set; }
        bool IsAdmin { get; }
        bool IsFormed { get; set; }

        Client Client { get; set; }
        Employee Employee { get; set; }
        User User { get; set; }
        DatabaseModel CurrentDB { get; set; }

        Session Session { get; set; }
        LicenceInfo ClientLicence { get; set; }


        bool CurrentPositionsIdListDefined { get; }
        List<int> CurrentPositionsIdList { get; set; }
        bool CurrentPositionsAccessLevelDefined { get; }
        Dictionary<int,int> CurrentPositionsAccessLevel { get; set; }
        List<string> GetAccessFilterForFullText(string addFilter);
        
        bool CurrentPositionDefined { get; }
        int CurrentPositionId { get; }
        int CurrentAgentId { get; }
        void SetCurrentPosition(int? position);

        IDmsDatabaseContext DbContext { get; set; }

    }
}