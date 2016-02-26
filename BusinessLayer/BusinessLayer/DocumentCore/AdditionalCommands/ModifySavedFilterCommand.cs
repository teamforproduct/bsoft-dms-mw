using BL.Database.Documents.Interfaces;
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
    public class ModifySavedFilterCommand : BaseDocumentCommand
    {
        private readonly IDocumentOperationsDbProcess _operationDb;
        private readonly IAdminsDbProcess _adminDb;

        protected InternalDocumentSavedFilter DocSavedFilter;

        public ModifySavedFilterCommand(IDocumentOperationsDbProcess operationDb, IAdminsDbProcess adminDb)
        {
            _adminDb = adminDb;
            _operationDb = operationDb;
        }

        private ModifyDocumentSavedFilter Model
        {
            get
            {
                if (!(_param is ModifyDocumentSavedFilter))
                {
                    throw new WrongParameterTypeError();
                }
                return (ModifyDocumentSavedFilter)_param;
            }
        }

        public override bool CanBeDisplayed()
        {
            return true;
        }

        public override bool CanExecute()
        {
            //TODO Добавить проверки
            //_context.SetCurrentPosition(_document.ExecutorPositionId);
            //_adminDb.VerifyAccess(_context, CommandType);

            DocSavedFilter = new InternalDocumentSavedFilter
            {
                Id = Model.Id,
                PositionId = _context.CurrentPositionId,
                Icon = Model.Icon,
                Filter = Model.Filter.ToString(),
                IsCommon = Model.IsCommon
            };

            return true;
        }

        public override object Execute()
        {
            CommonDocumentUtilities.SetLastChange(_context, DocSavedFilter);
            _operationDb.ModifySavedFilter(_context, DocSavedFilter);
            return null;
        }

        public override EnumDocumentActions CommandType => EnumDocumentActions.ModifySavedFilter;
    }
}