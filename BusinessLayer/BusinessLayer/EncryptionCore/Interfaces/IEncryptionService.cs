using System.Collections.Generic;
using BL.CrossCutting.Interfaces;
using BL.Model.SystemCore;
using BL.Model.Enums;
using BL.Model.EncryptionCore.FrontModel;
using BL.Model.EncryptionCore.Filters;

namespace BL.Logic.EncryptionCore.Interfaces
{
    public interface IEncryptionService
    {
        object ExecuteAction(EnumActions act, IContext context, object param);

        #region EncryptionCertificates
        FrontEncryptionCertificate GetCertificate(IContext ctx, int id);

        IEnumerable<FrontEncryptionCertificate> GetCertificates(IContext ctx, FilterEncryptionCertificate filter, UIPaging paging);

        #endregion
    }
}
