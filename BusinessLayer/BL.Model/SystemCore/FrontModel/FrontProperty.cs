using BL.Model.SystemCore.IncomingModel;
using BL.Model.SystemCore.InternalModel;

namespace BL.Model.SystemCore.FrontModel
{
    public class FrontProperty: ModifyProperty
    {
        public int Id { get; set; }

        public InternalSystemValueType ValueType { get; set; }
    }
}