using BL.Model.Common;
using BL.Model.DictionaryCore.IncomingModel;
using BL.Model.Enums;
using BL.Model.Tree;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace BL.Model.DictionaryCore.FrontModel
{
    public class FrontDIPUserRolesExecutor: TreeItem
    {
        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        //public string PositionName { get; set; }

        public string ExecutorTypeName { get; set; }
    }
}