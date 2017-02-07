using BL.Model.Common;

namespace BL.Model.DictionaryCore.FrontModel
{
    /// <summary>
    /// Карточка элемента из справочника "Штатное расписание". 
    /// </summary>
    public class FrontDictionaryPositionExecutorType : ListItem
    {
        /// <summary>
        /// Код
        /// </summary>
        public string Code { get; set; }

    }
}