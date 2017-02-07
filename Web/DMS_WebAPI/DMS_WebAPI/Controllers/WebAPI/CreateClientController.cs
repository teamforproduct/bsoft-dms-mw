using BL.CrossCutting.DependencyInjection;
using BL.Model.WebAPI.IncomingModel;
using DMS_WebAPI.Models;
using DMS_WebAPI.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace DMS_WebAPI.Controllers.WebAPI
{
    public class CreateClientController : Controller
    {
        // GET: CreateClient
        [AllowAnonymous]
        public ActionResult CreateClient()
        {
            return View();
        }

        //
        // POST: /Account/Register
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CreateClient(ClientViewModel model)
        {
            if (ModelState.IsValid)
            {
                var model2 = new AddAspNetClient
                {
                    Client = new ModifyAspNetClient
                    {
                        Name = model.ClientName
                    },
                    Admin = new ModifyAspNetUser
                    {
                        Email = model.AdminEmail,
                        Password = model.AdminPassword,
                        ConfirmPassword = model.AdminConfirmPassword,
                    },
                    Server = new ModifyAdminServer
                    {
                        Address=model.ServerAddress,
                        Name = model.ServerName,
                        DefaultDatabase= model.ServerDefaultDatabase,
                        DefaultSchema = model.ServerDefaultSchema,
                        IntegrateSecurity = model.ServerIntegrateSecurity,
                        ServerType = model.ServerServerType,
                        UserName = model.ServerUserName,
                        UserPassword = model.ServerUserPassword,
                        ConnectionString = model.ServerConnectionString,
                    }
                };
                var dbService = DmsResolver.Current.Get<WebAPIService>();
                var itemId = dbService.AddClient(model2);

                return RedirectToAction("Index", "Home");
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }
    }
}