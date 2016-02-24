using BL.Model.DocumentCore.InternalModel;
using BL.Model.Enums;

namespace BL.CrossCutting.Interfaces.Observers
{
    public interface ICommandObserver
    {
        EnumCommandType ObserverType { get; }
        void Inform(IContext context, InternalDocument document, EnumDocumentActions previousAction, object previousParameter);
    }
}