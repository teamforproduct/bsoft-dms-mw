using BL.Model.Common;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace BL.Model.DictionaryCore.FrontModel
{
    public class FrontMainDictionaryStandartSendList : ListItem
    {

        /// <summary>
        /// Исполнитель
        /// </summary>
        public int? ExecutorId { get; set; }

        /// <summary>
        /// Исполнитель
        /// </summary>
        public string ExecutorName { get; set; }


        /// <summary>
        /// Отдел
        /// </summary>
        public string DepartmentName { get; set; }

        /// <summary>
        /// Отдел. Код
        /// </summary>
        public string DepartmentIndex { get; set; }

        /// <summary>
        /// Тип исполнения
        /// </summary>
        public string ExecutorTypeSuffix { get; set; }

        public virtual IEnumerable<FrontDictionaryStandartSendList> SendLists { get; set; }
    }
}
