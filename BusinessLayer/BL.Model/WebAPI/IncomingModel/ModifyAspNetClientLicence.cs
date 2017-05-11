using System.Runtime.Serialization;

namespace BL.Model.WebAPI.IncomingModel
{
    public class ModifyAspNetClientLicence
    {
        [IgnoreDataMember]
        public int Id { get; set; }
        public bool IsActive { get; set; }
    }
}