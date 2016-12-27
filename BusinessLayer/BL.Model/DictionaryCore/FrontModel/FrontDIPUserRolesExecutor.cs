using BL.Model.Common;
using BL.Model.DictionaryCore.IncomingModel;
using BL.Model.Enums;
using BL.Model.Extensions;
using BL.Model.Tree;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace BL.Model.DictionaryCore.FrontModel
{
    public class FrontDIPUserRolesExecutor: TreeItem
    {
        public DateTime? StartDate { get { return _StartDate; } set { _StartDate=value.ToUTC(); } }
        private DateTime?  _StartDate; 

        public DateTime? EndDate { get { return _EndDate; } set { _EndDate=value.ToUTC(); } }
        private DateTime?  _EndDate; 

        //public string PositionName { get; set; }

        public string ExecutorTypeName { get; set; }

        public int PositionId { get; set; }
    }
}