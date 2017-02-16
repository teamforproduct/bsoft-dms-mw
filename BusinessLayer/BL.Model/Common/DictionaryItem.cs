namespace BL.Model.Common
{
    public class DictionaryItem : ListItem, IDictionaryItem
    {
        /// <summary>
        /// Описание
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Признак активности элемента
        /// </summary>
        public bool? IsActive { get; set; }
    }
}
