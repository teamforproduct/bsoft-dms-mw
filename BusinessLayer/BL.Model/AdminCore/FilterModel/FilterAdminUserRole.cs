using BL.Model.Common;
using BL.Model.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Model.AdminCore.FilterModel
{
    /// <summary>
    /// Фильтры FilterAdminUserRole
    /// </summary>
    public class FilterAdminUserRole: AdminBaseFilterParameters
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
        /// Пользователи
        /// </summary>
        public List<int> UserIDs { get; set; }

        /// <summary>
        /// Роли
        /// </summary>
        public List<int> RoleIDs { get; set; }

        /// <summary>
        /// Должности
        /// </summary>
        public List<int> PositionIDs { get; set; }

        /// <summary>
        /// Исполнители
        /// </summary>
        public List<int> PositionExecutorIDs { get; set; }


        /// <summary>
        /// Дата начала
        /// </summary>
        public DateTime? StartDate { get { return _StartDate; } set { _StartDate = value.ToUTC(); } }
        private DateTime? _StartDate;

        /// <summary>
        /// Дата окончания
        /// </summary>
        public DateTime? EndDate { get { return _EndDate; } set { _EndDate = value.ToUTC(); } }
        private DateTime? _EndDate;

        /// <summary>
        /// Отмечнено
        /// </summary>
        public bool? IsChecked { get; set; }

    }
}