using System;
using BL.Database.Documents.Interfaces;
using BL.Model.DocumentCore.Actions;
using BL.Model.DocumentCore.InternalModel;
using BL.Model.Enums;
using BL.Model.Exception;
using BL.Database.Admins.Interfaces;
using BL.Model.AdminCore;
using BL.Model.DocumentCore.IncomingModel;
using System.Linq;
using BL.Logic.Common;
using System.Collections.Generic;

namespace BL.Logic.DocumentCore.AdditionalCommands
{
    public class ModifyDocumentTagsCommand : BaseDocumentAdditionCommand
    {
        private readonly IDocumentOperationsDbProcess _operationDb;
        private readonly IAdminsDbProcess _adminDb;

        protected InternalDocumentTags DocTags;

        public ModifyDocumentTagsCommand(IDocumentOperationsDbProcess operationDb, IAdminsDbProcess adminDb)
        {
            _adminDb = adminDb;
            _operationDb = operationDb;
        }

        private ModifyDocumentTags Model
        {
            get
            {
                if (!(_param is ModifyDocumentTags))
                {
                    throw new WrongParameterTypeError();
                }
                return (ModifyDocumentTags)_param;
            }
        }

        public override bool CanBeDisplayed()
        {
            return true;
        }

        public override bool CanExecute()
        {
            _context.SetCurrentPosition(_document.ExecutorPositionId);
            _adminDb.VerifyAccess(_context, new VerifyAccess { DocumentActionCode = CommandType.ToString() });

            DocTags = new InternalDocumentTags
            {
                DocumentId = Model.DocumentId,
                Tags = Model.Tags
            };

            return true;
        }

        public override object Execute()
        {
            CommonDocumentUtilities.SetLastChange(_context, DocTags);
            _operationDb.ModifyDocumentTags(_context, DocTags);
            return null;
        }

        public override EnumDocumentAdditionActions CommandType => EnumDocumentAdditionActions.ModifyDocumentTags;
    }
}