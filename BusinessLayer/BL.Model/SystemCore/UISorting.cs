using BL.Model.Enums;

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

        public EnumSort SortPrimary { get; set; }
        public bool SortPrimaryAsc { get; set; }
        public EnumSort SortSecondary { get; set; }
        public bool SortSecondaryAsc { get; set; }
        public EnumSort SortThird { get; set; }
        public bool SortThirdAsc { get; set; }
    }
}
