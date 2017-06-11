
namespace BL.Database.DBModel.System
{
    using global::System.ComponentModel.DataAnnotations;
    using global::System.ComponentModel.DataAnnotations.Schema;

    public class SystemMenus
    {
        public int Id { get; set; }
        public int MenuTypeId { get; set; }
        public int ActionId { get; set; }
        public int Order { get; set; }
        [ForeignKey("ActionId")]
        public virtual SystemActions Action { get; set; }

    }
}
