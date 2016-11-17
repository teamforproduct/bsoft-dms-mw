using BL.CrossCutting.DependencyInjection;
using BL.CrossCutting.Interfaces;
using BL.Model.Enums;
using BL.Model.Exception;

namespace BL.Logic.SystemCore
{
    public static class SystemCommandFactory
    {
        public static ISystemCommand GetCommand(EnumSystemActions act, IContext ctx, object param)
        {
            if (ctx.ClientLicence?.LicenceError != null)
            {
                throw ctx.ClientLicence.LicenceError as DmsExceptions;
            }

            ISystemCommand cmd;
            switch (act)
            {

                #region Roles
                case EnumSystemActions.SetSetting:
                    cmd = DmsResolver.Current.Get<SetSettingCommand>();
                    break;
                #endregion

                default:
                    throw new CommandNotDefinedError(act.ToString());
            }
            cmd.InitializeCommand(act, ctx, param);
            return cmd;
        }
    }
}