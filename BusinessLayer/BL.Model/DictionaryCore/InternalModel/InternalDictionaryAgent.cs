using BL.Model.Common;
using BL.Model.DictionaryCore.IncomingModel;

namespace BL.Model.DictionaryCore.InternalModel
{
    /// <summary>
    /// контрагент
    /// </summary>
    public class InternalDictionaryAgent : LastChangeInfo
    {
        
        public InternalDictionaryAgent()
        { }

        public InternalDictionaryAgent(ModifyDictionaryAgent model)
        {
            Id = model.Id;
            Name = model.Name;
        }


        /// <summary>
        /// ИД
        /// </summary>
        public int Id { get; set; }
        
        /// <summary>
        /// Имя/наименование (используется в интерфейсе)
        /// </summary>
        public string Name { get; set; }
    }
}
