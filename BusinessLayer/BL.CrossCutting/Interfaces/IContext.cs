using BL.Model.Context;
using BL.Model.SystemCore;
using System;
using System.Collections.Generic;

namespace BL.CrossCutting.Interfaces
{
    public interface IContext
    {
        string Key { get; set; }

        bool IsFormed { get; set; }

        Client Client { get; set; }
        Employee Employee { get; set; }
        User User { get; set; }
        DatabaseModel CurrentDB { get; set; }

        void SetCurrentPosition(int? position);


        bool CurrentPositionsIdListDefined { get; }
        List<int> CurrentPositionsIdList { get; set; }
        bool CurrentPositionsAccessLevelDefined { get; }
        Dictionary<int,int> CurrentPositionsAccessLevel { get; set; }
        List<string> GetAccessFilterForFullText(string addFilter);
        bool CurrentPositionDefined { get; }
        int CurrentPositionId { get; }
        int CurrentAgentId { get; }
        bool IsAdmin { get; }
        LicenceInfo ClientLicence { get; set; }
        DateTime CreateDate { get; }
        /// <summary>
        /// KeepAlive - дата последнего обращения
        /// </summary>
        DateTime LastUsage { get; set; }

        /// <summary>
        /// KeepAlive дата последнего обновления в базе
        /// </summary>
        DateTime LastChangeDate { get; set; }
        int? LoginLogId { get; set; }
        IDmsDatabaseContext DbContext { get; set; }


    }
}