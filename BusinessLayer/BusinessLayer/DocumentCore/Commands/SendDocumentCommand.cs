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
using System;

namespace BL.Logic.DocumentCore.Commands
{
    public class SendDocumentCommand : BaseDocumentCommand
    {
        private readonly IDocumentsDbProcess _documentDb;

        public SendDocumentCommand(IDocumentsDbProcess documentDb)
        {
            _documentDb = documentDb;
        }

        private List<AddDocumentSendList> Model
        {
            get
            {
                if (!(_param is List<AddDocumentSendList>))
                {
                    throw new WrongParameterTypeError();
                }
                return (List<AddDocumentSendList>)_param;
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
            _adminProc.VerifyAccess(_context, CommandType);
            return true;
        }

        public override object Execute()
        {
            Dictionary<int, string> res = new Dictionary<int, string>();
            var docProc = DmsResolver.Current.Get<IDocumentService>();
            using (var transaction = Transactions.GetTransaction())
            {
                for (var i = 0; i < Model.Count; i++)
                {
                    try
                    {
                        docProc.ExecuteAction(EnumDocumentActions.AddDocumentSendList, _context, Model[i]);
                    }
                    catch (Exception ex)
                    {
                        res.Add(i, ((ex is DmsExceptions) ? "##l@DmsExceptions:" + ex.GetType().Name + "@l##" : ex.Message));
                        break;
                    }
                }
                if (!res.Any())
                    transaction.Complete();
            }
            return res;
        }

    }
}