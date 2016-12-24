using BL.Model.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Model.DictionaryCore.FrontModel
{
    /// <summary>
    /// Контрагент - физическое лицо
    /// </summary>
    public class FrontMainAgentPerson 
    {
        /// <summary>
        /// ID
        /// </summary>
        public int Id { get; set; }
        
        /// <summary>
        /// ИНН
        /// </summary>
        public string TaxCode { get; set; }
        
        /// <summary>
        /// Пол (true - мужской)
        /// </summary>
        public bool? IsMale { get; set; }

        /// <summary>
        /// Дата рождения
        /// </summary>
        public DateTime? BirthDate { get { return _BirthDate; } set { _BirthDate=value.ToUTC(); } }
        private DateTime?  _BirthDate;

        /// <summary>
        /// Должность (текстопое поле)
        /// </summary>
        public string Position { get; set; }

        /// <summary>
        /// Полное имя
        /// </summary>
        public string FullName { get; set; }

        /// <summary>
        /// Сокращенное имя
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Описание
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Активный
        /// </summary>
        public bool IsActive { get; set; }

    }
}
