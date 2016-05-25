using BL.Model.SystemCore.IncomingModel;

namespace BL.Model.SystemCore.FrontModel
{
    public class FrontPropertyValue
    {
        public int PropertyLinkId { get; set; }
        public object Value { get; set; }
        public object DisplayValue { get; set; }

        public string PropertyCode { get; set; }
        public string PropertyLabel { get; set; }
        public string PropertyValueTypeCode { get; set; }
    }
}