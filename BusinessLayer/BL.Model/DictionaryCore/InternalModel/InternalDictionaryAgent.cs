using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BL.Model.Common;

namespace BL.Model.DictionaryCore.InternalModel
{
    /// <summary>
    /// контрагент
    /// </summary>
    public class InternalDictionaryAgent : LastChangeInfo
    {
        /// <summary>
        /// ИД
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// Имя/наименование
        /// </summary>
        public string Name { get; set; }
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
        /// <summary>
        /// признак активности
        /// </summary>
        public bool IsActive { get; set; }
        /// <summary>
        /// резидентность
        /// </summary>
        public int ResidentTypeId { get; set; }
        /// <summary>
        /// комментарии
        /// </summary>
        public string Description { get; set; }
    }
}
