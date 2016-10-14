using BL.Database.DBModel.Admin;
using BL.Database.DBModel.Dictionary;
using BL.Database.DBModel.System;
using BL.Model.Constants;
using BL.Model.Enums;
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

            items.Add(GetSystemObjects(EnumObjects.Documents, "Документы"));
            items.Add(GetSystemObjects(EnumObjects.DocumentAccesses, "Документы - доступы"));
            items.Add(GetSystemObjects(EnumObjects.DocumentRestrictedSendLists, "Документы - ограничения рассылки"));
            items.Add(GetSystemObjects(EnumObjects.DocumentSendLists, "Документы - план работы"));
            items.Add(GetSystemObjects(EnumObjects.DocumentFiles, "Документы - файлы"));
            items.Add(GetSystemObjects(EnumObjects.DocumentLinks, "Документы - связи"));
            items.Add(GetSystemObjects(EnumObjects.DocumentSendListStages, "Документы - этапы плана работ"));
            items.Add(GetSystemObjects(EnumObjects.DocumentEvents, "Документы - события"));
            items.Add(GetSystemObjects(EnumObjects.DocumentWaits, "Документы - ожидания"));
            items.Add(GetSystemObjects(EnumObjects.DocumentSubscriptions, "Документы - подписи"));
            items.Add(GetSystemObjects(EnumObjects.DocumentTasks, "Документы - задачи"));
            items.Add(GetSystemObjects(EnumObjects.DocumentPapers, "Документы - бумажные носители"));
            items.Add(GetSystemObjects(EnumObjects.DocumentPaperEvents, "Документы - события по бумажным носителям"));
            items.Add(GetSystemObjects(EnumObjects.DocumentPaperLists, "Документы - реестры передачи бумажных носителей"));
            items.Add(GetSystemObjects(EnumObjects.DocumentSavedFilters, "Документы - сохраненные фильтры"));
            items.Add(GetSystemObjects(EnumObjects.DocumentTags, "Документы - тэги"));
            items.Add(GetSystemObjects(EnumObjects.DictionaryDocumentType, "Типы документов"));
            items.Add(GetSystemObjects(EnumObjects.DictionaryAddressType, "Типы адресов"));
            items.Add(GetSystemObjects(EnumObjects.DictionaryDocumentSubjects, "Тематики документов"));
            items.Add(GetSystemObjects(EnumObjects.DictionaryRegistrationJournals, "Журналы регистрации"));
            items.Add(GetSystemObjects(EnumObjects.DictionaryContactType, "Типы контактов"));
            items.Add(GetSystemObjects(EnumObjects.DictionaryAgents, "Контрагенты"));
            items.Add(GetSystemObjects(EnumObjects.DictionaryContacts, "Контакты"));
            items.Add(GetSystemObjects(EnumObjects.DictionaryAgentAddresses, "Адреса"));
            items.Add(GetSystemObjects(EnumObjects.DictionaryAgentPersons, "Физические лица"));
            items.Add(GetSystemObjects(EnumObjects.DictionaryDepartments, "Структура предприятия"));
            items.Add(GetSystemObjects(EnumObjects.DictionaryPositions, "Штатное расписание"));
            items.Add(GetSystemObjects(EnumObjects.DictionaryAgentEmployees, "Сотрудники"));
            items.Add(GetSystemObjects(EnumObjects.DictionaryAgentCompanies, "Юридические лица"));
            items.Add(GetSystemObjects(EnumObjects.DictionaryAgentBanks, "Контрагенты - банки"));
            items.Add(GetSystemObjects(EnumObjects.DictionaryAgentAccounts, "Расчетные счета"));
            items.Add(GetSystemObjects(EnumObjects.DictionaryStandartSendListContent, "Типовые списки рассылки (содержание)"));
            items.Add(GetSystemObjects(EnumObjects.DictionaryStandartSendLists, "Типовые списки рассылки"));
            items.Add(GetSystemObjects(EnumObjects.DictionaryAgentClientCompanies, "Компании"));
            items.Add(GetSystemObjects(EnumObjects.DictionaryPositionExecutorTypes, "Типы исполнителей"));
            items.Add(GetSystemObjects(EnumObjects.DictionaryPositionExecutors, "Исполнители должности"));
            items.Add(GetSystemObjects(EnumObjects.TemplateDocument, "Шаблоны документов"));
            items.Add(GetSystemObjects(EnumObjects.TemplateDocumentSendList, "Списки рассылки в шаблонах"));
            items.Add(GetSystemObjects(EnumObjects.TemplateDocumentRestrictedSendList, "Ограничительные списки рассылки в шаблонах"));
            items.Add(GetSystemObjects(EnumObjects.TemplateDocumentTask, "Задачи в шаблонах"));
            items.Add(GetSystemObjects(EnumObjects.TemplateDocumentAttachedFiles, "Прикрепленные к шаблонам файлы"));
            items.Add(GetSystemObjects(EnumObjects.DictionaryTag, "Теги"));
            items.Add(GetSystemObjects(EnumObjects.CustomDictionaryTypes, "Типы пользовательских словарей"));
            items.Add(GetSystemObjects(EnumObjects.CustomDictionaries, "Пользовательские словари"));
            items.Add(GetSystemObjects(EnumObjects.Properties, "Динамические аттрибуты"));
            items.Add(GetSystemObjects(EnumObjects.PropertyLinks, "Связи динамических аттрибутов с объектами системы"));
            items.Add(GetSystemObjects(EnumObjects.PropertyValues, "Значения динамических аттрибутов"));

            items.Add(GetSystemObjects(EnumObjects.EncryptionCertificates, "Хранилище сертификатов"));
            items.Add(GetSystemObjects(EnumObjects.EncryptionCertificateTypes, "Типы сертификатов"));

            items.Add(GetSystemObjects(EnumObjects.AdminRoles, "Роли"));
            items.Add(GetSystemObjects(EnumObjects.AdminPositionRoles, "Роли"));
            items.Add(GetSystemObjects(EnumObjects.AdminUserRoles, "Роли"));

            items.Add(GetSystemObjects(EnumObjects.SystemSettings, "Системные настройки"));

            return items;
        }

        private static SystemObjects GetSystemObjects(EnumObjects id, string description)
        {
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

            items.Add(GetSysAct(EnumDocumentActions.AddDocument, EnumObjects.Documents, "##l@DocumentActions:AddDocument@l##", "Документ"));
            items.Add(GetSysAct(EnumDocumentActions.CopyDocument, EnumObjects.Documents, "##l@DocumentActions:CopyDocument@l##", "Документ"));
            items.Add(GetSysAct(EnumDocumentActions.ModifyDocument, EnumObjects.Documents, "##l@DocumentActions:ModifyDocument@l##", "Документ"));
            items.Add(GetSysAct(EnumDocumentActions.DeleteDocument, EnumObjects.Documents, "##l@DocumentActions:DeleteDocument@l##", "Документ"));
            items.Add(GetSysAct(EnumDocumentActions.LaunchPlan, EnumObjects.Documents, "##l@DocumentActions:LaunchPlan@l##", "Документ"));
            items.Add(GetSysAct(EnumDocumentActions.AddDocumentSendListItem, EnumObjects.Documents, "##l@DocumentActions:AddDocumentSendListItem@l##", "Документ"));
            items.Add(GetSysAct(EnumDocumentActions.StopPlan, EnumObjects.Documents, "##l@DocumentActions:StopPlan@l##", "Документ"));
            items.Add(GetSysAct(EnumDocumentActions.ChangeExecutor, EnumObjects.Documents, "##l@DocumentActions:ChangeExecutor@l##", "Документ"));
            items.Add(GetSysAct(EnumDocumentActions.RegisterDocument, EnumObjects.Documents, "##l@DocumentActions:RegisterDocument@l##", "Документ"));
            items.Add(GetSysAct(EnumDocumentActions.MarkDocumentEventAsRead, EnumObjects.Documents, "##l@DocumentActions:MarkDocumentEventAsRead@l##", "Информирование"));
            items.Add(GetSysAct(EnumDocumentActions.SendForInformation, EnumObjects.Documents, "##l@DocumentActions:SendForInformation@l##", "Информирование"));
            items.Add(GetSysAct(EnumDocumentActions.SendForConsideration, EnumObjects.Documents, "##l@DocumentActions:SendForConsideration@l##", "Информирование"));
            items.Add(GetSysAct(EnumDocumentActions.SendForInformationExternal, EnumObjects.Documents, "##l@DocumentActions:SendForInformationExternal@l##", "Информирование"));
            items.Add(GetSysAct(EnumDocumentActions.SendForControl, EnumObjects.Documents, "##l@DocumentActions:SendForControl@l##", "Контроль"));
            items.Add(GetSysAct(EnumDocumentActions.SendForResponsibleExecution, EnumObjects.Documents, "##l@DocumentActions:SendForResponsibleExecution@l##", "Контроль"));
            items.Add(GetSysAct(EnumDocumentActions.SendForExecution, EnumObjects.Documents, "##l@DocumentActions:SendForExecution@l##", "Контроль"));
            items.Add(GetSysAct(EnumDocumentActions.SendForVisaing, EnumObjects.Documents, "##l@DocumentActions:SendForVisaing@l##", "Подписание"));
            items.Add(GetSysAct(EnumDocumentActions.SendForАgreement, EnumObjects.Documents, "##l@DocumentActions:SendForАgreement@l##", "Подписание"));
            items.Add(GetSysAct(EnumDocumentActions.SendForАpproval, EnumObjects.Documents, "##l@DocumentActions:SendForАpproval@l##", "Подписание"));
            items.Add(GetSysAct(EnumDocumentActions.SendForSigning, EnumObjects.Documents, "##l@DocumentActions:SendForSigning@l##", "Подписание"));
            items.Add(GetSysAct(EnumDocumentActions.ReportRegistrationCardDocument, EnumObjects.Documents, "##l@DocumentActions:ReportRegistrationCardDocument@l##", "Отчеты"));
            items.Add(GetSysAct(EnumDocumentActions.AddFavourite, EnumObjects.Documents, "##l@DocumentActions:AddFavourite@l##", "Дополнительно", false));
            items.Add(GetSysAct(EnumDocumentActions.DeleteFavourite, EnumObjects.Documents, "##l@DocumentActions:DeleteFavourite@l##", "Дополнительно", false));
            items.Add(GetSysAct(EnumDocumentActions.FinishWork, EnumObjects.Documents, "##l@DocumentActions:FinishWork@l##", "Дополнительно"));
            items.Add(GetSysAct(EnumDocumentActions.StartWork, EnumObjects.Documents, "##l@DocumentActions:StartWork@l##", "Дополнительно"));
            items.Add(GetSysAct(EnumDocumentActions.ChangePosition, EnumObjects.Documents, "##l@DocumentActions:ChangePosition@l##", "Администратор"));
            items.Add(GetSysAct(EnumDocumentActions.AddDocumentRestrictedSendList, EnumObjects.DocumentRestrictedSendLists, "##l@DocumentActions:AddDocumentRestrictedSendList@l##"));
            items.Add(GetSysAct(EnumDocumentActions.AddByStandartSendListDocumentRestrictedSendList, EnumObjects.DocumentRestrictedSendLists, "##l@DocumentActions:AddByStandartSendListDocumentRestrictedSendList@l##"));
            items.Add(GetSysAct(EnumDocumentActions.DeleteDocumentRestrictedSendList, EnumObjects.DocumentRestrictedSendLists, "##l@DocumentActions:DeleteDocumentRestrictedSendList@l##"));
            items.Add(GetSysAct(EnumDocumentActions.ModifyDocumentSendList, EnumObjects.DocumentSendLists, "##l@DocumentActions:ModifyDocumentSendList@l##"));
            items.Add(GetSysAct(EnumDocumentActions.DeleteDocumentSendList, EnumObjects.DocumentSendLists, "##l@DocumentActions:DeleteDocumentSendList@l##"));
            items.Add(GetSysAct(EnumDocumentActions.LaunchDocumentSendListItem, EnumObjects.DocumentSendLists, "##l@DocumentActions:LaunchDocumentSendListItem@l##"));
            items.Add(GetSysAct(EnumDocumentActions.AddDocumentFile, EnumObjects.DocumentFiles, "##l@DocumentActions:AddDocumentFile@l##"));
            items.Add(GetSysAct(EnumDocumentActions.ModifyDocumentFile, EnumObjects.DocumentFiles, "##l@DocumentActions:ModifyDocumentFile@l##"));
            items.Add(GetSysAct(EnumDocumentActions.DeleteDocumentFile, EnumObjects.DocumentFiles, "##l@DocumentActions:DeleteDocumentFile@l##"));
            items.Add(GetSysAct(EnumDocumentActions.AddDocumentFileUseMainNameFile, EnumObjects.DocumentFiles, "##l@DocumentActions:AddDocumentFileUseMainNameFile@l##"));
            items.Add(GetSysAct(EnumDocumentActions.AcceptDocumentFile, EnumObjects.DocumentFiles, "##l@DocumentActions:AcceptDocumentFile@l##"));
            items.Add(GetSysAct(EnumDocumentActions.RejectDocumentFile, EnumObjects.DocumentFiles, "##l@DocumentActions:RejectDocumentFile@l##"));
            items.Add(GetSysAct(EnumDocumentActions.RenameDocumentFile, EnumObjects.DocumentFiles, "##l@DocumentActions:RenameDocumentFile@l##"));
            items.Add(GetSysAct(EnumDocumentActions.DeleteDocumentFileVersion, EnumObjects.DocumentFiles, "##l@DocumentActions:DeleteDocumentFileVersion@l##"));
            items.Add(GetSysAct(EnumDocumentActions.DeleteDocumentFileVersionRecord, EnumObjects.DocumentFiles, "##l@DocumentActions:DeleteDocumentFileVersionRecord@l##"));
            items.Add(GetSysAct(EnumDocumentActions.AcceptMainVersionDocumentFile, EnumObjects.DocumentFiles, "##l@DocumentActions:AcceptMainVersionDocumentFile@l##"));
            items.Add(GetSysAct(EnumDocumentActions.AddDocumentLink, EnumObjects.DocumentLinks, "##l@DocumentActions:AddDocumentLink@l##"));
            items.Add(GetSysAct(EnumDocumentActions.DeleteDocumentLink, EnumObjects.DocumentLinks, "##l@DocumentActions:DeleteDocumentLink@l##"));
            items.Add(GetSysAct(EnumDocumentActions.AddDocumentSendList, EnumObjects.DocumentSendListStages, "##l@DocumentActions:AddDocumentSendList@l##"));
            items.Add(GetSysAct(EnumDocumentActions.AddByStandartSendListDocumentSendList, EnumObjects.DocumentSendListStages, "##l@DocumentActions:AddByStandartSendListDocumentSendList@l##"));
            items.Add(GetSysAct(EnumDocumentActions.AddDocumentSendListStage, EnumObjects.DocumentSendListStages, "##l@DocumentActions:AddDocumentSendListStage@l##"));
            items.Add(GetSysAct(EnumDocumentActions.DeleteDocumentSendListStage, EnumObjects.DocumentSendListStages, "##l@DocumentActions:DeleteDocumentSendListStage@l##"));
            items.Add(GetSysAct(EnumDocumentActions.SendMessage, EnumObjects.DocumentEvents, "##l@DocumentActions:SendMessage@l##", "Информирование"));
            items.Add(GetSysAct(EnumDocumentActions.AddNote, EnumObjects.DocumentEvents, "##l@DocumentActions:AddNote@l##", "Информирование"));
            items.Add(GetSysAct(EnumDocumentActions.ControlOn, EnumObjects.DocumentWaits, "##l@DocumentActions:ControlOn@l##", "Контроль"));
            items.Add(GetSysAct(EnumDocumentActions.ControlChange, EnumObjects.DocumentWaits, "##l@DocumentActions:ControlChange@l##", "Контроль"));
            items.Add(GetSysAct(EnumDocumentActions.SendForExecutionChange, EnumObjects.DocumentWaits, "##l@DocumentActions:SendForExecutionChange@l##", "Контроль"));
            items.Add(GetSysAct(EnumDocumentActions.SendForResponsibleExecutionChange, EnumObjects.DocumentWaits, "##l@DocumentActions:SendForResponsibleExecutionChange@l##", "Контроль"));
            items.Add(GetSysAct(EnumDocumentActions.ControlTargetChange, EnumObjects.DocumentWaits, "##l@DocumentActions:ControlTargetChange@l##", "Контроль"));
            items.Add(GetSysAct(EnumDocumentActions.ControlOff, EnumObjects.DocumentWaits, "##l@DocumentActions:ControlOff@l##", "Контроль"));
            items.Add(GetSysAct(EnumDocumentActions.MarkExecution, EnumObjects.DocumentWaits, "##l@DocumentActions:MarkExecution@l##", "Контроль"));
            items.Add(GetSysAct(EnumDocumentActions.AcceptResult, EnumObjects.DocumentWaits, "##l@DocumentActions:AcceptResult@l##", "Контроль"));
            items.Add(GetSysAct(EnumDocumentActions.RejectResult, EnumObjects.DocumentWaits, "##l@DocumentActions:RejectResult@l##", "Контроль"));
            items.Add(GetSysAct(EnumDocumentActions.WithdrawVisaing, EnumObjects.DocumentSubscriptions, "##l@DocumentActions:WithdrawVisaing@l##", "Подписание"));
            items.Add(GetSysAct(EnumDocumentActions.WithdrawАgreement, EnumObjects.DocumentSubscriptions, "##l@DocumentActions:WithdrawАgreement@l##", "Подписание"));
            items.Add(GetSysAct(EnumDocumentActions.WithdrawАpproval, EnumObjects.DocumentSubscriptions, "##l@DocumentActions:WithdrawАpproval@l##", "Подписание"));
            items.Add(GetSysAct(EnumDocumentActions.WithdrawSigning, EnumObjects.DocumentSubscriptions, "##l@DocumentActions:WithdrawSigning@l##", "Подписание"));
            items.Add(GetSysAct(EnumDocumentActions.AffixVisaing, EnumObjects.DocumentSubscriptions, "##l@DocumentActions:AffixVisaing@l##", "Подписание"));
            items.Add(GetSysAct(EnumDocumentActions.AffixАgreement, EnumObjects.DocumentSubscriptions, "##l@DocumentActions:AffixАgreement@l##", "Подписание"));
            items.Add(GetSysAct(EnumDocumentActions.AffixАpproval, EnumObjects.DocumentSubscriptions, "##l@DocumentActions:AffixАpproval@l##", "Подписание"));
            items.Add(GetSysAct(EnumDocumentActions.AffixSigning, EnumObjects.DocumentSubscriptions, "##l@DocumentActions:AffixSigning@l##", "Подписание"));
            items.Add(GetSysAct(EnumDocumentActions.SelfAffixSigning, EnumObjects.DocumentSubscriptions, "##l@DocumentActions:SelfAffixSigning@l##", "Подписание"));
            items.Add(GetSysAct(EnumDocumentActions.RejectVisaing, EnumObjects.DocumentSubscriptions, "##l@DocumentActions:RejectVisaing@l##", "Подписание"));
            items.Add(GetSysAct(EnumDocumentActions.RejectАgreement, EnumObjects.DocumentSubscriptions, "##l@DocumentActions:RejectАgreement@l##", "Подписание"));
            items.Add(GetSysAct(EnumDocumentActions.RejectАpproval, EnumObjects.DocumentSubscriptions, "##l@DocumentActions:RejectАpproval@l##", "Подписание"));
            items.Add(GetSysAct(EnumDocumentActions.RejectSigning, EnumObjects.DocumentSubscriptions, "##l@DocumentActions:RejectSigning@l##", "Подписание"));
            items.Add(GetSysAct(EnumDocumentActions.AddDocumentTask, EnumObjects.DocumentTasks, "##l@DocumentActions:AddDocumentTask@l##"));
            items.Add(GetSysAct(EnumDocumentActions.ModifyDocumentTask, EnumObjects.DocumentTasks, "##l@DocumentActions:ModifyDocumentTask@l##"));
            items.Add(GetSysAct(EnumDocumentActions.DeleteDocumentTask, EnumObjects.DocumentTasks, "##l@DocumentActions:DeleteDocumentTask@l##"));
            items.Add(GetSysAct(EnumDocumentActions.AddDocumentPaper, EnumObjects.DocumentPapers, "##l@DocumentActions:AddDocumentPaper@l##", "Бумажные носители"));
            items.Add(GetSysAct(EnumDocumentActions.ModifyDocumentPaper, EnumObjects.DocumentPapers, "##l@DocumentActions:ModifyDocumentPaper@l##", "Бумажные носители"));
            items.Add(GetSysAct(EnumDocumentActions.MarkOwnerDocumentPaper, EnumObjects.DocumentPapers, "##l@DocumentActions:MarkOwnerDocumentPaper@l##", "Бумажные носители"));
            items.Add(GetSysAct(EnumDocumentActions.MarkСorruptionDocumentPaper, EnumObjects.DocumentPapers, "##l@DocumentActions:MarkСorruptionDocumentPaper@l##", "Бумажные носители"));
            items.Add(GetSysAct(EnumDocumentActions.DeleteDocumentPaper, EnumObjects.DocumentPapers, "##l@DocumentActions:DeleteDocumentPaper@l##", "Бумажные носители"));
            items.Add(GetSysAct(EnumDocumentActions.PlanDocumentPaperEvent, EnumObjects.DocumentPaperEvents, "##l@DocumentActions:PlanDocumentPaperEvent@l##", "Бумажные носители"));
            items.Add(GetSysAct(EnumDocumentActions.CancelPlanDocumentPaperEvent, EnumObjects.DocumentPaperEvents, "##l@DocumentActions:CancelPlanDocumentPaperEvent@l##", "Бумажные носители"));
            items.Add(GetSysAct(EnumDocumentActions.SendDocumentPaperEvent, EnumObjects.DocumentPaperEvents, "##l@DocumentActions:SendDocumentPaperEvent@l##", "Бумажные носители"));
            items.Add(GetSysAct(EnumDocumentActions.CancelSendDocumentPaperEvent, EnumObjects.DocumentPaperEvents, "##l@DocumentActions:CancelSendDocumentPaperEvent@l##", "Бумажные носители"));
            items.Add(GetSysAct(EnumDocumentActions.RecieveDocumentPaperEvent, EnumObjects.DocumentPaperEvents, "##l@DocumentActions:RecieveDocumentPaperEvent@l##", "Бумажные носители"));
            items.Add(GetSysAct(EnumDocumentActions.AddDocumentPaperList, EnumObjects.DocumentPaperLists, "##l@DocumentActions:AddDocumentPaperList@l##", "Реестры бумажных носителей"));
            items.Add(GetSysAct(EnumDocumentActions.ModifyDocumentPaperList, EnumObjects.DocumentPaperLists, "##l@DocumentActions:ModifyDocumentPaperList@l##", "Реестры бумажных носителей"));
            items.Add(GetSysAct(EnumDocumentActions.DeleteDocumentPaperList, EnumObjects.DocumentPaperLists, "##l@DocumentActions:DeleteDocumentPaperList@l##", "Реестры бумажных носителей"));
            items.Add(GetSysAct(EnumDocumentActions.AddSavedFilter, EnumObjects.DocumentSavedFilters, "##l@DocumentActions:AddSavedFilter@l##"));
            items.Add(GetSysAct(EnumDocumentActions.ModifySavedFilter, EnumObjects.DocumentSavedFilters, "##l@DocumentActions:ModifySavedFilter@l##"));
            items.Add(GetSysAct(EnumDocumentActions.DeleteSavedFilter, EnumObjects.DocumentSavedFilters, "##l@DocumentActions:DeleteSavedFilter@l##"));
            items.Add(GetSysAct(EnumDocumentActions.ModifyDocumentTags, EnumObjects.DocumentTags, "##l@DocumentActions:ModifyDocumentTags@l##"));
            items.Add(GetSysAct(EnumDocumentActions.AddTemplateDocument, EnumObjects.TemplateDocument, "##l@DocumentActions:AddTemplateDocument@l##"));
            items.Add(GetSysAct(EnumDocumentActions.ModifyTemplateDocument, EnumObjects.TemplateDocument, "##l@DocumentActions:ModifyTemplateDocument@l##"));
            items.Add(GetSysAct(EnumDocumentActions.DeleteTemplateDocument, EnumObjects.TemplateDocument, "##l@DocumentActions:DeleteTemplateDocument@l##"));
            items.Add(GetSysAct(EnumDocumentActions.AddTemplateDocumentSendList, EnumObjects.TemplateDocumentSendList, "##l@DocumentActions:AddTemplateDocumentSendList@l##"));
            items.Add(GetSysAct(EnumDocumentActions.ModifyTemplateDocumentSendList, EnumObjects.TemplateDocumentSendList, "##l@DocumentActions:ModifyTemplateDocumentSendList@l##"));
            items.Add(GetSysAct(EnumDocumentActions.DeleteTemplateDocumentSendList, EnumObjects.TemplateDocumentSendList, "##l@DocumentActions:DeleteTemplateDocumentSendList@l##"));
            items.Add(GetSysAct(EnumDocumentActions.AddTemplateDocumentRestrictedSendList, EnumObjects.TemplateDocumentRestrictedSendList, "##l@DocumentActions:AddTemplateDocumentRestrictedSendList@l##"));
            items.Add(GetSysAct(EnumDocumentActions.ModifyTemplateDocumentRestrictedSendList, EnumObjects.TemplateDocumentRestrictedSendList, "##l@DocumentActions:ModifyTemplateDocumentRestrictedSendList@l##"));
            items.Add(GetSysAct(EnumDocumentActions.DeleteTemplateDocumentRestrictedSendList, EnumObjects.TemplateDocumentRestrictedSendList, "##l@DocumentActions:DeleteTemplateDocumentRestrictedSendList@l##"));
            items.Add(GetSysAct(EnumDocumentActions.AddTemplateDocumentTask, EnumObjects.TemplateDocumentTask, "##l@DocumentActions:AddTemplateDocumentTask@l##"));
            items.Add(GetSysAct(EnumDocumentActions.ModifyTemplateDocumentTask, EnumObjects.TemplateDocumentTask, "##l@DocumentActions:ModifyTemplateDocumentTask@l##"));
            items.Add(GetSysAct(EnumDocumentActions.DeleteTemplateDocumentTask, EnumObjects.TemplateDocumentTask, "##l@DocumentActions:DeleteTemplateDocumentTask@l##"));
            items.Add(GetSysAct(EnumDocumentActions.AddTemplateAttachedFile, EnumObjects.TemplateDocumentAttachedFiles, "##l@DocumentActions:AddTemplateAttachedFile@l##"));
            items.Add(GetSysAct(EnumDocumentActions.ModifyTemplateAttachedFile, EnumObjects.TemplateDocumentAttachedFiles, "##l@DocumentActions:ModifyTemplateAttachedFile@l##"));
            items.Add(GetSysAct(EnumDocumentActions.DeleteTemplateAttachedFile, EnumObjects.TemplateDocumentAttachedFiles, "##l@DocumentActions:DeleteTemplateAttachedFile@l##"));

            items.Add(GetSysAct(EnumDictionaryActions.AddDocumentType, EnumObjects.DictionaryDocumentType, "##l@DictionaryActions:AddDocumentType@l##"));
            items.Add(GetSysAct(EnumDictionaryActions.ModifyDocumentType, EnumObjects.DictionaryDocumentType, "##l@DictionaryActions:ModifyDocumentType@l##"));
            items.Add(GetSysAct(EnumDictionaryActions.DeleteDocumentType, EnumObjects.DictionaryDocumentType, "##l@DictionaryActions:DeleteDocumentType@l##"));
            items.Add(GetSysAct(EnumDictionaryActions.AddAddressType, EnumObjects.DictionaryAddressType, "##l@DictionaryActions:AddAddressType@l##"));
            items.Add(GetSysAct(EnumDictionaryActions.ModifyAddressType, EnumObjects.DictionaryAddressType, "##l@DictionaryActions:ModifyAddressType@l##"));
            items.Add(GetSysAct(EnumDictionaryActions.DeleteAddressType, EnumObjects.DictionaryAddressType, "##l@DictionaryActions:DeleteAddressType@l##"));
            items.Add(GetSysAct(EnumDictionaryActions.AddDocumentSubject, EnumObjects.DictionaryDocumentSubjects, "##l@DictionaryActions:AddDocumentSubject@l##"));
            items.Add(GetSysAct(EnumDictionaryActions.ModifyDocumentSubject, EnumObjects.DictionaryDocumentSubjects, "##l@DictionaryActions:ModifyDocumentSubject@l##"));
            items.Add(GetSysAct(EnumDictionaryActions.DeleteDocumentSubject, EnumObjects.DictionaryDocumentSubjects, "##l@DictionaryActions:DeleteDocumentSubject@l##"));
            items.Add(GetSysAct(EnumDictionaryActions.AddRegistrationJournal, EnumObjects.DictionaryRegistrationJournals, "##l@DictionaryActions:AddRegistrationJournal@l##"));
            items.Add(GetSysAct(EnumDictionaryActions.ModifyRegistrationJournal, EnumObjects.DictionaryRegistrationJournals, "##l@DictionaryActions:ModifyRegistrationJournal@l##"));
            items.Add(GetSysAct(EnumDictionaryActions.DeleteRegistrationJournal, EnumObjects.DictionaryRegistrationJournals, "##l@DictionaryActions:DeleteRegistrationJournal@l##"));
            items.Add(GetSysAct(EnumDictionaryActions.AddContactType, EnumObjects.DictionaryContactType, "##l@DictionaryActions:AddContactType@l##"));
            items.Add(GetSysAct(EnumDictionaryActions.ModifyContactType, EnumObjects.DictionaryContactType, "##l@DictionaryActions:ModifyContactType@l##"));
            items.Add(GetSysAct(EnumDictionaryActions.DeleteContactType, EnumObjects.DictionaryContactType, "##l@DictionaryActions:DeleteContactType@l##"));
            items.Add(GetSysAct(EnumDictionaryActions.AddAgent, EnumObjects.DictionaryAgents, "##l@DictionaryActions:AddAgent@l##"));
            items.Add(GetSysAct(EnumDictionaryActions.ModifyAgent, EnumObjects.DictionaryAgents, "##l@DictionaryActions:ModifyAgent@l##"));
            items.Add(GetSysAct(EnumDictionaryActions.DeleteAgent, EnumObjects.DictionaryAgents, "##l@DictionaryActions:DeleteAgent@l##"));
            items.Add(GetSysAct(EnumDictionaryActions.AddAgentContact, EnumObjects.DictionaryContacts, "##l@DictionaryActions:AddAgentContact@l##"));
            items.Add(GetSysAct(EnumDictionaryActions.ModifyAgentContact, EnumObjects.DictionaryContacts, "##l@DictionaryActions:ModifyAgentContact@l##"));
            items.Add(GetSysAct(EnumDictionaryActions.DeleteAgentContact, EnumObjects.DictionaryContacts, "##l@DictionaryActions:DeleteAgentContact@l##"));
            items.Add(GetSysAct(EnumDictionaryActions.AddAgentAddress, EnumObjects.DictionaryAgentAddresses, "##l@DictionaryActions:AddAgentAddress@l##"));
            items.Add(GetSysAct(EnumDictionaryActions.ModifyAgentAddress, EnumObjects.DictionaryAgentAddresses, "##l@DictionaryActions:ModifyAgentAddress@l##"));
            items.Add(GetSysAct(EnumDictionaryActions.DeleteAgentAddress, EnumObjects.DictionaryAgentAddresses, "##l@DictionaryActions:DeleteAgentAddress@l##"));
            items.Add(GetSysAct(EnumDictionaryActions.AddAgentPerson, EnumObjects.DictionaryAgentPersons, "##l@DictionaryActions:AddAgentPerson@l##"));
            items.Add(GetSysAct(EnumDictionaryActions.ModifyAgentPerson, EnumObjects.DictionaryAgentPersons, "##l@DictionaryActions:ModifyAgentPerson@l##"));
            items.Add(GetSysAct(EnumDictionaryActions.DeleteAgentPerson, EnumObjects.DictionaryAgentPersons, "##l@DictionaryActions:DeleteAgentPerson@l##"));
            items.Add(GetSysAct(EnumDictionaryActions.AddDepartment, EnumObjects.DictionaryDepartments, "##l@DictionaryActions:AddDepartment@l##"));
            items.Add(GetSysAct(EnumDictionaryActions.ModifyDepartment, EnumObjects.DictionaryDepartments, "##l@DictionaryActions:ModifyDepartment@l##"));
            items.Add(GetSysAct(EnumDictionaryActions.DeleteDepartment, EnumObjects.DictionaryDepartments, "##l@DictionaryActions:DeleteDepartment@l##"));
            items.Add(GetSysAct(EnumDictionaryActions.AddPosition, EnumObjects.DictionaryPositions, "##l@DictionaryActions:AddPosition@l##"));
            items.Add(GetSysAct(EnumDictionaryActions.ModifyPosition, EnumObjects.DictionaryPositions, "##l@DictionaryActions:ModifyPosition@l##"));
            items.Add(GetSysAct(EnumDictionaryActions.DeletePosition, EnumObjects.DictionaryPositions, "##l@DictionaryActions:DeletePosition@l##"));
            items.Add(GetSysAct(EnumDictionaryActions.AddAgentEmployee, EnumObjects.DictionaryAgentEmployees, "##l@DictionaryActions:AddAgentEmployee@l##"));
            items.Add(GetSysAct(EnumDictionaryActions.ModifyAgentEmployee, EnumObjects.DictionaryAgentEmployees, "##l@DictionaryActions:ModifyAgentEmployee@l##"));
            items.Add(GetSysAct(EnumDictionaryActions.DeleteAgentEmployee, EnumObjects.DictionaryAgentEmployees, "##l@DictionaryActions:DeleteAgentEmployee@l##"));
            items.Add(GetSysAct(EnumDictionaryActions.AddAgentCompany, EnumObjects.DictionaryAgentCompanies, "##l@DictionaryActions:AddAgentCompany@l##"));
            items.Add(GetSysAct(EnumDictionaryActions.ModifyAgentCompany, EnumObjects.DictionaryAgentCompanies, "##l@DictionaryActions:ModifyAgentCompany@l##"));
            items.Add(GetSysAct(EnumDictionaryActions.DeleteAgentCompany, EnumObjects.DictionaryAgentCompanies, "##l@DictionaryActions:DeleteAgentCompany@l##"));
            items.Add(GetSysAct(EnumDictionaryActions.AddAgentBank, EnumObjects.DictionaryAgentBanks, "##l@DictionaryActions:AddAgentBank@l##"));
            items.Add(GetSysAct(EnumDictionaryActions.ModifyAgentBank, EnumObjects.DictionaryAgentBanks, "##l@DictionaryActions:ModifyAgentBank@l##"));
            items.Add(GetSysAct(EnumDictionaryActions.DeleteAgentBank, EnumObjects.DictionaryAgentBanks, "##l@DictionaryActions:DeleteAgentBank@l##"));
            items.Add(GetSysAct(EnumDictionaryActions.AddAgentAccount, EnumObjects.DictionaryAgentAccounts, "##l@DictionaryActions:AddAgentAccount@l##"));
            items.Add(GetSysAct(EnumDictionaryActions.ModifyAgentAccount, EnumObjects.DictionaryAgentAccounts, "##l@DictionaryActions:ModifyAgentAccount@l##"));
            items.Add(GetSysAct(EnumDictionaryActions.DeleteAgentAccount, EnumObjects.DictionaryAgentAccounts, "##l@DictionaryActions:DeleteAgentAccount@l##"));
            items.Add(GetSysAct(EnumDictionaryActions.AddStandartSendListContent, EnumObjects.DictionaryStandartSendListContent, "##l@DictionaryActions:AddStandartSendListContent@l##"));
            items.Add(GetSysAct(EnumDictionaryActions.ModifyStandartSendListContent, EnumObjects.DictionaryStandartSendListContent, "##l@DictionaryActions:ModifyStandartSendListContent@l##"));
            items.Add(GetSysAct(EnumDictionaryActions.DeleteStandartSendListContent, EnumObjects.DictionaryStandartSendListContent, "##l@DictionaryActions:DeleteStandartSendListContent@l##"));
            items.Add(GetSysAct(EnumDictionaryActions.AddStandartSendList, EnumObjects.DictionaryStandartSendLists, "##l@DictionaryActions:AddStandartSendList@l##"));
            items.Add(GetSysAct(EnumDictionaryActions.ModifyStandartSendList, EnumObjects.DictionaryStandartSendLists, "##l@DictionaryActions:ModifyStandartSendList@l##"));
            items.Add(GetSysAct(EnumDictionaryActions.DeleteStandartSendList, EnumObjects.DictionaryStandartSendLists, "##l@DictionaryActions:DeleteStandartSendList@l##"));
            items.Add(GetSysAct(EnumDictionaryActions.AddAgentClientCompany, EnumObjects.DictionaryAgentClientCompanies, "##l@DictionaryActions:AddAgentClientCompany@l##"));
            items.Add(GetSysAct(EnumDictionaryActions.ModifyAgentClientCompany, EnumObjects.DictionaryAgentClientCompanies, "##l@DictionaryActions:ModifyAgentClientCompany@l##"));
            items.Add(GetSysAct(EnumDictionaryActions.DeleteAgentClientCompany, EnumObjects.DictionaryAgentClientCompanies, "##l@DictionaryActions:DeleteAgentClientCompany@l##"));
            items.Add(GetSysAct(EnumDictionaryActions.AddExecutorType, EnumObjects.DictionaryPositionExecutorTypes, "##l@DictionaryActions:AddExecutorType@l##"));
            items.Add(GetSysAct(EnumDictionaryActions.ModifyExecutorType, EnumObjects.DictionaryPositionExecutorTypes, "##l@DictionaryActions:ModifyExecutorType@l##"));
            items.Add(GetSysAct(EnumDictionaryActions.DeleteExecutorType, EnumObjects.DictionaryPositionExecutorTypes, "##l@DictionaryActions:DeleteExecutorType@l##"));
            items.Add(GetSysAct(EnumDictionaryActions.AddExecutor, EnumObjects.DictionaryPositionExecutors, "##l@DictionaryActions:AddExecutor@l##"));
            items.Add(GetSysAct(EnumDictionaryActions.ModifyExecutor, EnumObjects.DictionaryPositionExecutors, "##l@DictionaryActions:ModifyExecutor@l##"));
            items.Add(GetSysAct(EnumDictionaryActions.DeleteExecutor, EnumObjects.DictionaryPositionExecutors, "##l@DictionaryActions:DeleteExecutor@l##"));
            items.Add(GetSysAct(EnumDictionaryActions.AddTag, EnumObjects.DictionaryTag, "##l@DictionaryActions:AddTag@l##"));
            items.Add(GetSysAct(EnumDictionaryActions.ModifyTag, EnumObjects.DictionaryTag, "##l@DictionaryActions:ModifyTag@l##"));
            items.Add(GetSysAct(EnumDictionaryActions.DeleteTag, EnumObjects.DictionaryTag, "##l@DictionaryActions:DeleteTag@l##"));
            items.Add(GetSysAct(EnumDictionaryActions.AddCustomDictionaryType, EnumObjects.CustomDictionaryTypes, "##l@DictionaryActions:AddCustomDictionaryType@l##"));
            items.Add(GetSysAct(EnumDictionaryActions.ModifyCustomDictionaryType, EnumObjects.CustomDictionaryTypes, "##l@DictionaryActions:ModifyCustomDictionaryType@l##"));
            items.Add(GetSysAct(EnumDictionaryActions.DeleteCustomDictionaryType, EnumObjects.CustomDictionaryTypes, "##l@DictionaryActions:DeleteCustomDictionaryType@l##"));
            items.Add(GetSysAct(EnumDictionaryActions.AddCustomDictionary, EnumObjects.CustomDictionaries, "##l@DictionaryActions:AddCustomDictionary@l##"));
            items.Add(GetSysAct(EnumDictionaryActions.ModifyCustomDictionary, EnumObjects.CustomDictionaries, "##l@DictionaryActions:ModifyCustomDictionary@l##"));
            items.Add(GetSysAct(EnumDictionaryActions.DeleteCustomDictionary, EnumObjects.CustomDictionaries, "##l@DictionaryActions:DeleteCustomDictionary@l##"));
            items.Add(GetSysAct(EnumPropertyActions.AddProperty, EnumObjects.Properties, "##l@PropertyAction:AddProperty@l##"));
            items.Add(GetSysAct(EnumPropertyActions.ModifyProperty, EnumObjects.Properties, "##l@PropertyAction:ModifyProperty@l##"));
            items.Add(GetSysAct(EnumPropertyActions.DeleteProperty, EnumObjects.Properties, "##l@PropertyAction:DeleteProperty@l##"));
            items.Add(GetSysAct(EnumPropertyActions.AddPropertyLink, EnumObjects.PropertyLinks, "##l@PropertyAction:AddPropertyLink@l##"));
            items.Add(GetSysAct(EnumPropertyActions.ModifyPropertyLink, EnumObjects.PropertyLinks, "##l@PropertyAction:ModifyPropertyLink@l##"));
            items.Add(GetSysAct(EnumPropertyActions.DeletePropertyLink, EnumObjects.PropertyLinks, "##l@PropertyAction:DeletePropertyLink@l##"));
            items.Add(GetSysAct(EnumPropertyActions.ModifyPropertyValues, EnumObjects.PropertyValues, "##l@PropertyAction:ModifyPropertyValues@l##"));

            items.Add(GetSysAct(EnumEncryptionActions.AddEncryptionCertificate, EnumObjects.EncryptionCertificates, "##l@EncryptionActions:AddEncryptionCertificate@l##"));
            items.Add(GetSysAct(EnumEncryptionActions.ModifyEncryptionCertificate, EnumObjects.EncryptionCertificates, "##l@EncryptionActions:ModifyEncryptionCertificate@l##"));
            items.Add(GetSysAct(EnumEncryptionActions.VerifyPdf, EnumObjects.EncryptionCertificates, "##l@EncryptionActions:VerifyPdf@l##"));
            items.Add(GetSysAct(EnumEncryptionActions.DeleteEncryptionCertificate, EnumObjects.EncryptionCertificates, "##l@EncryptionActions:DeleteEncryptionCertificate@l##"));

            items.Add(GetSysAct(EnumAdminActions.AddRole, EnumObjects.AdminRoles, "##l@AdminActions:AddRole@l##"));
            items.Add(GetSysAct(EnumAdminActions.ModifyRole, EnumObjects.AdminRoles, "##l@AdminActions:ModifyRole@l##"));
            items.Add(GetSysAct(EnumAdminActions.DeleteRole, EnumObjects.AdminRoles, "##l@AdminActions:DeleteRole@l##"));

            items.Add(GetSysAct(EnumAdminActions.AddPositionRole, EnumObjects.AdminPositionRoles, "##l@AdminActions:AddPositionRole@l##"));
            items.Add(GetSysAct(EnumAdminActions.ModifyPositionRole, EnumObjects.AdminPositionRoles, "##l@AdminActions:ModifyPositionRole@l##"));
            items.Add(GetSysAct(EnumAdminActions.DeletePositionRole, EnumObjects.AdminPositionRoles, "##l@AdminActions:DeletePositionRole@l##"));

            items.Add(GetSysAct(EnumAdminActions.AddUserRole, EnumObjects.AdminUserRoles, "##l@AdminActions:AddUserRole@l##"));
            items.Add(GetSysAct(EnumAdminActions.ModifyUserRole, EnumObjects.AdminUserRoles, "##l@AdminActions:ModifyUserRole@l##"));
            items.Add(GetSysAct(EnumAdminActions.DeleteUserRole, EnumObjects.AdminUserRoles, "##l@AdminActions:DeleteUserRole@l##"));

            items.Add(GetSysAct(EnumAdminActions.SetSubordination, EnumObjects.AdminSubordination, "##l@AdminActions:SetSubordination@l##"));
            items.Add(GetSysAct(EnumAdminActions.SetSubordinationByCompany, EnumObjects.AdminSubordination, "##l@AdminActions:SetSubordinationByCompany@l##"));
            items.Add(GetSysAct(EnumAdminActions.SetSubordinationByDepartment, EnumObjects.AdminSubordination, "##l@AdminActions:SetSubordinationByDepartment@l##"));
            items.Add(GetSysAct(EnumAdminActions.SetDefaultSubordination, EnumObjects.AdminSubordination, "##l@AdminActions:SetDefaultSubordination@l##"));
            items.Add(GetSysAct(EnumAdminActions.DuplicateSubordinations, EnumObjects.AdminSubordination, "##l@AdminActions:DuplicateSubordinations@l##"));
            items.Add(GetSysAct(EnumAdminActions.SetAllSubordination, EnumObjects.AdminSubordination, "##l@AdminActions:SetAllSubordination@l##"));

            items.Add(GetSysAct(EnumAdminActions.AddDepartmentAdmin, EnumObjects.DictionaryDepartments, "##l@AdminActions:AddDepartmentAdmin@l##"));
            items.Add(GetSysAct(EnumAdminActions.DeleteDepartmentAdmin, EnumObjects.DictionaryDepartments, "##l@AdminActions:DeleteDepartmentAdmin@l##"));

            items.Add(GetSysAct(EnumSystemActions.SetSetting, EnumObjects.SystemSettings, "##l@SystemActions:SetSetting@l##"));

            // при добавлении действия не забудь добавить перевод! DMS_WebAPI.Models.ApplicationDbImportData GetAdminLanguageValuesForActions

            return items;
        }

        private static SystemActions GetSysAct(EnumAdminActions id, EnumObjects objId, string description, string category = null, bool isGrantable = true, bool isGrantableByRecordId = false, bool isVisible = true, int? grantId = null)
        { return GetSystemAction((int)id, id.ToString(), objId, description, category, isGrantable, isGrantableByRecordId, isVisible, grantId); }

        private static SystemActions GetSysAct(EnumEncryptionActions id, EnumObjects objId, string description, string category = null, bool isGrantable = true, bool isGrantableByRecordId = false, bool isVisible = true, int? grantId = null)
        { return GetSystemAction((int)id, id.ToString(), objId, description, category, isGrantable, isGrantableByRecordId, isVisible, grantId); }

        private static SystemActions GetSysAct(EnumPropertyActions id, EnumObjects objId, string description, string category = null, bool isGrantable = true, bool isGrantableByRecordId = false, bool isVisible = true, int? grantId = null)
        { return GetSystemAction((int)id, id.ToString(), objId, description, category, isGrantable, isGrantableByRecordId, isVisible, grantId); }

        private static SystemActions GetSysAct(EnumDictionaryActions id, EnumObjects objId, string description, string category = null, bool isGrantable = true, bool isGrantableByRecordId = false, bool isVisible = true, int? grantId = null)
        { return GetSystemAction((int)id, id.ToString(), objId, description, category, isGrantable, isGrantableByRecordId, isVisible, grantId); }

        private static SystemActions GetSysAct(EnumDocumentActions id, EnumObjects objId, string description, string category = null, bool isGrantable = true, bool isGrantableByRecordId = false, bool isVisible = true, int? grantId = null)
        { return GetSystemAction((int)id, id.ToString(), objId, description, category, isGrantable, isGrantableByRecordId, isVisible, grantId); }

        private static SystemActions GetSysAct(EnumSystemActions id, EnumObjects objId, string description, string category = null, bool isGrantable = true, bool isGrantableByRecordId = false, bool isVisible = true, int? grantId = null)
        { return GetSystemAction((int)id, id.ToString(), objId, description, category, isGrantable, isGrantableByRecordId, isVisible, grantId); }

        private static SystemActions GetSystemAction(int id, string code, EnumObjects objId, string description, string category = null, bool isGrantable = true, bool isGrantableByRecordId = false, bool isVisible = true, int? grantId = null)
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
                GrantId = grantId,
                Category = category
            };
        }
        #endregion

        public static List<AdminAccessLevels> GetAdminAccessLevels()
        {
            var items = new List<AdminAccessLevels>();

            items.Add(new AdminAccessLevels { Id = (int)EnumAccessLevels.Personally, Code = null, Name = "Только лично", LastChangeUserId = (int)EnumSystemUsers.AdminUser, LastChangeDate = DateTime.Now });
            items.Add(new AdminAccessLevels { Id = (int)EnumAccessLevels.PersonallyAndReferents, Code = null, Name = "Лично+референты", LastChangeUserId = (int)EnumSystemUsers.AdminUser, LastChangeDate = DateTime.Now });
            items.Add(new AdminAccessLevels { Id = (int)EnumAccessLevels.PersonallyAndIOAndReferents, Code = null, Name = "Лично+ИО+референты", LastChangeUserId = (int)EnumSystemUsers.AdminUser, LastChangeDate = DateTime.Now });

            return items;
        }

        public static List<SystemUIElements> GetSystemUIElements()
        {
            var items = new List<SystemUIElements>();

            items.Add(new SystemUIElements { Id = 1, ActionId = 100003, Code = "GeneralInfo", TypeCode = "display_only_text", Description = "Общая информация", Label = null, Hint = null, ValueTypeId = 1, IsMandatory = false, IsReadOnly = false, IsVisible = false, SelectAPI = null, SelectFilter = null, SelectFieldCode = null, SelectDescriptionFieldCode = null, ValueFieldCode = "GeneralInfo", ValueDescriptionFieldCode = "GeneralInfo", Format = null });
            items.Add(new SystemUIElements { Id = 2, ActionId = 100003, Code = "DocumentSubject", TypeCode = "select", Description = "Тематика документа", Label = "Тематика документа", Hint = "Выберите из словаря тематику документа", ValueTypeId = 2, IsMandatory = false, IsReadOnly = false, IsVisible = false, SelectAPI = "DictionaryDocumentSubjects", SelectFilter = null, SelectFieldCode = "Id", SelectDescriptionFieldCode = "Name", ValueFieldCode = "DocumentSubjectId", ValueDescriptionFieldCode = "DocumentSubjectName", Format = null });
            items.Add(new SystemUIElements { Id = 3, ActionId = 100003, Code = "Description", TypeCode = "textarea", Description = "Краткое содержание", Label = "Краткое содержание", Hint = "Введите краткое содержание", ValueTypeId = 1, IsMandatory = false, IsReadOnly = false, IsVisible = false, SelectAPI = null, SelectFilter = null, SelectFieldCode = null, SelectDescriptionFieldCode = null, ValueFieldCode = "Description", ValueDescriptionFieldCode = "Description", Format = null });
            items.Add(new SystemUIElements { Id = 4, ActionId = 100003, Code = "SenderAgent", TypeCode = "select", Description = "Контрагент, от которого получен документ", Label = "Организация", Hint = "Выберите из словаря контрагента, от которого получен документ", ValueTypeId = 2, IsMandatory = false, IsReadOnly = false, IsVisible = false, SelectAPI = "DictionaryAgents", SelectFilter = "{'IsCompany' : 'True'}", SelectFieldCode = "Id", SelectDescriptionFieldCode = "Name", ValueFieldCode = "SenderAgentId", ValueDescriptionFieldCode = "SenderAgentName", Format = null });
            items.Add(new SystemUIElements { Id = 5, ActionId = 100003, Code = "SenderAgentPerson", TypeCode = "select", Description = "Контактное лицо в организации", Label = "Контакт", Hint = "Выберите из словаря контактное лицо в организации, от которой получен документ", ValueTypeId = 2, IsMandatory = false, IsReadOnly = false, IsVisible = false, SelectAPI = "DictionaryAgentPersons", SelectFilter = "{'AgentCompanyId' : '@SenderAgentId'}", SelectFieldCode = "Id", SelectDescriptionFieldCode = "FullName", ValueFieldCode = "SenderAgentPersonId", ValueDescriptionFieldCode = "SenderAgentPersonName", Format = null });
            items.Add(new SystemUIElements { Id = 6, ActionId = 100003, Code = "SenderNumber", TypeCode = "input", Description = "Входящий номер документа", Label = "Входящий номер документа", Hint = "Введите входящий номер документа", ValueTypeId = 1, IsMandatory = false, IsReadOnly = false, IsVisible = false, SelectAPI = null, SelectFilter = null, SelectFieldCode = null, SelectDescriptionFieldCode = null, ValueFieldCode = "SenderNumber", ValueDescriptionFieldCode = "SenderNumber", Format = null });
            items.Add(new SystemUIElements { Id = 7, ActionId = 100003, Code = "SenderDate", TypeCode = "input", Description = "Дата входящего документа", Label = "Дата входящего документа", Hint = "Введите дату входящего документа", ValueTypeId = 3, IsMandatory = false, IsReadOnly = false, IsVisible = false, SelectAPI = null, SelectFilter = null, SelectFieldCode = null, SelectDescriptionFieldCode = null, ValueFieldCode = "SenderDate", ValueDescriptionFieldCode = "SenderDate", Format = null });
            items.Add(new SystemUIElements { Id = 8, ActionId = 100003, Code = "Addressee", TypeCode = "input", Description = "Кому адресован документ", Label = "Кому адресован документ", Hint = "Введите кому адресован документ", ValueTypeId = 1, IsMandatory = false, IsReadOnly = false, IsVisible = false, SelectAPI = null, SelectFilter = null, SelectFieldCode = null, SelectDescriptionFieldCode = null, ValueFieldCode = "Addressee", ValueDescriptionFieldCode = "Addressee", Format = null });
            items.Add(new SystemUIElements { Id = 9, ActionId = 100003, Code = "AccessLevel", TypeCode = "select", Description = "Уровень доступа", Label = "Уровень доступа", Hint = "Выберите из словаря уровень доступа", ValueTypeId = 2, IsMandatory = false, IsReadOnly = false, IsVisible = false, SelectAPI = "AdminAccessLevels", SelectFilter = null, SelectFieldCode = "Id", SelectDescriptionFieldCode = "Name", ValueFieldCode = "AccessLevelId", ValueDescriptionFieldCode = "AccessLevelName", Format = null });

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

            items.Add(new DictionaryFileTypes { Id = 0, Name = "Main", LastChangeUserId = (int)EnumSystemUsers.AdminUser, LastChangeDate = DateTime.Now });
            items.Add(new DictionaryFileTypes { Id = 1, Name = "Additional", LastChangeUserId = (int)EnumSystemUsers.AdminUser, LastChangeDate = DateTime.Now });
            items.Add(new DictionaryFileTypes { Id = 2, Name = "SubscribePdf", LastChangeUserId = (int)EnumSystemUsers.AdminUser, LastChangeDate = DateTime.Now });

            return items;
        }

        public static List<DictionarySigningTypes> GetDictionarySigningTypes()
        {
            var items = new List<DictionarySigningTypes>();

            items.Add(new DictionarySigningTypes { Id = 0, Name = "Hash", LastChangeUserId = (int)EnumSystemUsers.AdminUser, LastChangeDate = DateTime.Now });
            items.Add(new DictionarySigningTypes { Id = 1, Name = "InternalSign", LastChangeUserId = (int)EnumSystemUsers.AdminUser, LastChangeDate = DateTime.Now });
            items.Add(new DictionarySigningTypes { Id = 2, Name = "CertificateSign", LastChangeUserId = (int)EnumSystemUsers.AdminUser, LastChangeDate = DateTime.Now });

            return items;
        }

        public static List<DictionaryDocumentDirections> GetDictionaryDocumentDirections()
        {
            var items = new List<DictionaryDocumentDirections>();

            items.Add(new DictionaryDocumentDirections { Id = 1, Code = "1", Name = "##l@DictionaryDocumentDirections:Incoming@l##", LastChangeUserId = (int)EnumSystemUsers.AdminUser, LastChangeDate = DateTime.Now });
            items.Add(new DictionaryDocumentDirections { Id = 2, Code = "2", Name = "##l@DictionaryDocumentDirections:Outcoming@l##", LastChangeUserId = (int)EnumSystemUsers.AdminUser, LastChangeDate = DateTime.Now });
            items.Add(new DictionaryDocumentDirections { Id = 3, Code = "3", Name = "##l@DictionaryDocumentDirections:Internal@l##", LastChangeUserId = (int)EnumSystemUsers.AdminUser, LastChangeDate = DateTime.Now });

            return items;
        }

        public static List<DictionaryEventTypes> GetDictionaryEventTypes()
        {
            var items = new List<DictionaryEventTypes>();

            items.Add(new DictionaryEventTypes { Id = 100, Code = null, Name = "Поступил входящий документ", SourceDescription = null, TargetDescription = null, ImportanceEventTypeId = 1, LastChangeUserId = (int)EnumSystemUsers.AdminUser, LastChangeDate = DateTime.Now, WaitDescription = null });
            items.Add(new DictionaryEventTypes { Id = 110, Code = null, Name = "Создан проект", SourceDescription = null, TargetDescription = null, ImportanceEventTypeId = 1, LastChangeUserId = (int)EnumSystemUsers.AdminUser, LastChangeDate = DateTime.Now, WaitDescription = null });
            items.Add(new DictionaryEventTypes { Id = 115, Code = null, Name = "Добавлен файл", SourceDescription = null, TargetDescription = null, ImportanceEventTypeId = 2, LastChangeUserId = (int)EnumSystemUsers.AdminUser, LastChangeDate = DateTime.Now, WaitDescription = null });
            items.Add(new DictionaryEventTypes { Id = 117, Code = null, Name = "Изменен файл", SourceDescription = null, TargetDescription = null, ImportanceEventTypeId = 2, LastChangeUserId = (int)EnumSystemUsers.AdminUser, LastChangeDate = DateTime.Now, WaitDescription = null });
            items.Add(new DictionaryEventTypes { Id = 119, Code = null, Name = "Удален файл", SourceDescription = null, TargetDescription = null, ImportanceEventTypeId = 2, LastChangeUserId = (int)EnumSystemUsers.AdminUser, LastChangeDate = DateTime.Now, WaitDescription = null });
            items.Add(new DictionaryEventTypes { Id = 120, Code = null, Name = "Исполнение документа", SourceDescription = null, TargetDescription = null, ImportanceEventTypeId = 1, LastChangeUserId = (int)EnumSystemUsers.AdminUser, LastChangeDate = DateTime.Now, WaitDescription = null });
            items.Add(new DictionaryEventTypes { Id = 130, Code = null, Name = "Подписание документа", SourceDescription = null, TargetDescription = null, ImportanceEventTypeId = 1, LastChangeUserId = (int)EnumSystemUsers.AdminUser, LastChangeDate = DateTime.Now, WaitDescription = null });
            items.Add(new DictionaryEventTypes { Id = 140, Code = null, Name = "Визирование документа", SourceDescription = null, TargetDescription = null, ImportanceEventTypeId = 1, LastChangeUserId = (int)EnumSystemUsers.AdminUser, LastChangeDate = DateTime.Now, WaitDescription = null });
            items.Add(new DictionaryEventTypes { Id = 150, Code = null, Name = "Утверждение документа", SourceDescription = null, TargetDescription = null, ImportanceEventTypeId = 1, LastChangeUserId = (int)EnumSystemUsers.AdminUser, LastChangeDate = DateTime.Now, WaitDescription = null });
            items.Add(new DictionaryEventTypes { Id = 160, Code = null, Name = "Согласование документа", SourceDescription = null, TargetDescription = null, ImportanceEventTypeId = 1, LastChangeUserId = (int)EnumSystemUsers.AdminUser, LastChangeDate = DateTime.Now, WaitDescription = null });
            items.Add(new DictionaryEventTypes { Id = 200, Code = null, Name = "Направлен для сведения", SourceDescription = null, TargetDescription = null, ImportanceEventTypeId = 1, LastChangeUserId = (int)EnumSystemUsers.AdminUser, LastChangeDate = DateTime.Now, WaitDescription = null });
            items.Add(new DictionaryEventTypes { Id = 205, Code = null, Name = "Передано управление проектом", SourceDescription = null, TargetDescription = null, ImportanceEventTypeId = 1, LastChangeUserId = (int)EnumSystemUsers.AdminUser, LastChangeDate = DateTime.Now, WaitDescription = null });
            items.Add(new DictionaryEventTypes { Id = 207, Code = null, Name = "Замена должности в документе", SourceDescription = null, TargetDescription = null, ImportanceEventTypeId = 2, LastChangeUserId = (int)EnumSystemUsers.AdminUser, LastChangeDate = DateTime.Now, WaitDescription = null });
            items.Add(new DictionaryEventTypes { Id = 210, Code = null, Name = "Направлен для исполнения", SourceDescription = null, TargetDescription = null, ImportanceEventTypeId = 1, LastChangeUserId = (int)EnumSystemUsers.AdminUser, LastChangeDate = DateTime.Now, WaitDescription = "Исполнение" });
            items.Add(new DictionaryEventTypes { Id = 211, Code = null, Name = "Изменены параметры направлен для исполнения", SourceDescription = null, TargetDescription = null, ImportanceEventTypeId = 1, LastChangeUserId = (int)EnumSystemUsers.AdminUser, LastChangeDate = DateTime.Now, WaitDescription = "Исполнение" });
            items.Add(new DictionaryEventTypes { Id = 212, Code = null, Name = "Направлен для контроля", SourceDescription = null, TargetDescription = null, ImportanceEventTypeId = 1, LastChangeUserId = (int)EnumSystemUsers.AdminUser, LastChangeDate = DateTime.Now, WaitDescription = "Контроль" });
            items.Add(new DictionaryEventTypes { Id = 213, Code = null, Name = "Изменены параметры направлен для контроля", SourceDescription = null, TargetDescription = null, ImportanceEventTypeId = 1, LastChangeUserId = (int)EnumSystemUsers.AdminUser, LastChangeDate = DateTime.Now, WaitDescription = "Контроль" });
            items.Add(new DictionaryEventTypes { Id = 214, Code = null, Name = "Направлен для отв.исполнения", SourceDescription = null, TargetDescription = null, ImportanceEventTypeId = 1, LastChangeUserId = (int)EnumSystemUsers.AdminUser, LastChangeDate = DateTime.Now, WaitDescription = "Исполнение" });
            items.Add(new DictionaryEventTypes { Id = 215, Code = null, Name = "Изменены параметры направлен для отв.исполнения", SourceDescription = null, TargetDescription = null, ImportanceEventTypeId = 1, LastChangeUserId = (int)EnumSystemUsers.AdminUser, LastChangeDate = DateTime.Now, WaitDescription = "Исполнение" });
            items.Add(new DictionaryEventTypes { Id = 220, Code = null, Name = "Направлен для рассмотрения", SourceDescription = null, TargetDescription = null, ImportanceEventTypeId = 1, LastChangeUserId = (int)EnumSystemUsers.AdminUser, LastChangeDate = DateTime.Now, WaitDescription = null });
            items.Add(new DictionaryEventTypes { Id = 221, Code = null, Name = "Рассмотрен положительно", SourceDescription = null, TargetDescription = null, ImportanceEventTypeId = 2, LastChangeUserId = (int)EnumSystemUsers.AdminUser, LastChangeDate = DateTime.Now, WaitDescription = null });
            items.Add(new DictionaryEventTypes { Id = 222, Code = null, Name = "Рассмотрен отрицательно", SourceDescription = null, TargetDescription = null, ImportanceEventTypeId = 2, LastChangeUserId = (int)EnumSystemUsers.AdminUser, LastChangeDate = DateTime.Now, WaitDescription = null });
            items.Add(new DictionaryEventTypes { Id = 230, Code = null, Name = "Направлен для сведения внешнему агенту", SourceDescription = null, TargetDescription = null, ImportanceEventTypeId = 1, LastChangeUserId = (int)EnumSystemUsers.AdminUser, LastChangeDate = DateTime.Now, WaitDescription = null });
            items.Add(new DictionaryEventTypes { Id = 250, Code = null, Name = "Направлен на визирование", SourceDescription = null, TargetDescription = null, ImportanceEventTypeId = 1, LastChangeUserId = (int)EnumSystemUsers.AdminUser, LastChangeDate = DateTime.Now, WaitDescription = "Виза" });
            items.Add(new DictionaryEventTypes { Id = 251, Code = null, Name = "Завизирован", SourceDescription = null, TargetDescription = null, ImportanceEventTypeId = 2, LastChangeUserId = (int)EnumSystemUsers.AdminUser, LastChangeDate = DateTime.Now, WaitDescription = null });
            items.Add(new DictionaryEventTypes { Id = 252, Code = null, Name = "Отказано в визировании", SourceDescription = null, TargetDescription = null, ImportanceEventTypeId = 2, LastChangeUserId = (int)EnumSystemUsers.AdminUser, LastChangeDate = DateTime.Now, WaitDescription = null });
            items.Add(new DictionaryEventTypes { Id = 253, Code = null, Name = "Отозван с визирования", SourceDescription = null, TargetDescription = null, ImportanceEventTypeId = 2, LastChangeUserId = (int)EnumSystemUsers.AdminUser, LastChangeDate = DateTime.Now, WaitDescription = null });
            items.Add(new DictionaryEventTypes { Id = 260, Code = null, Name = "Направлен на согласование", SourceDescription = null, TargetDescription = null, ImportanceEventTypeId = 1, LastChangeUserId = (int)EnumSystemUsers.AdminUser, LastChangeDate = DateTime.Now, WaitDescription = "Согласование" });
            items.Add(new DictionaryEventTypes { Id = 261, Code = null, Name = "Согласован", SourceDescription = null, TargetDescription = null, ImportanceEventTypeId = 2, LastChangeUserId = (int)EnumSystemUsers.AdminUser, LastChangeDate = DateTime.Now, WaitDescription = null });
            items.Add(new DictionaryEventTypes { Id = 262, Code = null, Name = "Отказано в согласовании", SourceDescription = null, TargetDescription = null, ImportanceEventTypeId = 2, LastChangeUserId = (int)EnumSystemUsers.AdminUser, LastChangeDate = DateTime.Now, WaitDescription = null });
            items.Add(new DictionaryEventTypes { Id = 263, Code = null, Name = "Отозван с согласования", SourceDescription = null, TargetDescription = null, ImportanceEventTypeId = 2, LastChangeUserId = (int)EnumSystemUsers.AdminUser, LastChangeDate = DateTime.Now, WaitDescription = null });
            items.Add(new DictionaryEventTypes { Id = 270, Code = null, Name = "Направлен на утверждение", SourceDescription = null, TargetDescription = null, ImportanceEventTypeId = 1, LastChangeUserId = (int)EnumSystemUsers.AdminUser, LastChangeDate = DateTime.Now, WaitDescription = "Утверждение" });
            items.Add(new DictionaryEventTypes { Id = 271, Code = null, Name = "Утвержден", SourceDescription = null, TargetDescription = null, ImportanceEventTypeId = 2, LastChangeUserId = (int)EnumSystemUsers.AdminUser, LastChangeDate = DateTime.Now, WaitDescription = null });
            items.Add(new DictionaryEventTypes { Id = 272, Code = null, Name = "Отказано в утверждении", SourceDescription = null, TargetDescription = null, ImportanceEventTypeId = 2, LastChangeUserId = (int)EnumSystemUsers.AdminUser, LastChangeDate = DateTime.Now, WaitDescription = null });
            items.Add(new DictionaryEventTypes { Id = 273, Code = null, Name = "Отозван с утверждения", SourceDescription = null, TargetDescription = null, ImportanceEventTypeId = 2, LastChangeUserId = (int)EnumSystemUsers.AdminUser, LastChangeDate = DateTime.Now, WaitDescription = null });
            items.Add(new DictionaryEventTypes { Id = 280, Code = null, Name = "Направлен на подпись", SourceDescription = null, TargetDescription = null, ImportanceEventTypeId = 1, LastChangeUserId = (int)EnumSystemUsers.AdminUser, LastChangeDate = DateTime.Now, WaitDescription = "Подпись" });
            items.Add(new DictionaryEventTypes { Id = 281, Code = null, Name = "Подписан", SourceDescription = null, TargetDescription = null, ImportanceEventTypeId = 2, LastChangeUserId = (int)EnumSystemUsers.AdminUser, LastChangeDate = DateTime.Now, WaitDescription = null });
            items.Add(new DictionaryEventTypes { Id = 282, Code = null, Name = "Отказано в подписании", SourceDescription = null, TargetDescription = null, ImportanceEventTypeId = 2, LastChangeUserId = (int)EnumSystemUsers.AdminUser, LastChangeDate = DateTime.Now, WaitDescription = null });
            items.Add(new DictionaryEventTypes { Id = 283, Code = null, Name = "Отозван с подписания", SourceDescription = null, TargetDescription = null, ImportanceEventTypeId = 2, LastChangeUserId = (int)EnumSystemUsers.AdminUser, LastChangeDate = DateTime.Now, WaitDescription = null });
            items.Add(new DictionaryEventTypes { Id = 300, Code = null, Name = "Взят на контроль", SourceDescription = null, TargetDescription = null, ImportanceEventTypeId = 2, LastChangeUserId = (int)EnumSystemUsers.AdminUser, LastChangeDate = DateTime.Now, WaitDescription = "Контроль" });
            items.Add(new DictionaryEventTypes { Id = 301, Code = null, Name = "Снят с контроля", SourceDescription = null, TargetDescription = null, ImportanceEventTypeId = 2, LastChangeUserId = (int)EnumSystemUsers.AdminUser, LastChangeDate = DateTime.Now, WaitDescription = null });
            items.Add(new DictionaryEventTypes { Id = 310, Code = null, Name = "Изменить параметры контроля", SourceDescription = null, TargetDescription = null, ImportanceEventTypeId = 2, LastChangeUserId = (int)EnumSystemUsers.AdminUser, LastChangeDate = DateTime.Now, WaitDescription = "Контроль" });
            items.Add(new DictionaryEventTypes { Id = 315, Code = null, Name = "Изменить параметры контроля для исполнителя", SourceDescription = null, TargetDescription = null, ImportanceEventTypeId = 1, LastChangeUserId = (int)EnumSystemUsers.AdminUser, LastChangeDate = DateTime.Now, WaitDescription = null });
            items.Add(new DictionaryEventTypes { Id = 320, Code = null, Name = "Поручение выполнено", SourceDescription = null, TargetDescription = null, ImportanceEventTypeId = 2, LastChangeUserId = (int)EnumSystemUsers.AdminUser, LastChangeDate = DateTime.Now, WaitDescription = "Рассмотрение отчета" });
            items.Add(new DictionaryEventTypes { Id = 321, Code = null, Name = "Результат принят", SourceDescription = null, TargetDescription = null, ImportanceEventTypeId = 2, LastChangeUserId = (int)EnumSystemUsers.AdminUser, LastChangeDate = DateTime.Now, WaitDescription = null });
            items.Add(new DictionaryEventTypes { Id = 322, Code = null, Name = "Результат отклонен", SourceDescription = null, TargetDescription = null, ImportanceEventTypeId = 2, LastChangeUserId = (int)EnumSystemUsers.AdminUser, LastChangeDate = DateTime.Now, WaitDescription = null });
            items.Add(new DictionaryEventTypes { Id = 330, Code = null, Name = "Контролирую документ", SourceDescription = null, TargetDescription = null, ImportanceEventTypeId = 2, LastChangeUserId = (int)EnumSystemUsers.AdminUser, LastChangeDate = DateTime.Now, WaitDescription = null });
            items.Add(new DictionaryEventTypes { Id = 340, Code = null, Name = "Являюсь ответственным исполнителем", SourceDescription = null, TargetDescription = null, ImportanceEventTypeId = 2, LastChangeUserId = (int)EnumSystemUsers.AdminUser, LastChangeDate = DateTime.Now, WaitDescription = null });
            items.Add(new DictionaryEventTypes { Id = 350, Code = null, Name = "Являюсь соисполнителем", SourceDescription = null, TargetDescription = null, ImportanceEventTypeId = 2, LastChangeUserId = (int)EnumSystemUsers.AdminUser, LastChangeDate = DateTime.Now, WaitDescription = null });
            items.Add(new DictionaryEventTypes { Id = 360, Code = null, Name = "Принято", SourceDescription = null, TargetDescription = null, ImportanceEventTypeId = 3, LastChangeUserId = (int)EnumSystemUsers.AdminUser, LastChangeDate = DateTime.Now, WaitDescription = null });
            items.Add(new DictionaryEventTypes { Id = 400, Code = null, Name = "Отменено", SourceDescription = null, TargetDescription = null, ImportanceEventTypeId = 2, LastChangeUserId = (int)EnumSystemUsers.AdminUser, LastChangeDate = DateTime.Now, WaitDescription = null });
            items.Add(new DictionaryEventTypes { Id = 410, Code = null, Name = "Изменен текст", SourceDescription = null, TargetDescription = null, ImportanceEventTypeId = 2, LastChangeUserId = (int)EnumSystemUsers.AdminUser, LastChangeDate = DateTime.Now, WaitDescription = null });
            items.Add(new DictionaryEventTypes { Id = 420, Code = null, Name = "Установлен срок исполнения", SourceDescription = null, TargetDescription = null, ImportanceEventTypeId = 2, LastChangeUserId = (int)EnumSystemUsers.AdminUser, LastChangeDate = DateTime.Now, WaitDescription = null });
            items.Add(new DictionaryEventTypes { Id = 430, Code = null, Name = "Изменен срок исполнения", SourceDescription = null, TargetDescription = null, ImportanceEventTypeId = 2, LastChangeUserId = (int)EnumSystemUsers.AdminUser, LastChangeDate = DateTime.Now, WaitDescription = null });
            items.Add(new DictionaryEventTypes { Id = 440, Code = null, Name = "Назначен ответсвенный исполнитель", SourceDescription = null, TargetDescription = null, ImportanceEventTypeId = 2, LastChangeUserId = (int)EnumSystemUsers.AdminUser, LastChangeDate = DateTime.Now, WaitDescription = null });
            items.Add(new DictionaryEventTypes { Id = 441, Code = null, Name = "Отменено назначение ответсвенным исполнителем", SourceDescription = null, TargetDescription = null, ImportanceEventTypeId = 2, LastChangeUserId = (int)EnumSystemUsers.AdminUser, LastChangeDate = DateTime.Now, WaitDescription = null });
            items.Add(new DictionaryEventTypes { Id = 450, Code = null, Name = "Очередной срок исполнения", SourceDescription = null, TargetDescription = null, ImportanceEventTypeId = 2, LastChangeUserId = (int)EnumSystemUsers.AdminUser, LastChangeDate = DateTime.Now, WaitDescription = null });
            items.Add(new DictionaryEventTypes { Id = 460, Code = null, Name = "Истекает срок исполнения", SourceDescription = null, TargetDescription = null, ImportanceEventTypeId = 2, LastChangeUserId = (int)EnumSystemUsers.AdminUser, LastChangeDate = DateTime.Now, WaitDescription = null });
            items.Add(new DictionaryEventTypes { Id = 470, Code = null, Name = "Срок исполнения истек", SourceDescription = null, TargetDescription = null, ImportanceEventTypeId = 2, LastChangeUserId = (int)EnumSystemUsers.AdminUser, LastChangeDate = DateTime.Now, WaitDescription = null });
            items.Add(new DictionaryEventTypes { Id = 500, Code = null, Name = "Направлено сообщение", SourceDescription = null, TargetDescription = null, ImportanceEventTypeId = 9, LastChangeUserId = (int)EnumSystemUsers.AdminUser, LastChangeDate = DateTime.Now, WaitDescription = null });
            items.Add(new DictionaryEventTypes { Id = 505, Code = null, Name = "Добавлен бумажный носитель", SourceDescription = null, TargetDescription = null, ImportanceEventTypeId = 7, LastChangeUserId = (int)EnumSystemUsers.AdminUser, LastChangeDate = DateTime.Now, WaitDescription = null });
            items.Add(new DictionaryEventTypes { Id = 507, Code = null, Name = "Отметка нахождения бумажного носителя у себя", SourceDescription = null, TargetDescription = null, ImportanceEventTypeId = 7, LastChangeUserId = (int)EnumSystemUsers.AdminUser, LastChangeDate = DateTime.Now, WaitDescription = null });
            items.Add(new DictionaryEventTypes { Id = 509, Code = null, Name = "Отметка порчи бумажного носителя", SourceDescription = null, TargetDescription = null, ImportanceEventTypeId = 7, LastChangeUserId = (int)EnumSystemUsers.AdminUser, LastChangeDate = DateTime.Now, WaitDescription = null });
            items.Add(new DictionaryEventTypes { Id = 510, Code = null, Name = "Переданы бумажные носители", SourceDescription = null, TargetDescription = null, ImportanceEventTypeId = 7, LastChangeUserId = (int)EnumSystemUsers.AdminUser, LastChangeDate = DateTime.Now, WaitDescription = null });
            items.Add(new DictionaryEventTypes { Id = 600, Code = null, Name = "Примечание", SourceDescription = null, TargetDescription = null, ImportanceEventTypeId = 8, LastChangeUserId = (int)EnumSystemUsers.AdminUser, LastChangeDate = DateTime.Now, WaitDescription = null });
            items.Add(new DictionaryEventTypes { Id = 601, Code = null, Name = "Формулировка задачи", SourceDescription = null, TargetDescription = null, ImportanceEventTypeId = 8, LastChangeUserId = (int)EnumSystemUsers.AdminUser, LastChangeDate = DateTime.Now, WaitDescription = null });
            items.Add(new DictionaryEventTypes { Id = 610, Code = null, Name = "Передан на рассмотрение руководителю", SourceDescription = null, TargetDescription = null, ImportanceEventTypeId = 3, LastChangeUserId = (int)EnumSystemUsers.AdminUser, LastChangeDate = DateTime.Now, WaitDescription = null });
            items.Add(new DictionaryEventTypes { Id = 620, Code = null, Name = "Получен после рассмотрения руководителем", SourceDescription = null, TargetDescription = null, ImportanceEventTypeId = 3, LastChangeUserId = (int)EnumSystemUsers.AdminUser, LastChangeDate = DateTime.Now, WaitDescription = null });
            items.Add(new DictionaryEventTypes { Id = 700, Code = null, Name = "Направлен на регистрацию", SourceDescription = null, TargetDescription = null, ImportanceEventTypeId = 2, LastChangeUserId = (int)EnumSystemUsers.AdminUser, LastChangeDate = DateTime.Now, WaitDescription = null });
            items.Add(new DictionaryEventTypes { Id = 701, Code = null, Name = "Зарегистрирован", SourceDescription = null, TargetDescription = null, ImportanceEventTypeId = 2, LastChangeUserId = (int)EnumSystemUsers.AdminUser, LastChangeDate = DateTime.Now, WaitDescription = null });
            items.Add(new DictionaryEventTypes { Id = 702, Code = null, Name = "Отказано в регистрации", SourceDescription = null, TargetDescription = null, ImportanceEventTypeId = 2, LastChangeUserId = (int)EnumSystemUsers.AdminUser, LastChangeDate = DateTime.Now, WaitDescription = null });
            items.Add(new DictionaryEventTypes { Id = 712, Code = null, Name = "Отозван проект", SourceDescription = null, TargetDescription = null, ImportanceEventTypeId = 2, LastChangeUserId = (int)EnumSystemUsers.AdminUser, LastChangeDate = DateTime.Now, WaitDescription = null });
            items.Add(new DictionaryEventTypes { Id = 800, Code = null, Name = "Запущено исполнение плана работы по документу", SourceDescription = null, TargetDescription = null, ImportanceEventTypeId = 2, LastChangeUserId = (int)EnumSystemUsers.AdminUser, LastChangeDate = DateTime.Now, WaitDescription = null });
            items.Add(new DictionaryEventTypes { Id = 810, Code = null, Name = "Остановлено исполнение плана работы по документу", SourceDescription = null, TargetDescription = null, ImportanceEventTypeId = 2, LastChangeUserId = (int)EnumSystemUsers.AdminUser, LastChangeDate = DateTime.Now, WaitDescription = null });
            items.Add(new DictionaryEventTypes { Id = 998, Code = null, Name = "Работа возобновлена", SourceDescription = null, TargetDescription = null, ImportanceEventTypeId = 2, LastChangeUserId = (int)EnumSystemUsers.AdminUser, LastChangeDate = DateTime.Now, WaitDescription = null });
            items.Add(new DictionaryEventTypes { Id = 999, Code = null, Name = "Работа завершена", SourceDescription = null, TargetDescription = null, ImportanceEventTypeId = 2, LastChangeUserId = (int)EnumSystemUsers.AdminUser, LastChangeDate = DateTime.Now, WaitDescription = null });

            return items;
        }

        public static List<DictionaryImportanceEventTypes> GetDictionaryImportanceEventTypes()
        {
            var items = new List<DictionaryImportanceEventTypes>();

            items.Add(new DictionaryImportanceEventTypes { Id = 1, Code = null, Name = "##l@DictionaryImportanceEventTypes:DocumentMoovement@l##", LastChangeUserId = (int)EnumSystemUsers.AdminUser, LastChangeDate = DateTime.Now });
            items.Add(new DictionaryImportanceEventTypes { Id = 2, Code = null, Name = "##l@DictionaryImportanceEventTypes:ImportantEvents@l##", LastChangeUserId = (int)EnumSystemUsers.AdminUser, LastChangeDate = DateTime.Now });
            items.Add(new DictionaryImportanceEventTypes { Id = 3, Code = null, Name = "##l@DictionaryImportanceEventTypes:AdditionalEvents@l##", LastChangeUserId = (int)EnumSystemUsers.AdminUser, LastChangeDate = DateTime.Now });
            items.Add(new DictionaryImportanceEventTypes { Id = 7, Code = null, Name = "##l@DictionaryImportanceEventTypes:PaperMoovement@l##", LastChangeUserId = (int)EnumSystemUsers.AdminUser, LastChangeDate = DateTime.Now });
            items.Add(new DictionaryImportanceEventTypes { Id = 8, Code = null, Name = "##l@DictionaryImportanceEventTypes:Message@l##", LastChangeUserId = (int)EnumSystemUsers.AdminUser, LastChangeDate = DateTime.Now });
            items.Add(new DictionaryImportanceEventTypes { Id = 9, Code = null, Name = "##l@DictionaryImportanceEventTypes:Internal@l##", LastChangeUserId = (int)EnumSystemUsers.AdminUser, LastChangeDate = DateTime.Now });

            return items;
        }

        public static List<DictionaryResultTypes> GetDictionaryResultTypes()
        {
            var items = new List<DictionaryResultTypes>();

            items.Add(new DictionaryResultTypes { Id = -4, Name = "Подписание", IsExecute = true, IsActive = false, LastChangeUserId = (int)EnumSystemUsers.AdminUser, LastChangeDate = DateTime.Now });
            items.Add(new DictionaryResultTypes { Id = -3, Name = "Отказ", IsExecute = false, IsActive = false, LastChangeUserId = (int)EnumSystemUsers.AdminUser, LastChangeDate = DateTime.Now });
            items.Add(new DictionaryResultTypes { Id = -2, Name = "Отзыв", IsExecute = false, IsActive = false, LastChangeUserId = (int)EnumSystemUsers.AdminUser, LastChangeDate = DateTime.Now });
            items.Add(new DictionaryResultTypes { Id = -1, Name = "Изменение контроля", IsExecute = false, IsActive = false, LastChangeUserId = (int)EnumSystemUsers.AdminUser, LastChangeDate = DateTime.Now });
            items.Add(new DictionaryResultTypes { Id = 2841, Name = "Отлично", IsExecute = true, IsActive = true, LastChangeUserId = (int)EnumSystemUsers.AdminUser, LastChangeDate = DateTime.Now });
            items.Add(new DictionaryResultTypes { Id = 2842, Name = "Хорошо", IsExecute = true, IsActive = true, LastChangeUserId = (int)EnumSystemUsers.AdminUser, LastChangeDate = DateTime.Now });
            items.Add(new DictionaryResultTypes { Id = 2843, Name = "Удовлетворительно", IsExecute = true, IsActive = true, LastChangeUserId = (int)EnumSystemUsers.AdminUser, LastChangeDate = DateTime.Now });
            items.Add(new DictionaryResultTypes { Id = 2844, Name = "Плохо", IsExecute = false, IsActive = true, LastChangeUserId = (int)EnumSystemUsers.AdminUser, LastChangeDate = DateTime.Now });
            items.Add(new DictionaryResultTypes { Id = 4062, Name = "Без оценки", IsExecute = false, IsActive = true, LastChangeUserId = (int)EnumSystemUsers.AdminUser, LastChangeDate = DateTime.Now });

            return items;
        }

        public static List<DictionarySendTypes> GetDictionarySendTypes()
        {
            var items = new List<DictionarySendTypes>();

            items.Add(new DictionarySendTypes { Id = 10, Code = null, Name = "##l@DictionarySendTypes:SendForResponsibleExecution@l##", IsImportant = true, SubordinationTypeId = 2, LastChangeUserId = (int)EnumSystemUsers.AdminUser, LastChangeDate = DateTime.Now });
            items.Add(new DictionarySendTypes { Id = 20, Code = null, Name = "##l@DictionarySendTypes:SendForControl@l##", IsImportant = true, SubordinationTypeId = 2, LastChangeUserId = (int)EnumSystemUsers.AdminUser, LastChangeDate = DateTime.Now });
            items.Add(new DictionarySendTypes { Id = 30, Code = null, Name = "##l@DictionarySendTypes:SendForExecution@l##", IsImportant = true, SubordinationTypeId = 2, LastChangeUserId = (int)EnumSystemUsers.AdminUser, LastChangeDate = DateTime.Now });
            items.Add(new DictionarySendTypes { Id = 40, Code = null, Name = "##l@DictionarySendTypes:SendForInformation@l##", IsImportant = false, SubordinationTypeId = 1, LastChangeUserId = (int)EnumSystemUsers.AdminUser, LastChangeDate = DateTime.Now });
            items.Add(new DictionarySendTypes { Id = 45, Code = null, Name = "##l@DictionarySendTypes:SendForInformationExternal@l##", IsImportant = false, SubordinationTypeId = 1, LastChangeUserId = (int)EnumSystemUsers.AdminUser, LastChangeDate = DateTime.Now });
            items.Add(new DictionarySendTypes { Id = 50, Code = null, Name = "##l@DictionarySendTypes:SendForConsideration@l##", IsImportant = false, SubordinationTypeId = 1, LastChangeUserId = (int)EnumSystemUsers.AdminUser, LastChangeDate = DateTime.Now });
            items.Add(new DictionarySendTypes { Id = 250, Code = null, Name = "##l@DictionarySendTypes:SendForVisaing@l##", IsImportant = true, SubordinationTypeId = 1, LastChangeUserId = (int)EnumSystemUsers.AdminUser, LastChangeDate = DateTime.Now });
            items.Add(new DictionarySendTypes { Id = 260, Code = null, Name = "##l@DictionarySendTypes:SendForАgreement@l##", IsImportant = true, SubordinationTypeId = 1, LastChangeUserId = (int)EnumSystemUsers.AdminUser, LastChangeDate = DateTime.Now });
            items.Add(new DictionarySendTypes { Id = 270, Code = null, Name = "##l@DictionarySendTypes:SendForАpproval@l##", IsImportant = true, SubordinationTypeId = 1, LastChangeUserId = (int)EnumSystemUsers.AdminUser, LastChangeDate = DateTime.Now });
            items.Add(new DictionarySendTypes { Id = 280, Code = null, Name = "##l@DictionarySendTypes:SendForSigning@l##", IsImportant = true, SubordinationTypeId = 1, LastChangeUserId = (int)EnumSystemUsers.AdminUser, LastChangeDate = DateTime.Now });

            return items;
        }

        public static List<DictionarySubordinationTypes> GetDictionarySubordinationTypes()
        {
            var items = new List<DictionarySubordinationTypes>();

            items.Add(new DictionarySubordinationTypes { Id = 1, Code = "Informing", Name = "##l@DictionarySubordinationTypes:Informing@l##", LastChangeUserId = (int)EnumSystemUsers.AdminUser, LastChangeDate = DateTime.Now });
            items.Add(new DictionarySubordinationTypes { Id = 2, Code = "Execution", Name = "##l@DictionarySubordinationTypes:Execution@l##", LastChangeUserId = (int)EnumSystemUsers.AdminUser, LastChangeDate = DateTime.Now });

            return items;
        }

        public static List<DictionarySubscriptionStates> GetDictionarySubscriptionStates()
        {
            var items = new List<DictionarySubscriptionStates>();

            items.Add(new DictionarySubscriptionStates { Id = 1, Code = "No", Name = "##l@DictionarySubscriptionStates:No@l##", IsSuccess = false, LastChangeUserId = (int)EnumSystemUsers.AdminUser, LastChangeDate = DateTime.Now });
            items.Add(new DictionarySubscriptionStates { Id = 2, Code = "Violated", Name = "##l@DictionarySubscriptionStates:Violated@l##", IsSuccess = false, LastChangeUserId = (int)EnumSystemUsers.AdminUser, LastChangeDate = DateTime.Now });
            items.Add(new DictionarySubscriptionStates { Id = 11, Code = "Visa", Name = "##l@DictionarySubscriptionStates:Visa@l##", IsSuccess = true, LastChangeUserId = (int)EnumSystemUsers.AdminUser, LastChangeDate = DateTime.Now });
            items.Add(new DictionarySubscriptionStates { Id = 12, Code = "Аgreement", Name = "##l@DictionarySubscriptionStates:Аgreement@l##", IsSuccess = true, LastChangeUserId = (int)EnumSystemUsers.AdminUser, LastChangeDate = DateTime.Now });
            items.Add(new DictionarySubscriptionStates { Id = 13, Code = "Аpproval", Name = "##l@DictionarySubscriptionStates:Аpproval@l##", IsSuccess = true, LastChangeUserId = (int)EnumSystemUsers.AdminUser, LastChangeDate = DateTime.Now });
            items.Add(new DictionarySubscriptionStates { Id = 14, Code = "Sign", Name = "##l@DictionarySubscriptionStates:Sign@l##", IsSuccess = true, LastChangeUserId = (int)EnumSystemUsers.AdminUser, LastChangeDate = DateTime.Now });

            return items;
        }
        public static List<DictionaryPositionExecutorTypes> GetDictionaryPositionExecutorTypes()
        {
            var items = new List<DictionaryPositionExecutorTypes>();

            items.Add(new DictionaryPositionExecutorTypes { Id = (int)EnumPositionExecutionTypes.Personal, Code = EnumPositionExecutionTypes.Personal.ToString(), Name = "Назначен на должность", IsActive = true, LastChangeUserId = (int)EnumSystemUsers.AdminUser, LastChangeDate = DateTime.Now });
            items.Add(new DictionaryPositionExecutorTypes { Id = (int)EnumPositionExecutionTypes.IO, Code = EnumPositionExecutionTypes.IO.ToString(), Name = "Исполяет обязанности", IsActive = true, LastChangeUserId = (int)EnumSystemUsers.AdminUser, LastChangeDate = DateTime.Now });
            items.Add(new DictionaryPositionExecutorTypes { Id = (int)EnumPositionExecutionTypes.Referent, Code = EnumPositionExecutionTypes.Referent.ToString(), Name = "Является референтом", IsActive = true, LastChangeUserId = (int)EnumSystemUsers.AdminUser, LastChangeDate = DateTime.Now });

            return items;
        }

        public static List<DictionaryLinkTypes> GetDictionaryLinkTypes()
        {
            var items = new List<DictionaryLinkTypes>();

            items.Add(new DictionaryLinkTypes { Id = 100, Name = "В ответ", IsImportant = false, IsActive = true, LastChangeUserId = (int)EnumSystemUsers.AdminUser, LastChangeDate = DateTime.Now });
            items.Add(new DictionaryLinkTypes { Id = 110, Name = "Во исполнение", IsImportant = true, IsActive = true, LastChangeUserId = (int)EnumSystemUsers.AdminUser, LastChangeDate = DateTime.Now });
            items.Add(new DictionaryLinkTypes { Id = 200, Name = "В дополнение", IsImportant = false, IsActive = true, LastChangeUserId = (int)EnumSystemUsers.AdminUser, LastChangeDate = DateTime.Now });
            items.Add(new DictionaryLinkTypes { Id = 202, Name = "Повторно", IsImportant = false, IsActive = true, LastChangeUserId = (int)EnumSystemUsers.AdminUser, LastChangeDate = DateTime.Now });
            items.Add(new DictionaryLinkTypes { Id = 210, Name = "Во изменение", IsImportant = false, IsActive = true, LastChangeUserId = (int)EnumSystemUsers.AdminUser, LastChangeDate = DateTime.Now });
            items.Add(new DictionaryLinkTypes { Id = 220, Name = "В отмену", IsImportant = true, IsActive = true, LastChangeUserId = (int)EnumSystemUsers.AdminUser, LastChangeDate = DateTime.Now });

            return items;
        }

    }
}
