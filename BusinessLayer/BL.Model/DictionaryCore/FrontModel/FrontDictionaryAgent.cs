using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BL.Model.Enums;
using System.Runtime.Serialization;

namespace BL.Model.DictionaryCore.FrontModel
{

    /// <summary>
    /// ОСНОВНОЙ. Справочник контрагентов
    /// </summary>
    public class FrontDictionaryAgent
    {
        /// <summary>
        /// ИД
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// ФИО, Название, ...
        /// </summary>
        public string Name { get; set; }
        
        /// <summary>
        /// Список контактов контрагента
        /// </summary>
        public IEnumerable<FrontDictionaryContact> Contacts { get; set; }
        /// <summary>
         /// Список адресов контрагента
         /// </summary>
        public IEnumerable<FrontDictionaryAgentAddress> Addresses { get; set; }
        /// <summary>
        /// является физлицом
        /// </summary>
        [IgnoreDataMember]
        [Obsolete("Признак не используется", true)]
        public virtual bool IsIndividual { get; set; }
        /// <summary>
        /// является сотрудником
        /// </summary>
        [IgnoreDataMember]
        [Obsolete("Признак не используется", true)]
        public virtual bool IsEmployee { get; set; }
        /// <summary>
        /// является банком
        /// </summary>
        [IgnoreDataMember]
        [Obsolete("Признак не используется", true)]
        public virtual bool IsBank { get; set; }
        /// <summary>
        /// является юрлицом
        /// </summary>
        [IgnoreDataMember]
        [Obsolete("Признак не используется", true)]
        public virtual bool IsCompany { get; set; }

        /// <summary>
        /// Резидентность
        /// </summary>
        [IgnoreDataMember]
        public int? ResidentTypeId { get; set; }
        /// <summary>
        /// Дополнительная информация
        /// </summary>
        public virtual string Description { get; set; }
        /// <summary>
        /// Признак активности
        /// </summary>
        public virtual bool IsActive { get; set; }
    }
}
