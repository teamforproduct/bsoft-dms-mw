using System.Data.Entity;

namespace DMS_WebAPI.DatabaseContext
{
    public class ApplicationDbInitializer : CreateDatabaseIfNotExists<ApplicationDbContext>
    {
        protected override void Seed(ApplicationDbContext context)
        {
            context.AdminLanguagesSet.AddRange(ApplicationDbImportData.GetAdminLanguages());

            context.SystemValueTypesSet.AddRange(ApplicationDbImportData.GetSystemValueTypes());

            context.SystemSettingsSet.AddRange(ApplicationDbImportData.GetSystemSettings());

            context.AspNetLicencesSet.AddRange(ApplicationDbImportData.GetAspNetLicences());

            context.SystemControlQuestionsSet.AddRange(ApplicationDbImportData.GetSystemControlQuestions());


            //context.AdminLanguageValuesSet.AddRange(ApplicationDbImportData.GetAdminLanguageValues());

            base.Seed(context);
        }

    }
}
