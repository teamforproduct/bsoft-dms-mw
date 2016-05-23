using BL.Model.Database;
using BL.Model.SystemCore;
using System;

namespace BL.Model.WebAPI.FrontModel
{
    public class FrontAspNetClientLicence : LicenceInfo
    {
        public int Id { get; set; }
        public bool IsActive { get; set; }
        public bool IsTrial { get; set; }
    }
}