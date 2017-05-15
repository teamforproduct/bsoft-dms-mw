using BL.Model.Common;
using BL.Model.DictionaryCore.IncomingModel;

namespace BL.Model.DictionaryCore.InternalModel
{
    /// <summary>
    /// Internal элемент справочника "Штатное расписание"
    /// </summary>
    public class InternalDictionaryPositionExecutorForDocument
    {
        /// <summary>
        /// ID
        /// </summary>
        public int PositionId { get; set; }        
        /// <summary>
        /// Исполняющий обязанности, значения проставляются вертушкой
        /// </summary>
        public int? ExecutorAgentId { get; set; }
        public int? ExecutorTypeId { get; set; }
        /// <summary>
        /// Сотрудник на должности, значения проставляются вертушкой
        /// </summary>
        public int? MainExecutorAgentId { get; set; }



    }
}