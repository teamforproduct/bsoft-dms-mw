namespace BL.Model.DictionaryCore.FrontModel
{
    /// <summary>
    /// 
    /// </summary>
    // 
    public class FrontDIPSubordinationsPosition: FrontDIPSubordinationsBase
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
        /// Руководитель
        /// </summary>
        public int? SourcePositionId { get; set; }

        /// <summary>
        /// Исполнитель
        /// </summary>
        public int? TargetPositionId { get; set; }
        
        /// <summary>
        /// Исполнитель
        /// </summary>
        public int? Order { get; set; }
    }
}