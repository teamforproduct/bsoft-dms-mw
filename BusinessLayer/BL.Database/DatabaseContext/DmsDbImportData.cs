using BL.Database.DBModel.Admin;
using BL.Database.DBModel.Dictionary;
using BL.Database.DBModel.System;
using BL.Model.Common;
using BL.Model.Constants;
using BL.Model.Enums;
using LinqKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Database.DatabaseContext
{
    public static class DmsDbImportData
    {

        private static int IdSequence = 0;

        #region [+] SystemObjects ...

        public static List<SystemObjects> GetSystemObjects()
        {
            var items = new List<SystemObjects>();

            items.Add(GetSystemObjects(EnumObjects.System));
            items.Add(GetSystemObjects(EnumObjects.SystemObjects));
            items.Add(GetSystemObjects(EnumObjects.SystemActions));

            items.Add(GetSystemObjects(EnumObjects.Documents));
            items.Add(GetSystemObjects(EnumObjects.DocumentAccesses));
            items.Add(GetSystemObjects(EnumObjects.DocumentRestrictedSendLists));
            items.Add(GetSystemObjects(EnumObjects.DocumentSendLists));
            items.Add(GetSystemObjects(EnumObjects.DocumentFiles));
            items.Add(GetSystemObjects(EnumObjects.DocumentLinks));
            items.Add(GetSystemObjects(EnumObjects.DocumentSendListStages));
            items.Add(GetSystemObjects(EnumObjects.DocumentEvents));
            items.Add(GetSystemObjects(EnumObjects.DocumentWaits));
            items.Add(GetSystemObjects(EnumObjects.DocumentSubscriptions));
            items.Add(GetSystemObjects(EnumObjects.DocumentTasks));
            items.Add(GetSystemObjects(EnumObjects.DocumentPapers));
            items.Add(GetSystemObjects(EnumObjects.DocumentPaperEvents));
            items.Add(GetSystemObjects(EnumObjects.DocumentPaperLists));
            items.Add(GetSystemObjects(EnumObjects.DocumentSavedFilters));
            items.Add(GetSystemObjects(EnumObjects.DocumentTags));
            items.Add(GetSystemObjects(EnumObjects.DictionaryDocumentType));
            items.Add(GetSystemObjects(EnumObjects.DictionaryAddressType));
            items.Add(GetSystemObjects(EnumObjects.DictionaryDocumentSubjects));
            items.Add(GetSystemObjects(EnumObjects.DictionaryRegistrationJournals));
            items.Add(GetSystemObjects(EnumObjects.DictionaryContactType));
            items.Add(GetSystemObjects(EnumObjects.DictionaryAgents));
            items.Add(GetSystemObjects(EnumObjects.DictionaryContacts));
            items.Add(GetSystemObjects(EnumObjects.DictionaryAgentAddresses));
            items.Add(GetSystemObjects(EnumObjects.DictionaryAgentPersons));
            items.Add(GetSystemObjects(EnumObjects.DictionaryDepartments));
            items.Add(GetSystemObjects(EnumObjects.DictionaryPositions));
            items.Add(GetSystemObjects(EnumObjects.DictionaryAgentEmployees));
            items.Add(GetSystemObjects(EnumObjects.DictionaryAgentCompanies));
            items.Add(GetSystemObjects(EnumObjects.DictionaryAgentBanks));
            items.Add(GetSystemObjects(EnumObjects.DictionaryAgentAccounts));
            items.Add(GetSystemObjects(EnumObjects.DictionaryStandartSendListContent));
            items.Add(GetSystemObjects(EnumObjects.DictionaryStandartSendLists));
            items.Add(GetSystemObjects(EnumObjects.DictionaryAgentClientCompanies));
            items.Add(GetSystemObjects(EnumObjects.DictionaryPositionExecutorTypes));
            items.Add(GetSystemObjects(EnumObjects.DictionaryPositionExecutors));
            items.Add(GetSystemObjects(EnumObjects.TemplateDocument));
            items.Add(GetSystemObjects(EnumObjects.TemplateDocumentSendList));
            items.Add(GetSystemObjects(EnumObjects.TemplateDocumentRestrictedSendList));
            items.Add(GetSystemObjects(EnumObjects.TemplateDocumentTask));
            items.Add(GetSystemObjects(EnumObjects.TemplateDocumentAttachedFiles));
            items.Add(GetSystemObjects(EnumObjects.DictionaryTag));
            items.Add(GetSystemObjects(EnumObjects.CustomDictionaryTypes));
            items.Add(GetSystemObjects(EnumObjects.CustomDictionaries));
            items.Add(GetSystemObjects(EnumObjects.Properties));
            items.Add(GetSystemObjects(EnumObjects.PropertyLinks));
            items.Add(GetSystemObjects(EnumObjects.PropertyValues));

            items.Add(GetSystemObjects(EnumObjects.EncryptionCertificates));

            items.Add(GetSystemObjects(EnumObjects.AdminRoles));
            items.Add(GetSystemObjects(EnumObjects.AdminRoleAction));
            items.Add(GetSystemObjects(EnumObjects.AdminPositionRoles));
            items.Add(GetSystemObjects(EnumObjects.AdminUserRoles));
            items.Add(GetSystemObjects(EnumObjects.AdminSubordination));
            //items.Add(GetSystemObjects(EnumObjects.DepartmentAdmin));

            items.Add(GetSystemObjects(EnumObjects.SystemSettings));

            items.Add(GetSystemObjects(EnumObjects.DictionaryAgentUsers));

            return items;
        }

        private static SystemObjects GetSystemObjects(EnumObjects id)
        {
            string description = "##l@" + "Objects" + ":" + id.ToString() + "@l##";

            return new SystemObjects()
            {
                Id = (int)id,
                Code = id.ToString(),
                Description = description
            };
        }
        #endregion

        #region [+] SystemActions ...
        public static List<SystemActions> GetSystemActions()
        {
            var items = new List<SystemActions>();

            items.Add(GetSysAct(EnumDocumentActions.ViewDocument, EnumObjects.Documents, category: "Документ", isVisibleInMenu: false));
            items.Add(GetSysAct(EnumDocumentActions.AddDocument, EnumObjects.Documents, category: "Документ"));
            items.Add(GetSysAct(EnumDocumentActions.CopyDocument, EnumObjects.Documents, category: "Документ"));
            items.Add(GetSysAct(EnumDocumentActions.ModifyDocument, EnumObjects.Documents, category: "Документ"));
            items.Add(GetSysAct(EnumDocumentActions.DeleteDocument, EnumObjects.Documents, category: "Документ"));
            items.Add(GetSysAct(EnumDocumentActions.LaunchPlan, EnumObjects.Documents, category: "Действия"));
            items.Add(GetSysAct(EnumDocumentActions.AddDocumentSendListItem, EnumObjects.Documents, category: "Действия"));
            items.Add(GetSysAct(EnumDocumentActions.StopPlan, EnumObjects.Documents, category: "Действия"));
            items.Add(GetSysAct(EnumDocumentActions.ChangeExecutor, EnumObjects.Documents, category: "Действия"));
            items.Add(GetSysAct(EnumDocumentActions.RegisterDocument, EnumObjects.Documents, category: "Действия"));
            items.Add(GetSysAct(EnumDocumentActions.MarkDocumentEventAsRead, EnumObjects.Documents, category: "Информирование", isVisibleInMenu: false));
            items.Add(GetSysAct(EnumDocumentActions.SendDocument, EnumObjects.Documents, category: "Действия"));
            items.Add(GetSysAct(EnumDocumentActions.SendForInformation, EnumObjects.Documents, category: "Информирование", isVisibleInMenu: false));
            items.Add(GetSysAct(EnumDocumentActions.SendForConsideration, EnumObjects.Documents, category: "Информирование", isVisibleInMenu: false));
            items.Add(GetSysAct(EnumDocumentActions.SendForInformationExternal, EnumObjects.Documents, category: "Информирование", isVisibleInMenu: false));
            items.Add(GetSysAct(EnumDocumentActions.SendForControl, EnumObjects.Documents, category: "Контроль", isVisibleInMenu: false));
            //items.Add(GetSysAct(EnumDocumentActions.SendForControlChange, EnumObjects.Documents, category: "Контроль"));
            items.Add(GetSysAct(EnumDocumentActions.SendForResponsibleExecution, EnumObjects.Documents, category: "Контроль", isVisibleInMenu: false));
            items.Add(GetSysAct(EnumDocumentActions.SendForExecution, EnumObjects.Documents, category: "Контроль", isVisibleInMenu: false));
            items.Add(GetSysAct(EnumDocumentActions.SendForVisaing, EnumObjects.Documents, category: "Подписание", isVisibleInMenu: false));
            items.Add(GetSysAct(EnumDocumentActions.SendForАgreement, EnumObjects.Documents, category: "Подписание", isVisibleInMenu: false));
            items.Add(GetSysAct(EnumDocumentActions.SendForАpproval, EnumObjects.Documents, category: "Подписание", isVisibleInMenu: false));
            items.Add(GetSysAct(EnumDocumentActions.SendForSigning, EnumObjects.Documents, category: "Подписание", isVisibleInMenu: false));
            items.Add(GetSysAct(EnumDocumentActions.ReportRegistrationCardDocument, EnumObjects.Documents, category: "Документ"));
            items.Add(GetSysAct(EnumDocumentActions.ReportRegisterTransmissionDocuments, EnumObjects.Documents, category: "Отчеты"));
            items.Add(GetSysAct(EnumDocumentActions.ReportDocumentForDigitalSignature, EnumObjects.Documents, category: "Подписание"));
            items.Add(GetSysAct(EnumDocumentActions.AddFavourite, EnumObjects.Documents, category: "Дополнительно", isGrantable: false));
            items.Add(GetSysAct(EnumDocumentActions.DeleteFavourite, EnumObjects.Documents, category: "Дополнительно", isGrantable: false));
            items.Add(GetSysAct(EnumDocumentActions.FinishWork, EnumObjects.Documents, category: "Дополнительно", isGrantable: false));
            items.Add(GetSysAct(EnumDocumentActions.StartWork, EnumObjects.Documents, category: "Дополнительно", isGrantable: false));
            items.Add(GetSysAct(EnumDocumentActions.ChangePosition, EnumObjects.Documents, category: "Администратор", isVisibleInMenu: false));
            items.Add(GetSysAct(EnumDocumentActions.AddDocumentRestrictedSendList, EnumObjects.DocumentRestrictedSendLists));
            items.Add(GetSysAct(EnumDocumentActions.AddByStandartSendListDocumentRestrictedSendList, EnumObjects.DocumentRestrictedSendLists));
            items.Add(GetSysAct(EnumDocumentActions.DeleteDocumentRestrictedSendList, EnumObjects.DocumentRestrictedSendLists));
            items.Add(GetSysAct(EnumDocumentActions.ModifyDocumentSendList, EnumObjects.DocumentSendLists));
            items.Add(GetSysAct(EnumDocumentActions.DeleteDocumentSendList, EnumObjects.DocumentSendLists));
            items.Add(GetSysAct(EnumDocumentActions.CopyDocumentSendList, EnumObjects.DocumentSendLists, isVisibleInMenu: false));
            items.Add(GetSysAct(EnumDocumentActions.LaunchDocumentSendListItem, EnumObjects.DocumentSendLists));
            items.Add(GetSysAct(EnumDocumentActions.AddDocumentFile, EnumObjects.DocumentFiles));
            items.Add(GetSysAct(EnumDocumentActions.ModifyDocumentFile, EnumObjects.DocumentFiles));
            items.Add(GetSysAct(EnumDocumentActions.DeleteDocumentFile, EnumObjects.DocumentFiles));
            items.Add(GetSysAct(EnumDocumentActions.AddDocumentFileUseMainNameFile, EnumObjects.DocumentFiles));
            items.Add(GetSysAct(EnumDocumentActions.AcceptDocumentFile, EnumObjects.DocumentFiles));
            items.Add(GetSysAct(EnumDocumentActions.RejectDocumentFile, EnumObjects.DocumentFiles));
            items.Add(GetSysAct(EnumDocumentActions.RenameDocumentFile, EnumObjects.DocumentFiles));
            items.Add(GetSysAct(EnumDocumentActions.DeleteDocumentFileVersion, EnumObjects.DocumentFiles));
            items.Add(GetSysAct(EnumDocumentActions.DeleteDocumentFileVersionRecord, EnumObjects.DocumentFiles));
            items.Add(GetSysAct(EnumDocumentActions.AcceptMainVersionDocumentFile, EnumObjects.DocumentFiles));
            items.Add(GetSysAct(EnumDocumentActions.AddDocumentLink, EnumObjects.DocumentLinks));
            items.Add(GetSysAct(EnumDocumentActions.DeleteDocumentLink, EnumObjects.DocumentLinks));
            items.Add(GetSysAct(EnumDocumentActions.AddDocumentSendList, EnumObjects.DocumentSendListStages));
            items.Add(GetSysAct(EnumDocumentActions.AddByStandartSendListDocumentSendList, EnumObjects.DocumentSendListStages));
            items.Add(GetSysAct(EnumDocumentActions.AddDocumentSendListStage, EnumObjects.DocumentSendListStages));
            items.Add(GetSysAct(EnumDocumentActions.DeleteDocumentSendListStage, EnumObjects.DocumentSendListStages));
            items.Add(GetSysAct(EnumDocumentActions.SendMessage, EnumObjects.DocumentEvents, category: "Информирование", isGrantable: false));
            items.Add(GetSysAct(EnumDocumentActions.AddNote, EnumObjects.DocumentEvents, category: "Информирование", isGrantable: false));
            items.Add(GetSysAct(EnumDocumentActions.ControlOn, EnumObjects.DocumentWaits, category: "Контроль"));
            items.Add(GetSysAct(EnumDocumentActions.ControlChange, EnumObjects.DocumentWaits, category: "Контроль"));
            items.Add(GetSysAct(EnumDocumentActions.SendForExecutionChange, EnumObjects.DocumentWaits, category: "Контроль"));
            items.Add(GetSysAct(EnumDocumentActions.SendForResponsibleExecutionChange, EnumObjects.DocumentWaits, category: "Контроль"));
            items.Add(GetSysAct(EnumDocumentActions.ControlTargetChange, EnumObjects.DocumentWaits, category: "Контроль"));
            items.Add(GetSysAct(EnumDocumentActions.ControlOff, EnumObjects.DocumentWaits, category: "Контроль"));
            items.Add(GetSysAct(EnumDocumentActions.MarkExecution, EnumObjects.DocumentWaits, category: "Контроль"));
            items.Add(GetSysAct(EnumDocumentActions.AcceptResult, EnumObjects.DocumentWaits, category: "Контроль"));
            items.Add(GetSysAct(EnumDocumentActions.RejectResult, EnumObjects.DocumentWaits, category: "Контроль"));
            items.Add(GetSysAct(EnumDocumentActions.WithdrawVisaing, EnumObjects.DocumentSubscriptions, category: "Подписание"));
            items.Add(GetSysAct(EnumDocumentActions.WithdrawАgreement, EnumObjects.DocumentSubscriptions, category: "Подписание"));
            items.Add(GetSysAct(EnumDocumentActions.WithdrawАpproval, EnumObjects.DocumentSubscriptions, category: "Подписание"));
            items.Add(GetSysAct(EnumDocumentActions.WithdrawSigning, EnumObjects.DocumentSubscriptions, category: "Подписание"));
            items.Add(GetSysAct(EnumDocumentActions.AffixVisaing, EnumObjects.DocumentSubscriptions, category: "Подписание"));
            items.Add(GetSysAct(EnumDocumentActions.AffixАgreement, EnumObjects.DocumentSubscriptions, category: "Подписание"));
            items.Add(GetSysAct(EnumDocumentActions.AffixАpproval, EnumObjects.DocumentSubscriptions, category: "Подписание"));
            items.Add(GetSysAct(EnumDocumentActions.AffixSigning, EnumObjects.DocumentSubscriptions, category: "Подписание"));
            items.Add(GetSysAct(EnumDocumentActions.SelfAffixSigning, EnumObjects.DocumentSubscriptions, category: "Подписание", isVisibleInMenu: false));
            items.Add(GetSysAct(EnumDocumentActions.RejectVisaing, EnumObjects.DocumentSubscriptions, category: "Подписание"));
            items.Add(GetSysAct(EnumDocumentActions.RejectАgreement, EnumObjects.DocumentSubscriptions, category: "Подписание"));
            items.Add(GetSysAct(EnumDocumentActions.RejectАpproval, EnumObjects.DocumentSubscriptions, category: "Подписание"));
            items.Add(GetSysAct(EnumDocumentActions.RejectSigning, EnumObjects.DocumentSubscriptions, category: "Подписание"));
            items.Add(GetSysAct(EnumDocumentActions.VerifySigning, EnumObjects.DocumentSubscriptions, category: "Подписание"));
            items.Add(GetSysAct(EnumDocumentActions.AddDocumentTask, EnumObjects.DocumentTasks));
            items.Add(GetSysAct(EnumDocumentActions.ModifyDocumentTask, EnumObjects.DocumentTasks));
            items.Add(GetSysAct(EnumDocumentActions.DeleteDocumentTask, EnumObjects.DocumentTasks));
            items.Add(GetSysAct(EnumDocumentActions.AddDocumentPaper, EnumObjects.DocumentPapers, category: "Бумажные носители"));
            items.Add(GetSysAct(EnumDocumentActions.ModifyDocumentPaper, EnumObjects.DocumentPapers, category: "Бумажные носители"));
            items.Add(GetSysAct(EnumDocumentActions.MarkOwnerDocumentPaper, EnumObjects.DocumentPapers, category: "Бумажные носители"));
            items.Add(GetSysAct(EnumDocumentActions.MarkСorruptionDocumentPaper, EnumObjects.DocumentPapers, category: "Бумажные носители"));
            items.Add(GetSysAct(EnumDocumentActions.DeleteDocumentPaper, EnumObjects.DocumentPapers, category: "Бумажные носители"));
            items.Add(GetSysAct(EnumDocumentActions.PlanDocumentPaperEvent, EnumObjects.DocumentPaperEvents, category: "Бумажные носители"));
            items.Add(GetSysAct(EnumDocumentActions.CancelPlanDocumentPaperEvent, EnumObjects.DocumentPaperEvents, category: "Бумажные носители"));
            items.Add(GetSysAct(EnumDocumentActions.SendDocumentPaperEvent, EnumObjects.DocumentPaperEvents, category: "Бумажные носители"));
            items.Add(GetSysAct(EnumDocumentActions.CancelSendDocumentPaperEvent, EnumObjects.DocumentPaperEvents, category: "Бумажные носители"));
            items.Add(GetSysAct(EnumDocumentActions.RecieveDocumentPaperEvent, EnumObjects.DocumentPaperEvents, category: "Бумажные носители"));
            items.Add(GetSysAct(EnumDocumentActions.AddDocumentPaperList, EnumObjects.DocumentPaperLists, category: "Реестры бумажных носителей"));
            items.Add(GetSysAct(EnumDocumentActions.ModifyDocumentPaperList, EnumObjects.DocumentPaperLists, category: "Реестры бумажных носителей"));
            items.Add(GetSysAct(EnumDocumentActions.DeleteDocumentPaperList, EnumObjects.DocumentPaperLists, category: "Реестры бумажных носителей"));
            items.Add(GetSysAct(EnumDocumentActions.AddSavedFilter, EnumObjects.DocumentSavedFilters));
            items.Add(GetSysAct(EnumDocumentActions.ModifySavedFilter, EnumObjects.DocumentSavedFilters));
            items.Add(GetSysAct(EnumDocumentActions.DeleteSavedFilter, EnumObjects.DocumentSavedFilters));
            items.Add(GetSysAct(EnumDocumentActions.ModifyDocumentTags, EnumObjects.DocumentTags));
            items.Add(GetSysAct(EnumDocumentActions.AddTemplateDocument, EnumObjects.TemplateDocument));
            items.Add(GetSysAct(EnumDocumentActions.ModifyTemplateDocument, EnumObjects.TemplateDocument));
            items.Add(GetSysAct(EnumDocumentActions.DeleteTemplateDocument, EnumObjects.TemplateDocument));
            items.Add(GetSysAct(EnumDocumentActions.AddTemplateDocumentSendList, EnumObjects.TemplateDocumentSendList));
            items.Add(GetSysAct(EnumDocumentActions.ModifyTemplateDocumentSendList, EnumObjects.TemplateDocumentSendList));
            items.Add(GetSysAct(EnumDocumentActions.DeleteTemplateDocumentSendList, EnumObjects.TemplateDocumentSendList));
            items.Add(GetSysAct(EnumDocumentActions.AddTemplateDocumentRestrictedSendList, EnumObjects.TemplateDocumentRestrictedSendList));
            items.Add(GetSysAct(EnumDocumentActions.ModifyTemplateDocumentRestrictedSendList, EnumObjects.TemplateDocumentRestrictedSendList));
            items.Add(GetSysAct(EnumDocumentActions.DeleteTemplateDocumentRestrictedSendList, EnumObjects.TemplateDocumentRestrictedSendList));
            items.Add(GetSysAct(EnumDocumentActions.AddTemplateDocumentTask, EnumObjects.TemplateDocumentTask));
            items.Add(GetSysAct(EnumDocumentActions.ModifyTemplateDocumentTask, EnumObjects.TemplateDocumentTask));
            items.Add(GetSysAct(EnumDocumentActions.DeleteTemplateDocumentTask, EnumObjects.TemplateDocumentTask));
            items.Add(GetSysAct(EnumDocumentActions.AddTemplateAttachedFile, EnumObjects.TemplateDocumentAttachedFiles));
            items.Add(GetSysAct(EnumDocumentActions.ModifyTemplateAttachedFile, EnumObjects.TemplateDocumentAttachedFiles));
            items.Add(GetSysAct(EnumDocumentActions.DeleteTemplateAttachedFile, EnumObjects.TemplateDocumentAttachedFiles));

            items.Add(GetSysAct(EnumDictionaryActions.AddDocumentType, EnumObjects.DictionaryDocumentType));
            items.Add(GetSysAct(EnumDictionaryActions.ModifyDocumentType, EnumObjects.DictionaryDocumentType));
            items.Add(GetSysAct(EnumDictionaryActions.DeleteDocumentType, EnumObjects.DictionaryDocumentType));
            items.Add(GetSysAct(EnumDictionaryActions.AddAddressType, EnumObjects.DictionaryAddressType));
            items.Add(GetSysAct(EnumDictionaryActions.ModifyAddressType, EnumObjects.DictionaryAddressType));
            items.Add(GetSysAct(EnumDictionaryActions.DeleteAddressType, EnumObjects.DictionaryAddressType));
            items.Add(GetSysAct(EnumDictionaryActions.AddDocumentSubject, EnumObjects.DictionaryDocumentSubjects));
            items.Add(GetSysAct(EnumDictionaryActions.ModifyDocumentSubject, EnumObjects.DictionaryDocumentSubjects));
            items.Add(GetSysAct(EnumDictionaryActions.DeleteDocumentSubject, EnumObjects.DictionaryDocumentSubjects));
            items.Add(GetSysAct(EnumDictionaryActions.AddRegistrationJournal, EnumObjects.DictionaryRegistrationJournals));
            items.Add(GetSysAct(EnumDictionaryActions.ModifyRegistrationJournal, EnumObjects.DictionaryRegistrationJournals));
            items.Add(GetSysAct(EnumDictionaryActions.DeleteRegistrationJournal, EnumObjects.DictionaryRegistrationJournals));
            items.Add(GetSysAct(EnumDictionaryActions.AddContactType, EnumObjects.DictionaryContactType));
            items.Add(GetSysAct(EnumDictionaryActions.ModifyContactType, EnumObjects.DictionaryContactType));
            items.Add(GetSysAct(EnumDictionaryActions.DeleteContactType, EnumObjects.DictionaryContactType));
            items.Add(GetSysAct(EnumDictionaryActions.AddAgent, EnumObjects.DictionaryAgents));
            items.Add(GetSysAct(EnumDictionaryActions.ModifyAgent, EnumObjects.DictionaryAgents));
            items.Add(GetSysAct(EnumDictionaryActions.DeleteAgent, EnumObjects.DictionaryAgents));
            items.Add(GetSysAct(EnumDictionaryActions.SetAgentImage, EnumObjects.DictionaryAgents));
            items.Add(GetSysAct(EnumDictionaryActions.DeleteAgentImage, EnumObjects.DictionaryAgents));
            items.Add(GetSysAct(EnumDictionaryActions.AddAgentContact, EnumObjects.DictionaryContacts));
            items.Add(GetSysAct(EnumDictionaryActions.ModifyAgentContact, EnumObjects.DictionaryContacts));
            items.Add(GetSysAct(EnumDictionaryActions.DeleteAgentContact, EnumObjects.DictionaryContacts));
            items.Add(GetSysAct(EnumDictionaryActions.AddAgentAddress, EnumObjects.DictionaryAgentAddresses));
            items.Add(GetSysAct(EnumDictionaryActions.ModifyAgentAddress, EnumObjects.DictionaryAgentAddresses));
            items.Add(GetSysAct(EnumDictionaryActions.DeleteAgentAddress, EnumObjects.DictionaryAgentAddresses));
            items.Add(GetSysAct(EnumDictionaryActions.AddAgentPerson, EnumObjects.DictionaryAgentPersons));
            items.Add(GetSysAct(EnumDictionaryActions.ModifyAgentPerson, EnumObjects.DictionaryAgentPersons));
            items.Add(GetSysAct(EnumDictionaryActions.DeleteAgentPerson, EnumObjects.DictionaryAgentPersons));


            items.Add(GetSysAct(EnumDictionaryActions.AddAgentEmployee, EnumObjects.DictionaryAgentEmployees));
            items.Add(GetSysAct(EnumDictionaryActions.ModifyAgentEmployee, EnumObjects.DictionaryAgentEmployees));
            items.Add(GetSysAct(EnumDictionaryActions.DeleteAgentEmployee, EnumObjects.DictionaryAgentEmployees));
            items.Add(GetSysAct(EnumDictionaryActions.ModifyAgentEmployeeLanguage, EnumObjects.DictionaryAgentEmployees, isVisible: false , grantId : (int)EnumDictionaryActions.ModifyAgentEmployee));

            items.Add(GetSysAct(EnumDictionaryActions.AddDepartment, EnumObjects.DictionaryDepartments));
            items.Add(GetSysAct(EnumDictionaryActions.ModifyDepartment, EnumObjects.DictionaryDepartments));
            items.Add(GetSysAct(EnumDictionaryActions.DeleteDepartment, EnumObjects.DictionaryDepartments));

            items.Add(GetSysAct(EnumDictionaryActions.AddPosition, EnumObjects.DictionaryPositions));
            items.Add(GetSysAct(EnumDictionaryActions.ModifyPosition, EnumObjects.DictionaryPositions));
            items.Add(GetSysAct(EnumDictionaryActions.DeletePosition, EnumObjects.DictionaryPositions));

            items.Add(GetSysAct(EnumDictionaryActions.AddAgentCompany, EnumObjects.DictionaryAgentCompanies));
            items.Add(GetSysAct(EnumDictionaryActions.ModifyAgentCompany, EnumObjects.DictionaryAgentCompanies));
            items.Add(GetSysAct(EnumDictionaryActions.DeleteAgentCompany, EnumObjects.DictionaryAgentCompanies));

            items.Add(GetSysAct(EnumDictionaryActions.AddAgentBank, EnumObjects.DictionaryAgentBanks));
            items.Add(GetSysAct(EnumDictionaryActions.ModifyAgentBank, EnumObjects.DictionaryAgentBanks));
            items.Add(GetSysAct(EnumDictionaryActions.DeleteAgentBank, EnumObjects.DictionaryAgentBanks));
            items.Add(GetSysAct(EnumDictionaryActions.AddAgentAccount, EnumObjects.DictionaryAgentAccounts));
            items.Add(GetSysAct(EnumDictionaryActions.ModifyAgentAccount, EnumObjects.DictionaryAgentAccounts));
            items.Add(GetSysAct(EnumDictionaryActions.DeleteAgentAccount, EnumObjects.DictionaryAgentAccounts));
            items.Add(GetSysAct(EnumDictionaryActions.AddStandartSendListContent, EnumObjects.DictionaryStandartSendListContent));
            items.Add(GetSysAct(EnumDictionaryActions.ModifyStandartSendListContent, EnumObjects.DictionaryStandartSendListContent));
            items.Add(GetSysAct(EnumDictionaryActions.DeleteStandartSendListContent, EnumObjects.DictionaryStandartSendListContent));
            items.Add(GetSysAct(EnumDictionaryActions.AddStandartSendList, EnumObjects.DictionaryStandartSendLists));
            items.Add(GetSysAct(EnumDictionaryActions.ModifyStandartSendList, EnumObjects.DictionaryStandartSendLists));
            items.Add(GetSysAct(EnumDictionaryActions.DeleteStandartSendList, EnumObjects.DictionaryStandartSendLists));
            items.Add(GetSysAct(EnumDictionaryActions.AddAgentClientCompany, EnumObjects.DictionaryAgentClientCompanies));
            items.Add(GetSysAct(EnumDictionaryActions.ModifyAgentClientCompany, EnumObjects.DictionaryAgentClientCompanies));
            items.Add(GetSysAct(EnumDictionaryActions.DeleteAgentClientCompany, EnumObjects.DictionaryAgentClientCompanies));
            items.Add(GetSysAct(EnumDictionaryActions.AddExecutorType, EnumObjects.DictionaryPositionExecutorTypes));
            items.Add(GetSysAct(EnumDictionaryActions.ModifyExecutorType, EnumObjects.DictionaryPositionExecutorTypes));
            items.Add(GetSysAct(EnumDictionaryActions.DeleteExecutorType, EnumObjects.DictionaryPositionExecutorTypes));
            items.Add(GetSysAct(EnumDictionaryActions.AddExecutor, EnumObjects.DictionaryPositionExecutors));
            items.Add(GetSysAct(EnumDictionaryActions.ModifyExecutor, EnumObjects.DictionaryPositionExecutors));
            items.Add(GetSysAct(EnumDictionaryActions.DeleteExecutor, EnumObjects.DictionaryPositionExecutors));
            items.Add(GetSysAct(EnumDictionaryActions.AddTag, EnumObjects.DictionaryTag));
            items.Add(GetSysAct(EnumDictionaryActions.ModifyTag, EnumObjects.DictionaryTag));
            items.Add(GetSysAct(EnumDictionaryActions.DeleteTag, EnumObjects.DictionaryTag));
            items.Add(GetSysAct(EnumDictionaryActions.AddCustomDictionaryType, EnumObjects.CustomDictionaryTypes));
            items.Add(GetSysAct(EnumDictionaryActions.ModifyCustomDictionaryType, EnumObjects.CustomDictionaryTypes));
            items.Add(GetSysAct(EnumDictionaryActions.DeleteCustomDictionaryType, EnumObjects.CustomDictionaryTypes));
            items.Add(GetSysAct(EnumDictionaryActions.AddCustomDictionary, EnumObjects.CustomDictionaries));
            items.Add(GetSysAct(EnumDictionaryActions.ModifyCustomDictionary, EnumObjects.CustomDictionaries));
            items.Add(GetSysAct(EnumDictionaryActions.DeleteCustomDictionary, EnumObjects.CustomDictionaries));
            items.Add(GetSysAct(EnumPropertyActions.AddProperty, EnumObjects.Properties));
            items.Add(GetSysAct(EnumPropertyActions.ModifyProperty, EnumObjects.Properties));
            items.Add(GetSysAct(EnumPropertyActions.DeleteProperty, EnumObjects.Properties));
            items.Add(GetSysAct(EnumPropertyActions.AddPropertyLink, EnumObjects.PropertyLinks));
            items.Add(GetSysAct(EnumPropertyActions.ModifyPropertyLink, EnumObjects.PropertyLinks));
            items.Add(GetSysAct(EnumPropertyActions.DeletePropertyLink, EnumObjects.PropertyLinks));
            items.Add(GetSysAct(EnumPropertyActions.ModifyPropertyValues, EnumObjects.PropertyValues));

            items.Add(GetSysAct(EnumEncryptionActions.AddEncryptionCertificate, EnumObjects.EncryptionCertificates));
            items.Add(GetSysAct(EnumEncryptionActions.ModifyEncryptionCertificate, EnumObjects.EncryptionCertificates));
            items.Add(GetSysAct(EnumEncryptionActions.VerifyPdf, EnumObjects.EncryptionCertificates));
            items.Add(GetSysAct(EnumEncryptionActions.DeleteEncryptionCertificate, EnumObjects.EncryptionCertificates));

            items.Add(GetSysAct(EnumAdminActions.AddRole, EnumObjects.AdminRoles));
            items.Add(GetSysAct(EnumAdminActions.ModifyRole, EnumObjects.AdminRoles));
            items.Add(GetSysAct(EnumAdminActions.DeleteRole, EnumObjects.AdminRoles));

            items.Add(GetSysAct(EnumAdminActions.SetPositionRole, EnumObjects.AdminPositionRoles));
            items.Add(GetSysAct(EnumAdminActions.AddPositionRole, EnumObjects.AdminPositionRoles, isVisible: false, grantId: (int)EnumAdminActions.SetPositionRole));
            items.Add(GetSysAct(EnumAdminActions.DeletePositionRole, EnumObjects.AdminPositionRoles, isVisible: false, grantId: (int)EnumAdminActions.SetPositionRole));
            items.Add(GetSysAct(EnumAdminActions.DuplicatePositionRoles, EnumObjects.AdminPositionRoles, isVisible: false, grantId: (int)EnumAdminActions.SetPositionRole));

            items.Add(GetSysAct(EnumAdminActions.SetRoleAction, EnumObjects.AdminRoleAction));
            items.Add(GetSysAct(EnumAdminActions.AddRoleAction, EnumObjects.AdminRoleAction, isVisible: false, grantId: (int)EnumAdminActions.SetRoleAction));
            items.Add(GetSysAct(EnumAdminActions.DeleteRoleAction, EnumObjects.AdminRoleAction, isVisible: false, grantId: (int)EnumAdminActions.SetRoleAction));
            items.Add(GetSysAct(EnumAdminActions.SetRoleActionByObject, EnumObjects.AdminRoleAction, isVisible: false, grantId: (int)EnumAdminActions.SetRoleAction));


            items.Add(GetSysAct(EnumAdminActions.SetUserRole, EnumObjects.AdminUserRoles));
            items.Add(GetSysAct(EnumAdminActions.AddUserRole, EnumObjects.AdminUserRoles, isVisible: false, grantId: (int)EnumAdminActions.SetUserRole));
            items.Add(GetSysAct(EnumAdminActions.DeleteUserRole, EnumObjects.AdminUserRoles, isVisible: false, grantId: (int)EnumAdminActions.SetUserRole));

            items.Add(GetSysAct(EnumAdminActions.SetSubordination, EnumObjects.AdminSubordination));
            items.Add(GetSysAct(EnumAdminActions.SetSubordinationByCompany, EnumObjects.AdminSubordination, isVisible: false, grantId: (int)EnumAdminActions.SetSubordination));
            items.Add(GetSysAct(EnumAdminActions.SetSubordinationByDepartment, EnumObjects.AdminSubordination, isVisible: false, grantId: (int)EnumAdminActions.SetSubordination));
            items.Add(GetSysAct(EnumAdminActions.SetDefaultSubordination, EnumObjects.AdminSubordination, isVisible: false, grantId: (int)EnumAdminActions.SetSubordination));
            items.Add(GetSysAct(EnumAdminActions.DuplicateSubordinations, EnumObjects.AdminSubordination, isVisible: false, grantId: (int)EnumAdminActions.SetSubordination));
            items.Add(GetSysAct(EnumAdminActions.SetAllSubordination, EnumObjects.AdminSubordination, isVisible: false, grantId: (int)EnumAdminActions.SetSubordination));

            items.Add(GetSysAct(EnumAdminActions.AddDepartmentAdmin, EnumObjects.DictionaryDepartments));
            items.Add(GetSysAct(EnumAdminActions.DeleteDepartmentAdmin, EnumObjects.DictionaryDepartments));

            items.Add(GetSysAct(EnumSystemActions.Login, EnumObjects.System, isGrantable: false, isVisible: false, isVisibleInMenu: false));
            items.Add(GetSysAct(EnumSystemActions.SetSetting, EnumObjects.SystemSettings));

            items.Add(GetSysAct(EnumAdminActions.ChangePasswordAgentUser, EnumObjects.DictionaryAgentUsers));
            items.Add(GetSysAct(EnumAdminActions.ChangeLockoutAgentUser, EnumObjects.DictionaryAgentUsers));
            items.Add(GetSysAct(EnumAdminActions.KillSessionsAgentUser, EnumObjects.DictionaryAgentUsers));
            items.Add(GetSysAct(EnumAdminActions.ChangeLoginAgentUser, EnumObjects.DictionaryAgentUsers));
            items.Add(GetSysAct(EnumAdminActions.GetAgentUserInfo, EnumObjects.DictionaryAgentUsers));

            // при добавлении действия не забудь добавить перевод! DMS_WebAPI.Models.ApplicationDbImportData GetAdminLanguageValuesForActions

            return items;
        }

        public static void CheckSystemActions()
        {
            int actionsCountByEnums =
            Enum.GetValues(typeof(EnumAdminActions)).Cast<EnumAdminActions>().Where(x => x > 0).Count() +
            Enum.GetValues(typeof(EnumEncryptionActions)).Cast<EnumEncryptionActions>().Where(x => x > 0).Count() +
            Enum.GetValues(typeof(EnumPropertyActions)).Cast<EnumPropertyActions>().Where(x => x > 0).Count() +
            Enum.GetValues(typeof(EnumDictionaryActions)).Cast<EnumDictionaryActions>().Where(x => x > 0).Count() +
            Enum.GetValues(typeof(EnumDocumentActions)).Cast<EnumDocumentActions>().Where(x => x > 0).Count() +
            Enum.GetValues(typeof(EnumSystemActions)).Cast<EnumSystemActions>().Where(x => x > 0).Count();

            var actionsCountByList = GetSystemActions().Count();

            if (actionsCountByEnums != actionsCountByList)
            {
                List<EnumModel> list = CheckSystemActions2();
                string s = string.Empty;
                foreach (var item in list)
                {
                    s += item.Value.ToString() + " - " + item.Name + "\r\n";
                }
                throw new Exception("Так не пойдет! Нужно GetSystemActions поддерживать в актуальном состоянии \r\n" + s);
            }


        }

        public static List<EnumModel> CheckSystemActions2()
        {
            var AdminActionsList = GetListByEnum<EnumAdminActions>().Where(x => x.Value > 0);
            var EncryptionActionsList = GetListByEnum<EnumEncryptionActions>().Where(x => x.Value > 0);
            var PropertyActionsList = GetListByEnum<EnumPropertyActions>().Where(x => x.Value > 0);
            var DictionaryActionsList = GetListByEnum<EnumDictionaryActions>().Where(x => x.Value > 0);
            var DocumentActionsList = GetListByEnum<EnumDocumentActions>().Where(x => x.Value > 0);
            var SystemActionsList = GetListByEnum<EnumSystemActions>().Where(x => x.Value > 0);

            var actionsList = GetSystemActions();
            List<EnumModel> ActionsList = new List<EnumModel>();
            ActionsList.AddRange(AdminActionsList);
            ActionsList.AddRange(EncryptionActionsList);
            ActionsList.AddRange(PropertyActionsList);
            ActionsList.AddRange(DictionaryActionsList);
            ActionsList.AddRange(DocumentActionsList);
            ActionsList.AddRange(SystemActionsList);

            List<EnumModel> ResList = new List<EnumModel>();

            foreach (var action in ActionsList)
            {
                if (!ExistAction(actionsList, action))
                {
                    ResList.Add(action);
                }
            }

            return ResList;

        }

        private static bool ExistAction(List<SystemActions> items, EnumModel item)
        {
            foreach (var i in items)
            {
                if (i.Id == item.Value) return true;
            }

            return false;
        }

        public static List<EnumModel> GetListByEnum<T>()
        {
            var array = (T[])(Enum.GetValues(typeof(T)).Cast<T>());
            return array
              .Select(a => new EnumModel
              {
                  Value = Convert.ToInt32(a),
                  Name = a.ToString(),
              })
              .OrderBy(kvp => kvp.Name)
              .ToList();
        }


        private static SystemActions GetSysAct(EnumAdminActions id, EnumObjects objId, string category = null, bool isGrantable = true, bool isGrantableByRecordId = false, bool isVisible = true, bool isVisibleInMenu = true, int? grantId = null)
        {
            string description = "##l@" + "AdminActions" + ":" + id.ToString() + "@l##";
            return GetSystemAction((int)id, id.ToString(), objId, description, category, isGrantable, isGrantableByRecordId, isVisible, isVisibleInMenu, grantId);
        }

        private static SystemActions GetSysAct(EnumEncryptionActions id, EnumObjects objId, string category = null, bool isGrantable = true, bool isGrantableByRecordId = false, bool isVisible = true, bool isVisibleInMenu = true, int? grantId = null)
        {
            string description = "##l@" + "EncryptionActions" + ":" + id.ToString() + "@l##";
            return GetSystemAction((int)id, id.ToString(), objId, description, category, isGrantable, isGrantableByRecordId, isVisible, isVisibleInMenu, grantId);
        }

        private static SystemActions GetSysAct(EnumPropertyActions id, EnumObjects objId, string category = null, bool isGrantable = true, bool isGrantableByRecordId = false, bool isVisible = true, bool isVisibleInMenu = true, int? grantId = null)
        {
            string description = "##l@" + "PropertyActions" + ":" + id.ToString() + "@l##";
            return GetSystemAction((int)id, id.ToString(), objId, description, category, isGrantable, isGrantableByRecordId, isVisible, isVisibleInMenu, grantId);
        }

        private static SystemActions GetSysAct(EnumDictionaryActions id, EnumObjects objId, string category = null, bool isGrantable = true, bool isGrantableByRecordId = false, bool isVisible = true, bool isVisibleInMenu = true, int? grantId = null)
        {
            string description = "##l@" + "DictionaryActions" + ":" + id.ToString() + "@l##";
            return GetSystemAction((int)id, id.ToString(), objId, description, category, isGrantable, isGrantableByRecordId, isVisible, isVisibleInMenu, grantId);
        }

        private static SystemActions GetSysAct(EnumDocumentActions id, EnumObjects objId, string category = null, bool isGrantable = true, bool isGrantableByRecordId = false, bool isVisible = true, bool isVisibleInMenu = true, int? grantId = null)
        {
            string description = "##l@" + "DocumentActions" + ":" + id.ToString() + "@l##";
            return GetSystemAction((int)id, id.ToString(), objId, description, category, isGrantable, isGrantableByRecordId, isVisible, isVisibleInMenu, grantId);
        }

        private static SystemActions GetSysAct(EnumSystemActions id, EnumObjects objId, string category = null, bool isGrantable = true, bool isGrantableByRecordId = false, bool isVisible = true, bool isVisibleInMenu = true, int? grantId = null)
        {
            string description = "##l@" + "SystemActions" + ":" + id.ToString() + "@l##";
            return GetSystemAction((int)id, id.ToString(), objId, description, category, isGrantable, isGrantableByRecordId, isVisible, isVisibleInMenu, grantId);
        }

        private static SystemActions GetSystemAction(int id, string code, EnumObjects objId, string description, string category = null, bool isGrantable = true, bool isGrantableByRecordId = false, bool isVisible = true, bool isVisibleInMenu = true, int? grantId = null)
        {
            return new SystemActions()
            {
                Id = id,
                ObjectId = (int)objId,
                Code = code,
                Description = description,
                API = "",
                IsGrantable = isGrantable,
                IsGrantableByRecordId = isGrantableByRecordId,
                IsVisible = isVisible,
                IsVisibleInMenu = isVisibleInMenu,
                GrantId = grantId,
                Category = category
            };
        }
        #endregion

        public static List<AdminAccessLevels> GetAdminAccessLevels()
        {
            var items = new List<AdminAccessLevels>();

            items.Add(GetAdminAccessLevel(EnumAccessLevels.Personally));
            items.Add(GetAdminAccessLevel(EnumAccessLevels.PersonallyAndReferents));
            items.Add(GetAdminAccessLevel(EnumAccessLevels.PersonallyAndIOAndReferents));

            return items;
        }

        private static AdminAccessLevels GetAdminAccessLevel(EnumAccessLevels id)
        {
            string name = "##l@" + "AccessLevels" + ":" + id.ToString() + "@l##";
            return new AdminAccessLevels()
            {
                Id = (int)id,
                Code = null,
                Name = name,
                LastChangeUserId = (int)EnumSystemUsers.AdminUser,
                LastChangeDate = DateTime.UtcNow,
            };
        }

        public static List<SystemUIElements> GetSystemUIElements()
        {
            var items = new List<SystemUIElements>();

            items.Add(new SystemUIElements { Id = 1, ActionId = (int)EnumDocumentActions.ModifyDocument, Code = "GeneralInfo", TypeCode = "display_only_text", Description = "Общая информация", Label = null, Hint = null, ValueTypeId = 1, IsMandatory = false, IsReadOnly = false, IsVisible = false, SelectAPI = null, SelectFilter = null, SelectFieldCode = null, SelectDescriptionFieldCode = null, ValueFieldCode = "GeneralInfo", ValueDescriptionFieldCode = "GeneralInfo", Format = null });
            items.Add(new SystemUIElements { Id = 2, ActionId = (int)EnumDocumentActions.ModifyDocument, Code = "DocumentSubject", TypeCode = "select", Description = "Тематика документа", Label = "Тематика документа", Hint = "Выберите из словаря тематику документа", ValueTypeId = 2, IsMandatory = false, IsReadOnly = false, IsVisible = false, SelectAPI = "DictionaryDocumentSubjects", SelectFilter = null, SelectFieldCode = "Id", SelectDescriptionFieldCode = "Name", ValueFieldCode = "DocumentSubjectId", ValueDescriptionFieldCode = "DocumentSubjectName", Format = null });
            items.Add(new SystemUIElements { Id = 3, ActionId = (int)EnumDocumentActions.ModifyDocument, Code = "Description", TypeCode = "textarea", Description = "Краткое содержание", Label = "Краткое содержание", Hint = "Введите краткое содержание", ValueTypeId = 1, IsMandatory = false, IsReadOnly = false, IsVisible = false, SelectAPI = null, SelectFilter = null, SelectFieldCode = null, SelectDescriptionFieldCode = null, ValueFieldCode = "Description", ValueDescriptionFieldCode = "Description", Format = null });
            items.Add(new SystemUIElements { Id = 4, ActionId = (int)EnumDocumentActions.ModifyDocument, Code = "SenderAgent", TypeCode = "select", Description = "Контрагент, от которого получен документ", Label = "Организация", Hint = "Выберите из словаря контрагента, от которого получен документ", ValueTypeId = 2, IsMandatory = false, IsReadOnly = false, IsVisible = false, SelectAPI = "DictionaryAgents", SelectFilter = "{'IsCompany' : 'True'}", SelectFieldCode = "Id", SelectDescriptionFieldCode = "Name", ValueFieldCode = "SenderAgentId", ValueDescriptionFieldCode = "SenderAgentName", Format = null });
            items.Add(new SystemUIElements { Id = 5, ActionId = (int)EnumDocumentActions.ModifyDocument, Code = "SenderAgentPerson", TypeCode = "select", Description = "Контактное лицо в организации", Label = "Контакт", Hint = "Выберите из словаря контактное лицо в организации, от которой получен документ", ValueTypeId = 2, IsMandatory = false, IsReadOnly = false, IsVisible = false, SelectAPI = "DictionaryAgentPersons", SelectFilter = "{'AgentCompanyId' : '@SenderAgentId'}", SelectFieldCode = "Id", SelectDescriptionFieldCode = "FullName", ValueFieldCode = "SenderAgentPersonId", ValueDescriptionFieldCode = "SenderAgentPersonName", Format = null });
            items.Add(new SystemUIElements { Id = 6, ActionId = (int)EnumDocumentActions.ModifyDocument, Code = "SenderNumber", TypeCode = "input", Description = "Входящий номер документа", Label = "Входящий номер документа", Hint = "Введите входящий номер документа", ValueTypeId = 1, IsMandatory = false, IsReadOnly = false, IsVisible = false, SelectAPI = null, SelectFilter = null, SelectFieldCode = null, SelectDescriptionFieldCode = null, ValueFieldCode = "SenderNumber", ValueDescriptionFieldCode = "SenderNumber", Format = null });
            items.Add(new SystemUIElements { Id = 7, ActionId = (int)EnumDocumentActions.ModifyDocument, Code = "SenderDate", TypeCode = "input", Description = "Дата входящего документа", Label = "Дата входящего документа", Hint = "Введите дату входящего документа", ValueTypeId = 3, IsMandatory = false, IsReadOnly = false, IsVisible = false, SelectAPI = null, SelectFilter = null, SelectFieldCode = null, SelectDescriptionFieldCode = null, ValueFieldCode = "SenderDate", ValueDescriptionFieldCode = "SenderDate", Format = null });
            items.Add(new SystemUIElements { Id = 8, ActionId = (int)EnumDocumentActions.ModifyDocument, Code = "Addressee", TypeCode = "input", Description = "Кому адресован документ", Label = "Кому адресован документ", Hint = "Введите кому адресован документ", ValueTypeId = 1, IsMandatory = false, IsReadOnly = false, IsVisible = false, SelectAPI = null, SelectFilter = null, SelectFieldCode = null, SelectDescriptionFieldCode = null, ValueFieldCode = "Addressee", ValueDescriptionFieldCode = "Addressee", Format = null });
            items.Add(new SystemUIElements { Id = 9, ActionId = (int)EnumDocumentActions.ModifyDocument, Code = "AccessLevel", TypeCode = "select", Description = "Уровень доступа", Label = "Уровень доступа", Hint = "Выберите из словаря уровень доступа", ValueTypeId = 2, IsMandatory = false, IsReadOnly = false, IsVisible = false, SelectAPI = "AdminAccessLevels", SelectFilter = null, SelectFieldCode = "Id", SelectDescriptionFieldCode = "Name", ValueFieldCode = "AccessLevelId", ValueDescriptionFieldCode = "AccessLevelName", Format = null });

            return items;
        }

        public static List<SystemValueTypes> GetSystemValueTypes()
        {
            var items = new List<SystemValueTypes>();

            items.Add(new SystemValueTypes { Id = (int)EnumValueTypes.Text, Code = "text", Description = "text" });
            items.Add(new SystemValueTypes { Id = (int)EnumValueTypes.Number, Code = "number", Description = "number" });
            items.Add(new SystemValueTypes { Id = (int)EnumValueTypes.Date, Code = "date", Description = "date" });
            items.Add(new SystemValueTypes { Id = (int)EnumValueTypes.Api, Code = "api", Description = "api" });
            items.Add(new SystemValueTypes { Id = (int)EnumValueTypes.Bool, Code = "bool", Description = "boolean" });

            return items;
        }

        public static List<DictionaryFileTypes> GetDictionaryFileTypes()
        {
            var items = new List<DictionaryFileTypes>();

            items.Add(new DictionaryFileTypes { Id = 0, Name = "Main", LastChangeUserId = (int)EnumSystemUsers.AdminUser, LastChangeDate = DateTime.UtcNow });
            items.Add(new DictionaryFileTypes { Id = 1, Name = "Additional", LastChangeUserId = (int)EnumSystemUsers.AdminUser, LastChangeDate = DateTime.UtcNow });
            items.Add(new DictionaryFileTypes { Id = 2, Name = "SubscribePdf", LastChangeUserId = (int)EnumSystemUsers.AdminUser, LastChangeDate = DateTime.UtcNow });

            return items;
        }

        public static List<DictionarySigningTypes> GetDictionarySigningTypes()
        {
            var items = new List<DictionarySigningTypes>();

            items.Add(new DictionarySigningTypes { Id = 0, Name = "Hash", LastChangeUserId = (int)EnumSystemUsers.AdminUser, LastChangeDate = DateTime.UtcNow });
            items.Add(new DictionarySigningTypes { Id = 1, Name = "InternalSign", LastChangeUserId = (int)EnumSystemUsers.AdminUser, LastChangeDate = DateTime.UtcNow });
            items.Add(new DictionarySigningTypes { Id = 2, Name = "CertificateSign", LastChangeUserId = (int)EnumSystemUsers.AdminUser, LastChangeDate = DateTime.UtcNow });

            return items;
        }

        public static List<DictionaryDocumentDirections> GetDictionaryDocumentDirections()
        {
            var items = new List<DictionaryDocumentDirections>();

            items.Add(GetDictionaryDocumentDirection(EnumDocumentDirections.Incoming));
            items.Add(GetDictionaryDocumentDirection(EnumDocumentDirections.Outcoming));
            items.Add(GetDictionaryDocumentDirection(EnumDocumentDirections.Internal));

            return items;
        }

        private static DictionaryDocumentDirections GetDictionaryDocumentDirection(EnumDocumentDirections id)
        {
            string name = "##l@" + "DictionaryDocumentDirections" + ":" + id.ToString() + "@l##";
            return new DictionaryDocumentDirections()
            {
                Id = (int)id,
                Code = ((int)id).ToString(),
                Name = name,
                LastChangeUserId = (int)EnumSystemUsers.AdminUser,
                LastChangeDate = DateTime.UtcNow,
            };
        }

        public static List<DictionaryEventTypes> GetDictionaryEventTypes()
        {
            var items = new List<DictionaryEventTypes>();

            items.Add(GetDictionaryEventType(EnumEventTypes.ПоступилВходящийДокумент, EnumImportanceEventTypes.DocumentMoovement, null));
            items.Add(GetDictionaryEventType(EnumEventTypes.AddNewDocument, EnumImportanceEventTypes.DocumentMoovement, null));
            items.Add(GetDictionaryEventType(EnumEventTypes.AddDocumentFile, EnumImportanceEventTypes.ImportantEvents, null));
            items.Add(GetDictionaryEventType(EnumEventTypes.RanameDocumentFile, EnumImportanceEventTypes.ImportantEvents, null));
            items.Add(GetDictionaryEventType(EnumEventTypes.ModifyDocumentFile, EnumImportanceEventTypes.ImportantEvents, null));
            items.Add(GetDictionaryEventType(EnumEventTypes.DeleteDocumentFileVersion, EnumImportanceEventTypes.DocumentMoovement, null));
            items.Add(GetDictionaryEventType(EnumEventTypes.DeleteDocumentFile, EnumImportanceEventTypes.DocumentMoovement, null));
            items.Add(GetDictionaryEventType(EnumEventTypes.RejectDocumentFile, EnumImportanceEventTypes.DocumentMoovement, null));
            items.Add(GetDictionaryEventType(EnumEventTypes.AcceptDocumentFile, EnumImportanceEventTypes.DocumentMoovement, null));
            items.Add(GetDictionaryEventType(EnumEventTypes.SendForInformation, EnumImportanceEventTypes.DocumentMoovement, null));
            items.Add(GetDictionaryEventType(EnumEventTypes.ChangeExecutor, EnumImportanceEventTypes.DocumentMoovement, null));
            items.Add(GetDictionaryEventType(EnumEventTypes.ChangePosition, EnumImportanceEventTypes.DocumentMoovement, null));
            items.Add(GetDictionaryEventType(EnumEventTypes.SendForExecution, EnumImportanceEventTypes.ImportantEvents, null));
            items.Add(GetDictionaryEventType(EnumEventTypes.SendForExecutionChange, EnumImportanceEventTypes.DocumentMoovement, "Исполнение"));
            items.Add(GetDictionaryEventType(EnumEventTypes.SendForControl, EnumImportanceEventTypes.DocumentMoovement, "Исполнение"));
            items.Add(GetDictionaryEventType(EnumEventTypes.SendForResponsibleExecution, EnumImportanceEventTypes.DocumentMoovement, "Контроль"));
            items.Add(GetDictionaryEventType(EnumEventTypes.SendForResponsibleExecutionChange, EnumImportanceEventTypes.DocumentMoovement, "Контроль"));
            items.Add(GetDictionaryEventType(EnumEventTypes.SendForConsideration, EnumImportanceEventTypes.DocumentMoovement, "Исполнение"));
            items.Add(GetDictionaryEventType(EnumEventTypes.SendForInformationExternal, EnumImportanceEventTypes.DocumentMoovement, "Исполнение"));
            items.Add(GetDictionaryEventType(EnumEventTypes.SendForVisaing, EnumImportanceEventTypes.DocumentMoovement, null));
            items.Add(GetDictionaryEventType(EnumEventTypes.AffixVisaing, EnumImportanceEventTypes.ImportantEvents, null));
            items.Add(GetDictionaryEventType(EnumEventTypes.RejectVisaing, EnumImportanceEventTypes.ImportantEvents, null));
            items.Add(GetDictionaryEventType(EnumEventTypes.WithdrawVisaing, EnumImportanceEventTypes.DocumentMoovement, null));
            items.Add(GetDictionaryEventType(EnumEventTypes.SendForАgreement, EnumImportanceEventTypes.DocumentMoovement, "Виза"));
            items.Add(GetDictionaryEventType(EnumEventTypes.AffixАgreement, EnumImportanceEventTypes.ImportantEvents, null));
            items.Add(GetDictionaryEventType(EnumEventTypes.RejectАgreement, EnumImportanceEventTypes.ImportantEvents, null));
            items.Add(GetDictionaryEventType(EnumEventTypes.WithdrawАgreement, EnumImportanceEventTypes.ImportantEvents, null));
            items.Add(GetDictionaryEventType(EnumEventTypes.SendForАpproval, EnumImportanceEventTypes.DocumentMoovement, "Согласование"));
            items.Add(GetDictionaryEventType(EnumEventTypes.AffixАpproval, EnumImportanceEventTypes.ImportantEvents, null));
            items.Add(GetDictionaryEventType(EnumEventTypes.RejectАpproval, EnumImportanceEventTypes.ImportantEvents, null));
            items.Add(GetDictionaryEventType(EnumEventTypes.WithdrawАpproval, EnumImportanceEventTypes.ImportantEvents, null));
            items.Add(GetDictionaryEventType(EnumEventTypes.SendForSigning, EnumImportanceEventTypes.DocumentMoovement, "Утверждение"));
            items.Add(GetDictionaryEventType(EnumEventTypes.AffixSigning, EnumImportanceEventTypes.ImportantEvents, null));
            items.Add(GetDictionaryEventType(EnumEventTypes.RejectSigning, EnumImportanceEventTypes.ImportantEvents, null));
            items.Add(GetDictionaryEventType(EnumEventTypes.WithdrawSigning, EnumImportanceEventTypes.ImportantEvents, null));
            items.Add(GetDictionaryEventType(EnumEventTypes.ControlOn, EnumImportanceEventTypes.DocumentMoovement, "Подпись"));
            items.Add(GetDictionaryEventType(EnumEventTypes.ControlOff, EnumImportanceEventTypes.ImportantEvents, null));
            items.Add(GetDictionaryEventType(EnumEventTypes.ControlChange, EnumImportanceEventTypes.ImportantEvents, null));
            items.Add(GetDictionaryEventType(EnumEventTypes.ControlTargetChange, EnumImportanceEventTypes.ImportantEvents, null));
            items.Add(GetDictionaryEventType(EnumEventTypes.MarkExecution, EnumImportanceEventTypes.ImportantEvents, "Контроль"));
            items.Add(GetDictionaryEventType(EnumEventTypes.AcceptResult, EnumImportanceEventTypes.ImportantEvents, null));
            items.Add(GetDictionaryEventType(EnumEventTypes.RejectResult, EnumImportanceEventTypes.ImportantEvents, "Контроль"));
            items.Add(GetDictionaryEventType(EnumEventTypes.SendMessage, EnumImportanceEventTypes.DocumentMoovement, null));
            items.Add(GetDictionaryEventType(EnumEventTypes.AddNewPaper, EnumImportanceEventTypes.ImportantEvents, "Рассмотрение отчета"));
            items.Add(GetDictionaryEventType(EnumEventTypes.MarkOwnerDocumentPaper, EnumImportanceEventTypes.ImportantEvents, null));
            items.Add(GetDictionaryEventType(EnumEventTypes.MarkСorruptionDocumentPaper, EnumImportanceEventTypes.ImportantEvents, null));
            items.Add(GetDictionaryEventType(EnumEventTypes.MoveDocumentPaper, EnumImportanceEventTypes.ImportantEvents, null));
            items.Add(GetDictionaryEventType(EnumEventTypes.AddLink, EnumImportanceEventTypes.ImportantEvents, null));
            items.Add(GetDictionaryEventType(EnumEventTypes.DeleteLink, EnumImportanceEventTypes.ImportantEvents, null));
            items.Add(GetDictionaryEventType(EnumEventTypes.AddNote, EnumImportanceEventTypes.AdditionalEvents, null));
            items.Add(GetDictionaryEventType(EnumEventTypes.TaskFormulation, EnumImportanceEventTypes.ImportantEvents, null));
            items.Add(GetDictionaryEventType(EnumEventTypes.Registered, EnumImportanceEventTypes.ImportantEvents, null));
            items.Add(GetDictionaryEventType(EnumEventTypes.LaunchPlan, EnumImportanceEventTypes.ImportantEvents, null));
            items.Add(GetDictionaryEventType(EnumEventTypes.StopPlan, EnumImportanceEventTypes.ImportantEvents, null));
            items.Add(GetDictionaryEventType(EnumEventTypes.SetInWork, EnumImportanceEventTypes.ImportantEvents, null));
            items.Add(GetDictionaryEventType(EnumEventTypes.SetOutWork, EnumImportanceEventTypes.ImportantEvents, null));
            items.Add(GetDictionaryEventType(EnumEventTypes.ОчереднойСрокИсполнения, EnumImportanceEventTypes.ImportantEvents, null));
            items.Add(GetDictionaryEventType(EnumEventTypes.ИстекаетСрокИсполнения, EnumImportanceEventTypes.ImportantEvents, null));
            items.Add(GetDictionaryEventType(EnumEventTypes.СрокИсполненияИстек, EnumImportanceEventTypes.ImportantEvents, null));
            items.Add(GetDictionaryEventType(EnumEventTypes.НаправленоСообщение, EnumImportanceEventTypes.Internal, null));
            items.Add(GetDictionaryEventType(EnumEventTypes.ДобавленБумажныйНоситель, EnumImportanceEventTypes.PaperMoovement, null));
            items.Add(GetDictionaryEventType(EnumEventTypes.ОтметкаНахожденияБумажногоНосителяУСебя, EnumImportanceEventTypes.PaperMoovement, null));
            items.Add(GetDictionaryEventType(EnumEventTypes.ОтметкаПорчиБумажногоНосителя, EnumImportanceEventTypes.PaperMoovement, null));
            items.Add(GetDictionaryEventType(EnumEventTypes.ПереданыБумажныеНосители, EnumImportanceEventTypes.PaperMoovement, null));
            items.Add(GetDictionaryEventType(EnumEventTypes.Примечание, EnumImportanceEventTypes.Message, null));
            items.Add(GetDictionaryEventType(EnumEventTypes.ФормулировкаЗадачи, EnumImportanceEventTypes.Message, null));
            items.Add(GetDictionaryEventType(EnumEventTypes.ПереданНаРассмотрениеРуководителю, EnumImportanceEventTypes.AdditionalEvents, null));
            items.Add(GetDictionaryEventType(EnumEventTypes.ПолученПослеРассмотренияРуководителем, EnumImportanceEventTypes.AdditionalEvents, null));
            items.Add(GetDictionaryEventType(EnumEventTypes.НаправленНаРегистрацию, EnumImportanceEventTypes.ImportantEvents, null));
            items.Add(GetDictionaryEventType(EnumEventTypes.Зарегистрирован, EnumImportanceEventTypes.ImportantEvents, null));
            items.Add(GetDictionaryEventType(EnumEventTypes.ОтказаноВРегистрации, EnumImportanceEventTypes.ImportantEvents, null));
            items.Add(GetDictionaryEventType(EnumEventTypes.ОтозванПроект, EnumImportanceEventTypes.ImportantEvents, null));
            items.Add(GetDictionaryEventType(EnumEventTypes.ЗапущеноИсполнениеПланаРаботыПоДокументу, EnumImportanceEventTypes.ImportantEvents, null));
            items.Add(GetDictionaryEventType(EnumEventTypes.ОстановленоИсполнениеПланаРаботыПоДокументу, EnumImportanceEventTypes.ImportantEvents, null));
            items.Add(GetDictionaryEventType(EnumEventTypes.РаботаВозобновлена, EnumImportanceEventTypes.ImportantEvents, null));
            items.Add(GetDictionaryEventType(EnumEventTypes.РаботаЗавершена, EnumImportanceEventTypes.ImportantEvents, null));

            return items;
        }

        private static DictionaryEventTypes GetDictionaryEventType(EnumEventTypes id, EnumImportanceEventTypes importanceEventTypeId, string waitDescription = null)
        {
            string name = "##l@" + "DictionaryEventTypes" + ":" + id.ToString() + "@l##";
            return new DictionaryEventTypes()
            {
                Id = (int)id,
                Code = null,
                Name = name,
                SourceDescription = null,
                TargetDescription = null,
                ImportanceEventTypeId = (int)importanceEventTypeId,
                WaitDescription = waitDescription,
                LastChangeUserId = (int)EnumSystemUsers.AdminUser,
                LastChangeDate = DateTime.UtcNow,
            };
        }

        public static List<DictionaryImportanceEventTypes> GetDictionaryImportanceEventTypes()
        {
            var items = new List<DictionaryImportanceEventTypes>();

            items.Add(GetDictionaryImportanceEventType(EnumImportanceEventTypes.DocumentMoovement));
            items.Add(GetDictionaryImportanceEventType(EnumImportanceEventTypes.ImportantEvents));
            items.Add(GetDictionaryImportanceEventType(EnumImportanceEventTypes.AdditionalEvents));
            items.Add(GetDictionaryImportanceEventType(EnumImportanceEventTypes.PaperMoovement));
            items.Add(GetDictionaryImportanceEventType(EnumImportanceEventTypes.Message));
            items.Add(GetDictionaryImportanceEventType(EnumImportanceEventTypes.Internal));

            return items;
        }

        private static DictionaryImportanceEventTypes GetDictionaryImportanceEventType(EnumImportanceEventTypes id)
        {
            string name = "##l@" + "DictionaryImportanceEventTypes" + ":" + id.ToString() + "@l##";
            return new DictionaryImportanceEventTypes()
            {
                Id = (int)id,
                Code = null,
                Name = name,
                LastChangeUserId = (int)EnumSystemUsers.AdminUser,
                LastChangeDate = DateTime.UtcNow,
            };
        }

        public static List<DictionaryResultTypes> GetDictionaryResultTypes()
        {
            var items = new List<DictionaryResultTypes>();

            items.Add(GetDictionaryResultType(EnumResultTypes.CloseByAffixing, IsExecute: true, IsActive: false));
            items.Add(GetDictionaryResultType(EnumResultTypes.CloseByRejecting, IsExecute: false, IsActive: false));
            items.Add(GetDictionaryResultType(EnumResultTypes.CloseByWithdrawing, IsExecute: false, IsActive: false));
            items.Add(GetDictionaryResultType(EnumResultTypes.CloseByChanging, IsExecute: false, IsActive: false));
            items.Add(GetDictionaryResultType(EnumResultTypes.Excellent, IsExecute: true, IsActive: true));
            items.Add(GetDictionaryResultType(EnumResultTypes.Good, IsExecute: true, IsActive: true));
            items.Add(GetDictionaryResultType(EnumResultTypes.Satisfactorily, IsExecute: true, IsActive: true));
            items.Add(GetDictionaryResultType(EnumResultTypes.Bad, IsExecute: false, IsActive: true));
            items.Add(GetDictionaryResultType(EnumResultTypes.WithoutEvaluation, IsExecute: false, IsActive: true));

            return items;
        }

        private static DictionaryResultTypes GetDictionaryResultType(EnumResultTypes id, bool IsExecute, bool IsActive)
        {
            string name = "##l@" + "DictionaryResultTypes" + ":" + id.ToString() + "@l##";
            return new DictionaryResultTypes()
            {
                Id = (int)id,
                Name = name,
                IsExecute = IsExecute,
                IsActive = IsActive,
                LastChangeUserId = (int)EnumSystemUsers.AdminUser,
                LastChangeDate = DateTime.UtcNow,
            };
        }

        public static List<DictionarySendTypes> GetDictionarySendTypes()
        {
            var items = new List<DictionarySendTypes>();

            items.Add(new DictionarySendTypes { Id = 10, Code = null, Name = "##l@DictionarySendTypes:SendForResponsibleExecution@l##", IsImportant = true, SubordinationTypeId = 2, LastChangeUserId = (int)EnumSystemUsers.AdminUser, LastChangeDate = DateTime.UtcNow });
            items.Add(new DictionarySendTypes { Id = 20, Code = null, Name = "##l@DictionarySendTypes:SendForControl@l##", IsImportant = true, SubordinationTypeId = 2, LastChangeUserId = (int)EnumSystemUsers.AdminUser, LastChangeDate = DateTime.UtcNow });
            items.Add(new DictionarySendTypes { Id = 30, Code = null, Name = "##l@DictionarySendTypes:SendForExecution@l##", IsImportant = true, SubordinationTypeId = 2, LastChangeUserId = (int)EnumSystemUsers.AdminUser, LastChangeDate = DateTime.UtcNow });
            items.Add(new DictionarySendTypes { Id = 40, Code = null, Name = "##l@DictionarySendTypes:SendForInformation@l##", IsImportant = false, SubordinationTypeId = 1, LastChangeUserId = (int)EnumSystemUsers.AdminUser, LastChangeDate = DateTime.UtcNow });
            items.Add(new DictionarySendTypes { Id = 45, Code = null, Name = "##l@DictionarySendTypes:SendForInformationExternal@l##", IsImportant = false, SubordinationTypeId = 1, LastChangeUserId = (int)EnumSystemUsers.AdminUser, LastChangeDate = DateTime.UtcNow });
            items.Add(new DictionarySendTypes { Id = 50, Code = null, Name = "##l@DictionarySendTypes:SendForConsideration@l##", IsImportant = false, SubordinationTypeId = 1, LastChangeUserId = (int)EnumSystemUsers.AdminUser, LastChangeDate = DateTime.UtcNow });
            items.Add(new DictionarySendTypes { Id = 250, Code = null, Name = "##l@DictionarySendTypes:SendForVisaing@l##", IsImportant = true, SubordinationTypeId = 1, LastChangeUserId = (int)EnumSystemUsers.AdminUser, LastChangeDate = DateTime.UtcNow });
            items.Add(new DictionarySendTypes { Id = 260, Code = null, Name = "##l@DictionarySendTypes:SendForАgreement@l##", IsImportant = true, SubordinationTypeId = 1, LastChangeUserId = (int)EnumSystemUsers.AdminUser, LastChangeDate = DateTime.UtcNow });
            items.Add(new DictionarySendTypes { Id = 270, Code = null, Name = "##l@DictionarySendTypes:SendForАpproval@l##", IsImportant = true, SubordinationTypeId = 1, LastChangeUserId = (int)EnumSystemUsers.AdminUser, LastChangeDate = DateTime.UtcNow });
            items.Add(new DictionarySendTypes { Id = 280, Code = null, Name = "##l@DictionarySendTypes:SendForSigning@l##", IsImportant = true, SubordinationTypeId = 1, LastChangeUserId = (int)EnumSystemUsers.AdminUser, LastChangeDate = DateTime.UtcNow });

            return items;
        }

        public static List<DictionarySubordinationTypes> GetDictionarySubordinationTypes()
        {
            var items = new List<DictionarySubordinationTypes>();

            items.Add(new DictionarySubordinationTypes { Id = 1, Code = "Informing", Name = "##l@DictionarySubordinationTypes:Informing@l##", LastChangeUserId = (int)EnumSystemUsers.AdminUser, LastChangeDate = DateTime.UtcNow });
            items.Add(new DictionarySubordinationTypes { Id = 2, Code = "Execution", Name = "##l@DictionarySubordinationTypes:Execution@l##", LastChangeUserId = (int)EnumSystemUsers.AdminUser, LastChangeDate = DateTime.UtcNow });

            return items;
        }

        public static List<DictionarySubscriptionStates> GetDictionarySubscriptionStates()
        {
            var items = new List<DictionarySubscriptionStates>();

            items.Add(new DictionarySubscriptionStates { Id = 1, Code = "No", Name = "##l@DictionarySubscriptionStates:No@l##", IsSuccess = false, LastChangeUserId = (int)EnumSystemUsers.AdminUser, LastChangeDate = DateTime.UtcNow });
            items.Add(new DictionarySubscriptionStates { Id = 2, Code = "Violated", Name = "##l@DictionarySubscriptionStates:Violated@l##", IsSuccess = false, LastChangeUserId = (int)EnumSystemUsers.AdminUser, LastChangeDate = DateTime.UtcNow });
            items.Add(new DictionarySubscriptionStates { Id = 11, Code = "Visa", Name = "##l@DictionarySubscriptionStates:Visa@l##", IsSuccess = true, LastChangeUserId = (int)EnumSystemUsers.AdminUser, LastChangeDate = DateTime.UtcNow });
            items.Add(new DictionarySubscriptionStates { Id = 12, Code = "Аgreement", Name = "##l@DictionarySubscriptionStates:Аgreement@l##", IsSuccess = true, LastChangeUserId = (int)EnumSystemUsers.AdminUser, LastChangeDate = DateTime.UtcNow });
            items.Add(new DictionarySubscriptionStates { Id = 13, Code = "Аpproval", Name = "##l@DictionarySubscriptionStates:Аpproval@l##", IsSuccess = true, LastChangeUserId = (int)EnumSystemUsers.AdminUser, LastChangeDate = DateTime.UtcNow });
            items.Add(new DictionarySubscriptionStates { Id = 14, Code = "Sign", Name = "##l@DictionarySubscriptionStates:Sign@l##", IsSuccess = true, LastChangeUserId = (int)EnumSystemUsers.AdminUser, LastChangeDate = DateTime.UtcNow });

            return items;
        }
        public static List<DictionaryPositionExecutorTypes> GetDictionaryPositionExecutorTypes()
        {
            var items = new List<DictionaryPositionExecutorTypes>();

            items.Add(new DictionaryPositionExecutorTypes { Id = (int)EnumPositionExecutionTypes.Personal, Code = EnumPositionExecutionTypes.Personal.ToString(), Name = "##l@PositionExecutorTypes:Personal@l##", IsActive = true, LastChangeUserId = (int)EnumSystemUsers.AdminUser, LastChangeDate = DateTime.UtcNow });
            items.Add(new DictionaryPositionExecutorTypes { Id = (int)EnumPositionExecutionTypes.IO, Code = EnumPositionExecutionTypes.IO.ToString(), Name = "##l@PositionExecutorTypes:IO@l##", IsActive = true, LastChangeUserId = (int)EnumSystemUsers.AdminUser, LastChangeDate = DateTime.UtcNow });
            items.Add(new DictionaryPositionExecutorTypes { Id = (int)EnumPositionExecutionTypes.Referent, Code = EnumPositionExecutionTypes.Referent.ToString(), Name = "##l@PositionExecutorTypes:Referent@l##", IsActive = true, LastChangeUserId = (int)EnumSystemUsers.AdminUser, LastChangeDate = DateTime.UtcNow });

            return items;
        }

        public static List<DictionaryLinkTypes> GetDictionaryLinkTypes()
        {
            var items = new List<DictionaryLinkTypes>();

            items.Add(GetDictionaryLinkType(EnumLinkTypes.Answer, false));
            items.Add(GetDictionaryLinkType(EnumLinkTypes.Execution, true));
            items.Add(GetDictionaryLinkType(EnumLinkTypes.Additionally, false));
            items.Add(GetDictionaryLinkType(EnumLinkTypes.Repeatedly, false));
            items.Add(GetDictionaryLinkType(EnumLinkTypes.Change, false));
            items.Add(GetDictionaryLinkType(EnumLinkTypes.Cancel, true));

            return items;
        }

        private static DictionaryLinkTypes GetDictionaryLinkType(EnumLinkTypes id, bool IsImportant)
        {
            string name = "##l@" + "LinkTypes" + ":" + id.ToString() + "@l##";
            return new DictionaryLinkTypes()
            {
                Id = (int)id,
                Name = name,
                IsImportant = IsImportant,
                LastChangeUserId = (int)EnumSystemUsers.AdminUser,
                LastChangeDate = DateTime.UtcNow,
            };
        }

    }
}
