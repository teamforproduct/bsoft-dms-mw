using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

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
        public string Name { get; set; }

       /// <summary>
       /// Резидентность
       /// </summary>
        public int? ResidentTypeId { get; set; }

        /// <summary>
        /// Дополнительная информация
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// Признак активности
        /// </summary>
        public bool IsActive { get; set; }
        /// <summary>
        /// является юрлицом
        /// </summary>
        public bool IsCompany { get; set; }
        /// <summary>
        /// является физлицом
        /// </summary>
        public bool IsIndividual { get; set; }
        /// <summary>
        /// является сотрудником
        /// </summary>
        public bool IsEmployee { get; set; }
        /// <summary>
        /// является банком
        /// </summary>
        public bool IsBank { get; set; }
    }
}
