using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BL.Model.Common
{
    /// <summary>
    /// Модель пункт меню
    /// </summary>
    public class MenuItem: ITreeItem
    {
      
        /// <summary>
        /// ID
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// ID
        /// </summary>
        public int? ParentItemId { get; set; }

        /// <summary>
        /// Наименование
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// URL
        /// </summary>
        public string InterfaceName { get; set; }

        public int? ObjectId { get; set; }

        public bool IsUsed { get; set; }

        public IEnumerable<ITreeItem> Childs { get; set; }
       
    }
}
