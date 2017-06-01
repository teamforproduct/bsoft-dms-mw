using System;
using System.IO;
using System.Linq;
using BL.Database.Documents.Interfaces;
using BL.Database.FileWorker;
using BL.Logic.Common;
using BL.Model.DocumentCore.FrontModel;
using BL.Model.DocumentCore.IncomingModel;
using BL.Model.DocumentCore.InternalModel;
using BL.Model.Enums;
using BL.Model.Exception;
using System.Collections.Generic;

namespace BL.Logic.DocumentCore.AdditionalCommands
{
    public class ModifyDocumentFileCommand : BaseDocumentCommand
    {
        private readonly IDocumentFileDbProcess _operationDb;
        private readonly IFileStore _fStore;

        private InternalDocumentFile _file;

        public ModifyDocumentFileCommand(IDocumentFileDbProcess operationDb, IFileStore fStore)
        {
            _operationDb = operationDb;
            _fStore = fStore;
        }

        private ModifyDocumentFile Model
        {
            get
            {
                if (!(_param is ModifyDocumentFile))
                {
                    throw new WrongParameterTypeError();
                }
                return (ModifyDocumentFile)_param;
            }
        }

        public override bool CanBeDisplayed(int positionId)
        {
            if ((_document.Accesses?.Count() ?? 0) != 0 && !_document.Accesses.Any(x => x.PositionId == positionId && x.IsInWork))
                return false;
            _actionRecords = _document.DocumentFiles.Where(x => !x.IsDeleted && x.ExecutorPositionId == positionId)
                                                   .Select(x => new InternalActionRecord
                                                   {
                                                       FileId = x.Id,
                                                   });
            if (!_actionRecords.Any())
            {
                return false;
            }
            return true;
        }

        public override bool CanExecute()
        {
            _document = _operationDb.ModifyDocumentFilePrepare(_context, Model.FileId);
            if (_document == null)
            {
                throw new DocumentNotFoundOrUserHasNoAccess();
            }
            if (_document.DocumentFiles == null || !_document.DocumentFiles.Any())
            {
                throw new UnknownDocumentFile();
            }
            _file = _document.DocumentFiles.First();
            _context.SetCurrentPosition(_file.ExecutorPositionId);
            _adminProc.VerifyAccess(_context, CommandType);
            if ((Model.Type != null && Model.Type == EnumFileTypes.Main && _file.ExecutorPositionId != _file.ExecutorPositionId)
                || !CanBeDisplayed(_context.CurrentPositionId))
            {
                throw new CouldNotPerformOperation();
            }

            return true;
        }

        public override object Execute()
        {
            if (Model.Type.HasValue && _file.Type != Model.Type)
            {
                _file.IsTypeChange = true;
                _file.Type = Model.Type.Value;
            }
            _file.Description = Model.Description;
            _file.IsBaseChange = true;

            CommonDocumentUtilities.SetLastChange(_context, _file);
            var newEvent = CommonDocumentUtilities.GetNewDocumentEvent(_context, (int)EnumEntytiTypes.Document, _file.DocumentId, EnumEventTypes.ModifyDocumentFile, Model.EventDate, Model.Description, null, _file.EventId, null, Model.TargetAccessGroups);
            CommonDocumentUtilities.VerifyAndSetDocumentAccess(_context, _document, newEvent.Accesses);
            _document.Events = new List<InternalDocumentEvent> { newEvent };

            _operationDb.ModifyDocumentFile(_context, _document);
            return _file.Id;
        }
    }
}