using System.Collections.Generic;

namespace BL.Model.Common
{
    /// <summary>
    /// Элемент автокомплита
    /// </summary>
    public class AutocompleteItem : ListItem, IListItem
    {
        /// <summary>
        /// Детали
        /// </summary>
        public List<string> Details { get; set; }
    }
}
