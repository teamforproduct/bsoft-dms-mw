using BL.Model.SystemCore;

namespace BL.Model.WebAPI.FrontModel
{
    public class FrontAspNetClientLicence : LicenceInfo
    {
        public int Id { get; set; }
        public bool IsActive { get; set; }
    }
}