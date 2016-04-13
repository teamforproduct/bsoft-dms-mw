﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BL.Model.Enums;

namespace BL.Model.DictionaryCore.FrontModel
{
    /// <summary>
    /// Типы контрагентов. Модель пока не используется
    /// </summary>
    public class FrontDictionaryAgentType
    {
        /// <summary>
        /// ИД
        /// </summary>
        public EnumDictionaryAgentTypes Id { get; set; }
        /// <summary>
        /// Тип контакта
        /// </summary>
        public string Name { get; set; }
        

    }
}