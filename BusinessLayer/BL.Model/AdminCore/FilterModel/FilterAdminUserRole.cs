using BL.Model.Common;
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
        public List<int> PositionsIDs { get; set; }

        /// <summary>
        /// Рассматриваемый период
        /// </summary>
        //public Period Period { get; set; }

        /// <summary>
        /// Дата начала
        /// </summary>
        public DateTime? StartDate { get; set; }

        /// <summary>
        /// Дата окончания
        /// </summary>
        public DateTime? EndDate { get; set; }

        /// <summary>
        /// Отмечнено
        /// </summary>
        public bool? IsChecked { get; set; }

    }
}