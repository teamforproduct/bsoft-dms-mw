﻿using BL.Database.Documents.Interfaces;
using BL.Model.DocumentCore.InternalModel;
using BL.Model.Exception;
using BL.Model.DocumentCore.IncomingModel;
using BL.Logic.Common;
using BL.Model.Enums;

namespace BL.Logic.DocumentCore.AdditionalCommands
{
    public class ModifyDocumentTagsCommand : BaseDocumentCommand
    {
        private readonly IDocumentOperationsDbProcess _operationDb;

        protected InternalDocumentTag DocTags;

        public ModifyDocumentTagsCommand(IDocumentOperationsDbProcess operationDb)
        {
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

        public override bool CanBeDisplayed(int positionId)
        {
            return true;
        }

        public override bool CanExecute()
        {
            _admin.VerifyAccess(_context, CommandType);

            DocTags = new InternalDocumentTag
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

        public override EnumDocumentActions CommandType => EnumDocumentActions.ModifyDocumentTags;
    }
}