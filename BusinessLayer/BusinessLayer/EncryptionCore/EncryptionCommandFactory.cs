using BL.CrossCutting.DependencyInjection;
using BL.CrossCutting.Interfaces;
using BL.Logic.EncryptionCore.Certificate;
using BL.Logic.EncryptionCore.Commands;
using BL.Model.Enums;
using BL.Model.Exception;

namespace BL.Logic.EncryptionCore
{
    public static class EncryptionCommandFactory
    {
        public static IEncryptionCommand GetEncryptionCommand(EnumEncryptionActions act, IContext ctx, object param)
        {
            if (ctx.ClientLicence?.LicenceError != null)
            {
                throw ctx.ClientLicence.LicenceError as DmsExceptions;
            }

            IEncryptionCommand cmd;
            switch (act)
            {
                #region EncryptionCertificates
                case EnumEncryptionActions.AddEncryptionCertificate:
                    cmd = DmsResolver.Current.Get<AddEncryptionCertificateCommand>();
                    break;
                case EnumEncryptionActions.ModifyEncryptionCertificate:
                    cmd = DmsResolver.Current.Get<ModifyEncryptionCertificateCommand>();
                    break;
                case EnumEncryptionActions.DeleteEncryptionCertificate:
                    cmd = DmsResolver.Current.Get<DeleteEncryptionCertificateCommand>();
                    break;
                #endregion EncryptionCertificates

                case EnumEncryptionActions.VerifyPdf:
                    cmd = DmsResolver.Current.Get<VerifyPdf>();
                    break;
                default:
                    throw new CommandNotDefinedError();
            }
            cmd.InitializeCommand(act, ctx, param);
            return cmd;
        }
    }
}