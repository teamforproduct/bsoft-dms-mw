using BL.Model.Common;
using BL.Model.Extensions;
using System;

namespace BL.Model.DictionaryCore.FrontModel
{
    /// <summary>
    /// Результаты исполнения
    /// </summary>
    public class FrontDictionaryResultType : ListItem
    {
        /// <summary>
        /// Считать выполненным
        /// </summary>
        public bool IsExecute { get; set; }

    }
}