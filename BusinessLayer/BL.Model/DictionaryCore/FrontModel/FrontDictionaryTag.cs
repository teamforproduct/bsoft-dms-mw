using BL.Model.DictionaryCore.IncomingModel;

namespace BL.Model.DictionaryCore.FrontModel
{
    /// <summary>
    /// ОСНОВНОЙ. Справочник типов документов. 
    /// </summary>
    public class FrontDictionaryTag : ModifyDictionaryTag
    {
        /// <summary>
        /// ID
        /// </summary>
        public int Id { get; set; }
        public int? PositionId { get; set; }
        public bool IsSystem { get; set; }
        public string Color { get; set; }
        public string PositionName { get; set; }
    }
}