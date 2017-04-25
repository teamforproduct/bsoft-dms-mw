namespace BL.Model.WebAPI.IncomingModel
{
    public class AddAspNetClient
    {
        public ModifyAspNetUser Admin { get; set; }
        public ModifyAspNetClient Client { get; set; }
        public ModifyAdminServer Server { get; set; }
        public int? LicenceId { get; set; }
    }
}