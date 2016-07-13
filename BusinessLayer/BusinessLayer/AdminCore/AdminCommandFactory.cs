using BL.CrossCutting.DependencyInjection;
using BL.CrossCutting.Interfaces;
using BL.Model.Enums;
using BL.Model.Exception;

namespace BL.Logic.AdminCore
{
    public static class AdminCommandFactory
    {
        public static IAdminCommand GetAdminCommand(EnumAdminActions act, IContext ctx, object param)
        {
            if (ctx.ClientLicence?.LicenceError != null)
            {
                throw ctx.ClientLicence.LicenceError as DmsExceptions;
            }

            IAdminCommand cmd;
            switch (act)
            {
                // Типы документов
                #region AdminDocumentTypes
                case EnumAdminActions.AddPositionRole:
                    cmd = DmsResolver.Current.Get<AddPositionRoleCommand>();
                    break;
                case EnumAdminActions.ModifyPositionRole:
                    cmd = DmsResolver.Current.Get<ModifyPositionRoleCommand>();
                    break;
                case EnumAdminActions.DeletePositionRole:
                    cmd = DmsResolver.Current.Get<DeletePositionRoleCommand>();
                    break;
                #endregion AdminDocumentTypes


                default:
                    throw new CommandNotDefinedError();
            }
            cmd.InitializeCommand(act, ctx, param);
            return cmd;
        }
    }
}