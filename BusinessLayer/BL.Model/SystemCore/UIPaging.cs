using System.Runtime.Serialization;

namespace BL.Model.SystemCore
{
    public class UIPaging
    {
        public int CurrentPage { get; set; } = 1;
        public int PageSize { get; set; } = 50;
        [IgnoreDataMember]
        public int TotalPageCount { get; set; }
    }
}