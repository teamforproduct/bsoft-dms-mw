﻿using DMS_WebAPI.DBModel;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace DMS_WebAPI.Models
{
    public class ApplicationDbInitializer : CreateDatabaseIfNotExists<ApplicationDbContext>
    {
        protected override void Seed(ApplicationDbContext context)
        {
            context.AspNetLicencesSet.AddRange(ApplicationDbImportData.GetAspNetLicences());

            context.AdminLanguagesSet.AddRange(ApplicationDbImportData.GetAdminLanguages());

            context.AdminLanguageValuesSet.AddRange(ApplicationDbImportData.GetAdminLanguageValues());

            base.Seed(context);
        }

    }
}
