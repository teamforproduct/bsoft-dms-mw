﻿using System.Data.Entity;

namespace BL.Database.DatabaseContext
{
    public class DmsDbInitializer : CreateDatabaseIfNotExists<DmsContext>
    {
        protected override void Seed(DmsContext context)
        {

            // ModuleFeatures последовательность вызовов важна!!!
            DmsDbImportData.InitPermissions();
            context.SystemAccessTypesSet.AddRange(DmsDbImportData.GetSystemAccessTypes());
            context.SystemModulesSet.AddRange(DmsDbImportData.GetSystemModules());
            context.SystemFeaturesSet.AddRange(DmsDbImportData.GetSystemFeatures());
            context.SystemPermissionsSet.AddRange(DmsDbImportData.GetSystemPermissions());
            // ModuleFeatures

            context.AdminRolesTypesSet.AddRange(DmsDbImportData.GetAdminRoleTypes());
            context.AdminAccessLevelsSet.AddRange(DmsDbImportData.GetAdminAccessLevels());
            context.SystemObjectsSet.AddRange(DmsDbImportData.GetSystemObjects());
            context.SystemActionsSet.AddRange(DmsDbImportData.GetSystemActions());
            context.SystemUIElementsSet.AddRange(DmsDbImportData.GetSystemUIElements());
            context.SystemValueTypesSet.AddRange(DmsDbImportData.GetSystemValueTypes());
            context.SystemFormatsSet.AddRange(DmsDbImportData.GetSystemFormats());
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
            context.DictionarySettingTypesSet.AddRange(DmsDbImportData.GetDictionarySettingTypes());
            base.Seed(context);
        }
    }
}
