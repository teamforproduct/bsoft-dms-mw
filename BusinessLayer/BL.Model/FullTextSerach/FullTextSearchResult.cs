namespace BL.Model.FullTextSerach
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
        /// Type if document part, where text was found (i.e. document, event etc.)
        /// </summary>
        public EnumSearchObjectType ObjectType { get; set; }
        /// <summary>
        /// ID of document part. 0 if it was document itself. 
        /// </summary>
        public int ObjectId { get; set; }
    }
}