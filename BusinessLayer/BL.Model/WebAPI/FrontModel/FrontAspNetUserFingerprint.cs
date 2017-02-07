using BL.Model.WebAPI.IncomingModel;

namespace BL.Model.WebAPI.FrontModel
{
    public class FrontAspNetUserFingerprint  : ModifyAspNetUserFingerprint
    {
        public new string Browser { get; set; }
        public new string Platform { get; set; }
    }
}