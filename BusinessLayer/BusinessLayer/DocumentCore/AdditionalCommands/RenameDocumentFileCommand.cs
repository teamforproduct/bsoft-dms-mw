﻿using System;
using System.IO;
using System.Linq;
using BL.Database.Documents.Interfaces;
using BL.Database.FileWorker;
using BL.Logic.Common;
using BL.Model.DocumentCore.IncomingModel;
using BL.Model.DocumentCore.InternalModel;
using BL.Model.Enums;
using BL.Model.Exception;
using System.Collections.Generic;

namespace BL.Logic.DocumentCore.AdditionalCommands
{
    public class RenameDocumentFileCommand : BaseDocumentCommand
    {
        private readonly IDocumentFileDbProcess _operationDb;
        private readonly IFileStore _fStore;

        private IEnumerable<InternalDocumentAttachedFile> _files;

        public RenameDocumentFileCommand(IDocumentFileDbProcess operationDb, IFileStore fStore)
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
            _actionRecords =
                   _document.DocumentFiles.Where(
                       x =>
                           (x.ExecutorPositionId == positionId && x.IsAdditional)
                           || (_document.ExecutorPositionId == positionId && !x.IsAdditional))
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
            //TODO potential two user could add same new version in same time. Probably need to implement CheckOut flag in future
            _document = _operationDb.ModifyDocumentFilePrepare(_context, Model.DocumentId, Model.OrderInDocument, Model.Version);
            if (_document == null)
            {
                throw new UserHasNoAccessToDocument();
            }
            if (_document.DocumentFiles == null || !_document.DocumentFiles.Any())
            {
                throw new UnknownDocumentFile();
            }
            _files = _document.DocumentFiles;

            _admin.VerifyAccess(_context, CommandType);

            if (!CanBeDisplayed(_context.CurrentPositionId))
            {
                throw new CouldNotPerformOperation();
            }

            return true;
        }

        public override object Execute()
        {
            var oldName = _files.First().Name;
            var extension = _files.First().Extension;
            foreach (var file in _files)
            {
                _fStore.RenameFile(_context, file, Model.FileName);

                file.Name = Model.FileName;
                CommonDocumentUtilities.SetLastChange(_context, file);
            }

            var events = CommonDocumentUtilities.GetNewDocumentEvents(_context, Model.DocumentId, EnumEventTypes.RanameDocumentFile, null, oldName + "." + extension, Model.FileName + "." + extension);

            _operationDb.RenameFile(_context, _files, events);

            return null;
        }

        public override EnumDocumentActions CommandType => EnumDocumentActions.RenameDocumentFile;
    }
}