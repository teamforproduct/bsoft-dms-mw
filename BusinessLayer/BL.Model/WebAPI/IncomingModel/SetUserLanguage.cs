using BL.Model.Database;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace BL.Model.WebAPI.IncomingModel
{
    public class SetUserLanguage
    {
        /// <summary>
        /// Код языка (ru_RU)
        /// </summary>
        [Required]
        public string LanguageCode { get; set; }
    }
}