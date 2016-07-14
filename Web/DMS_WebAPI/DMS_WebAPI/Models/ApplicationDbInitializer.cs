using DMS_WebAPI.DBModel;
using System.Collections.Generic;
using System.Data.Entity;

namespace DMS_WebAPI.Models
{
    public class ApplicationDbInitializer : CreateDatabaseIfNotExists<ApplicationDbContext>
    {
        protected override void Seed(ApplicationDbContext context)
        {
            context.AspNetLicencesSet.AddRange(GetAspNetLicences());

            context.AdminLanguagesSet.AddRange(GetAdminLanguages());

            context.AdminLanguageValuesSet.AddRange(GetAdminLanguageValues());

            base.Seed(context);
        }

        private List<AspNetLicences> GetAspNetLicences()
        {
            var items = new List<AspNetLicences>();

            //items.Add(new AspNetLicences { Id = 0, Name = "", Description = "", NamedNumberOfConnections = null, ConcurenteNumberOfConnections = null, DurationDay = null, Functionals = null, IsActive = true });
            items.Add(new AspNetLicences { Id = 1, Name = "Base licence", Description = "", NamedNumberOfConnections = null, ConcurenteNumberOfConnections = 10, DurationDay = 365, Functionals = null, IsActive = true });
            items.Add(new AspNetLicences { Id = 2, Name = "Small business licence", Description = "", NamedNumberOfConnections = null, ConcurenteNumberOfConnections = 50, DurationDay = 365, Functionals = null, IsActive = true });
            items.Add(new AspNetLicences { Id = 3, Name = "Fixed Name business", Description = "", NamedNumberOfConnections = null, ConcurenteNumberOfConnections = null, DurationDay = 365, Functionals = null, IsActive = true });
            items.Add(new AspNetLicences { Id = 4, Name = "Unlimited", Description = "", NamedNumberOfConnections = 50, ConcurenteNumberOfConnections = null, DurationDay = null, Functionals = null, IsActive = true });

            return items;
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
    }
}
