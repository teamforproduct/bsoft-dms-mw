using BL.Model.DocumentCore.InternalModel;
using BL.Model.Enums;

namespace BL.CrossCutting.Interfaces
{
    public interface ICommandObserver
    {
        EnumObserverType ObserverType { get; }
        void Inform(IContext context, InternalDocument document, EnumActions previousAction, object previousParameter);
    }
}