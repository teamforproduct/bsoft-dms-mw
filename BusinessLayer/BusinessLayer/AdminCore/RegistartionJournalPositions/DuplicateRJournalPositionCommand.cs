﻿using BL.Logic.Common;
using BL.Model.AdminCore.FilterModel;
using BL.Model.AdminCore.IncomingModel;
using BL.Model.AdminCore.InternalModel;
using BL.Model.Enums;
using BL.Model.Exception;
using System;
using System.Collections.Generic;

namespace BL.Logic.AdminCore
{
    public class DuplicateRJournalPositionCommand : BaseRJournalPositionCommand
    {
        private CopyAdminSettingsByPosition Model
        {
            get
            {
                if (!(_param is CopyAdminSettingsByPosition)) throw new WrongParameterTypeError();
                return (CopyAdminSettingsByPosition)_param;
            }
        }

        public override object Execute()
        {
            try
            {
                if (Model.CopyMode == BL.Model.Enums.EnumCopyMode.Сoverage)
                {
                    // ощищаю настроки для Model.TargetPositionId
                    _adminDb.DeleteSubordinationsBySourcePositionId(_context, new InternalAdminSubordination() { SourcePositionId = Model.TargetPositionId });
                }

                // выгребаю все настройки для Model.SourcePosition
                var items = _adminDb.GetSubordinations(_context, new FilterAdminSubordination() { SourcePositionIDs = new List<int> { Model.SourcePositionId } });

                // добавляю 
                foreach (var item in items)
                {
                    // подменил SourcePosition
                    var model = new ModifyAdminSubordination()
                    {
                        SourcePositionId = Model.TargetPositionId,
                        TargetPositionId = item.TargetPositionId,
                        SubordinationTypeId = item.SubordinationTypeId
                    };

                    if (!_adminDb.ExistsSubordination(_context, new FilterAdminSubordination()
                    {
                        SourcePositionIDs = new List<int> { model.SourcePositionId },
                        TargetPositionIDs = new List<int> { model.TargetPositionId },
                        SubordinationTypeIDs = new List<EnumSubordinationTypes>() { model.SubordinationTypeId }
                    }))
                        _adminDb.AddSubordination(_context, CommonAdminUtilities.SubordinationModifyToInternal(_context, model));
                }

                return null;
            }
            catch (Exception ex)
            {
                throw new AdminRecordCouldNotBeAdded(ex);
            }

        }
    }
}