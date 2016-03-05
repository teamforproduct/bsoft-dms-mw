﻿using System.Linq;
using BL.Logic.Common;
using BL.Database.Documents.Interfaces;
using BL.Logic.AdminCore.Interfaces;
using BL.Model.Enums;
using BL.Model.Exception;
using BL.Model.SystemCore;

namespace BL.Logic.DocumentCore.Commands
{
    public class LaunchPlanDocumentCommand : BaseDocumentCommand
    {
        private readonly IDocumentsDbProcess _documentDb;
        private readonly IAdminService _admin;

        public LaunchPlanDocumentCommand(IDocumentsDbProcess documentDb, IAdminService admin)
        {
            _documentDb = documentDb;
            _admin = admin;
        }

        private int Model
        {
            get
            {
                if (!(_param is int))
                {
                    throw new WrongParameterTypeError();
                }
                return (int) _param;
            }
        }

        public override bool CanBeDisplayed(int positionId)
        {
            if (_document.ExecutorPositionId != positionId
                || _document.IsLaunchPlan
                )
            {
                return false;
            }

            return true;
        }

        public override bool CanExecute()
        {
            //TODO ОСТАЛЬНЫЕ ПРОВЕРКИ!
            _document = _documentDb.ChangeIsLaunchPlanDocumentPrepare(_context, Model);
            if (_document == null)
            {
                throw new DocumentNotFoundOrUserHasNoAccess();
            }
            if (!CanBeDisplayed(_context.CurrentPositionId))
            {
                throw new CouldNotChangeAttributeLaunchPlan();
            }
            _context.SetCurrentPosition(_document.ExecutorPositionId);
            _admin.VerifyAccess(_context, CommandType);

            return true;
        }

        public override object Execute()
        {
            CommonDocumentUtilities.SetLastChange(_context, _document);
            _document.IsLaunchPlan = true;
            _document.Events = CommonDocumentUtilities.GetNewDocumentEvents(_context, _document.Id, EnumEventTypes.LaunchPlan);
            _documentDb.ChangeIsLaunchPlanDocument(_context, _document);
            return Model;
        }

        public override EnumDocumentActions CommandType => EnumDocumentActions.LaunchPlan;
    }
}