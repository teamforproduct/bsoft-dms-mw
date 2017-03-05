using BL.Model.Enums;
using System.Runtime.Serialization;

namespace BL.Model.Common
{
    public class UISorting
    {
        public UISorting()
        {
            SortPrimaryAsc = true;
            SortSecondaryAsc = true;
            SortThirdAsc = true;
        }

        [IgnoreDataMember]
        public EnumSort SortPrimary { get; set; }
        [IgnoreDataMember]
        public bool SortPrimaryAsc { get; set; }
        [IgnoreDataMember]
        public EnumSort SortSecondary { get; set; }
        [IgnoreDataMember]
        public bool SortSecondaryAsc { get; set; }
        [IgnoreDataMember]
        public EnumSort SortThird { get; set; }
        [IgnoreDataMember]
        public bool SortThirdAsc { get; set; }
    }
}
