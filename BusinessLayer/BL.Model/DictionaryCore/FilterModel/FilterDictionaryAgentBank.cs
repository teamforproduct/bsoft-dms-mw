using BL.Model.Common;
using System.Runtime.Serialization;

namespace BL.Model.DictionaryCore.FilterModel
{

    /// <summary>
    /// Фильр для контрагентов - банков
    /// </summary>
    public class FilterDictionaryAgentBank : BaseFilterNameIsActive
    {

        /// <summary>
        /// МФО
        /// </summary>
        public string MFOCode { get; set; }
        [IgnoreDataMember]
        public string MFOCodeExact { get; set; }
    }
}
