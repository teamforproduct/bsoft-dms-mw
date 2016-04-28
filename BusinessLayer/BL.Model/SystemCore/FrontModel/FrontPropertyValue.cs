using BL.Model.SystemCore.IncomingModel;

namespace BL.Model.SystemCore.FrontModel
{
    public class FrontPropertyValue
    {
        public int Id { get; set; }

        public int PropertyLinkId { get; set; }
        public string Value { get; set; }
        public string DisplayValue { get; set; }

        public string PropertyCode { get; set; }
        public string PropertyLabel { get; set; }
    }
}