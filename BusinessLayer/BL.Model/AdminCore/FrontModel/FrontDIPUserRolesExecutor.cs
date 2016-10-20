using BL.Model.Common;
using BL.Model.DictionaryCore.IncomingModel;
using BL.Model.Enums;
using BL.Model.Tree;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace BL.Model.AdminCore.FrontModel
{ 
    public class FrontDIPUserRolesRoles: TreeItem
    {
        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public int PositionId { get; set; }

        public bool IsChecked { get; set; }
    }
}