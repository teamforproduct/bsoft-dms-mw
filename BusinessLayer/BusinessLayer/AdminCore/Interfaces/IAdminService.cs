﻿using BL.CrossCutting.Interfaces;
using BL.Model.AdminCore;
using System.Collections.Generic;

namespace BL.Logic.AdminCore.Interfaces
{
    public interface IAdminService
    {
        #region AdminAccessLevels
        BaseAdminAccessLevel GetAdminAccessLevel(IContext context, int id);

        IEnumerable<BaseAdminAccessLevel> GetAdminAccessLevels(IContext context, FilterAdminAccessLevel filter);
        #endregion AdminAccessLevels
        IEnumerable<BaseAdminUserRole> GetPositionsByCurrentUser(IContext context);
        void VerifyAccessForCurrentUser(IContext context, string obj, string act, int? id = null);
    }
}
