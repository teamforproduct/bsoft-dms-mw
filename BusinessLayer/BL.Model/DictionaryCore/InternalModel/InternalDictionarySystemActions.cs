using BL.Model.Enums;

namespace BL.Model.DictionaryCore.InternalModel
{
    public class InternalDictionarySystemActions
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public EnumObjects Object { get; set; }
        public bool IsGrantable { get; set; }
        public bool IsGrantableByRecordId { get; set; }
        public int? GrantId { get; set; }
        public bool IsVisible { get; set; }
    }
}