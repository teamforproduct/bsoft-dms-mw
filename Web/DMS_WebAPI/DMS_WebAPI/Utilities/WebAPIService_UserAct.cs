using BL.CrossCutting.DependencyInjection;
using BL.CrossCutting.Interfaces;
using BL.Logic.AdminCore.Interfaces;
using BL.Logic.SystemServices.MailWorker;
using BL.Model.DictionaryCore.IncomingModel;
using BL.Model.Enums;
using BL.Model.Exception;
using BL.Model.Users;
using BL.Model.WebAPI.IncomingModel;
using DMS_WebAPI.DBModel;
using DMS_WebAPI.Models;
using DMS_WebAPI.Providers;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web;

namespace DMS_WebAPI.Utilities
{
    // Тут собраны действия, которые пользователь пожет делать со своей учеткой
    internal partial class WebAPIService
    {

        public void UpdateUserParms(string userId, ModifyAgentUser model)
        {


        }

        public async Task ChangeFingerprintEnabled(string userId, bool parm)
        {
            var user = GetUserById(userId);

            user.IsFingerprintEnabled = parm;
            user.LastChangeDate = DateTime.UtcNow;

            var result = await UserManager.UpdateAsync(user);

            if (!result.Succeeded) throw new UserParmsCouldNotBeChanged(result.Errors);

        }

        public void SetUserLanguage(string userId, string languageCode)
        {
            var languages = DmsResolver.Current.Get<ILanguages>();
            var langId = languages.GetLanguageIdByCode(languageCode);
            SetUserLanguage(userId, langId);
        }

        public void SetUserLanguage(string userId, int languageId)
        {
            var user = GetUserById(userId);

            user.LanguageId = languageId;
            user.LastChangeDate = DateTime.UtcNow;

            var result = UserManager.Update(user);

            if (!result.Succeeded) throw new UserParmsCouldNotBeChanged(result.Errors);

            var userContexts = DmsResolver.Current.Get<UserContexts>();
            userContexts.UpdateLanguageId(userId, languageId);
        }


        public async Task SetUserPasswordAsync(string userId, SetUserPassword model)
        {
            IdentityResult result = await UserManager.AddPasswordAsync(userId, model.NewPassword);

            if (!result.Succeeded) throw new UserParmsCouldNotBeChanged(result.Errors);
        }

        public async Task ChangeUserPasswordAsync(string userId, ChangeUserPassword model)
        {
            var result = await UserManager.PasswordValidator.ValidateAsync(model.NewPassword);

            if (!result.Succeeded) throw new UserPasswordIsIncorrect(result.Errors);

            result = await UserManager.ChangePasswordAsync(userId, model.OldPassword, model.NewPassword);

            if (!result.Succeeded) throw new UserParmsCouldNotBeChanged(result.Errors);

            AspNetUsers user = await GetUserByIdAsync(userId);

            user.IsChangePasswordRequired = false;
            user.LastChangeDate = DateTime.UtcNow;

            result = await UserManager.UpdateAsync(user);

            if (!result.Succeeded) throw new UserParmsCouldNotBeChanged(result.Errors);

            var user_context = DmsResolver.Current.Get<UserContexts>();
            user_context.UpdateChangePasswordRequired(userId, false);
        }

        public void ChangeControlQuestion(string userId, ModifyAspNetUserControlQuestion model)
        {
            var user = GetUserById(userId);

            user.ControlQuestionId = model.QuestionId;
            user.ControlAnswer = model.Answer;
            user.LastChangeDate = DateTime.UtcNow;

            var result = UserManager.Update(user);

            if (!result.Succeeded) throw new DatabaseError(result.Errors);

            var webService = DmsResolver.Current.Get<WebAPIService>();

            var m = new AddSessionLog
            {
                Platform = HttpContext.Current.Request.Browser.Platform,
                Browser = HttpContext.Current.Request.Browser.Name(),
                IP = HttpContext.Current.Request.Browser.IP(),

                Date = DateTime.UtcNow,
                UserId = userId,
                Type = EnumLogTypes.Information,
                Event = "ChangeSecurityQuestion",
                Message = "SecurityQuestionChanged",
            };

            webService.AddSessionLog(m);

        }

