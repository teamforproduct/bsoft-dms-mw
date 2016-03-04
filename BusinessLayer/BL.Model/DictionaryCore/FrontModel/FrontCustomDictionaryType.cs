using BL.Model.DictionaryCore.IncomingModel;
using System.Collections.Generic;

namespace BL.Model.DictionaryCore.FrontModel
{
    /// <summary>
    /// ДОПОЛНИТЕЛЬНЫЙ. Справочник типов дополнительных справочников. 
    /// </summary>
    public class FrontCustomDictionaryType : ModifyCustomDictionaryType
    {
        /// <summary>
        /// ID
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Значения словаря
        /// </summary>
        public IEnumerable<FrontCustomDictionary> CustomDictionaries { get; set; }
    }
}