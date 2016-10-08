using BL.Database.Documents.Interfaces;
using BL.Model.DocumentCore.InternalModel;
using BL.Model.Enums;
using BL.Model.Exception;
using BL.Model.DocumentCore.IncomingModel;
using System.Linq;
using BL.Logic.Common;
using System.Collections.Generic;

namespace BL.Logic.DocumentCore.AdditionalCommands
{
    public class AddSavedFilterCommand : BaseDocumentCommand
    {
        private readonly IDocumentOperationsDbProcess _operationDb;

        protected InternalDocumentSavedFilter DocSavedFilter;

        public AddSavedFilterCommand(IDocumentOperationsDbProcess operationDb)
        {
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

        public override bool CanBeDisplayed(int positionId)
        {
            return true;
        }

        public override bool CanExecute()
        {
            //TODO Добавить проверки
            //_context.SetCurrentPosition(_document.ExecutorPositionId);
            //_admin.VerifyAccess(_context, CommandType);

            DocSavedFilter = new InternalDocumentSavedFilter
            {
                PositionId = _context.CurrentPositionId,
                Name = Model.Name,
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