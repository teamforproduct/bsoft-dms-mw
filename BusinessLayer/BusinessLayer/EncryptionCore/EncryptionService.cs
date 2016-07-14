using System.Collections.Generic;
using BL.Model.SystemCore;
using System.Linq;
using BL.CrossCutting.Interfaces;
using BL.Logic.DocumentCore.Interfaces;
using BL.Model.Enums;
using BL.Database.Encryption.Interfaces;
using BL.Logic.EncryptionCore.Interfaces;
using BL.Model.EncryptionCore.FrontModel;
using BL.Model.EncryptionCore.Filters;

namespace BL.Logic.EncryptionCore
{
    public class EncryptionService : IEncryptionService
    {
        private readonly IEncryptionDbProcess _encryptionDb;
        private readonly ICommandService _commandService;

        public EncryptionService(IEncryptionDbProcess encryptionDb, ICommandService commandService)
        {
            _encryptionDb = encryptionDb;
            _commandService = commandService;
        }

        public object ExecuteAction(EnumEncryptionActions act, IContext ctx, object param)
        {
            var cmd = EncryptionCommandFactory.GetEncryptionCommand(act, ctx, param);
            var res = _commandService.ExecuteCommand(cmd);
            return res;
        }

        #region EncryptionCertificates
        public FrontEncryptionCertificate GetCertificate(IContext ctx, int id)
        {
            return _encryptionDb.GetCertificates(ctx, new FilterEncryptionCertificate { CertificateId = new List<int> { id } }, null).FirstOrDefault();
        }

        public IEnumerable<FrontEncryptionCertificate> GetCertificates(IContext ctx, FilterEncryptionCertificate filter, UIPaging paging)
        {
            return _encryptionDb.GetCertificates(ctx, filter, paging);   
        }

        #endregion
    }
}
