namespace BL.Model.FullTextSearch
{
    /// <summary>
    /// Describe structure of parameters for the full text search
    /// </summary>
    public class FullTextSettings
    {
        /// <summary>
        /// Path where full text indexes will be stored
        /// </summary>
        public string StorePath { get; set; }
        /// <summary>
        /// how often we should update indexes
        /// </summary>
        public int TimeToUpdate { get; set; }

        /// <summary>
        /// Key for searching DB
        /// </summary>
        public string DatabaseKey { get; set; }

        /// <summary>
        /// Indixate when full text database was initialized for current server and client
        /// </summary>
        public bool IsFullTextInitialized { get; set; }
    }
}