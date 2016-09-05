using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace BL.Model.Tree
{
    /// <summary>
    /// Фильтр для иерархического списка
    /// </summary>
    public class FilterTree
    {

        /// <summary>
        /// Начинает построение дерева с указанного ID. Внимание! Нужно передавать уникальный TreeId.
        /// </summary>
        public string StartWithTreeId { get; set; }

        /// <summary>
        /// Начинает построение дерева с указанного ParentID. 
        /// </summary>
        [IgnoreDataMember]
        public string StartWithTreeParentId { get; set; }

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
