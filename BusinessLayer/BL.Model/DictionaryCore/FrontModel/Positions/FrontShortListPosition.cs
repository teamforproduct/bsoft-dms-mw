using BL.Model.Common;

namespace BL.Model.DictionaryCore.FrontModel
{
    /// <summary>
    /// Карточка элемента из справочника "Штатное расписание". 
    /// </summary>
    public class FrontShortListPosition : ListItem
    {

        /// <summary>
        /// Код подразделения
        /// </summary>
        public string DepartmentCodePath { get; set; }
        /// <summary>
        /// Наименование подразделения
        /// </summary>
        public string DepartmentName { get; set; }
        /// <summary>
        /// Наименование компании
        /// </summary>
        public string CompanyName { get; set; }

        /// <summary>
        /// Имя исполнителя
        /// </summary>
        public string ExecutorName { get; set; }

        /// <summary>
        /// Тип исполнения (приставка)
        /// </summary>
        public string ExecutorTypeSuffix { get; set; }

    }
}