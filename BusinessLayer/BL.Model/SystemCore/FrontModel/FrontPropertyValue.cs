using BL.Model.SystemCore.IncomingModel;

namespace BL.Model.SystemCore.FrontModel
{
    public class FrontPropertyValue : ModifyPropertyValue
    {
        public int Id { get; set; }

        public FrontPropertyLink PropertyLink { get; set; }
    }
}