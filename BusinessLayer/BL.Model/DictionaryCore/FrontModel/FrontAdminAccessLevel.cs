using BL.Model.Common;

namespace BL.Model.DictionaryCore.FrontModel
{
    public class FrontAdminAccessLevel : LastChangeInfo
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
    }
}