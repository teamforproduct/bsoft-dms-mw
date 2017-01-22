using System.ComponentModel.DataAnnotations;

namespace BL.Model.Common
{
    /// <summary>
    /// Id + Сheck
    /// </summary>
    public class ItemCheck : Item
    {
        /// <summary>
        /// Id
        /// </summary>
        [Required]
        public bool IsChecked { get; set; }
    }
}
