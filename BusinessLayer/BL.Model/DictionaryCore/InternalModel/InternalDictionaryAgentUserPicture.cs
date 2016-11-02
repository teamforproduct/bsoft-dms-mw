using System;
using BL.Model.Common;
using BL.Model.DictionaryCore.IncomingModel;

namespace BL.Model.DictionaryCore.InternalModel
{
    /// <summary>
    /// Контрагент - физическое лицо
    /// </summary>
    public class InternalDictionaryAgentImage : LastChangeInfo
    {

        public InternalDictionaryAgentImage()
        { }

        public InternalDictionaryAgentImage(ModifyDictionaryAgentImage model)
        {
            Id = model.AgentId;
        }

        /// <summary>
        /// ID
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Файл
        /// </summary>
        public byte[] Image { get; set; }

    }
}
