using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using System.ComponentModel.DataAnnotations;

namespace BL.Model.DictionaryCore.IncomingModel
{
    public class ModifyDictionaryAgent
    {

        /// <summary>
        /// ИД
        /// </summary>
        [IgnoreDataMember]
        public int Id { get; set; }
        /// <summary>
        /// ФИО, Название, ...
        /// </summary>
        [Required]
        public string Name { get; set; }

       /// <summary>
       /// Резидентность
       /// </summary>
        public int? ResidentTypeId { get; set; }

        /// <summary>
        /// Дополнительная информация
        /// </summary>
        [System.Obsolete("Договорились не использовать", true)]
        public string Description { get; set; }
        /// <summary>
        /// Признак активности
        /// </summary>
        [System.Obsolete("Договорились не использовать", true)]
        public bool IsActive { get; set; }
       
    }
}
