using BL.Model.SystemCore.IncomingModel;

namespace BL.Model.SystemCore.FrontModel
{
    public class FrontPropertyValue : ModifyPropertyValue
    {
        public int Id { get; set; }

        public int PropertyLinkId { get; set; }
        public int RecordId { get; set; }
        public string Value { get; set; }

        public FrontPropertyLink PropertyLink { get; set; }

        public int PropertyId { get; set; }
        public int ObjectId { get; set; }
        public string Filers { get; set; }
        public bool IsMandatory { get; set; }

        public FrontProperty Property { get; set; }

        public string PropertyCode { get; set; }
        public string PropertyDescription { get; set; }
        public string PropertyLabel { get; set; }
        public string PropertyHint { get; set; }
        public int? PropertyValueTypeId { get; set; }
        public string PropertyOutFormat { get; set; }
        public string PropertyInputFormat { get; set; }
        public string PropertySelectAPI { get; set; }
        public string PropertySelectFilter { get; set; }
        public string PropertySelectFieldCode { get; set; }
        public string PropertySelectDescriptionFieldCode { get; set; }

        public string PropertyValueTypeCode { get; set; }
        public string PropertyValueTypeDescription { get; set; }
    }
}