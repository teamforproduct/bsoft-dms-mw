using BL.CrossCutting.Context;
using BL.CrossCutting.DependencyInjection;
using BL.CrossCutting.Extensions;
using BL.CrossCutting.Helpers;
using BL.CrossCutting.Interfaces;
using BL.Logic.AdminCore.Interfaces;
using BL.Logic.ClientCore.Interfaces;
using BL.Logic.DictionaryCore.Interfaces;
using BL.Logic.SystemServices.MailWorker;
using BL.Logic.SystemServices.TempStorage;
using BL.Model.AdminCore.Clients;
using BL.Model.AdminCore.WebUser;
using BL.Model.Common;
using BL.Model.Context;
using BL.Model.DictionaryCore.FrontModel.Employees;
using BL.Model.DictionaryCore.IncomingModel;
using BL.Model.DictionaryCore.InternalModel;
using BL.Model.Enums;
using BL.Model.Exception;
using BL.Model.Users;
using BL.Model.WebAPI.Filters;
using BL.Model.WebAPI.FrontModel;
using BL.Model.WebAPI.IncomingModel;
using DMS_WebAPI.DatabaseContext;
using DMS_WebAPI.DBModel;
using DMS_WebAPI.Infrastructure;
using DMS_WebAPI.Models;
using DMS_WebAPI.Providers;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security.OAuth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using BL.Logic.SystemServices.TaskManagerService;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace DMS_WebAPI.Utilities
{
    internal partial class WebAPIService
    {

        // Администратор меняет логин - жуть
        public async Task ChangeLoginAgentUser(ChangeLoginAgentUser model)
        {
            var userContexts = DmsResolver.Current.Get<UserContexts>();

            var context = userContexts.Get();

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
            userContexts.RemoveByUserId(user.Id);

            await ConfirmEmailAgentUser(model.Id);

        }


        public async Task ChangePasswordAgentUserAsync(ChangePasswordAgentUser model)
        {
            var userContexts = DmsResolver.Current.Get<UserContexts>();

            var ctx = userContexts.Get();

            var admService = DmsResolver.Current.Get<IAdminService>();
            admService.ExecuteAction(EnumAdminActions.ChangePassword, ctx, model.Id);

            var user = await GetUserAsync(ctx, model.Id);

            if (user == null) throw new UserIsNotDefined();

            var token = await UserManager.GeneratePasswordResetTokenAsync(user.Id);

            var result = await UserManager.ResetPasswordAsync(user.Id, token, model.NewPassword);

            if (!result.Succeeded) throw new DatabaseError(result.Errors);

            user.IsChangePasswordRequired = model.IsChangePasswordRequired;//true;
            user.LastChangeDate = DateTime.UtcNow;

            result = await UserManager.UpdateAsync(user);

            if (!result.Succeeded) throw new DatabaseError(result.Errors);

            if (model.IsKillSessions)
                userContexts.RemoveByClientId(ctx.Client.Id, user.Id);

        }


        public async Task ConfirmEmailAgentUser(int agentId)
        {
            var context = DmsResolver.Current.Get<UserContexts>().Get();

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

            var htmlContent = callbackurl.RenderPartialViewToString(RenderPartialView.PartialViewNameChangeLoginAgentUserVerificationEmail);

            var mailService = DmsResolver.Current.Get<IMailSenderWorkerService>();

            var languages = DmsResolver.Current.Get<ILanguages>();

            mailService.SendMessage(context, MailServers.Noreply, user.Email, languages.GetTranslation("##l@EmailSubject:EmailConfirmation@l##"), htmlContent);
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