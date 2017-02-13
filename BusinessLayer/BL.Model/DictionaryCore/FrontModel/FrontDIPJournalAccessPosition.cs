namespace BL.Model.DictionaryCore.FrontModel
{
    /// <summary>
    /// 
    /// </summary>
    // 
    public class FrontDIPJournalAccessPosition : FrontDIPJournalAccessBase
    {

        /// <summary>
        /// Сотрудник на должности
        /// </summary>
        public string ExecutorName { get; set; }

        /// <summary>
        /// Тип исполнения должности
        /// </summary>
        public string ExecutorTypeSuffix { get; set; }

        /// <summary>
        /// Id должности
        /// </summary>
        public int? PositionId { get; set; }
        
        /// <summary>
        /// Порядок
        /// </summary>
        public int? Order { get; set; }
    }
}