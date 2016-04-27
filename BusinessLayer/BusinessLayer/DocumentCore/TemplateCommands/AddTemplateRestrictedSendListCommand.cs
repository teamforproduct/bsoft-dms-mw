﻿
using BL.Database.Documents.Interfaces;
using BL.Database.FileWorker;
using BL.Logic.Common;
using BL.Model.DocumentCore.IncomingModel;
using BL.Model.Exception;

namespace BL.Logic.DocumentCore.TemplateCommands
{
    public class AddTemplateRestrictedSendListCommand : BaseDocumentCommand
    {
        private readonly ITemplateDocumentsDbProcess _operationDb;


        public AddTemplateRestrictedSendListCommand(ITemplateDocumentsDbProcess operationDb)
        {
            _operationDb = operationDb;

        }

        private ModifyTemplateDocumentRestrictedSendLists Model
        {
            get
            {
                if (!(_param is ModifyTemplateDocumentRestrictedSendLists))
                {
                    throw new WrongParameterTypeError();
                }
                return (ModifyTemplateDocumentRestrictedSendLists)_param;
            }
        }

        public override bool CanBeDisplayed(int positionId)
        {
            return true;
        }

        public override bool CanExecute()
        {
            _admin.VerifyAccess(_context, CommandType, false);


            return true;
        }

        public override object Execute()
        {
            CommonDocumentUtilities.SetLastChange(_context, Model);
            return _operationDb.AddOrUpdateTemplateRestrictedSendList(_context, Model);

        }


    }
}