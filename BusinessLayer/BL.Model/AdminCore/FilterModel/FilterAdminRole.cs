using BL.Model.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace BL.Model.AdminCore.FilterModel
{
    /// <summary>
    /// Фильтры FilterAdminRole
    /// </summary>
    public class FilterAdminRole: AdminBaseFilterParameters
    {

        /// <summary>
        /// Сужение по наименованию элементов (по входжению)
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Сужение по наименованию элементов (по равенству)
        /// </summary>
        public string NameExact { get; set; }

        /// <summary>
        /// Сужение по описанию
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Сужение по ролям
        /// </summary>
        public List<int> RoleIDs { get; set; }

        /// <summary>
        /// Сужение по типам
        /// </summary>
        [IgnoreDataMember]
        public List<int> RoleTypeIDs { get; set; }

        /// <summary>
        /// Список должностей
        /// </summary>
        public List<int> PositionIDs { get; set; }

        /// <summary>
        /// Список сотрудников
        /// </summary>
        public List<int> UserIDs { get; set; }

        /// <summary>
        /// Дата разрешения роли для сотрудника
        /// </summary>
        public DateTime? StartDate { get { return _StartDate; } set { _StartDate = value.ToUTC(); } }
        private DateTime? _StartDate;

        /// <summary>
        /// Дата запрета роли для сотрудника
        /// </summary>
        public DateTime? EndDate { get { return _EndDate; } set { _EndDate = value.ToUTC(); } }
        private DateTime? _EndDate;

        /// <summary>
        /// Отмечнено
        /// </summary>
        public bool? IsChecked { get; set; }

    }
}