using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Model.DocumentCore.Actions
{
    /// <summary>
    /// Модель для изменения признака фаворита
    /// </summary>
    public class ChangeFavourites
    {
        /// <summary>
        /// ИД документа
        /// </summary>
        [Required]
        public int DocumentId { get; set; }
        /// <summary>
        /// Признак фаворита
        /// </summary>
        [Required]
        public bool IsFavourite { get; set; }
    }
}
