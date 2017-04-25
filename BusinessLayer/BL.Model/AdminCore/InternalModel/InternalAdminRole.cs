using BL.Model.Common;
using BL.Model.AdminCore.IncomingModel;

namespace BL.Model.AdminCore.InternalModel
{
    
    public class InternalAdminRole : LastChangeInfo
    {
        public InternalAdminRole()
        { }

        public InternalAdminRole(AddAdminRole model)
        {
            SetInternalAdminRole(model);
        }

        public InternalAdminRole(ModifyAdminRole model)
        {
            Id = model.Id;
            SetInternalAdminRole(model);
        }

        private void SetInternalAdminRole(AddAdminRole model)
        {
            Name = model.Name;
            Description = model.Description;
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
        /// Описание
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Классификатор роли
        /// </summary>
        public int? RoleTypeId { get; set; }

        /// <summary>
        /// ИД клиента
        /// </summary>
        public int ClientId { get; set; }
    }
}