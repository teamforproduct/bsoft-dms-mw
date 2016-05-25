namespace BL.Model.ClearTrashDocuments
{
    /// <summary>
    /// Setting for clear trash documents service
    /// </summary>
    public class ClearTrashDocumentsSettings
    {
        /// <summary>
        /// how often we should check documents to start clear trash documents
        /// </summary>
        public int TimeToUpdate { get; set; }

        public int TimeForClearTrashDocuments { get; set; }

        /// <summary>
        /// Key for searching DB
        /// </summary>
        public string DatabaseKey { get; set; }
    }
}