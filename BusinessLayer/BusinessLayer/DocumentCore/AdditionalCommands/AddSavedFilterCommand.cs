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
    public class AddSavedFilterCommand : BaseDocumentCommand
    {
        private readonly IDocumentOperationsDbProcess _operationDb;
        private readonly IAdminsDbProcess _adminDb;

        protected InternalDocumentSavedFilter DocSavedFilter;

        public AddSavedFilterCommand(IDocumentOperationsDbProcess operationDb, IAdminsDbProcess adminDb)
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
            var ids = _operationDb.AddSavedFilter(_context, new List<InternalDocumentSavedFilter> { DocSavedFilter });
            return ids.FirstOrDefault();
        }

        public override EnumDocumentActions CommandType => EnumDocumentActions.AddSavedFilter;
    }
}