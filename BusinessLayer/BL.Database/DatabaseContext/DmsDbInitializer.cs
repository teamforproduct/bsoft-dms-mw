using BL.Database.DBModel.Admin;
using BL.Database.DBModel.Dictionary;
using BL.Database.DBModel.Encryption;
using BL.Database.DBModel.System;
using BL.Model.Enums;
using System;
using System.Collections.Generic;
using System.Data.Entity;

namespace BL.Database.DatabaseContext
{
    public class DmsDbInitializer : CreateDatabaseIfNotExists<DmsContext>
    {
        protected override void Seed(DmsContext context)
        {
            //context.AdminLanguagesSet.AddRange(DmsDbImportData.GetAdminLanguages());
            //context.AdminLanguageValuesSet.AddRange(DmsDbImportData.GetAdminLanguageValues());

            // ModuleFeatures последовательность вызовов важна!11
            DmsDbImportData.InitPermissions();
            context.SystemAccessTypesSet.AddRange(DmsDbImportData.GetSystemAccessTypes());
            context.SystemModulesSet.AddRange(DmsDbImportData.GetSystemModules());
            context.SystemFeaturesSet.AddRange(DmsDbImportData.GetSystemFeatures());
            context.SystemPermissionsSet.AddRange(DmsDbImportData.GetSystemPermissions());
            // ModuleFeatures

            context.AdminAccessLevelsSet.AddRange(DmsDbImportData.GetAdminAccessLevels());
            context.SystemObjectsSet.AddRange(DmsDbImportData.GetSystemObjects());
            context.SystemActionsSet.AddRange(DmsDbImportData.GetSystemActions());
            context.SystemUIElementsSet.AddRange(DmsDbImportData.GetSystemUIElements());
            context.SystemValueTypesSet.AddRange(DmsDbImportData.GetSystemValueTypes());
            context.SystemFormulasSet.AddRange(DmsDbImportData.GetSystemFormulas());
            context.SystemPatternsSet.AddRange(DmsDbImportData.GetSystemPatterns());
            context.DictionaryDocumentDirectionsSet.AddRange(DmsDbImportData.GetDictionaryDocumentDirections());
            context.DictionaryEventTypesSet.AddRange(DmsDbImportData.GetDictionaryEventTypes());
            context.DictionaryImportanceEventTypesSet.AddRange(DmsDbImportData.GetDictionaryImportanceEventTypes());
            context.DictionaryResultTypesSet.AddRange(DmsDbImportData.GetDictionaryResultTypes());
            context.DictionarySendTypesSet.AddRange(DmsDbImportData.GetDictionarySendTypes());
            context.DictionaryStageTypesSet.AddRange(DmsDbImportData.GetDictionaryStageTypes());
            context.DictionarySubordinationTypesSet.AddRange(DmsDbImportData.GetDictionarySubordinationTypes());
            context.DictionaryRegistrationJournalAccessTypesSet.AddRange(DmsDbImportData.GetDictionaryRegistrationJournalAccessTypes());
            context.DictionarySubscriptionStatesSet.AddRange(DmsDbImportData.GetDictionarySubscriptionStates());
            context.DictionaryPositionExecutorTypesSet.AddRange(DmsDbImportData.GetDictionaryPositionExecutorTypes());
            context.DictionaryLinkTypesSet.AddRange(DmsDbImportData.GetDictionaryLinkTypes());
            context.DictionaryFileTypesSet.AddRange(DmsDbImportData.GetDictionaryFileTypes());
            context.DictionarySigningTypesSet.AddRange(DmsDbImportData.GetDictionarySigningTypes());
            base.Seed(context);
        }
    }
}
