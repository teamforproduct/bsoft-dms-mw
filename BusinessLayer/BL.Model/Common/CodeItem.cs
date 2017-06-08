using System.ComponentModel.DataAnnotations;

namespace BL.Model.Common
{
    /// <summary>
    /// Id
    /// </summary>
    public class CodeItem
    {
        /// <summary>
        /// Code
        /// </summary>
        [Required]
        public string Code { get; set; }
    }
}
