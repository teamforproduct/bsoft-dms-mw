using System;
using BL.Model.Common;
using BL.Model.DictionaryCore.IncomingModel;

namespace BL.Model.DictionaryCore.InternalModel
{
    /// <summary>
    /// Контрагент - физическое лицо
    /// </summary>
    public class InternalDictionaryAgentUserPicture : LastChangeInfo
    {

        public InternalDictionaryAgentUserPicture()
        { }

        public InternalDictionaryAgentUserPicture(ModifyDictionaryAgentUserPicture model)
        {
            Id = model.Id;
        }

        /// <summary>
        /// ID
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Файл
        /// </summary>
        public byte[] Picture { get; set; }

    }
}
