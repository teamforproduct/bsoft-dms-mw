using BL.Model.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Model.AdminCore.FilterModel
{
    /// <summary>
    /// Фильтры FilterPermissionsAccess
    /// </summary>
    public class FilterPermissionsAccess //: AdminBaseFilterParameters
    {
        /// <summary>
        /// Permissions
        /// </summary>
        public List<int> PermissionIDs { get; set; }
        /// <summary>
        /// Role
        /// </summary>
        public List<int> RoleIDs { get; set; }
        /// <summary>
        /// ИД юзера, по умолчанию возьмет из контекста
        /// </summary>
        public int UserId { get; set; }
        /// <summary>
        /// Код действия
        /// </summary>
        public int? ActionId { get; set; }
        /// <summary>
        /// Массив ИД должностей, для проверки
        /// </summary>
        public List<int> PositionsIdList { get; set; }
        /// <summary>
        /// Код модуля
        /// </summary>
        public int? ModuleId { get; set; }


    }
}