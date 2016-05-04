using DMS_WebAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DMS_WebAPI.Controllers
{
    public class LicenseController : Controller
    {
        public ActionResult ActivationProgram()
        {
            return View(new ActivationProgramViewModel { ProgramCode = "spfJA7G6UW/Y+53YjhUPEkGi4YA3GZX1Ci4aIkzh24lj4nJGW1GiuX4V1QGpxjZuMiy" });
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