        public async Task RestorePassword(RestorePassword model)
        {
            var user = await GetUserAsync(model.Email);

            if (user == null) throw new UserIsNotDefined();

            // Для заблокированного пользователя запрещаю смену пароля
            if (UserManager.SupportsUserLockout && await UserManager.GetLockoutEnabledAsync(user.Id) && await UserManager.IsLockedOutAsync(user.Id)) throw new UserIsLockout();

            var settVal = DmsResolver.Current.Get<ISettingValues>();

            var baseUri = string.IsNullOrEmpty(model.ClientCode) ? new Uri(settVal.GetAuthAddress()) : new Uri(settVal.GetClientAddress(model.ClientCode));

            string url = new Uri(baseUri, "restore-password").ToString();

            var passwordResetToken = await UserManager.GeneratePasswordResetTokenAsync(user.Id);

            var builder = new UriBuilder(url);
            var newQuery = HttpUtility.ParseQueryString(builder.Query);
            newQuery.Add("UserId", user.Id);
            newQuery.Add("Code", passwordResetToken);

            //var query = new NameValueCollection();
            //newQuery.Add(query);

            builder.Query = newQuery.ToString();// string.Join("&", nvc.AllKeys.Select(key => string.Format("{0}={1}", HttpUtility.UrlEncode(key), HttpUtility.UrlEncode(nvc[key]))));

            var languages = DmsResolver.Current.Get<ILanguages>();

            var m = new MailWithCallToActionModel()
            {
                Greeting = languages.GetTranslation(user.LanguageId, "##l@Mail:Greeting@l##", new List<string> { user.FirstName }),
                Closing = languages.GetTranslation(user.LanguageId, "##l@Mail:Closing@l##"),
                // сылка на восстановление пароля
                CallToActionUrl = builder.ToString(),
                CallToActionName = languages.GetTranslation(user.LanguageId, "##l@Mail.RestorePassword.CallToActionName@l##"),
                CallToActionDescription = languages.GetTranslation(user.LanguageId, "##l@Mail.RestorePassword.CallToActionDescription@l##"),
                PostScriptum = languages.GetTranslation(user.LanguageId, "##l@Mail.RestorePassword.PostScriptum@l##"),
            };

            // html с подставленной моделью
            var htmlContent = m.RenderPartialViewToString(RenderPartialView.MailWithCallToAction);


            var mailService = DmsResolver.Current.Get<IMailSenderWorkerService>();
            mailService.SendMessage(null, MailServers.Noreply, model.Email, languages.GetTranslation(user.LanguageId, "##l@Mail.RestorePassword.Subject@l##"), htmlContent);
        }




        public async Task ConfirmEmail(string userId, string code)
        {
            // Для заблокированного пользователя запрещаю смену пароля
            if (UserManager.SupportsUserLockout && await UserManager.GetLockoutEnabledAsync(userId) && await UserManager.IsLockedOutAsync(userId)) throw new UserIsLockout();

            var result = await UserManager.ConfirmEmailAsync(userId, code);

            if (!result.Succeeded) throw new UserEmailCouldNotBeConfirmd(result.Errors);

            var webService = DmsResolver.Current.Get<WebAPIService>();

            var m = new AddSessionLog
            {
                Platform = HttpContext.Current.Request.Browser.Platform,
                Browser = HttpContext.Current.Request.Browser.Name(),
                IP = HttpContext.Current.Request.Browser.IP(),

                Date = DateTime.UtcNow,
                UserId = userId,
                Type = EnumLogTypes.Information,
                Event = "ConfirmEmail",
                Message = "EmailConfirmed",
            };

            webService.AddSessionLog(m);
        }

        public async Task<string> ResetPassword(ResetPassword model)
        {
            var result = await UserManager.PasswordValidator.ValidateAsync(model.NewPassword);

            if (!result.Succeeded) throw new UserPasswordIsIncorrect(result.Errors);

            result = await UserManager.ResetPasswordAsync(model.UserId, model.Code, model.NewPassword);

            if (!result.Succeeded) throw new ResetPasswordCodeInvalid(result.Errors);

            AspNetUsers user = await UserManager.FindByIdAsync(model.UserId);

            if (user == null) throw new UserIsNotDefined();

            user.IsChangePasswordRequired = false;
            user.EmailConfirmed = true;
            user.LastChangeDate = DateTime.UtcNow;

            result = await UserManager.UpdateAsync(user);

            if (!result.Succeeded) throw new UserParmsCouldNotBeChanged(result.Errors);

            var userContexts = DmsResolver.Current.Get<UserContexts>();
            userContexts.RemoveByUserId(model.UserId);


            var webService = DmsResolver.Current.Get<WebAPIService>();

            var m = new AddSessionLog
            {
                Platform = HttpContext.Current.Request.Browser.Platform,
                Browser = HttpContext.Current.Request.Browser.Name(),
                IP = HttpContext.Current.Request.Browser.IP(),

                Date = DateTime.UtcNow,
                UserId = model.UserId,
                Type = EnumLogTypes.Information,
                Event = "ResetPassword",
                Message = "PasswordReset",
            };

            webService.AddSessionLog(m);

            return user.Email;
        }

        public async Task ValidatePassword(string password)
        {
            var result = await UserManager.PasswordValidator.ValidateAsync(password);

            if (!result.Succeeded) throw new UserPasswordIsIncorrect(result.Errors);
        }

    }
}