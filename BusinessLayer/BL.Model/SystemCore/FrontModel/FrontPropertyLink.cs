using BL.Model.SystemCore.IncomingModel;

namespace BL.Model.SystemCore.FrontModel
{
    public class FrontPropertyLink : ModifyPropertyLink
    {
        public int Id { get; set; }
        
        public FrontProperty Property { get; set; }

        public FrontSystemObject Object { get; set; }
    }
}