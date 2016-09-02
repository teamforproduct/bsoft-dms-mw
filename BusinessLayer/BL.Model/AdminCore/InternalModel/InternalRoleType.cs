using BL.Model.Common;
using BL.Model.AdminCore.IncomingModel;

namespace BL.Model.AdminCore.InternalModel
{
    
    public class InternalAdminRoleType : LastChangeInfo
    {

        /// <summary>
        /// ID
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// идентификатор классификатора ролей
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// наименование классификатора ролей
        /// </summary>
        public string Name { get; set; }

    }
}