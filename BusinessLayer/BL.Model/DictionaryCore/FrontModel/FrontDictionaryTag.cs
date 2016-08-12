using System;
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
        public int LastChangeUserId { get; set; }
        public DateTime LastChangeDate { get; set; }
        public string LastChangeUserName { get; set; }
        public int? DocCount { get; set; }

    }
}