using BL.Model.Common;
using BL.Model.DictionaryCore.IncomingModel;
using BL.Model.Enums;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace BL.Model.DictionaryCore.FrontModel
{
    /// <summary>
    /// Для отображения списка должностей с основными (назначен на должность)
    /// </summary>
    public class FrontPositionExecutorList : ListItem
    {

        /// <summary>
        /// Код подразделения
        /// </summary>
        public int? ExecutorId { get; set; }

        /// <summary>
        /// Код подразделения
        /// </summary>
        public string ExecutorName { get; set; }


        /// <summary>
        /// Код подразделения
        /// </summary>
        public string ExecutorTypeSuffix { get; set; }

    }
}