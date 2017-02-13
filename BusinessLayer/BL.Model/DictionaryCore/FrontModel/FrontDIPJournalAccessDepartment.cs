namespace BL.Model.DictionaryCore.FrontModel
{
    /// <summary>
    /// 
    /// </summary>
    public class FrontDIPJournalAccessDepartment : FrontDIPJournalAccessBase
    {

        /// <summary>
        /// Код подразделения
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// Исполнитель
        /// </summary>
        public int? DepartmentId { get; set; }
        
    }
}