using BL.Model.DocumentCore.InternalModel;

namespace BL.Model.FullTextSearch
{
    /// <summary>
    /// Information about the object which should be updated on FullText index
    /// </summary>
    public class FullTextUpdateCashInfo
    {
        /// <summary>
        /// Which server data should be updated
        /// </summary>
        public string ServerKey { get; set; }
        /// <summary>
        /// Document
        /// </summary>
        public InternalDocument Document { get; set; }
        /// <summary>
        /// Which part of document should be updated
        /// </summary>
        public EnumSearchObjectType PartType { get; set; }
        /// <summary>
        /// type of operation add/delete/update
        /// </summary>
        public EnumOperationType OperationType { get; set; }
    }
}