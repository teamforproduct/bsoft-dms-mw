namespace BL.Model.FullTextSerach
{
    /// <summary>
    /// Model that describe one elements in FullText index and operation with it.
    /// </summary>
    public class FullTextIndexIem
    {
        
        /// <summary>
        /// ID record in DB table
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// ID of document
        /// </summary>
        public int DocumentId { get; set; }
        /// <summary>
        /// Which part of document should be updated
        /// </summary>
        public EnumSearchObjectType ItemType { get; set; }
        /// <summary>
        /// type of operation add/delete/update
        /// </summary>
        public EnumOperationType OperationType { get; set; }

        /// <summary>
        /// ID of the document part, which is indexed.
        /// </summary>
        public int ObjectId { get; set; }

        /// <summary>
        /// Text which should be added to full text search
        /// </summary>
        public string ObjectText { get; set; }
    }
}