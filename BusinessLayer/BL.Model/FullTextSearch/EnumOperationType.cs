namespace BL.Model.FullTextSearch
{
    /// <summary>
    /// Possible opearations with document, which should be processed for FullText Index. 
    /// </summary>
    public enum EnumOperationType
    {
        /// <summary>
        /// Add new element, we should add new element to index
        /// </summary>
        AddNew = 0,

        /// <summary>
        /// update exist elemen, we should update index
        /// </summary>
        Update = 1,

        /// <summary>
        /// delete element. We shoould remove it from index
        /// </summary>
        Delete = 2,

        /// <summary>
        /// add all information about document (include SendLists, Files etc.)
        /// </summary>
        AddFull = 5,

        /// <summary>
        /// update all information about document (include SendLists, Files etc.)
        /// </summary>
        UpdateFull = 3,

    }
}