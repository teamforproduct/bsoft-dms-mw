using BL.Logic.Common;
using BL.Database.Documents.Interfaces;
using BL.Model.Enums;
using BL.Model.Exception;
using BL.CrossCutting.DependencyInjection;
using BL.Logic.SystemServices.AutoPlan;
using System.Linq;
using System.Collections.Generic;
using BL.Model.DocumentCore.IncomingModel;
using BL.Logic.DocumentCore.Interfaces;
using BL.CrossCutting.Helpers;

namespace BL.Logic.DocumentCore.Commands
{
    public class SendDocumentCommand : BaseDocumentCommand
    {
        private readonly IDocumentsDbProcess _documentDb;

        public SendDocumentCommand(IDocumentsDbProcess documentDb)
        {
            _documentDb = documentDb;
        }

        private List<ModifyDocumentSendList> Model
        {
            get
            {
                if (!(_param is List<ModifyDocumentSendList>))
                {
                    throw new WrongParameterTypeError();
                }
                return (List<ModifyDocumentSendList>)_param;
            }
        }

        public override bool CanBeDisplayed(int positionId)
        {
            if ((_document.Accesses?.Count() ?? 0) != 0 && !_document.Accesses.Any(x => x.PositionId == positionId && x.IsInWork))
                return false;
            return true;
        }

        public override bool CanExecute()
        {
            _admin.VerifyAccess(_context, CommandType);
            return true;
        }

        public override object Execute()
        {
            var docProc = DmsResolver.Current.Get<IDocumentService>();
            using (var transaction = Transactions.GetTransaction())
            {
                foreach (var sendList in Model)
                {
                    docProc.ExecuteAction(EnumDocumentActions.AddDocumentSendList, _context, sendList);
                }
                transaction.Complete();
            }
            return null;
        }

    }
}