using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BL.Model.Common;

namespace BL.Model.DictionaryCore.InternalModel
{
    /// <summary>
    /// тип контакта
    /// </summary>
    public class InternalDictionaryContactType : LastChangeInfo
    {
     
        /// <summary>
        /// ИД
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// Наименование
        /// </summary>
        public string Name { get; set; }
   
        /// <summary>
        /// Маска для ввода
        /// </summary>
        public string InputMask { get; set; }
        /// <summary>
        /// признак активности
        /// </summary>
        public bool IsActive { get; set; }
    }
}
