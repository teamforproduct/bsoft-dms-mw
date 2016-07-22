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
        //TODO
        //DictionaryAgentEmployees -> AgentPersonId
        //DictionaryLinkTypes
        protected override void Seed(DmsContext context)
        {
            context.AdminLanguagesSet.AddRange(GetAdminLanguages());
            context.AdminLanguageValuesSet.AddRange(GetAdminLanguageValues());
            context.AdminAccessLevelsSet.AddRange(GetAdminAccessLevels());
            context.SystemObjectsSet.AddRange(GetSystemObjects());
            context.SystemActionsSet.AddRange(GetSystemActions());
            context.SystemUIElementsSet.AddRange(GetSystemUIElements());
            context.SystemValueTypesSet.AddRange(GetSystemValueTypes());
            context.EncryptionCertificateTypesSet.AddRange(GetEncryptionCertificateTypes());
            context.DictionaryDocumentDirectionsSet.AddRange(GetDictionaryDocumentDirections());
            context.DictionaryEventTypesSet.AddRange(GetDictionaryEventTypes());
            context.DictionaryImportanceEventTypesSet.AddRange(GetDictionaryImportanceEventTypes());
            context.DictionaryResultTypesSet.AddRange(GetDictionaryResultTypes());
            context.DictionarySendTypesSet.AddRange(GetDictionarySendTypes());
            context.DictionarySubordinationTypesSet.AddRange(GetDictionarySubordinationTypes());
            context.DictionarySubscriptionStatesSet.AddRange(GetDictionarySubscriptionStates());
            context.DictionaryPositionExecutorTypesSet.AddRange(GetDictionaryPositionExecutorTypes());
            context.DictionaryLinkTypesSet.AddRange(GetDictionaryLinkTypes());

            base.Seed(context);
        }

        private List<AdminLanguages> GetAdminLanguages()
        {
            var items = new List<AdminLanguages>();

            items.Add(new AdminLanguages { Id = 1, Code = "ru", Name = "Русский", IsDefault = true });
            items.Add(new AdminLanguages { Id = 2, Code = "en", Name = "English", IsDefault = false });

            return items;
        }

        private List<AdminLanguageValues> GetAdminLanguageValues()
        {
            var items = new List<AdminLanguageValues>();

            items.Add(new AdminLanguageValues { Id = 1, LanguageId = 1, Label = "##l@DictionaryDocumentDirections:Incoming@l##", Value = "Входящий" });
            items.Add(new AdminLanguageValues { Id = 2, LanguageId = 2, Label = "##l@DictionaryDocumentDirections:Incoming@l##", Value = "Incoming" });
            items.Add(new AdminLanguageValues { Id = 3, LanguageId = 1, Label = "##l@DictionaryDocumentDirections:Outcoming@l##", Value = "Иcходящий" });
            items.Add(new AdminLanguageValues { Id = 4, LanguageId = 2, Label = "##l@DictionaryDocumentDirections:Outcoming@l##", Value = "Outcoming" });
            items.Add(new AdminLanguageValues { Id = 5, LanguageId = 1, Label = "##l@DictionaryDocumentDirections:Internal@l##", Value = "Собственный" });
            items.Add(new AdminLanguageValues { Id = 6, LanguageId = 2, Label = "##l@DictionaryDocumentDirections:Internal@l##", Value = "Internal" });
            items.Add(new AdminLanguageValues { Id = 7, LanguageId = 1, Label = "##l@DictionaryImportanceEventTypes:DocumentMoovement@l##", Value = "Факты движения документов" });
            items.Add(new AdminLanguageValues { Id = 8, LanguageId = 2, Label = "##l@DictionaryImportanceEventTypes:DocumentMoovement@l##", Value = "Facts movement documents" });
            items.Add(new AdminLanguageValues { Id = 9, LanguageId = 1, Label = "##l@DictionaryImportanceEventTypes:ImportantEvents@l##", Value = "Важные события" });
            items.Add(new AdminLanguageValues { Id = 10, LanguageId = 2, Label = "##l@DictionaryImportanceEventTypes:ImportantEvents@l##", Value = "Important events" });
            items.Add(new AdminLanguageValues { Id = 11, LanguageId = 1, Label = "##l@DictionaryImportanceEventTypes:AdditionalEvents@l##", Value = "Второстепенные события" });
            items.Add(new AdminLanguageValues { Id = 12, LanguageId = 2, Label = "##l@DictionaryImportanceEventTypes:AdditionalEvents@l##", Value = "Secondary events" });
            items.Add(new AdminLanguageValues { Id = 13, LanguageId = 1, Label = "##l@DictionaryImportanceEventTypes:Message@l##", Value = "Сообщения" });
            items.Add(new AdminLanguageValues { Id = 14, LanguageId = 2, Label = "##l@DictionaryImportanceEventTypes:Message@l##", Value = "Messages" });
            items.Add(new AdminLanguageValues { Id = 15, LanguageId = 1, Label = "##l@DictionaryImportanceEventTypes:Internal@l##", Value = "Собственные примечания" });
            items.Add(new AdminLanguageValues { Id = 16, LanguageId = 2, Label = "##l@DictionaryImportanceEventTypes:Internal@l##", Value = "Own notes" });
            items.Add(new AdminLanguageValues { Id = 17, LanguageId = 1, Label = "##l@DictionarySendTypes:SendForResponsibleExecution@l##", Value = "Исполненителю(отв.исп.)" });
            items.Add(new AdminLanguageValues { Id = 18, LanguageId = 2, Label = "##l@DictionarySendTypes:SendForResponsibleExecution@l##", Value = "ResponsibleExecution" });
            items.Add(new AdminLanguageValues { Id = 19, LanguageId = 1, Label = "##l@DictionarySendTypes:SendForControl@l##", Value = "На контроль(отв.исп.)" });
            items.Add(new AdminLanguageValues { Id = 20, LanguageId = 2, Label = "##l@DictionarySendTypes:SendForControl@l##", Value = "In control" });
            items.Add(new AdminLanguageValues { Id = 21, LanguageId = 1, Label = "##l@DictionarySendTypes:SendForExecution@l##", Value = "Соисполнителю" });
            items.Add(new AdminLanguageValues { Id = 22, LanguageId = 2, Label = "##l@DictionarySendTypes:SendForExecution@l##", Value = "For execution" });
            items.Add(new AdminLanguageValues { Id = 23, LanguageId = 1, Label = "##l@DictionarySendTypes:SendForInformation@l##", Value = "Для сведения" });
            items.Add(new AdminLanguageValues { Id = 24, LanguageId = 2, Label = "##l@DictionarySendTypes:SendForInformation@l##", Value = "For information" });
            items.Add(new AdminLanguageValues { Id = 25, LanguageId = 1, Label = "##l@DictionarySendTypes:SendForInformationExternal@l##", Value = "Для сведения внешнему агенту" });
            items.Add(new AdminLanguageValues { Id = 26, LanguageId = 2, Label = "##l@DictionarySendTypes:SendForInformationExternal@l##", Value = "For information external agents" });
            items.Add(new AdminLanguageValues { Id = 27, LanguageId = 1, Label = "##l@DictionarySendTypes:SendForConsideration@l##", Value = "Для рассмотрения" });
            items.Add(new AdminLanguageValues { Id = 28, LanguageId = 2, Label = "##l@DictionarySendTypes:SendForConsideration@l##", Value = "For consideration" });
            items.Add(new AdminLanguageValues { Id = 29, LanguageId = 1, Label = "##l@DictionarySendTypes:SendForVisaing@l##", Value = "На визирование" });
            items.Add(new AdminLanguageValues { Id = 30, LanguageId = 2, Label = "##l@DictionarySendTypes:SendForVisaing@l##", Value = "For visaing" });
            items.Add(new AdminLanguageValues { Id = 31, LanguageId = 1, Label = "##l@DictionarySendTypes:SendForАgreement@l##", Value = "На согласование" });
            items.Add(new AdminLanguageValues { Id = 32, LanguageId = 2, Label = "##l@DictionarySendTypes:SendForАgreement@l##", Value = "For agreement" });
            items.Add(new AdminLanguageValues { Id = 33, LanguageId = 1, Label = "##l@DictionarySendTypes:SendForАpproval@l##", Value = "На утверждение" });
            items.Add(new AdminLanguageValues { Id = 34, LanguageId = 2, Label = "##l@DictionarySendTypes:SendForАpproval@l##", Value = "For approval" });
            items.Add(new AdminLanguageValues { Id = 35, LanguageId = 1, Label = "##l@DictionarySendTypes:SendForSigning@l##", Value = "На подпись" });
            items.Add(new AdminLanguageValues { Id = 36, LanguageId = 2, Label = "##l@DictionarySendTypes:SendForSigning@l##", Value = "For signing" });
            items.Add(new AdminLanguageValues { Id = 37, LanguageId = 1, Label = "##l@DictionarySubordinationTypes:Informing@l##", Value = "Информирование" });
            items.Add(new AdminLanguageValues { Id = 38, LanguageId = 2, Label = "##l@DictionarySubordinationTypes:Informing@l##", Value = "Informing" });
            items.Add(new AdminLanguageValues { Id = 39, LanguageId = 1, Label = "##l@DictionarySubordinationTypes:Execution@l##", Value = "Исполнение" });
            items.Add(new AdminLanguageValues { Id = 40, LanguageId = 2, Label = "##l@DictionarySubordinationTypes:Execution@l##", Value = "Execution" });
            items.Add(new AdminLanguageValues { Id = 41, LanguageId = 1, Label = "##l@DictionarySubscriptionStates:No@l##", Value = "Нет" });
            items.Add(new AdminLanguageValues { Id = 42, LanguageId = 2, Label = "##l@DictionarySubscriptionStates:No@l##", Value = "No" });
            items.Add(new AdminLanguageValues { Id = 43, LanguageId = 1, Label = "##l@DictionarySubscriptionStates:Violated@l##", Value = "Нарушена" });
            items.Add(new AdminLanguageValues { Id = 44, LanguageId = 2, Label = "##l@DictionarySubscriptionStates:Violated@l##", Value = "Violated" });
            items.Add(new AdminLanguageValues { Id = 45, LanguageId = 1, Label = "##l@DictionarySubscriptionStates:Visa@l##", Value = "Виза" });
            items.Add(new AdminLanguageValues { Id = 46, LanguageId = 2, Label = "##l@DictionarySubscriptionStates:Visa@l##", Value = "Visa" });
            items.Add(new AdminLanguageValues { Id = 47, LanguageId = 1, Label = "##l@DictionarySubscriptionStates:Аgreement@l##", Value = "Согласование" });
            items.Add(new AdminLanguageValues { Id = 48, LanguageId = 2, Label = "##l@DictionarySubscriptionStates:Аgreement@l##", Value = "Аgreement" });
            items.Add(new AdminLanguageValues { Id = 49, LanguageId = 1, Label = "##l@DictionarySubscriptionStates:Аpproval@l##", Value = "Утверждение" });
            items.Add(new AdminLanguageValues { Id = 50, LanguageId = 2, Label = "##l@DictionarySubscriptionStates:Аpproval@l##", Value = "Аpproval" });
            items.Add(new AdminLanguageValues { Id = 51, LanguageId = 1, Label = "##l@DictionarySubscriptionStates:Sign@l##", Value = "Подпись" });
            items.Add(new AdminLanguageValues { Id = 52, LanguageId = 2, Label = "##l@DictionarySubscriptionStates:Sign@l##", Value = "Sign" });
            items.Add(new AdminLanguageValues { Id = 53, LanguageId = 1, Label = "##l@DmsExceptions:DatabaseError@l##", Value = "Ошибка при обращении к базе данных!" });
            items.Add(new AdminLanguageValues { Id = 54, LanguageId = 2, Label = "##l@DmsExceptions:DatabaseError@l##", Value = "An error occurred while accessing the database!" });
            items.Add(new AdminLanguageValues { Id = 55, LanguageId = 1, Label = "##l@DmsExceptions:CommandNotDefinedError@l##", Value = "Искомой комманды не найдено" });
            items.Add(new AdminLanguageValues { Id = 56, LanguageId = 2, Label = "##l@DmsExceptions:CommandNotDefinedError@l##", Value = "The desired commands not found" });
            items.Add(new AdminLanguageValues { Id = 57, LanguageId = 1, Label = "##l@DmsExceptions:WrongParameterValueError@l##", Value = "Параметры комманды неверные!" });
            items.Add(new AdminLanguageValues { Id = 58, LanguageId = 2, Label = "##l@DmsExceptions:WrongParameterValueError@l##", Value = "Parameters commands incorrect!" });
            items.Add(new AdminLanguageValues { Id = 59, LanguageId = 1, Label = "##l@DmsExceptions:WrongParameterTypeError@l##", Value = "Тип параметра комманды указан неверно!" });
            items.Add(new AdminLanguageValues { Id = 60, LanguageId = 2, Label = "##l@DmsExceptions:WrongParameterTypeError@l##", Value = "Parameter type commands is incorrect!" });
            items.Add(new AdminLanguageValues { Id = 61, LanguageId = 1, Label = "##l@DmsExceptions:AccessIsDenied@l##", Value = "В доступе отказано!" });
            items.Add(new AdminLanguageValues { Id = 62, LanguageId = 2, Label = "##l@DmsExceptions:AccessIsDenied@l##", Value = "Access is Denied!" });
            items.Add(new AdminLanguageValues { Id = 63, LanguageId = 1, Label = "##l@DmsExceptions:DocumentHasAlreadyHasLink@l##", Value = "Документ уже имеет ссылку!" });
            items.Add(new AdminLanguageValues { Id = 64, LanguageId = 2, Label = "##l@DmsExceptions:DocumentHasAlreadyHasLink@l##", Value = "Document has already has link!" });
            items.Add(new AdminLanguageValues { Id = 65, LanguageId = 1, Label = "##l@DmsExceptions:DocumentCannotBeModifiedOrDeleted@l##", Value = "Документ не может быть изменен или удален!" });
            items.Add(new AdminLanguageValues { Id = 66, LanguageId = 2, Label = "##l@DmsExceptions:DocumentCannotBeModifiedOrDeleted@l##", Value = "Document cannot be Modified or Deleted!" });
            items.Add(new AdminLanguageValues { Id = 67, LanguageId = 1, Label = "##l@DmsExceptions:UserHasNoAccessToDocument@l##", Value = "Пользователь не может получить доступ к этот документ!" });
            items.Add(new AdminLanguageValues { Id = 68, LanguageId = 2, Label = "##l@DmsExceptions:UserHasNoAccessToDocument@l##", Value = "User could not access this document!" });
            items.Add(new AdminLanguageValues { Id = 69, LanguageId = 1, Label = "##l@DmsExceptions:CannotSaveFile@l##", Value = "Ошибка при сохранения файла пользователя!" });
            items.Add(new AdminLanguageValues { Id = 70, LanguageId = 2, Label = "##l@DmsExceptions:CannotSaveFile@l##", Value = "Error when save user file!" });
            items.Add(new AdminLanguageValues { Id = 71, LanguageId = 1, Label = "##l@DmsExceptions:UserFileNotExists@l##", Value = "Пользовательский файл не существует в файловом хранилище" });
            items.Add(new AdminLanguageValues { Id = 72, LanguageId = 2, Label = "##l@DmsExceptions:UserFileNotExists@l##", Value = "User file does not exists on Filestore!" });
            items.Add(new AdminLanguageValues { Id = 73, LanguageId = 1, Label = "##l@DmsExceptions:UnknownDocumentFile@l##", Value = "Не удалось найти соответствующий файл документа!" });
            items.Add(new AdminLanguageValues { Id = 74, LanguageId = 2, Label = "##l@DmsExceptions:UnknownDocumentFile@l##", Value = "Could not find appropriate document file!" });
            items.Add(new AdminLanguageValues { Id = 75, LanguageId = 1, Label = "##l@DmsExceptions:CannotAccessToFile@l##", Value = "Нет доступ к файлу пользователя!" });
            items.Add(new AdminLanguageValues { Id = 76, LanguageId = 2, Label = "##l@DmsExceptions:CannotAccessToFile@l##", Value = "Cannot access to user file!" });
            items.Add(new AdminLanguageValues { Id = 77, LanguageId = 1, Label = "##l@DmsExceptions:DocumentNotFoundOrUserHasNoAccess@l##", Value = "Пользователь не имеет доступ к этот документ!" });
            items.Add(new AdminLanguageValues { Id = 78, LanguageId = 2, Label = "##l@DmsExceptions:DocumentNotFoundOrUserHasNoAccess@l##", Value = "User could not access this document!" });
            items.Add(new AdminLanguageValues { Id = 79, LanguageId = 1, Label = "##l@DmsExceptions:TemplateDocumentNotFoundOrUserHasNoAccess@l##", Value = "Пользователь не имеет доступ к этот шаблону документа!" });
            items.Add(new AdminLanguageValues { Id = 80, LanguageId = 2, Label = "##l@DmsExceptions:TemplateDocumentNotFoundOrUserHasNoAccess@l##", Value = "User could not access this template document!" });
            items.Add(new AdminLanguageValues { Id = 81, LanguageId = 1, Label = "##l@DmsExceptions:DocumentCouldNotBeRegistered@l##", Value = "Регистрационный документ не была успешной! Попробуй еще раз!" });
            items.Add(new AdminLanguageValues { Id = 82, LanguageId = 2, Label = "##l@DmsExceptions:DocumentCouldNotBeRegistered@l##", Value = "Document registration has non been successfull! Try again!" });
            items.Add(new AdminLanguageValues { Id = 83, LanguageId = 1, Label = "##l@DmsExceptions:CouldNotChangeAttributeLaunchPlan@l##", Value = "Невозможно изменить атрибут LaunchPlan" });
            items.Add(new AdminLanguageValues { Id = 84, LanguageId = 2, Label = "##l@DmsExceptions:CouldNotChangeAttributeLaunchPlan@l##", Value = "Couldn\"t change attribute LaunchPlan" });
            items.Add(new AdminLanguageValues { Id = 85, LanguageId = 1, Label = "##l@DmsExceptions:CouldNotChangeFavourite@l##", Value = "Невозможно изменить атрибут Favourite" });
            items.Add(new AdminLanguageValues { Id = 86, LanguageId = 2, Label = "##l@DmsExceptions:CouldNotChangeFavourite@l##", Value = "Couldn\"t change attribute Favourite" });
            items.Add(new AdminLanguageValues { Id = 87, LanguageId = 1, Label = "##l@DmsExceptions:CouldNotChangeIsInWork@l##", Value = "Невозможно изменить атрибут IsInWork" });
            items.Add(new AdminLanguageValues { Id = 88, LanguageId = 2, Label = "##l@DmsExceptions:CouldNotChangeIsInWork@l##", Value = "Couldn\"t change attribute IsInWork" });
            items.Add(new AdminLanguageValues { Id = 89, LanguageId = 1, Label = "##l@DmsExceptions:DocumentHasAlredyBeenRegistered@l##", Value = "Документ уже зарегистрирован!" });
            items.Add(new AdminLanguageValues { Id = 90, LanguageId = 2, Label = "##l@DmsExceptions:DocumentHasAlredyBeenRegistered@l##", Value = "Document has already been registered!" });
            items.Add(new AdminLanguageValues { Id = 91, LanguageId = 1, Label = "##l@DmsExceptions:PlanPointHasAlredyBeenLaunched@l##", Value = "Пункт плана уже запущен!" });
            items.Add(new AdminLanguageValues { Id = 92, LanguageId = 2, Label = "##l@DmsExceptions:PlanPointHasAlredyBeenLaunched@l##", Value = "Plan Point has already been Launched!" });
            items.Add(new AdminLanguageValues { Id = 93, LanguageId = 1, Label = "##l@DmsExceptions:UserPositionIsNotDefined@l##", Value = "Позиция для текущего пользователя не может быть определена!" });
            items.Add(new AdminLanguageValues { Id = 94, LanguageId = 2, Label = "##l@DmsExceptions:UserPositionIsNotDefined@l##", Value = "Position for the current user could not be defined!" });
            items.Add(new AdminLanguageValues { Id = 95, LanguageId = 1, Label = "##l@DmsExceptions:NeedInformationAboutCorrespondent@l##", Value = "Нужна информация о корреспонденте!" });
            items.Add(new AdminLanguageValues { Id = 96, LanguageId = 2, Label = "##l@DmsExceptions:NeedInformationAboutCorrespondent@l##", Value = "Need information about correspondent!" });
            items.Add(new AdminLanguageValues { Id = 97, LanguageId = 1, Label = "##l@DmsExceptions:UserNameIsNotDefined@l##", Value = "Сотрудник для текущего пользователя не может быть определен!" });
            items.Add(new AdminLanguageValues { Id = 98, LanguageId = 2, Label = "##l@DmsExceptions:UserNameIsNotDefined@l##", Value = "Employee for the current user could not be defined!" });
            items.Add(new AdminLanguageValues { Id = 99, LanguageId = 1, Label = "##l@DmsExceptions:UserUnauthorized@l##", Value = "Пользователь не авторизован" });
            items.Add(new AdminLanguageValues { Id = 100, LanguageId = 2, Label = "##l@DmsExceptions:UserUnauthorized@l##", Value = "Authorization has been denied for this request." });
            items.Add(new AdminLanguageValues { Id = 101, LanguageId = 1, Label = "##l@DmsExceptions:DocumentRestrictedSendListDuplication@l##", Value = "Дублирование записей в разрешающем списке рассылке для документа" });
            items.Add(new AdminLanguageValues { Id = 102, LanguageId = 2, Label = "##l@DmsExceptions:DocumentRestrictedSendListDuplication@l##", Value = "Duplicate Entry DocumentRestrictSendList" });
            items.Add(new AdminLanguageValues { Id = 103, LanguageId = 1, Label = "##l@DmsExceptions:WrongDocumentSendListEntry@l##", Value = "Некорректный пункт плана" });
            items.Add(new AdminLanguageValues { Id = 104, LanguageId = 2, Label = "##l@DmsExceptions:WrongDocumentSendListEntry@l##", Value = "Plan item is wrong." });
            items.Add(new AdminLanguageValues { Id = 105, LanguageId = 1, Label = "##l@DmsExceptions:DocumentSendListNotFoundInDocumentRestrictedSendList@l##", Value = "Список рассылок для документа не найден в разрешающем списке рассылок для документа" });
            items.Add(new AdminLanguageValues { Id = 106, LanguageId = 2, Label = "##l@DmsExceptions:DocumentSendListNotFoundInDocumentRestrictedSendList@l##", Value = "DocumentSendList not found in DocumentRestrictedSendList" });
            items.Add(new AdminLanguageValues { Id = 107, LanguageId = 1, Label = "##l@DmsExceptions:DocumentSendListDoesNotMatchTheTemplate@l##", Value = "Список рассылок для документа не соответствует шаблону" });
            items.Add(new AdminLanguageValues { Id = 108, LanguageId = 2, Label = "##l@DmsExceptions:DocumentSendListDoesNotMatchTheTemplate@l##", Value = "Document SendList does not match the template" });
            items.Add(new AdminLanguageValues { Id = 109, LanguageId = 1, Label = "##l@DmsExceptions:DocumentRestrictedSendListDoesNotMatchTheTemplate@l##", Value = "Разрешающий список рассылок для документа не соответствует шаблону" });
            items.Add(new AdminLanguageValues { Id = 110, LanguageId = 2, Label = "##l@DmsExceptions:DocumentRestrictedSendListDoesNotMatchTheTemplate@l##", Value = "Document Restricted SendList does not match the template" });
            items.Add(new AdminLanguageValues { Id = 111, LanguageId = 1, Label = "##l@DmsExceptions:EventNotFoundOrUserHasNoAccess@l##", Value = "Пользователь не имеет доступа к этому событию!" });
            items.Add(new AdminLanguageValues { Id = 112, LanguageId = 2, Label = "##l@DmsExceptions:EventNotFoundOrUserHasNoAccess@l##", Value = "User could not access this event!" });
            items.Add(new AdminLanguageValues { Id = 113, LanguageId = 1, Label = "##l@DmsExceptions:CouldNotPerformThisOperation@l##", Value = "Не удалось выполнить эту операцию!" });
            items.Add(new AdminLanguageValues { Id = 114, LanguageId = 2, Label = "##l@DmsExceptions:CouldNotPerformThisOperation@l##", Value = "Could Not Perform This Operation!" });
            items.Add(new AdminLanguageValues { Id = 115, LanguageId = 1, Label = "##l@DmsExceptions:WaitNotFoundOrUserHasNoAccess@l##", Value = "Пользователь не имеет доступа к этим ожиданиям!" });
            items.Add(new AdminLanguageValues { Id = 116, LanguageId = 2, Label = "##l@DmsExceptions:WaitNotFoundOrUserHasNoAccess@l##", Value = "User could not access this wait!" });
            items.Add(new AdminLanguageValues { Id = 117, LanguageId = 1, Label = "##l@DmsExceptions:WaitHasAlreadyClosed@l##", Value = "Ожидание уже закрыто!" });
            items.Add(new AdminLanguageValues { Id = 118, LanguageId = 2, Label = "##l@DmsExceptions:WaitHasAlreadyClosed@l##", Value = "Wait has already closed!" });
            items.Add(new AdminLanguageValues { Id = 119, LanguageId = 1, Label = "##l@DmsExceptions:DictionaryTagNotFoundOrUserHasNoAccess@l##", Value = "Пользователь не имеет доступа к этому тегу!" });
            items.Add(new AdminLanguageValues { Id = 120, LanguageId = 2, Label = "##l@DmsExceptions:DictionaryTagNotFoundOrUserHasNoAccess@l##", Value = "User could not access this tag!" });
            items.Add(new AdminLanguageValues { Id = 121, LanguageId = 1, Label = "##l@DmsExceptions:ExecutorAgentForPositionIsNotDefined@l##", Value = "Исполняющий агент для позиции не определен!" });
            items.Add(new AdminLanguageValues { Id = 122, LanguageId = 2, Label = "##l@DmsExceptions:ExecutorAgentForPositionIsNotDefined@l##", Value = "Executor agent for position is not defined!" });
            items.Add(new AdminLanguageValues { Id = 123, LanguageId = 1, Label = "##l@DmsExceptions:DictionaryRecordCouldNotBeAdded@l##", Value = "Невозможно добавить данные в справочник" });
            items.Add(new AdminLanguageValues { Id = 124, LanguageId = 2, Label = "##l@DmsExceptions:DictionaryRecordCouldNotBeAdded@l##", Value = "You could not add this dictionary data!" });
            items.Add(new AdminLanguageValues { Id = 125, LanguageId = 1, Label = "##l@DmsExceptions:DictionaryRecordCouldNotBeDeleted@l##", Value = "Невозможно удаления записи из справочника" });
            items.Add(new AdminLanguageValues { Id = 126, LanguageId = 2, Label = "##l@DmsExceptions:DictionaryRecordCouldNotBeDeleted@l##", Value = "You could not delete from this dictionary data!" });
            items.Add(new AdminLanguageValues { Id = 127, LanguageId = 1, Label = "##l@DmsExceptions:DictionaryRecordWasNotFound@l##", Value = "Невозможно обновить несуществующую строку справочника в БД" });
            items.Add(new AdminLanguageValues { Id = 128, LanguageId = 2, Label = "##l@DmsExceptions:DictionaryRecordWasNotFound@l##", Value = "Dictionary record was not found!" });
            items.Add(new AdminLanguageValues { Id = 129, LanguageId = 1, Label = "##l@DmsExceptions:DictionaryRecordNotUnique@l##", Value = "Невозможно обновить несуществующую строку справочника в БД" });
            items.Add(new AdminLanguageValues { Id = 130, LanguageId = 2, Label = "##l@DmsExceptions:DictionaryRecordNotUnique@l##", Value = "Dictionary record should be unique!" });
            items.Add(new AdminLanguageValues { Id = 131, LanguageId = 1, Label = "##l@DmsExceptions:IncomingModelIsNotValid@l##", Value = "Входящая модель недействительна!" });
            items.Add(new AdminLanguageValues { Id = 132, LanguageId = 2, Label = "##l@DmsExceptions:IncomingModelIsNotValid@l##", Value = "Incoming Model is not valid!" });
            items.Add(new AdminLanguageValues { Id = 133, LanguageId = 1, Label = "##l@DmsExceptions:NotFilledWithAdditionalRequiredAttributes@l##", Value = "Не заполнены обязательные дополнительные атрибуты!" });
            items.Add(new AdminLanguageValues { Id = 134, LanguageId = 2, Label = "##l@DmsExceptions:NotFilledWithAdditionalRequiredAttributes@l##", Value = "Not filled with additional required attributes!" });
            items.Add(new AdminLanguageValues { Id = 135, LanguageId = 1, Label = "##l@DmsExceptions:DatabaseIsNotSet@l##", Value = "База данных не установлена" });
            items.Add(new AdminLanguageValues { Id = 136, LanguageId = 2, Label = "##l@DmsExceptions:DatabaseIsNotSet@l##", Value = "The database is not set" });
            items.Add(new AdminLanguageValues { Id = 137, LanguageId = 1, Label = "##l@DmsExceptions:TaskNotFoundOrUserHasNoAccess@l##", Value = "Task не найден" });
            items.Add(new AdminLanguageValues { Id = 138, LanguageId = 2, Label = "##l@DmsExceptions:TaskNotFoundOrUserHasNoAccess@l##", Value = "Task not found" });
            items.Add(new AdminLanguageValues { Id = 139, LanguageId = 1, Label = "##l@DmsExceptions:PaperNotFoundOrUserHasNoAccess@l##", Value = "Бумага не найдена или пользователь не имеет доступа" });
            items.Add(new AdminLanguageValues { Id = 140, LanguageId = 2, Label = "##l@DmsExceptions:PaperNotFoundOrUserHasNoAccess@l##", Value = "Paper not found or user has no access" });
            items.Add(new AdminLanguageValues { Id = 141, LanguageId = 1, Label = "##l@DmsExceptions:PaperListNotFoundOrUserHasNoAccess@l##", Value = "Список бумага не найдена или пользователь не имеет доступа" });
            items.Add(new AdminLanguageValues { Id = 142, LanguageId = 2, Label = "##l@DmsExceptions:PaperListNotFoundOrUserHasNoAccess@l##", Value = "Paper list not found or user has no access" });
            items.Add(new AdminLanguageValues { Id = 143, LanguageId = 1, Label = "##l@DmsExceptions:DocumentFileWasChangedExternally@l##", Value = "Файл документа был изменен извне" });
            items.Add(new AdminLanguageValues { Id = 144, LanguageId = 2, Label = "##l@DmsExceptions:DocumentFileWasChangedExternally@l##", Value = "The document file has been modified from the outside" });
            items.Add(new AdminLanguageValues { Id = 145, LanguageId = 1, Label = "##l@DmsExceptions:DatabaseIsNotFound@l##", Value = "База данных не найдена" });
            items.Add(new AdminLanguageValues { Id = 146, LanguageId = 2, Label = "##l@DmsExceptions:DatabaseIsNotFound@l##", Value = "Database not found" });
            items.Add(new AdminLanguageValues { Id = 147, LanguageId = 1, Label = "##l@DictionaryImportanceEventTypes:PaperMoovement@l##", Value = "Движение БН" });
            items.Add(new AdminLanguageValues { Id = 148, LanguageId = 2, Label = "##l@DictionaryImportanceEventTypes:PaperMoovement@l##", Value = "Paper movement" });
            items.Add(new AdminLanguageValues { Id = 149, LanguageId = 1, Label = "##l@DmsExceptions:RecordNotUnique@l##", Value = "Запись не уникальна" });
            items.Add(new AdminLanguageValues { Id = 150, LanguageId = 2, Label = "##l@DmsExceptions:RecordNotUnique@l##", Value = "Record is not Unique" });
            items.Add(new AdminLanguageValues { Id = 151, LanguageId = 1, Label = "##l@DmsExceptions:TemplateDocumentIsNotValid@l##", Value = "Шаблон документа не корректен" });
            items.Add(new AdminLanguageValues { Id = 152, LanguageId = 2, Label = "##l@DmsExceptions:TemplateDocumentIsNotValid@l##", Value = "The document template is not valid" });
            items.Add(new AdminLanguageValues { Id = 153, LanguageId = 1, Label = "##l@DmsExceptions:LicenceInformationError@l##", Value = "Лицензия недействительна" });
            items.Add(new AdminLanguageValues { Id = 154, LanguageId = 2, Label = "##l@DmsExceptions:LicenceInformationError@l##", Value = "The licence is not valid" });
            items.Add(new AdminLanguageValues { Id = 155, LanguageId = 1, Label = "##l@DmsExceptions:CryptographicError@l##", Value = "Ошибка шифрования" });
            items.Add(new AdminLanguageValues { Id = 156, LanguageId = 2, Label = "##l@DmsExceptions:CryptographicError@l##", Value = "Encryption Error" });
            items.Add(new AdminLanguageValues { Id = 157, LanguageId = 1, Label = "##l@DmsExceptions:LicenceExpired@l##", Value = "Лицензия истекла" });
            items.Add(new AdminLanguageValues { Id = 158, LanguageId = 2, Label = "##l@DmsExceptions:LicenceExpired@l##", Value = "Licence expired" });
            items.Add(new AdminLanguageValues { Id = 159, LanguageId = 1, Label = "##l@DmsExceptions:LicenceExceededNumberOfRegisteredUsers@l##", Value = "Превышено разрешенное количество зарегистрированных пользователей" });
            items.Add(new AdminLanguageValues { Id = 160, LanguageId = 2, Label = "##l@DmsExceptions:LicenceExceededNumberOfRegisteredUsers@l##", Value = "You have exceeded the allowed number of registered users" });
            items.Add(new AdminLanguageValues { Id = 161, LanguageId = 1, Label = "##l@DmsExceptions:LicenceExceededNumberOfConnectedUsers@l##", Value = "Превышено разрешенное количество подключенных пользователей" });
            items.Add(new AdminLanguageValues { Id = 162, LanguageId = 2, Label = "##l@DmsExceptions:LicenceExceededNumberOfConnectedUsers@l##", Value = "You have exceeded the allowed number of connected users" });
            items.Add(new AdminLanguageValues { Id = 163, LanguageId = 1, Label = "##l@DmsExceptions:ClientNameAlreadyExists@l##", Value = "Имя клиента уже существует" });
            items.Add(new AdminLanguageValues { Id = 164, LanguageId = 2, Label = "##l@DmsExceptions:ClientNameAlreadyExists@l##", Value = "Client Name already exists" });
            items.Add(new AdminLanguageValues { Id = 165, LanguageId = 1, Label = "##l@DmsExceptions:UserNameAlreadyExists@l##", Value = "Имя пользователя уже существует" });
            items.Add(new AdminLanguageValues { Id = 166, LanguageId = 2, Label = "##l@DmsExceptions:UserNameAlreadyExists@l##", Value = "User Name already exists" });
            items.Add(new AdminLanguageValues { Id = 167, LanguageId = 1, Label = "##l@DmsExceptions:ClientVerificationCodeIncorrect@l##", Value = "Проверочный код неверен" });
            items.Add(new AdminLanguageValues { Id = 168, LanguageId = 2, Label = "##l@DmsExceptions:ClientVerificationCodeIncorrect@l##", Value = "Verification code is invalid" });
            items.Add(new AdminLanguageValues { Id = 169, LanguageId = 1, Label = "##l@DmsExceptions:ClientIsNotFound@l##", Value = "Клиент не найден" });
            items.Add(new AdminLanguageValues { Id = 170, LanguageId = 2, Label = "##l@DmsExceptions:ClientIsNotFound@l##", Value = "Client not found" });

            items.Add(new AdminLanguageValues { Id = 171, LanguageId = 1, Label = "##l@DmsExceptions:EncryptionCertificatePrivateKeyСanNotBeExported@l##", Value = "Приватный ключ нельзя экспортировать" });
            items.Add(new AdminLanguageValues { Id = 172, LanguageId = 2, Label = "##l@DmsExceptions:EncryptionCertificatePrivateKeyСanNotBeExported@l##", Value = "The private key can not be exported" });
            items.Add(new AdminLanguageValues { Id = 173, LanguageId = 1, Label = "##l@DmsExceptions:EncryptionCertificateWasNotFound@l##", Value = "Сертификат не был найден" });
            items.Add(new AdminLanguageValues { Id = 174, LanguageId = 2, Label = "##l@DmsExceptions:EncryptionCertificateWasNotFound@l##", Value = "The certificate was not found" });

            return items;
        }

        private List<AdminAccessLevels> GetAdminAccessLevels()
        {
            var items = new List<AdminAccessLevels>();

            items.Add(new AdminAccessLevels { Id = 10, Code = null, Name = "Только лично", LastChangeUserId = (int)EnumSystemUsers.AdminUser, LastChangeDate = DateTime.Now });
            items.Add(new AdminAccessLevels { Id = 20, Code = null, Name = "Лично+референты", LastChangeUserId = (int)EnumSystemUsers.AdminUser, LastChangeDate = DateTime.Now });
            items.Add(new AdminAccessLevels { Id = 30, Code = null, Name = "Лично+референты+ИО", LastChangeUserId = (int)EnumSystemUsers.AdminUser, LastChangeDate = DateTime.Now });

            return items;
        }

        private List<SystemObjects> GetSystemObjects()
        {
            var items = new List<SystemObjects>();

            items.Add(new SystemObjects { Id = 100, Code = "Documents", Description = "Документы" });
            items.Add(new SystemObjects { Id = 101, Code = "DocumentAccesses", Description = "Документы - доступы" });
            items.Add(new SystemObjects { Id = 102, Code = "DocumentRestrictedSendLists", Description = "Документы - ограничения рассылки" });
            items.Add(new SystemObjects { Id = 103, Code = "DocumentSendLists", Description = "Документы - план работы" });
            items.Add(new SystemObjects { Id = 104, Code = "DocumentFiles", Description = "Документы - файлы" });
            items.Add(new SystemObjects { Id = 105, Code = "DocumentLinks", Description = "Документы - связи" });
            items.Add(new SystemObjects { Id = 106, Code = "DocumentSendListStages", Description = "Документы - этапы плана работ" });
            items.Add(new SystemObjects { Id = 111, Code = "DocumentEvents", Description = "Документы - события" });
            items.Add(new SystemObjects { Id = 112, Code = "DocumentWaits", Description = "Документы - ожидания" });
            items.Add(new SystemObjects { Id = 113, Code = "DocumentSubscriptions", Description = "Документы - подписи" });
            items.Add(new SystemObjects { Id = 115, Code = "DocumentTasks", Description = "Документы - задачи" });
            items.Add(new SystemObjects { Id = 121, Code = "DocumentPapers", Description = "Документы - бумажные носители" });
            items.Add(new SystemObjects { Id = 122, Code = "DocumentPaperEvents", Description = "Документы - события по бумажным носителям" });
            items.Add(new SystemObjects { Id = 123, Code = "DocumentPaperLists", Description = "Документы - реестры передачи бумажных носителей" });
            items.Add(new SystemObjects { Id = 191, Code = "DocumentSavedFilters", Description = "Документы - сохраненные фильтры" });
            items.Add(new SystemObjects { Id = 192, Code = "DocumentTags", Description = "Документы - тэги" });
            items.Add(new SystemObjects { Id = 201, Code = "DictionaryDocumentType", Description = "Типы документов" });
            items.Add(new SystemObjects { Id = 202, Code = "DictionaryAddressType", Description = "Типы адресов" });
            items.Add(new SystemObjects { Id = 203, Code = "DictionaryDocumentSubjects", Description = "Тематики документов" });
            items.Add(new SystemObjects { Id = 204, Code = "DictionaryRegistrationJournals", Description = "Журналы регистрации" });
            items.Add(new SystemObjects { Id = 205, Code = "DictionaryContactType", Description = "Типы контактов" });
            items.Add(new SystemObjects { Id = 206, Code = "DictionaryAgents", Description = "Контрагенты" });
            items.Add(new SystemObjects { Id = 207, Code = "DictionaryContacts", Description = "Контакты" });
            items.Add(new SystemObjects { Id = 208, Code = "DictionaryAgentAddresses", Description = "Адреса" });
            items.Add(new SystemObjects { Id = 209, Code = "DictionaryAgentPersons", Description = "Физические лица" });
            items.Add(new SystemObjects { Id = 210, Code = "DictionaryDepartments", Description = "Структура предприятия" });
            items.Add(new SystemObjects { Id = 211, Code = "DictionaryPositions", Description = "Штатное расписание" });
            items.Add(new SystemObjects { Id = 212, Code = "DictionaryAgentEmployees", Description = "Сотрудники" });
            items.Add(new SystemObjects { Id = 213, Code = "DictionaryAgentCompanies", Description = "Юридические лица" });
            items.Add(new SystemObjects { Id = 214, Code = "DictionaryAgentBanks", Description = "Контрагенты - банки" });
            items.Add(new SystemObjects { Id = 215, Code = "DictionaryAgentAccounts", Description = "Расчетные счета" });
            items.Add(new SystemObjects { Id = 216, Code = "DictionaryStandartSendListContent", Description = "Типовые списки рассылки (содержание)" });
            items.Add(new SystemObjects { Id = 217, Code = "DictionaryStandartSendLists", Description = "Типовые списки рассылки" });
            items.Add(new SystemObjects { Id = 218, Code = "DictionaryCompanies", Description = "Компании" });
            items.Add(new SystemObjects { Id = 219, Code = "DictionaryPositionExecutorTypes", Description = "Типы исполнителей" });
            items.Add(new SystemObjects { Id = 220, Code = "DictionaryPositionExecutors", Description = "Исполнители должности" });
            items.Add(new SystemObjects { Id = 251, Code = "TemplateDocument", Description = "Шаблоны документов" });
            items.Add(new SystemObjects { Id = 252, Code = "TemplateDocumentSendList", Description = "Списки рассылки в шаблонах" });
            items.Add(new SystemObjects { Id = 253, Code = "TemplateDocumentRestrictedSendList", Description = "Ограничительные списки рассылки в шаблонах" });
            items.Add(new SystemObjects { Id = 254, Code = "TemplateDocumentTask", Description = "Задачи в шаблонах" });
            items.Add(new SystemObjects { Id = 255, Code = "TemplateDocumentAttachedFiles", Description = "Прикрепленные к шаблонам файлы" });
            items.Add(new SystemObjects { Id = 291, Code = "DictionaryTag", Description = "Теги" });
            items.Add(new SystemObjects { Id = 301, Code = "CustomDictionaryTypes", Description = "Типы пользовательских словарей" });
            items.Add(new SystemObjects { Id = 302, Code = "CustomDictionaries", Description = "Пользовательские словари" });
            items.Add(new SystemObjects { Id = 311, Code = "Properties", Description = "Динамические аттрибуты" });
            items.Add(new SystemObjects { Id = 312, Code = "PropertyLinks", Description = "Связи динамических аттрибутов с объектами системы" });
            items.Add(new SystemObjects { Id = 313, Code = "PropertyValues", Description = "Значения динамических аттрибутов" });
            items.Add(new SystemObjects { Id = 401, Code = "EncryptionCertificates", Description = "Хранилище сертификатов" });
            items.Add(new SystemObjects { Id = 402, Code = "EncryptionCertificateTypes", Description = "Типы сертификатов" });

            return items;
        }

        private List<SystemActions> GetSystemActions()
        {
            var items = new List<SystemActions>();

            items.Add(new SystemActions { Id = 100001, ObjectId = 100, Code = "AddDocument", API = "", Description = "Создать документ по шаблону", IsGrantable = false, IsGrantableByRecordId = false, IsVisible = false, GrantId = null, Category = "Документ" });
            items.Add(new SystemActions { Id = 100002, ObjectId = 100, Code = "CopyDocument", API = "", Description = "Создать документ копированием", IsGrantable = false, IsGrantableByRecordId = false, IsVisible = false, GrantId = null, Category = "Документ" });
            items.Add(new SystemActions { Id = 100003, ObjectId = 100, Code = "ModifyDocument", API = "", Description = "Изменить документ", IsGrantable = false, IsGrantableByRecordId = false, IsVisible = false, GrantId = null, Category = "Документ" });
            items.Add(new SystemActions { Id = 100004, ObjectId = 100, Code = "DeleteDocument", API = "", Description = "Удалить проект", IsGrantable = false, IsGrantableByRecordId = false, IsVisible = false, GrantId = null, Category = "Документ" });
            items.Add(new SystemActions { Id = 100005, ObjectId = 100, Code = "LaunchPlan", API = "", Description = "Запустить выполнение плана", IsGrantable = false, IsGrantableByRecordId = false, IsVisible = false, GrantId = null, Category = "Документ" });
            items.Add(new SystemActions { Id = 100006, ObjectId = 100, Code = "AddDocumentSendListItem", API = "", Description = "Добавить пункт плана", IsGrantable = false, IsGrantableByRecordId = false, IsVisible = false, GrantId = null, Category = "Документ" });
            items.Add(new SystemActions { Id = 100007, ObjectId = 100, Code = "StopPlan", API = "", Description = "Остановить выполнение плана", IsGrantable = false, IsGrantableByRecordId = false, IsVisible = false, GrantId = null, Category = "Документ" });
            items.Add(new SystemActions { Id = 100008, ObjectId = 100, Code = "ChangeExecutor", API = "", Description = "Передать управление", IsGrantable = false, IsGrantableByRecordId = false, IsVisible = false, GrantId = null, Category = "Документ" });
            items.Add(new SystemActions { Id = 100009, ObjectId = 100, Code = "RegisterDocument", API = "", Description = "Зарегистрировать проект", IsGrantable = false, IsGrantableByRecordId = false, IsVisible = false, GrantId = null, Category = "Документ" });
            items.Add(new SystemActions { Id = 100010, ObjectId = 100, Code = "MarkDocumentEventAsRead", API = "", Description = "Отметить прочтение событий по документу", IsGrantable = false, IsGrantableByRecordId = false, IsVisible = false, GrantId = null, Category = "Информирование" });
            items.Add(new SystemActions { Id = 100011, ObjectId = 100, Code = "SendForInformation", API = "", Description = "Направить для сведения", IsGrantable = false, IsGrantableByRecordId = false, IsVisible = false, GrantId = null, Category = "Информирование" });
            items.Add(new SystemActions { Id = 100012, ObjectId = 100, Code = "SendForConsideration", API = "", Description = "Направить для рассмотрения", IsGrantable = false, IsGrantableByRecordId = false, IsVisible = false, GrantId = null, Category = "Информирование" });
            items.Add(new SystemActions { Id = 100015, ObjectId = 100, Code = "SendForInformationExternal", API = "", Description = "Направить для сведения внешнему агенту", IsGrantable = false, IsGrantableByRecordId = false, IsVisible = false, GrantId = null, Category = "Информирование" });
            items.Add(new SystemActions { Id = 100021, ObjectId = 112, Code = "ControlOn", API = "", Description = "Взять на контроль", IsGrantable = false, IsGrantableByRecordId = false, IsVisible = false, GrantId = null, Category = "Контроль" });
            items.Add(new SystemActions { Id = 100023, ObjectId = 112, Code = "ControlChange", API = "", Description = "Изменить параметры контроля", IsGrantable = false, IsGrantableByRecordId = false, IsVisible = false, GrantId = null, Category = "Контроль" });
            items.Add(new SystemActions { Id = 100024, ObjectId = 112, Code = "SendForExecutionChange", API = "", Description = "Изменить параметры направлен для исполнения", IsGrantable = false, IsGrantableByRecordId = false, IsVisible = false, GrantId = null, Category = "Контроль" });
            items.Add(new SystemActions { Id = 100026, ObjectId = 112, Code = "SendForResponsibleExecutionChange", API = "", Description = "Изменить параметры направлен для отв.исполнения", IsGrantable = false, IsGrantableByRecordId = false, IsVisible = false, GrantId = null, Category = "Контроль" });
            items.Add(new SystemActions { Id = 100027, ObjectId = 112, Code = "ControlTargetChange", API = "", Description = "Изменить параметры контроля для исполнителя", IsGrantable = false, IsGrantableByRecordId = false, IsVisible = false, GrantId = null, Category = "Контроль" });
            items.Add(new SystemActions { Id = 100029, ObjectId = 112, Code = "ControlOff", API = "", Description = "Снять с контроля", IsGrantable = false, IsGrantableByRecordId = false, IsVisible = false, GrantId = null, Category = "Контроль" });
            items.Add(new SystemActions { Id = 100030, ObjectId = 100, Code = "SendForControl", API = "", Description = "Направить для контроля", IsGrantable = false, IsGrantableByRecordId = false, IsVisible = false, GrantId = null, Category = "Контроль" });
            items.Add(new SystemActions { Id = 100031, ObjectId = 100, Code = "SendForResponsibleExecution", API = "", Description = "Направить для отв.исполнения", IsGrantable = false, IsGrantableByRecordId = false, IsVisible = false, GrantId = null, Category = "Контроль" });
            items.Add(new SystemActions { Id = 100032, ObjectId = 100, Code = "SendForExecution", API = "", Description = "Направить для исполнения", IsGrantable = false, IsGrantableByRecordId = false, IsVisible = false, GrantId = null, Category = "Контроль" });
            items.Add(new SystemActions { Id = 100035, ObjectId = 112, Code = "MarkExecution", API = "", Description = "Отметить исполнение", IsGrantable = false, IsGrantableByRecordId = false, IsVisible = false, GrantId = null, Category = "Контроль" });
            items.Add(new SystemActions { Id = 100037, ObjectId = 112, Code = "AcceptResult", API = "", Description = "Принять результат", IsGrantable = false, IsGrantableByRecordId = false, IsVisible = false, GrantId = null, Category = "Контроль" });
            items.Add(new SystemActions { Id = 100038, ObjectId = 112, Code = "RejectResult", API = "", Description = "Отклонить результат", IsGrantable = false, IsGrantableByRecordId = false, IsVisible = false, GrantId = null, Category = "Контроль" });
            items.Add(new SystemActions { Id = 100041, ObjectId = 100, Code = "SendForVisaing", API = "", Description = "Направить для визирования", IsGrantable = false, IsGrantableByRecordId = false, IsVisible = false, GrantId = null, Category = "Подписание" });
            items.Add(new SystemActions { Id = 100042, ObjectId = 100, Code = "SendForАgreement", API = "", Description = "Направить для согласование", IsGrantable = false, IsGrantableByRecordId = false, IsVisible = false, GrantId = null, Category = "Подписание" });
            items.Add(new SystemActions { Id = 100043, ObjectId = 100, Code = "SendForАpproval", API = "", Description = "Направить для утверждения", IsGrantable = false, IsGrantableByRecordId = false, IsVisible = false, GrantId = null, Category = "Подписание" });
            items.Add(new SystemActions { Id = 100044, ObjectId = 100, Code = "SendForSigning", API = "", Description = "Направить для подписи", IsGrantable = false, IsGrantableByRecordId = false, IsVisible = false, GrantId = null, Category = "Подписание" });
            items.Add(new SystemActions { Id = 100046, ObjectId = 113, Code = "WithdrawVisaing", API = "", Description = "Отозвать с визирования", IsGrantable = false, IsGrantableByRecordId = false, IsVisible = false, GrantId = null, Category = "Подписание" });
            items.Add(new SystemActions { Id = 100047, ObjectId = 113, Code = "WithdrawАgreement", API = "", Description = "Отозвать с согласования", IsGrantable = false, IsGrantableByRecordId = false, IsVisible = false, GrantId = null, Category = "Подписание" });
            items.Add(new SystemActions { Id = 100048, ObjectId = 113, Code = "WithdrawАpproval", API = "", Description = "Отозвать с утверждения", IsGrantable = false, IsGrantableByRecordId = false, IsVisible = false, GrantId = null, Category = "Подписание" });
            items.Add(new SystemActions { Id = 100049, ObjectId = 113, Code = "WithdrawSigning", API = "", Description = "Отозвать с подписи", IsGrantable = false, IsGrantableByRecordId = false, IsVisible = false, GrantId = null, Category = "Подписание" });
            items.Add(new SystemActions { Id = 100051, ObjectId = 113, Code = "AffixVisaing", API = "", Description = "Завизировать", IsGrantable = false, IsGrantableByRecordId = false, IsVisible = false, GrantId = null, Category = "Подписание" });
            items.Add(new SystemActions { Id = 100052, ObjectId = 113, Code = "AffixАgreement", API = "", Description = "Согласовать", IsGrantable = false, IsGrantableByRecordId = false, IsVisible = false, GrantId = null, Category = "Подписание" });
            items.Add(new SystemActions { Id = 100053, ObjectId = 113, Code = "AffixАpproval", API = "", Description = "Утвердить", IsGrantable = false, IsGrantableByRecordId = false, IsVisible = false, GrantId = null, Category = "Подписание" });
            items.Add(new SystemActions { Id = 100054, ObjectId = 113, Code = "AffixSigning", API = "", Description = "Подписать", IsGrantable = false, IsGrantableByRecordId = false, IsVisible = false, GrantId = null, Category = "Подписание" });
            items.Add(new SystemActions { Id = 100055, ObjectId = 113, Code = "SelfAffixSigning", API = "", Description = "Самоподписание", IsGrantable = false, IsGrantableByRecordId = false, IsVisible = false, GrantId = null, Category = "Подписание" });
            items.Add(new SystemActions { Id = 100056, ObjectId = 113, Code = "RejectVisaing", API = "", Description = "Отказать в визирования", IsGrantable = false, IsGrantableByRecordId = false, IsVisible = false, GrantId = null, Category = "Подписание" });
            items.Add(new SystemActions { Id = 100057, ObjectId = 113, Code = "RejectАgreement", API = "", Description = "Отказать в согласование", IsGrantable = false, IsGrantableByRecordId = false, IsVisible = false, GrantId = null, Category = "Подписание" });
            items.Add(new SystemActions { Id = 100058, ObjectId = 113, Code = "RejectАpproval", API = "", Description = "Отказать в утверждения", IsGrantable = false, IsGrantableByRecordId = false, IsVisible = false, GrantId = null, Category = "Подписание" });
            items.Add(new SystemActions { Id = 100059, ObjectId = 113, Code = "RejectSigning", API = "", Description = "Отказать в подписи", IsGrantable = false, IsGrantableByRecordId = false, IsVisible = false, GrantId = null, Category = "Подписание" });
            items.Add(new SystemActions { Id = 100081, ObjectId = 111, Code = "SendMessage", API = "", Description = "Направить сообщение участникам рабочей группы", IsGrantable = false, IsGrantableByRecordId = false, IsVisible = false, GrantId = null, Category = "Информирование" });
            items.Add(new SystemActions { Id = 100083, ObjectId = 111, Code = "AddNote", API = "", Description = "Добавить примечание", IsGrantable = false, IsGrantableByRecordId = false, IsVisible = false, GrantId = null, Category = "Информирование" });
            items.Add(new SystemActions { Id = 100085, ObjectId = 100, Code = "ReportRegistrationCardDocument", API = "", Description = "Регистрационная карточка", IsGrantable = false, IsGrantableByRecordId = false, IsVisible = false, GrantId = null, Category = "Отчеты" });
            items.Add(new SystemActions { Id = 100091, ObjectId = 100, Code = "AddFavourite", API = "", Description = "Добавить в избранное", IsGrantable = false, IsGrantableByRecordId = false, IsVisible = false, GrantId = null, Category = "Дополнительно" });
            items.Add(new SystemActions { Id = 100093, ObjectId = 100, Code = "DeleteFavourite", API = "", Description = "Удалить из избранного", IsGrantable = false, IsGrantableByRecordId = false, IsVisible = false, GrantId = null, Category = "Дополнительно" });
            items.Add(new SystemActions { Id = 100095, ObjectId = 100, Code = "FinishWork", API = "", Description = "Закончить работу с документом", IsGrantable = false, IsGrantableByRecordId = false, IsVisible = false, GrantId = null, Category = "Дополнительно" });
            items.Add(new SystemActions { Id = 100097, ObjectId = 100, Code = "StartWork", API = "", Description = "Возобновить работу с документом", IsGrantable = false, IsGrantableByRecordId = false, IsVisible = false, GrantId = null, Category = "Дополнительно" });
            items.Add(new SystemActions { Id = 100099, ObjectId = 100, Code = "ChangePosition", API = "", Description = "Поменять должность в документе", IsGrantable = false, IsGrantableByRecordId = false, IsVisible = false, GrantId = null, Category = "Администратор" });
            items.Add(new SystemActions { Id = 102001, ObjectId = 102, Code = "AddDocumentRestrictedSendList", API = "", Description = "Добавить ограничение рассылки", IsGrantable = false, IsGrantableByRecordId = false, IsVisible = false, GrantId = null, Category = null });
            items.Add(new SystemActions { Id = 102002, ObjectId = 102, Code = "AddByStandartSendListDocumentRestrictedSendList", API = "", Description = "Добавить ограничения рассылки по стандартному списку", IsGrantable = false, IsGrantableByRecordId = false, IsVisible = false, GrantId = null, Category = null });
            items.Add(new SystemActions { Id = 102009, ObjectId = 102, Code = "DeleteDocumentRestrictedSendList", API = "", Description = "Удалить ограничение рассылки", IsGrantable = false, IsGrantableByRecordId = false, IsVisible = false, GrantId = null, Category = null });
            items.Add(new SystemActions { Id = 103001, ObjectId = 106, Code = "AddDocumentSendList", API = "", Description = "Добавить пункт плана", IsGrantable = false, IsGrantableByRecordId = false, IsVisible = false, GrantId = null, Category = null });
            items.Add(new SystemActions { Id = 103002, ObjectId = 106, Code = "AddByStandartSendListDocumentSendList", API = "", Description = "Добавить план работы по стандартному списку", IsGrantable = false, IsGrantableByRecordId = false, IsVisible = false, GrantId = null, Category = null });
            items.Add(new SystemActions { Id = 103005, ObjectId = 103, Code = "ModifyDocumentSendList", API = "", Description = "Изменить пункт плана", IsGrantable = false, IsGrantableByRecordId = false, IsVisible = false, GrantId = null, Category = null });
            items.Add(new SystemActions { Id = 103009, ObjectId = 103, Code = "DeleteDocumentSendList", API = "", Description = "Удалить пункт плана", IsGrantable = false, IsGrantableByRecordId = false, IsVisible = false, GrantId = null, Category = null });
            items.Add(new SystemActions { Id = 103011, ObjectId = 106, Code = "AddDocumentSendListStage", API = "", Description = "Добавить этап плана", IsGrantable = false, IsGrantableByRecordId = false, IsVisible = false, GrantId = null, Category = null });
            items.Add(new SystemActions { Id = 103019, ObjectId = 106, Code = "DeleteDocumentSendListStage", API = "", Description = "Удалить этап плана", IsGrantable = false, IsGrantableByRecordId = false, IsVisible = false, GrantId = null, Category = null });
            items.Add(new SystemActions { Id = 103021, ObjectId = 103, Code = "LaunchDocumentSendListItem", API = "", Description = "Запустить пункт плана на исполнение", IsGrantable = false, IsGrantableByRecordId = false, IsVisible = false, GrantId = null, Category = null });
            items.Add(new SystemActions { Id = 104001, ObjectId = 104, Code = "AddDocumentFile", API = "", Description = "Добавить файл", IsGrantable = false, IsGrantableByRecordId = false, IsVisible = false, GrantId = null, Category = null });
            items.Add(new SystemActions { Id = 104005, ObjectId = 104, Code = "ModifyDocumentFile", API = "", Description = "Изменить файл", IsGrantable = false, IsGrantableByRecordId = false, IsVisible = false, GrantId = null, Category = null });
            items.Add(new SystemActions { Id = 104009, ObjectId = 104, Code = "DeleteDocumentFile", API = "", Description = "Удалить файл", IsGrantable = false, IsGrantableByRecordId = false, IsVisible = false, GrantId = null, Category = null });
            items.Add(new SystemActions { Id = 104010, ObjectId = 104, Code = "AddDocumentFileUseMainNameFile", API = "", Description = "Добавить версию файла к файлу", IsGrantable = false, IsGrantableByRecordId = false, IsVisible = false, GrantId = null, Category = null });
            items.Add(new SystemActions { Id = 104011, ObjectId = 104, Code = "AcceptDocumentFile", API = "", Description = "Файл принят", IsGrantable = false, IsGrantableByRecordId = false, IsVisible = false, GrantId = null, Category = null });
            items.Add(new SystemActions { Id = 104012, ObjectId = 104, Code = "RejectDocumentFile", API = "", Description = "Файл не принят", IsGrantable = false, IsGrantableByRecordId = false, IsVisible = false, GrantId = null, Category = null });
            items.Add(new SystemActions { Id = 104013, ObjectId = 104, Code = "RenameDocumentFile", API = "", Description = "Переименовать файл", IsGrantable = false, IsGrantableByRecordId = false, IsVisible = false, GrantId = null, Category = null });
            items.Add(new SystemActions { Id = 104014, ObjectId = 104, Code = "DeleteDocumentFileVersion", API = "", Description = "Удалить версию файл", IsGrantable = false, IsGrantableByRecordId = false, IsVisible = false, GrantId = null, Category = null });
            items.Add(new SystemActions { Id = 104015, ObjectId = 104, Code = "DeleteDocumentFileVersionRecord", API = "", Description = "Удалить запись о версим файла", IsGrantable = false, IsGrantableByRecordId = false, IsVisible = false, GrantId = null, Category = null });
            items.Add(new SystemActions { Id = 104016, ObjectId = 104, Code = "AcceptMainVersionDocumentFile", API = "", Description = "Удалить запись о версим файла", IsGrantable = false, IsGrantableByRecordId = false, IsVisible = false, GrantId = null, Category = null });
            items.Add(new SystemActions { Id = 105001, ObjectId = 105, Code = "AddDocumentLink", API = "", Description = "Добавить связь между документами", IsGrantable = false, IsGrantableByRecordId = false, IsVisible = false, GrantId = null, Category = null });
            items.Add(new SystemActions { Id = 105009, ObjectId = 105, Code = "DeleteDocumentLink", API = "", Description = "Удалить связь между документами", IsGrantable = false, IsGrantableByRecordId = false, IsVisible = false, GrantId = null, Category = null });
            items.Add(new SystemActions { Id = 115001, ObjectId = 115, Code = "AddDocumentTask", API = "", Description = "Добавить задачу", IsGrantable = false, IsGrantableByRecordId = false, IsVisible = false, GrantId = null, Category = null });
            items.Add(new SystemActions { Id = 115005, ObjectId = 115, Code = "ModifyDocumentTask", API = "", Description = "Изменить задачу", IsGrantable = false, IsGrantableByRecordId = false, IsVisible = false, GrantId = null, Category = null });
            items.Add(new SystemActions { Id = 115009, ObjectId = 115, Code = "DeleteDocumentTask", API = "", Description = "Удалить задачу", IsGrantable = false, IsGrantableByRecordId = false, IsVisible = false, GrantId = null, Category = null });
            items.Add(new SystemActions { Id = 121001, ObjectId = 121, Code = "AddDocumentPaper", API = "", Description = "Добавить бумажный носитель", IsGrantable = false, IsGrantableByRecordId = false, IsVisible = false, GrantId = null, Category = "Бумажные носители" });
            items.Add(new SystemActions { Id = 121005, ObjectId = 121, Code = "ModifyDocumentPaper", API = "", Description = "Изменить бумажный носитель", IsGrantable = false, IsGrantableByRecordId = false, IsVisible = false, GrantId = null, Category = "Бумажные носители" });
            items.Add(new SystemActions { Id = 121007, ObjectId = 121, Code = "MarkOwnerDocumentPaper", API = "", Description = "Отметить нахождение бумажного носителя у себя", IsGrantable = false, IsGrantableByRecordId = false, IsVisible = false, GrantId = null, Category = "Бумажные носители" });
            items.Add(new SystemActions { Id = 121008, ObjectId = 121, Code = "MarkСorruptionDocumentPaper", API = "", Description = "Отметить порчу бумажного носителя", IsGrantable = false, IsGrantableByRecordId = false, IsVisible = false, GrantId = null, Category = "Бумажные носители" });
            items.Add(new SystemActions { Id = 121009, ObjectId = 121, Code = "DeleteDocumentPaper", API = "", Description = "Удалить бумажный носитель", IsGrantable = false, IsGrantableByRecordId = false, IsVisible = false, GrantId = null, Category = "Бумажные носители" });
            items.Add(new SystemActions { Id = 122001, ObjectId = 122, Code = "PlanDocumentPaperEvent", API = "", Description = "Планировать движение бумажного носителя", IsGrantable = false, IsGrantableByRecordId = false, IsVisible = false, GrantId = null, Category = "Бумажные носители" });
            items.Add(new SystemActions { Id = 122009, ObjectId = 122, Code = "CancelPlanDocumentPaperEvent", API = "", Description = "Отменить планирование движения бумажного носителя", IsGrantable = false, IsGrantableByRecordId = false, IsVisible = false, GrantId = null, Category = "Бумажные носители" });
            items.Add(new SystemActions { Id = 122011, ObjectId = 122, Code = "SendDocumentPaperEvent", API = "", Description = "Отметить передачу бумажного носителя", IsGrantable = false, IsGrantableByRecordId = false, IsVisible = false, GrantId = null, Category = "Бумажные носители" });
            items.Add(new SystemActions { Id = 122019, ObjectId = 122, Code = "CancelSendDocumentPaperEvent", API = "", Description = "Отменить передачу бумажного носителя", IsGrantable = false, IsGrantableByRecordId = false, IsVisible = false, GrantId = null, Category = "Бумажные носители" });
            items.Add(new SystemActions { Id = 122021, ObjectId = 122, Code = "RecieveDocumentPaperEvent", API = "", Description = "Отметить прием бумажного носителя", IsGrantable = false, IsGrantableByRecordId = false, IsVisible = false, GrantId = null, Category = "Бумажные носители" });
            items.Add(new SystemActions { Id = 123001, ObjectId = 123, Code = "AddDocumentPaperList", API = "", Description = "Добавить реестр бумажных носителей", IsGrantable = false, IsGrantableByRecordId = false, IsVisible = false, GrantId = null, Category = "Реестры бумажных носителей" });
            items.Add(new SystemActions { Id = 123005, ObjectId = 123, Code = "ModifyDocumentPaperList", API = "", Description = "Изменить реестр бумажных носителей", IsGrantable = false, IsGrantableByRecordId = false, IsVisible = false, GrantId = null, Category = "Реестры бумажных носителей" });
            items.Add(new SystemActions { Id = 123009, ObjectId = 123, Code = "DeleteDocumentPaperList", API = "", Description = "Удалить реестр бумажных носителей", IsGrantable = false, IsGrantableByRecordId = false, IsVisible = false, GrantId = null, Category = "Реестры бумажных носителей" });
            items.Add(new SystemActions { Id = 191001, ObjectId = 191, Code = "AddSavedFilter", API = "", Description = "Добавить сохраненный фильтр", IsGrantable = false, IsGrantableByRecordId = false, IsVisible = false, GrantId = null, Category = null });
            items.Add(new SystemActions { Id = 191005, ObjectId = 191, Code = "ModifySavedFilter", API = "", Description = "Изменить сохраненный фильтр", IsGrantable = false, IsGrantableByRecordId = false, IsVisible = false, GrantId = null, Category = null });
            items.Add(new SystemActions { Id = 191009, ObjectId = 191, Code = "DeleteSavedFilter", API = "", Description = "Удалить сохраненный фильтр", IsGrantable = false, IsGrantableByRecordId = false, IsVisible = false, GrantId = null, Category = null });
            items.Add(new SystemActions { Id = 192005, ObjectId = 192, Code = "ModifyDocumentTags", API = "", Description = "Изменить тэги по документу", IsGrantable = false, IsGrantableByRecordId = false, IsVisible = false, GrantId = null, Category = null });
            items.Add(new SystemActions { Id = 201001, ObjectId = 201, Code = "AddDocumentType", API = "", Description = "Добавить тип документа", IsGrantable = false, IsGrantableByRecordId = false, IsVisible = false, GrantId = null, Category = null });
            items.Add(new SystemActions { Id = 201005, ObjectId = 201, Code = "ModifyDocumentType", API = "", Description = "Изменить тип документа", IsGrantable = false, IsGrantableByRecordId = false, IsVisible = false, GrantId = null, Category = null });
            items.Add(new SystemActions { Id = 201009, ObjectId = 201, Code = "DeleteDocumentType", API = "", Description = "Удалить тип документа", IsGrantable = false, IsGrantableByRecordId = false, IsVisible = false, GrantId = null, Category = null });
            items.Add(new SystemActions { Id = 202001, ObjectId = 202, Code = "AddAddressType", API = "", Description = "Добавить тип адреса", IsGrantable = false, IsGrantableByRecordId = false, IsVisible = false, GrantId = null, Category = null });
            items.Add(new SystemActions { Id = 202005, ObjectId = 202, Code = "ModifyAddressType", API = "", Description = "Изменить тип адреса", IsGrantable = false, IsGrantableByRecordId = false, IsVisible = false, GrantId = null, Category = null });
            items.Add(new SystemActions { Id = 202009, ObjectId = 202, Code = "DeleteAddressType", API = "", Description = "Удалить тип адреса", IsGrantable = false, IsGrantableByRecordId = false, IsVisible = false, GrantId = null, Category = null });
            items.Add(new SystemActions { Id = 203001, ObjectId = 203, Code = "AddDocumentSubject", API = "", Description = "Добавить тематику", IsGrantable = false, IsGrantableByRecordId = false, IsVisible = false, GrantId = null, Category = null });
            items.Add(new SystemActions { Id = 203005, ObjectId = 203, Code = "ModifyDocumentSubject", API = "", Description = "Изменить тематику", IsGrantable = false, IsGrantableByRecordId = false, IsVisible = false, GrantId = null, Category = null });
            items.Add(new SystemActions { Id = 203009, ObjectId = 203, Code = "DeleteDocumentSubject", API = "", Description = "Удалить тематику", IsGrantable = false, IsGrantableByRecordId = false, IsVisible = false, GrantId = null, Category = null });
            items.Add(new SystemActions { Id = 204001, ObjectId = 204, Code = "AddRegistrationJournal", API = "", Description = "Добавить журнал регистрации", IsGrantable = false, IsGrantableByRecordId = false, IsVisible = false, GrantId = null, Category = null });
            items.Add(new SystemActions { Id = 204005, ObjectId = 204, Code = "ModifyRegistrationJournal", API = "", Description = "Изменить журнал регистрации", IsGrantable = false, IsGrantableByRecordId = false, IsVisible = false, GrantId = null, Category = null });
            items.Add(new SystemActions { Id = 204009, ObjectId = 204, Code = "DeleteRegistrationJournal", API = "", Description = "Удалить журнал регистрации", IsGrantable = false, IsGrantableByRecordId = false, IsVisible = false, GrantId = null, Category = null });
            items.Add(new SystemActions { Id = 205001, ObjectId = 205, Code = "AddContactType", API = "", Description = "Добавить тип контакта", IsGrantable = false, IsGrantableByRecordId = false, IsVisible = false, GrantId = null, Category = null });
            items.Add(new SystemActions { Id = 205005, ObjectId = 205, Code = "ModifyContactType", API = "", Description = "Изменить тип контакта", IsGrantable = false, IsGrantableByRecordId = false, IsVisible = false, GrantId = null, Category = null });
            items.Add(new SystemActions { Id = 205009, ObjectId = 205, Code = "DeleteContactType", API = "", Description = "Удалить тип контакта", IsGrantable = false, IsGrantableByRecordId = false, IsVisible = false, GrantId = null, Category = null });
            items.Add(new SystemActions { Id = 206001, ObjectId = 206, Code = "AddAgent", API = "", Description = "Добавить контрагента", IsGrantable = false, IsGrantableByRecordId = false, IsVisible = false, GrantId = null, Category = null });
            items.Add(new SystemActions { Id = 206005, ObjectId = 206, Code = "ModifyAgent", API = "", Description = "Изменить контрагента", IsGrantable = false, IsGrantableByRecordId = false, IsVisible = false, GrantId = null, Category = null });
            items.Add(new SystemActions { Id = 206009, ObjectId = 206, Code = "DeleteAgent", API = "", Description = "Удалить контрагента", IsGrantable = false, IsGrantableByRecordId = false, IsVisible = false, GrantId = null, Category = null });
            items.Add(new SystemActions { Id = 207001, ObjectId = 207, Code = "AddContact", API = "", Description = "Добавить контакт", IsGrantable = false, IsGrantableByRecordId = false, IsVisible = false, GrantId = null, Category = null });
            items.Add(new SystemActions { Id = 207005, ObjectId = 207, Code = "ModifyContact", API = "", Description = "Изменить контакт", IsGrantable = false, IsGrantableByRecordId = false, IsVisible = false, GrantId = null, Category = null });
            items.Add(new SystemActions { Id = 207009, ObjectId = 207, Code = "DeleteContact", API = "", Description = "Удалить контакт", IsGrantable = false, IsGrantableByRecordId = false, IsVisible = false, GrantId = null, Category = null });
            items.Add(new SystemActions { Id = 208001, ObjectId = 208, Code = "AddAddress", API = "", Description = "Добавить адрес", IsGrantable = false, IsGrantableByRecordId = false, IsVisible = false, GrantId = null, Category = null });
            items.Add(new SystemActions { Id = 208005, ObjectId = 208, Code = "ModifyAddress", API = "", Description = "Изменить адрес", IsGrantable = false, IsGrantableByRecordId = false, IsVisible = false, GrantId = null, Category = null });
            items.Add(new SystemActions { Id = 208009, ObjectId = 208, Code = "DeleteAddress", API = "", Description = "Удалить адрес", IsGrantable = false, IsGrantableByRecordId = false, IsVisible = false, GrantId = null, Category = null });
            items.Add(new SystemActions { Id = 209001, ObjectId = 209, Code = "AddAgentPerson", API = "", Description = "Добавить физическое лицо", IsGrantable = false, IsGrantableByRecordId = false, IsVisible = false, GrantId = null, Category = null });
            items.Add(new SystemActions { Id = 209005, ObjectId = 209, Code = "ModifyAgentPerson", API = "", Description = "Изменить физическое лицо", IsGrantable = false, IsGrantableByRecordId = false, IsVisible = false, GrantId = null, Category = null });
            items.Add(new SystemActions { Id = 209009, ObjectId = 209, Code = "DeleteAgentPerson", API = "", Description = "Удалить физическое лицо", IsGrantable = false, IsGrantableByRecordId = false, IsVisible = false, GrantId = null, Category = null });
            items.Add(new SystemActions { Id = 210001, ObjectId = 210, Code = "AddDepartment", API = "", Description = "Добавить подразделение", IsGrantable = false, IsGrantableByRecordId = false, IsVisible = false, GrantId = null, Category = null });
            items.Add(new SystemActions { Id = 210005, ObjectId = 210, Code = "ModifyDepartment", API = "", Description = "Изменить подразделение", IsGrantable = false, IsGrantableByRecordId = false, IsVisible = false, GrantId = null, Category = null });
            items.Add(new SystemActions { Id = 210009, ObjectId = 210, Code = "DeleteDepartment", API = "", Description = "Удалить подразделение", IsGrantable = false, IsGrantableByRecordId = false, IsVisible = false, GrantId = null, Category = null });
            items.Add(new SystemActions { Id = 211001, ObjectId = 211, Code = "AddPosition", API = "", Description = "Добавить должность", IsGrantable = false, IsGrantableByRecordId = false, IsVisible = false, GrantId = null, Category = null });
            items.Add(new SystemActions { Id = 211005, ObjectId = 211, Code = "ModifyPosition", API = "", Description = "Изменить должность", IsGrantable = false, IsGrantableByRecordId = false, IsVisible = false, GrantId = null, Category = null });
            items.Add(new SystemActions { Id = 211009, ObjectId = 211, Code = "DeletePosition", API = "", Description = "Удалить должность", IsGrantable = false, IsGrantableByRecordId = false, IsVisible = false, GrantId = null, Category = null });
            items.Add(new SystemActions { Id = 212001, ObjectId = 212, Code = "AddAgentEmployee", API = "", Description = "Добавить сотрудника", IsGrantable = false, IsGrantableByRecordId = false, IsVisible = false, GrantId = null, Category = null });
            items.Add(new SystemActions { Id = 212005, ObjectId = 212, Code = "ModifyAgentEmployee", API = "", Description = "Изменить сотрудника", IsGrantable = false, IsGrantableByRecordId = false, IsVisible = false, GrantId = null, Category = null });
            items.Add(new SystemActions { Id = 212009, ObjectId = 212, Code = "DeleteAgentEmployee", API = "", Description = "Удалить сотрудника", IsGrantable = false, IsGrantableByRecordId = false, IsVisible = false, GrantId = null, Category = null });
            items.Add(new SystemActions { Id = 213001, ObjectId = 213, Code = "AddAgentCompany", API = "", Description = "Добавить юридическое лицо", IsGrantable = false, IsGrantableByRecordId = false, IsVisible = false, GrantId = null, Category = null });
            items.Add(new SystemActions { Id = 213005, ObjectId = 213, Code = "ModifyAgentCompany", API = "", Description = "Изменить юридическое лицо", IsGrantable = false, IsGrantableByRecordId = false, IsVisible = false, GrantId = null, Category = null });
            items.Add(new SystemActions { Id = 213009, ObjectId = 213, Code = "DeleteAgentCompany", API = "", Description = "Удалить юридическое лицо", IsGrantable = false, IsGrantableByRecordId = false, IsVisible = false, GrantId = null, Category = null });
            items.Add(new SystemActions { Id = 214001, ObjectId = 214, Code = "AddAgentBank", API = "", Description = "Добавить банк", IsGrantable = false, IsGrantableByRecordId = false, IsVisible = false, GrantId = null, Category = null });
            items.Add(new SystemActions { Id = 214005, ObjectId = 214, Code = "ModifyAgentBank", API = "", Description = "Изменить банк", IsGrantable = false, IsGrantableByRecordId = false, IsVisible = false, GrantId = null, Category = null });
            items.Add(new SystemActions { Id = 214009, ObjectId = 214, Code = "DeleteAgentBank", API = "", Description = "Удалить банк", IsGrantable = false, IsGrantableByRecordId = false, IsVisible = false, GrantId = null, Category = null });
            items.Add(new SystemActions { Id = 215001, ObjectId = 215, Code = "AddAgentAccount", API = "", Description = "Добавить расчетный счет", IsGrantable = false, IsGrantableByRecordId = false, IsVisible = false, GrantId = null, Category = null });
            items.Add(new SystemActions { Id = 215005, ObjectId = 215, Code = "ModifyAgentAccount", API = "", Description = "Изменить расчетный счет", IsGrantable = false, IsGrantableByRecordId = false, IsVisible = false, GrantId = null, Category = null });
            items.Add(new SystemActions { Id = 215009, ObjectId = 215, Code = "DeleteAgentAccount", API = "", Description = "Удалить расчетный счет", IsGrantable = false, IsGrantableByRecordId = false, IsVisible = false, GrantId = null, Category = null });
            items.Add(new SystemActions { Id = 216001, ObjectId = 216, Code = "AddStandartSendListContent", API = "", Description = "Добавить содержание типового списка рассылки", IsGrantable = false, IsGrantableByRecordId = false, IsVisible = false, GrantId = null, Category = null });
            items.Add(new SystemActions { Id = 216005, ObjectId = 216, Code = "ModifyStandartSendListContent", API = "", Description = "Изменить содержание типового списка рассылки", IsGrantable = false, IsGrantableByRecordId = false, IsVisible = false, GrantId = null, Category = null });
            items.Add(new SystemActions { Id = 216009, ObjectId = 216, Code = "DeleteStandartSendListContent", API = "", Description = "Удалить содержание типового списка рассылки", IsGrantable = false, IsGrantableByRecordId = false, IsVisible = false, GrantId = null, Category = null });
            items.Add(new SystemActions { Id = 217001, ObjectId = 217, Code = "AddStandartSendList", API = "", Description = "Добавить типовой список рассылки", IsGrantable = false, IsGrantableByRecordId = false, IsVisible = false, GrantId = null, Category = null });
            items.Add(new SystemActions { Id = 217005, ObjectId = 217, Code = "ModifyStandartSendList", API = "", Description = "Изменить типовой список рассылки", IsGrantable = false, IsGrantableByRecordId = false, IsVisible = false, GrantId = null, Category = null });
            items.Add(new SystemActions { Id = 217009, ObjectId = 217, Code = "DeleteStandartSendList", API = "", Description = "Удалить типовой список рассылки", IsGrantable = false, IsGrantableByRecordId = false, IsVisible = false, GrantId = null, Category = null });
            items.Add(new SystemActions { Id = 218001, ObjectId = 218, Code = "AddCompany", API = "", Description = "Добавить компанию", IsGrantable = false, IsGrantableByRecordId = false, IsVisible = false, GrantId = null, Category = null });
            items.Add(new SystemActions { Id = 218005, ObjectId = 218, Code = "ModifyCompany", API = "", Description = "Изменить компанию", IsGrantable = false, IsGrantableByRecordId = false, IsVisible = false, GrantId = null, Category = null });
            items.Add(new SystemActions { Id = 218009, ObjectId = 218, Code = "DeleteCompany", API = "", Description = "Удалить компанию", IsGrantable = false, IsGrantableByRecordId = false, IsVisible = false, GrantId = null, Category = null });
            items.Add(new SystemActions { Id = 219001, ObjectId = 219, Code = "AddExecutorType", API = "", Description = "Добавить тип исполнителя", IsGrantable = false, IsGrantableByRecordId = false, IsVisible = false, GrantId = null, Category = null });
            items.Add(new SystemActions { Id = 219005, ObjectId = 219, Code = "ModifyExecutorType", API = "", Description = "Изменить тип исполнителя", IsGrantable = false, IsGrantableByRecordId = false, IsVisible = false, GrantId = null, Category = null });
            items.Add(new SystemActions { Id = 219009, ObjectId = 219, Code = "DeleteExecutorType", API = "", Description = "Удалить тип исполнителя", IsGrantable = false, IsGrantableByRecordId = false, IsVisible = false, GrantId = null, Category = null });
            items.Add(new SystemActions { Id = 220001, ObjectId = 220, Code = "AddExecutor", API = "", Description = "Добавить исполнителя", IsGrantable = false, IsGrantableByRecordId = false, IsVisible = false, GrantId = null, Category = null });
            items.Add(new SystemActions { Id = 220005, ObjectId = 220, Code = "ModifyExecutor", API = "", Description = "Изменить исполнителя", IsGrantable = false, IsGrantableByRecordId = false, IsVisible = false, GrantId = null, Category = null });
            items.Add(new SystemActions { Id = 220009, ObjectId = 220, Code = "DeleteExecutor", API = "", Description = "Удалить исполнителя", IsGrantable = false, IsGrantableByRecordId = false, IsVisible = false, GrantId = null, Category = null });
            items.Add(new SystemActions { Id = 251001, ObjectId = 251, Code = "AddTemplateDocument", API = "", Description = "Добавить шаблон документа", IsGrantable = false, IsGrantableByRecordId = false, IsVisible = false, GrantId = null, Category = null });
            items.Add(new SystemActions { Id = 251005, ObjectId = 251, Code = "ModifyTemplateDocument", API = "", Description = "Изменить шаблон документа", IsGrantable = false, IsGrantableByRecordId = false, IsVisible = false, GrantId = null, Category = null });
            items.Add(new SystemActions { Id = 251009, ObjectId = 251, Code = "DeleteTemplateDocument", API = "", Description = "Удалить шаблон документа", IsGrantable = false, IsGrantableByRecordId = false, IsVisible = false, GrantId = null, Category = null });
            items.Add(new SystemActions { Id = 252001, ObjectId = 252, Code = "AddTemplateDocumentSendList", API = "", Description = "Добавить список рассылки в шаблон", IsGrantable = false, IsGrantableByRecordId = false, IsVisible = false, GrantId = null, Category = null });
            items.Add(new SystemActions { Id = 252005, ObjectId = 252, Code = "ModifyTemplateDocumentSendList", API = "", Description = "Изменить список рассылки в шаблоне", IsGrantable = false, IsGrantableByRecordId = false, IsVisible = false, GrantId = null, Category = null });
            items.Add(new SystemActions { Id = 252009, ObjectId = 252, Code = "DeleteTemplateDocumentSendList", API = "", Description = "Удалить список рассылки в шаблоне", IsGrantable = false, IsGrantableByRecordId = false, IsVisible = false, GrantId = null, Category = null });
            items.Add(new SystemActions { Id = 253001, ObjectId = 253, Code = "AddTemplateDocumentRestrictedSendList", API = "", Description = "Добавить ограничительный список рассылки в шаблон", IsGrantable = false, IsGrantableByRecordId = false, IsVisible = false, GrantId = null, Category = null });
            items.Add(new SystemActions { Id = 253005, ObjectId = 253, Code = "ModifyTemplateDocumentRestrictedSendList", API = "", Description = "Изменить ограничительный список рассылки в шаблоне", IsGrantable = false, IsGrantableByRecordId = false, IsVisible = false, GrantId = null, Category = null });
            items.Add(new SystemActions { Id = 253009, ObjectId = 253, Code = "DeleteTemplateDocumentRestrictedSendList", API = "", Description = "Удалить ограничительный список рассылки в шаблоне", IsGrantable = false, IsGrantableByRecordId = false, IsVisible = false, GrantId = null, Category = null });
            items.Add(new SystemActions { Id = 254001, ObjectId = 254, Code = "AddTemplateDocumentTask", API = "", Description = "Добавить задачу в шаблон", IsGrantable = false, IsGrantableByRecordId = false, IsVisible = false, GrantId = null, Category = null });
            items.Add(new SystemActions { Id = 254005, ObjectId = 254, Code = "ModifyTemplateDocumentTask", API = "", Description = "Изменить задачу в шаблоне", IsGrantable = false, IsGrantableByRecordId = false, IsVisible = false, GrantId = null, Category = null });
            items.Add(new SystemActions { Id = 254009, ObjectId = 254, Code = "DeleteTemplateDocumentTask", API = "", Description = "Удалить задачу в шаблоне", IsGrantable = false, IsGrantableByRecordId = false, IsVisible = false, GrantId = null, Category = null });
            items.Add(new SystemActions { Id = 255001, ObjectId = 255, Code = "AddTemplateAttachedFile", API = "", Description = "Добавить файл в шаблон", IsGrantable = false, IsGrantableByRecordId = false, IsVisible = false, GrantId = null, Category = null });
            items.Add(new SystemActions { Id = 255005, ObjectId = 255, Code = "ModifyTemplateAttachedFile", API = "", Description = "Изменить файл в шаблоне", IsGrantable = false, IsGrantableByRecordId = false, IsVisible = false, GrantId = null, Category = null });
            items.Add(new SystemActions { Id = 255009, ObjectId = 255, Code = "DeleteTemplateAttachedFile", API = "", Description = "Удалить файл в шаблоне", IsGrantable = false, IsGrantableByRecordId = false, IsVisible = false, GrantId = null, Category = null });
            items.Add(new SystemActions { Id = 291001, ObjectId = 291, Code = "AddTag", API = "", Description = "Добавить тэг", IsGrantable = false, IsGrantableByRecordId = false, IsVisible = false, GrantId = null, Category = null });
            items.Add(new SystemActions { Id = 291005, ObjectId = 291, Code = "ModifyTag", API = "", Description = "Изменить тэг", IsGrantable = false, IsGrantableByRecordId = false, IsVisible = false, GrantId = null, Category = null });
            items.Add(new SystemActions { Id = 291009, ObjectId = 291, Code = "DeleteTag", API = "", Description = "Удалить тэг", IsGrantable = false, IsGrantableByRecordId = false, IsVisible = false, GrantId = null, Category = null });
            items.Add(new SystemActions { Id = 301001, ObjectId = 301, Code = "AddCustomDictionaryType", API = "", Description = "Добавить тип пользовательского словаря", IsGrantable = false, IsGrantableByRecordId = false, IsVisible = false, GrantId = null, Category = null });
            items.Add(new SystemActions { Id = 301005, ObjectId = 301, Code = "ModifyCustomDictionaryType", API = "", Description = "Изменить тип пользовательского словаря", IsGrantable = false, IsGrantableByRecordId = false, IsVisible = false, GrantId = null, Category = null });
            items.Add(new SystemActions { Id = 301009, ObjectId = 301, Code = "DeleteCustomDictionaryType", API = "", Description = "Удалить тип пользовательского словаря", IsGrantable = false, IsGrantableByRecordId = false, IsVisible = false, GrantId = null, Category = null });
            items.Add(new SystemActions { Id = 302001, ObjectId = 302, Code = "AddCustomDictionary", API = "", Description = "Добавить запись пользовательского словаря", IsGrantable = false, IsGrantableByRecordId = false, IsVisible = false, GrantId = null, Category = null });
            items.Add(new SystemActions { Id = 302005, ObjectId = 302, Code = "ModifyCustomDictionary", API = "", Description = "Изменить запись пользовательского словаря", IsGrantable = false, IsGrantableByRecordId = false, IsVisible = false, GrantId = null, Category = null });
            items.Add(new SystemActions { Id = 302009, ObjectId = 302, Code = "DeleteCustomDictionary", API = "", Description = "Удалить запись пользовательского словаря", IsGrantable = false, IsGrantableByRecordId = false, IsVisible = false, GrantId = null, Category = null });
            items.Add(new SystemActions { Id = 311001, ObjectId = 311, Code = "AddProperty", API = "", Description = "Добавить динамический аттрибут", IsGrantable = false, IsGrantableByRecordId = false, IsVisible = false, GrantId = null, Category = null });
            items.Add(new SystemActions { Id = 311005, ObjectId = 311, Code = "ModifyProperty", API = "", Description = "Изменить динамический аттрибут", IsGrantable = false, IsGrantableByRecordId = false, IsVisible = false, GrantId = null, Category = null });
            items.Add(new SystemActions { Id = 311009, ObjectId = 311, Code = "DeleteProperty", API = "", Description = "Удалить динамический аттрибут", IsGrantable = false, IsGrantableByRecordId = false, IsVisible = false, GrantId = null, Category = null });
            items.Add(new SystemActions { Id = 312001, ObjectId = 312, Code = "AddPropertyLink", API = "", Description = "Добавить связь динамических аттрибутов", IsGrantable = false, IsGrantableByRecordId = false, IsVisible = false, GrantId = null, Category = null });
            items.Add(new SystemActions { Id = 312005, ObjectId = 312, Code = "ModifyPropertyLink", API = "", Description = "Изменить связь динамических аттрибутов", IsGrantable = false, IsGrantableByRecordId = false, IsVisible = false, GrantId = null, Category = null });
            items.Add(new SystemActions { Id = 312009, ObjectId = 312, Code = "DeletePropertyLink", API = "", Description = "Удалить связь динамических аттрибутов", IsGrantable = false, IsGrantableByRecordId = false, IsVisible = false, GrantId = null, Category = null });
            items.Add(new SystemActions { Id = 313005, ObjectId = 313, Code = "ModifyPropertyValues", API = "", Description = "Изменить значение динамических аттрибутов", IsGrantable = false, IsGrantableByRecordId = false, IsVisible = false, GrantId = null, Category = null });
            items.Add(new SystemActions { Id = 401001, ObjectId = 401, Code = "AddEncryptionCertificate", API = "", Description = "Добавить сертификат", IsGrantable = false, IsGrantableByRecordId = false, IsVisible = false, GrantId = null, Category = null });
            items.Add(new SystemActions { Id = 401002, ObjectId = 401, Code = "ModifyEncryptionCertificate", API = "", Description = "Изменить сертификат", IsGrantable = false, IsGrantableByRecordId = false, IsVisible = false, GrantId = null, Category = null });
            items.Add(new SystemActions { Id = 401003, ObjectId = 401, Code = "ExportEncryptionCertificate", API = "", Description = "Экспорт сертификата", IsGrantable = false, IsGrantableByRecordId = false, IsVisible = false, GrantId = null, Category = null });
            items.Add(new SystemActions { Id = 401004, ObjectId = 401, Code = "DeleteEncryptionCertificate", API = "", Description = "Удалить сертификат", IsGrantable = false, IsGrantableByRecordId = false, IsVisible = false, GrantId = null, Category = null });
            items.Add(new SystemActions { Id = 401005, ObjectId = 401, Code = "GenerateKeyEncryptionCertificate", API = "", Description = "Сгенерировать сертификат", IsGrantable = false, IsGrantableByRecordId = false, IsVisible = false, GrantId = null, Category = null });
            items.Add(new SystemActions { Id = 402001, ObjectId = 402, Code = "AddEncryptionCertificateType", API = "", Description = "Добавить тип сертификат", IsGrantable = false, IsGrantableByRecordId = false, IsVisible = false, GrantId = null, Category = null });
            items.Add(new SystemActions { Id = 402002, ObjectId = 402, Code = "ModifyEncryptionCertificateType", API = "", Description = "Изменить тип сертификат", IsGrantable = false, IsGrantableByRecordId = false, IsVisible = false, GrantId = null, Category = null });
            items.Add(new SystemActions { Id = 402003, ObjectId = 402, Code = "DeleteEncryptionCertificateType", API = "", Description = "Удалить тип сертификат", IsGrantable = false, IsGrantableByRecordId = false, IsVisible = false, GrantId = null, Category = null });

            return items;
        }

        private List<SystemUIElements> GetSystemUIElements()
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

        private List<SystemValueTypes> GetSystemValueTypes()
        {
            var items = new List<SystemValueTypes>();

            items.Add(new SystemValueTypes { Id = 1, Code = "text", Description = "text" });
            items.Add(new SystemValueTypes { Id = 2, Code = "number", Description = "number" });
            items.Add(new SystemValueTypes { Id = 3, Code = "date", Description = "date" });

            return items;
        }

        private List<EncryptionCertificateTypes> GetEncryptionCertificateTypes()
        {
            var items = new List<EncryptionCertificateTypes>();

            items.Add(new EncryptionCertificateTypes { Id = 1, Code = "RSA", Name = "RSA", LastChangeUserId = (int)EnumSystemUsers.AdminUser, LastChangeDate = DateTime.Now });
            items.Add(new EncryptionCertificateTypes { Id = 2, Code = "X509", Name = "X509", LastChangeUserId = (int)EnumSystemUsers.AdminUser, LastChangeDate = DateTime.Now });

            return items;
        }

        private List<DictionaryDocumentDirections> GetDictionaryDocumentDirections()
        {
            var items = new List<DictionaryDocumentDirections>();

            items.Add(new DictionaryDocumentDirections { Id = 1, Code = "1", Name = "##l@DictionaryDocumentDirections:Incoming@l##", LastChangeUserId = (int)EnumSystemUsers.AdminUser, LastChangeDate = DateTime.Now });
            items.Add(new DictionaryDocumentDirections { Id = 2, Code = "2", Name = "##l@DictionaryDocumentDirections:Outcoming@l##", LastChangeUserId = (int)EnumSystemUsers.AdminUser, LastChangeDate = DateTime.Now });
            items.Add(new DictionaryDocumentDirections { Id = 3, Code = "3", Name = "##l@DictionaryDocumentDirections:Internal@l##", LastChangeUserId = (int)EnumSystemUsers.AdminUser, LastChangeDate = DateTime.Now });

            return items;
        }

        private List<DictionaryEventTypes> GetDictionaryEventTypes()
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

        private List<DictionaryImportanceEventTypes> GetDictionaryImportanceEventTypes()
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

        private List<DictionaryResultTypes> GetDictionaryResultTypes()
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

        private List<DictionarySendTypes> GetDictionarySendTypes()
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

        private List<DictionarySubordinationTypes> GetDictionarySubordinationTypes()
        {
            var items = new List<DictionarySubordinationTypes>();

            items.Add(new DictionarySubordinationTypes { Id = 1, Code = "Informing", Name = "##l@DictionarySubordinationTypes:Informing@l##", LastChangeUserId = (int)EnumSystemUsers.AdminUser, LastChangeDate = DateTime.Now });
            items.Add(new DictionarySubordinationTypes { Id = 2, Code = "Execution", Name = "##l@DictionarySubordinationTypes:Execution@l##", LastChangeUserId = (int)EnumSystemUsers.AdminUser, LastChangeDate = DateTime.Now });

            return items;
        }

        private List<DictionarySubscriptionStates> GetDictionarySubscriptionStates()
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
        private List<DictionaryPositionExecutorTypes> GetDictionaryPositionExecutorTypes()
        {
            var items = new List<DictionaryPositionExecutorTypes>();

            items.Add(new DictionaryPositionExecutorTypes { Id = 4, Code = "Personal", Name = "Назначен на должность", IsActive = true, LastChangeUserId = (int)EnumSystemUsers.AdminUser, LastChangeDate = DateTime.Now });
            items.Add(new DictionaryPositionExecutorTypes { Id = 5, Code = "Referent", Name = "Является референтом", IsActive = true, LastChangeUserId = (int)EnumSystemUsers.AdminUser, LastChangeDate = DateTime.Now });
            items.Add(new DictionaryPositionExecutorTypes { Id = 6, Code = "IO", Name = "Исполяет обязанности", IsActive = true, LastChangeUserId = (int)EnumSystemUsers.AdminUser, LastChangeDate = DateTime.Now });

            return items;
        }

        private List<DictionaryLinkTypes> GetDictionaryLinkTypes()
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
