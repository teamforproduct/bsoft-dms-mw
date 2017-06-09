using BL.CrossCutting.Interfaces;
using BL.Model.DocumentCore.InternalModel;
using BL.Model.Enums;

namespace BL.Logic.Observers
{
    public class AfterDocumentAddObserver: ICommandObserver
    {
        public EnumObserverType ObserverType => EnumObserverType.After;

        public void Inform(IContext context, InternalDocument document, EnumActions previousAction, object previousParameter)
        {
            if (previousAction == EnumActions.AddDocument)
            {
                //do some code hire
            }
        }
    }
}