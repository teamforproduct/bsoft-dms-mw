﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BL.Model.Enums;

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
        
        public virtual bool IsIndividual { get; set; }
        public virtual bool IsEmployee { get; set; }
        public virtual bool IsBank { get; set; }
        public virtual bool IsCompany { get; set; }

        /// <summary>
        /// Резидентность
        /// </summary>
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
