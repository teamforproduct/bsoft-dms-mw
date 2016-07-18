using BL.CrossCutting.DependencyInjection;
using BL.CrossCutting.Interfaces;
using BL.Logic.EncryptionCore.Certificate;
using BL.Logic.EncryptionCore.CertificateType;
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
                case EnumEncryptionActions.ExportEncryptionCertificate:
                    cmd = DmsResolver.Current.Get<ExportEncryptionCertificateCommand>();
                    break;
                case EnumEncryptionActions.GenerateKeyEncryptionCertificate:
                    cmd = DmsResolver.Current.Get<GenerateKeyEncryptionCertificateCommand>();
                    break;
                #endregion EncryptionCertificates

                #region EncryptionCertificateTypes
                case EnumEncryptionActions.AddEncryptionCertificateType:
                    cmd = DmsResolver.Current.Get<AddEncryptionCertificateTypeCommand>();
                    break;
                case EnumEncryptionActions.ModifyEncryptionCertificateType:
                    cmd = DmsResolver.Current.Get<ModifyEncryptionCertificateTypeCommand>();
                    break;
                case EnumEncryptionActions.DeleteEncryptionCertificateType:
                    cmd = DmsResolver.Current.Get<DeleteEncryptionCertificateTypeCommand>();
                    break;
                #endregion EncryptionCertificateTypes

                default:
                    throw new CommandNotDefinedError();
            }
            cmd.InitializeCommand(act, ctx, param);
            return cmd;
        }
    }
}