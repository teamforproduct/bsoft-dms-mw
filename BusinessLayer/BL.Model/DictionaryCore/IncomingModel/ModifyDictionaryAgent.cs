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

//       /// <summary>
//       /// Резидентность
//       /// </summary>
//        public int ResidentTypeId { get; set; }

        /// <summary>
        /// Дополнительная информация
        /// </summary>
        public string Description { get; set; }
        
        /// <summary>
        /// Список контактов
        /// </summary>
        public List<ModifyDictionaryContact> Contacts { get; set; }
    }
}
