using System.Runtime.Serialization;

namespace BL.Model.WebAPI.IncomingModel
{
    public class ModifyAspNetClient
    {
        [IgnoreDataMember]
        public int Id { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public int LanguageId { get; set; }
    }
}