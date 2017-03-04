using System.Runtime.Serialization;

namespace BL.Model.Common
{
    /// <summary>
    /// Фильт для списков Id Name
    /// </summary>
    public class BaseFilterName : BaseFilter
    {
        /// <summary>
        /// Сужение по наименованию (вхождение)
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Сужение по наименованию (равенство)
        /// </summary>
        [IgnoreDataMember]
        public string NameExact { get; set; }

    }

}
