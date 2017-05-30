using BL.CrossCutting.DependencyInjection;
using BL.CrossCutting.Interfaces;
using BL.Logic.AdminCore.Interfaces;
using BL.Logic.DictionaryCore.Interfaces;
using BL.Logic.SystemServices.MailWorker;
using BL.Model.Common;
using BL.Model.Enums;
using BL.Model.Exception;
using BL.Model.Users;
using System;
using System.Threading.Tasks;
using System.Web;

namespace DMS_WebAPI.Utilities
{
    internal partial class WebAPIService
    {

        // Администратор меняет логин - жуть
        public async Task ChangeLoginAgentUserAsync(IContext context, ChangeLoginAgentUser model)
        {

            var user = GetUser(context, model.Id);
            if (user == null) throw new UserIsNotDefined();

            if (user.UserName == model.NewEmail) return;

            user.UserName = model.NewEmail;
            user.Email = model.NewEmail;

            var result = await UserManager.UpdateAsync(user);

            if (result.Succeeded)
            {
                //TODO тут нужно пробежаться по всем клиентским базам и заменить логин
                var admService = DmsResolver.Current.Get<IAdminService>();
                admService.ChangeLoginAgentUser(context, model);
            }
            else throw new UserLoginCouldNotBeChanged(model.NewEmail, result.Errors);


            // выкидываю пользователя из системы
            var userContexts = DmsResolver.Current.Get<UserContexts>();
            userContexts.RemoveByUserId(user.Id);

            await ConfirmEmailAgentUser(context, model.Id);

        }


        public async Task SetMustChangePasswordAgentUserAsync(IContext context, MustChangePasswordAgentUser model)
        {
            var user = GetUser(context, model.Id);

            if (user == null) throw new UserIsNotDefined();

            user.IsChangePasswordRequired = model.MustChangePassword;
            user.LastChangeDate = DateTime.UtcNow;

            var result = await UserManager.UpdateAsync(user);

            if (!result.Succeeded) throw new UserParmsCouldNotBeChanged(result.Errors);
        }

        public void ChangeLokoutAgentUser(IContext context, ChangeLockoutAgentUser model)
        {
            var tmpService = DmsResolver.Current.Get<IDictionaryService>();
            tmpService.SetAgentUserLockout(context, model.Id, model.IsLockout);

            if (model.IsLockout)
            {
                var userContexts = DmsResolver.Current.Get<UserContexts>();
                userContexts.RemoveByClientId(context.Client.Id, model.Id);
            }
        }

        public async Task ChangePasswordAgentUserAsync(IContext context, ChangePasswordAgentUser model)
        {
            var user = await GetUserAsync(context, model.Id);

            if (user == null) throw new UserIsNotDefined();

            var token = await UserManager.GeneratePasswordResetTokenAsync(user.Id);

            var result = await UserManager.ResetPasswordAsync(user.Id, token, model.NewPassword);

            if (!result.Succeeded) throw new UserParmsCouldNotBeChanged(result.Errors);

            user.IsChangePasswordRequired = model.IsChangePasswordRequired;//true;
            user.LastChangeDate = DateTime.UtcNow;

            result = await UserManager.UpdateAsync(user);

            if (!result.Succeeded) throw new UserParmsCouldNotBeChanged(result.Errors);

            if (model.IsKillSessions)
            {
                var userContexts = DmsResolver.Current.Get<UserContexts>();
                userContexts.RemoveByClientId(context.Client.Id, model.Id);
            }

        }


        public async Task ConfirmEmailAgentUser(IContext context, int agentId)
        {
            var user = GetUser(context, agentId);
            if (user == null) throw new UserIsNotDefined();
            user.EmailConfirmed = false;
            user.LastChangeDate = DateTime.UtcNow;

            var result = await UserManager.UpdateAsync(user);

            if (!result.Succeeded) throw new UserParmsCouldNotBeChanged(result.Errors);

            var emailConfirmationCode = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);

            var tmpService = DmsResolver.Current.Get<ISettingValues>();
            var addr = tmpService.GetClientAddress(context.Client.Code);
            var callbackurl = new Uri(new Uri(addr), "email-confirmation").AbsoluteUri;

            callbackurl += String.Format("?userId={0}&code={1}", user.Id, HttpUtility.UrlEncode(emailConfirmationCode));

            var htmlContent = callbackurl.RenderPartialViewToString(RenderPartialView.ChangeLogin);

            var mailService = DmsResolver.Current.Get<IMailSenderWorkerService>();

            var languages = DmsResolver.Current.Get<ILanguages>();

            mailService.SendMessage(context, MailServers.Noreply, user.Email, languages.GetTranslation(user.LanguageId, "##l@EmailSubject:EmailConfirmation@l##"), htmlContent);
        }

        public async Task SwitchOffFingerprint(IContext context, Item model)
        {
            var user = await GetUserAsync(context, model.Id);

            if (user == null) throw new UserIsNotDefined();

            user.IsFingerprintEnabled = false;
            user.LastChangeDate = DateTime.UtcNow;

            var result = await UserManager.UpdateAsync(user);

            if (!result.Succeeded) throw new DatabaseError(result.Errors);
        }

    }
}