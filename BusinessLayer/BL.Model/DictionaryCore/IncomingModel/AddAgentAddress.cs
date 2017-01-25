using System.ComponentModel.DataAnnotations;

namespace BL.Model.DictionaryCore.IncomingModel
{
    public class AddAgentAddress : BaseAgentAddress
    {

        public AddAgentAddress() { }

        public AddAgentAddress(BaseAgentAddress model)
        {
            AddressTypeId = model.AddressTypeId;
            PostCode = model.PostCode;
            Address = model.Address;
            IsActive = model.IsActive;
            Description = model.Description;
        }
        /// <summary>
        /// Агент (сотрудник, юр.лицо, банк, физ.лицо)
        /// </summary>
        [Required]
        public int AgentId { get; set; }
    }
    /// <summary>
    /// адреса контрагентов
    /// </summary>
    public class BaseAgentAddress
    {
        /// <summary>
        /// ссылка на тип адреса
        /// </summary>
        [Required]
        public int AddressTypeId { get; set; }

        /// <summary>
        /// индекс
        /// </summary>
        public string PostCode { get; set; }

        /// <summary>
        /// адрес
        /// </summary>
        [Required]
        public string Address { get; set; }

        /// <summary>
        /// комментарии
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// признак активности
        /// </summary>
        [Required]
        public bool IsActive { get; set; }
    }
}
