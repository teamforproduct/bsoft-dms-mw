﻿using BL.Model.AdminCore;
using System.Collections.Generic;
using BL.CrossCutting.Interfaces;
using BL.Model.Enums;
using BL.Model.Users;

namespace BL.Logic.AdminCore.Interfaces
{
    public interface IAdminService
    {
        IEnumerable<BaseAdminUserRole> GetPositionsByCurrentUser(IContext context);
        bool VerifyAccess(IContext context, VerifyAccess verifyAccess, bool isThrowExeception = true);
        bool VerifyAccess(IContext context, EnumDocumentActions action, bool isPositionFromContext = true, bool isThrowExeception = true);
        bool VerifyAccess(IContext context, EnumDictionaryAction action, bool isPositionFromContext = true, bool isThrowExeception = true);
        bool VerifyAccess(IContext context, EnumDictionaryAction action, bool isThrowExeception = true);

        Employee GetEmployee(IContext context, int id);
        IEnumerable<CurrentPosition> GetPositionsByUser(Employee employee);
    }
}
