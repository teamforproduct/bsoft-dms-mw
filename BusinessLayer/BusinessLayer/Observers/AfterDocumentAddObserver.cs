using BL.CrossCutting.Interfaces;
using BL.Model.DocumentCore.InternalModel;
using BL.Model.Enums;

namespace BL.Logic.Observers
{
    public class AfterDocumentAddObserver: ICommandObserver
    {
        public EnumObserverType ObserverType { get { return EnumObserverType.After; } }

        public void Inform(IContext context, InternalDocument document, EnumDocumentActions previousAction, object previousParameter)
        {
            if (previousAction == EnumDocumentActions.AddDocument)
            {
                //do some code hire
            }
        }
    }
}