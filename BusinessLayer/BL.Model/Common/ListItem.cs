namespace BL.Model.Common
{
    /// <summary>
    /// Элемент простого списка
    /// </summary>
    public class ListItem : IListItem
    {
        /// <summary>
        /// Id
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Наименование
        /// </summary>
        public string Name { get; set; }
    }
}
