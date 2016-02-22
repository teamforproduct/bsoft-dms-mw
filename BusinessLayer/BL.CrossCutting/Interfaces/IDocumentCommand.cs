using BL.Model.DocumentCore.InternalModel;
using BL.Model.Enums;

namespace BL.CrossCutting.Interfaces
{
    public interface IDocumentCommand:ICommand
    {
        void InitializeCommand(IContext ctx, InternalDocument doc);
        void InitializeCommand(IContext ctx, InternalDocument doc, object param);

        EnumDocumentActions CommandType { get; }
    }
}