namespace BL.Model.Common
{
    public class DictionaryItem : ListItem, IDictionaryItem
    {
        public string Description { get; set; }

        public bool? IsActive { get; set; }
    }
}
