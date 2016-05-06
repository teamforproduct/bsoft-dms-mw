using DMS_WebAPI.Models;
using DMS_WebAPI.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DMS_WebAPI.Controllers
{
    public class LicenseController : Controller
    {
        public ActionResult ActivationProgram(int id)
        {
            var si = new SystemInfo();
            var dbw = new SystemDbWorker();

            var cd = si.GetRegCode(dbw.GetLicenceInfo(id));

            return View(new ActivationProgramViewModel { ProgramCode = cd });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ActivationProgram(ActivationProgramViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            return View();
        }
    }
}