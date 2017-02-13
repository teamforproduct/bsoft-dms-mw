using BL.Model.Tree;

namespace BL.Model.DictionaryCore.FrontModel
{
    /// <summary>
    /// 
    /// </summary>
    // 
    public class FrontDIPJournalAccessBase: TreeItem
    {

        /// <summary>
        /// Id журнала
        /// </summary>
        public int? JournalId { get; set; }

        /// <summary>
        /// Для просмотра
        /// </summary>
        public int? IsView { get; set; }

        /// <summary>
        /// Для регистрации
        /// </summary>
        public int? IsRegistration { get; set; }

    }
}