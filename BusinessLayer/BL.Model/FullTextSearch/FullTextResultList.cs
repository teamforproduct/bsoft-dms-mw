using BL.Model.Enums;

namespace BL.Model.FullTextSearch
{
    public class FullTextResultList
    {
        /// <summary>
        /// how often we should update indexes
        /// </summary>
        public int ObjectId { get; set; }

        /// <summary>
        /// Key for searching DB
        /// </summary>
        public EnumObjects ObjectType { get; set; }

    }
}