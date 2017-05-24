﻿using System.Linq;
using BL.Database.Documents.Interfaces;
using BL.Database.FileWorker;
using BL.Logic.Common;
using BL.Model.DocumentCore.Filters;
using BL.Model.DocumentCore.InternalModel;
using BL.Model.Enums;
using BL.Model.Exception;

namespace BL.Logic.DocumentCore.AdditionalCommands
{
    public class DeleteDocumentFileCommand : BaseDocumentCommand
    {
        private readonly IDocumentFileDbProcess _operationDb;
        private readonly IFileStore _fStore;

        private InternalDocumentFile _file;

        public DeleteDocumentFileCommand(IDocumentFileDbProcess operationDb, IFileStore fStore)
        {
            _operationDb = operationDb;
            _fStore = fStore;
        }

        private int Model
        {
            get
            {
                if (!(_param is int))
                {
                    throw new WrongParameterTypeError();
                }
                return (int)_param;
            }
        }

        public override bool CanBeDisplayed(int positionId)
        {
            _actionRecords =
                          _document.DocumentFiles.Where(x => x.IsMainVersion && !x.IsDeleted)
                            .Where(
                              x =>
                                  (x.Type == EnumFileTypes.Additional && x.ExecutorPositionId == positionId)
                                    || (x.Type == EnumFileTypes.Main && _document.ExecutorPositionId == positionId))
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
            _document = _operationDb.DeleteDocumentFilePrepare(_context, Model);
            if (_document == null)
            {
                throw new DocumentNotFoundOrUserHasNoAccess();
            }
            if (_document.DocumentFiles == null || !_document.DocumentFiles.Any())
            {
                throw new UnknownDocumentFile();
            }

            _file = _document.DocumentFiles.Where(x => x.IsMainVersion).First();

            if (_file.Type != EnumFileTypes.Additional)
            {
                _context.SetCurrentPosition(_document.ExecutorPositionId);
            }
            else
            {
                _context.SetCurrentPosition(_file.ExecutorPositionId);
            }
            _adminProc.VerifyAccess(_context, CommandType);

            if (!CanBeDisplayed(_context.CurrentPositionId))
            {
                throw new CouldNotPerformOperationWithPaper();
            }

            return true;
        }

        public override object Execute()
        {
            var docFile = new InternalDocumentFile
            {
                ClientId = _document.ClientId,
                EntityTypeId = _document.EntityTypeId,
                DocumentId = _file.DocumentId,
                OrderInDocument = _file.OrderInDocument
            };
            //try
            //{
            //    //_fStore.DeleteFile(_context, docFile);
            //}
            //catch (CannotAccessToFile ex)
            //{
            //}
            if (_document.IsRegistered.HasValue)
            {
                docFile.Event = CommonDocumentUtilities.GetNewDocumentEvent(_context, (int)EnumEntytiTypes.Document, docFile.DocumentId, EnumEventTypes.DeleteDocumentFile, null, null, _file.File.FileName);
            }
            _operationDb.DeleteAttachedFile(_context, docFile);
            return null;
        }
    }
}