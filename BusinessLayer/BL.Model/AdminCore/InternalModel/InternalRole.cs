using BL.Model.Common;
using BL.Model.AdminCore.IncomingModel;

namespace BL.Model.AdminCore.InternalModel
{
    
    public class InternalAdminRole : LastChangeInfo
    {
        public InternalAdminRole()
        { }

        public InternalAdminRole(ModifyAdminRole model)
        {
            Id = model.Id;
            Name = model.Name;
            RoleTypeId = model.RoleTypeId;
        }

        /// <summary>
        /// ID
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Наименование роли
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Классификатор роли
        /// </summary>
        public int? RoleTypeId { get; set; }

    }
}