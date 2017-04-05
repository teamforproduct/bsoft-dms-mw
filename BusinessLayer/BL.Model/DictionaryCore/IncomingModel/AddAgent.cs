using System.ComponentModel.DataAnnotations;

namespace BL.Model.DictionaryCore.IncomingModel
{
    /// <summary>
    /// Агент
    /// </summary>
    public class AddAgent
    {
        /// <summary>
        /// Краткое название/имя (отображается в интерфейсе как основное)
        /// </summary>
        [Required]
        public string Name { get; set; }

    }
}
