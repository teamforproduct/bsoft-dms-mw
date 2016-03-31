namespace BL.Model.FullTextSearch
{
    /// <summary>
    /// Describe the result of Fulltext search
    /// </summary>
    public class FullTextSearchResult
    {
        /// <summary>
        /// Document ID
        /// </summary>
        public int DocumentId { get; set; }
        /// <summary>
        /// Type if document part, where text was found (i.e. Document, event etc.)
        /// </summary>
        public EnumSearchObjectType ObjectType { get; set; }
        /// <summary>
        /// ID of document part. 0 if it was Document itself. 
        /// </summary>
        public int ObjectId { get; set; }

        /// <summary>
        /// Score in FullText search result
        /// </summary>
        public float Score { get; set; }
    }
}