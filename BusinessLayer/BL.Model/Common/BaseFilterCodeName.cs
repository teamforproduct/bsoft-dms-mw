using System.Collections.Generic;
using System.Runtime.Serialization;

namespace BL.Model.Common
{
    /// <summary>
    /// Фильт для списков Id Name
    /// </summary>
    public class BaseFilterCodeName : BaseFilterName
    {
        /// <summary>
        /// Сужение по ключу (вхождение)
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// Сужение по ключу (равенство)
        /// </summary>
        [IgnoreDataMember]
        public string CodeExact { get; set; }

    }

}
