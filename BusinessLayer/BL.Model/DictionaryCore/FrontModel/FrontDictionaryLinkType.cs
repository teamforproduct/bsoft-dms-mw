using BL.Model.Common;
using BL.Model.Extensions;
using System;

namespace BL.Model.DictionaryCore.FrontModel
{

    /// <summary>
    /// Типы связей документов
    /// </summary>
    public class FrontDictionaryLinkType : ListItem
    {
        /// <summary>
        /// Является важной
        /// </summary>
        public bool IsImportant { get; set; }
    }
}