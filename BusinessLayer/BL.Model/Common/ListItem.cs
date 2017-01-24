namespace BL.Model.Common
{
    /// <summary>
    /// Элемент простого списка
    /// </summary>
    public class ListItem : Item, IListItem
    {
        /// <summary>
        /// Наименование
        /// </summary>
        public string Name { get; set; }
    }
}
