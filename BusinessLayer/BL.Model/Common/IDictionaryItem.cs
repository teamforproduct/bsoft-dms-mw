namespace BL.Model.Common
{
    public interface IDictionaryItem : IListItem
    {
        bool? IsActive { get; set; }
        string Description { get; set; }
    }

}
