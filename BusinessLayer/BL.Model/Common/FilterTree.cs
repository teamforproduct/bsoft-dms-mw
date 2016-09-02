using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Model.Common
{
    /// <summary>
    /// Фильтр для иерархического списка
    /// </summary>
    public class FilterTree
    {

        /// <summary>
        /// Начинает построение дерева с указанного ID. Внимание! Нужно передавать уникальный TreeId.
        /// </summary>
        public string StartWith { get; set; }

        /// <summary>
        /// Сужение по наименованию
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Только активные
        /// </summary>
        public bool? IsActive { get; set; }
    }
}
