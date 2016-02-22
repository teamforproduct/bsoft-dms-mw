using BL.CrossCutting.Common;
using BL.Model.Enums;

namespace BL.Logic.DocumentCore.Commands
{
    public class SendMessageDocumentCommand: BaseDocumentCommand
    {
        public override bool CanBeDisplayed()
        {
            throw new System.NotImplementedException();
        }

        public override bool CanExecute()
        {
            throw new System.NotImplementedException();
        }

        public override object Execute()
        {
            throw new System.NotImplementedException();
        }

        public override EnumDocumentActions CommandType { get { return EnumDocumentActions.SendMessage; } }
    }
}