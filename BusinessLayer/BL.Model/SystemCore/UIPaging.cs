using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace BL.Model.SystemCore
{
    public class UIPaging
    {
        public int CurrentPage { get; set; } = 1;
        public int PageSize { get; set; } = 50;
        [XmlIgnore]
        public int TotalPageCount { get; set; }
    }
}