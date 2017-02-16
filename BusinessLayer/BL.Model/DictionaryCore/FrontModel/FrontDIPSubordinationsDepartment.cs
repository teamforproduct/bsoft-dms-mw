namespace BL.Model.DictionaryCore.FrontModel
{
    /// <summary>
    /// 
    /// </summary>
    public class FrontDIPSubordinationsDepartment: FrontDIPSubordinationsBase
    {

        /// <summary>
        /// Код подразделения
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// Руководитель
        /// </summary>
        public int? SourcePositionId { get; set; }

        /// <summary>
        /// Исполнитель
        /// </summary>
        public int? DepartmentId { get; set; }
        
    }
}