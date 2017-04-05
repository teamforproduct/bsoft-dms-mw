using System.ComponentModel.DataAnnotations;

namespace BL.Model.DictionaryCore.IncomingModel
{
    public class AddAgentBank
    {
        /// <summary>
        /// Краткое название (отображается в интерфейсе как основное)
        /// </summary>
        [Required]
        public string Name { get; set; }
        
        /// <summary>
        /// Полное наименование банка
        /// </summary>
        [Required]
        public string FullName { get; set; }

        /// <summary>
        /// МФО
        /// </summary>
        [Required]
        public string MFOCode { get; set; }

        /// <summary>
        /// Код Свифт
        /// </summary>
        public string Swift { get; set; }

        /// <summary>
        /// Комментарии
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Признак активности
        /// </summary>
        [Required]
        public bool IsActive { get; set; }

    }
}
