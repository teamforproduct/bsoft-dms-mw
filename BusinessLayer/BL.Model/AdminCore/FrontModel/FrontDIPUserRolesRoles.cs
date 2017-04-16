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
        public int RoleId { get; set; }

        public int PositionExecutorId { get; set; }

        public bool IsChecked { get; set; }

        /// <summary>
        /// Признак, роль по умолчанию (заводская) - true, пользовательская - false
        /// </summary>
        public bool IsDefault { get; set; }
    }
}