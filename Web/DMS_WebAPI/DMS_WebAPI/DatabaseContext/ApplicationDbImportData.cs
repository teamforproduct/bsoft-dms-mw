using BL.Model.Enums;
using DMS_WebAPI.DBModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DMS_WebAPI.Models
{
    public static class ApplicationDbImportData
    {

        private static int IdSequence = 0;

        #region [+] Languages ...

        public static List<AdminLanguages> GetAdminLanguages()
        {
            var items = new List<AdminLanguages>();

            items.Add(new AdminLanguages { Id = (int)EnumLanguages.ru, Code = EnumLanguages.ru.ToString(), Name = "Русский", IsDefault = (int)EnumLanguages.ru == (int)EnumSystemLanguageId.LanguageId });
            items.Add(new AdminLanguages { Id = (int)EnumLanguages.en, Code = EnumLanguages.en.ToString(), Name = "English", IsDefault = (int)EnumLanguages.en == (int)EnumSystemLanguageId.LanguageId });

            return items;
        }

        private static void AddALV(List<AdminLanguageValues> list, string label, string EngLang, string RusLang)
        {
            list.Add(new AdminLanguageValues()
            {
                Id = ++IdSequence,
                LanguageId = 1,
                Label = label,
                Value = RusLang
            });
            list.Add(new AdminLanguageValues()
            {
                Id = ++IdSequence,
                LanguageId = 2,
                Label = label,
                Value = EngLang
            });
        }

        public static List<AdminLanguageValues> GetAdminLanguageValues()
        {
            IdSequence = 0;

            var list = new List<AdminLanguageValues>();

            list.AddRange(GetAdminLanguageValuesForObjects());
            list.AddRange(GetAdminLanguageValuesForActions());

            AddALV(list, "##l@DmsExceptions:IncomingModelIsNotValid@l##", "Incoming Model is not valid! {0}", "Входящая модель недействительна! {0}");
            AddALV(list, "##l@DmsExceptions:WrongParameterTypeError@l##", "Parameter type commands is incorrect!", "Тип параметра комманды указан неверно!");
            AddALV(list, "##l@DmsExceptions:WrongParameterValueError@l##", "Parameters commands incorrect!", "Параметры комманды неверные!");
            AddALV(list, "##l@DmsExceptions:UserUnauthorized@l##", "Authorization has been denied for this request.", "Пользователь не авторизован");
            AddALV(list, "##l@DmsExceptions:RecordNotUnique@l##", "Record is not Unique", "Запись не уникальна");

            AddALV(list, "##l@System@l##", "System", "Система");
            AddALV(list, "##l@DictionaryDocumentDirections:Incoming@l##", "Incoming", "Входящий");
            AddALV(list, "##l@DictionaryDocumentDirections:Internal@l##", "Internal", "Собственный");
            AddALV(list, "##l@DictionaryDocumentDirections:Outcoming@l##", "Outcoming", "Иcходящий");
            AddALV(list, "##l@DictionaryImportanceEventTypes:AdditionalEvents@l##", "Secondary events", "Второстепенные события");
            AddALV(list, "##l@DictionaryImportanceEventTypes:DocumentMoovement@l##", "Facts movement documents", "Факты движения документов");
            AddALV(list, "##l@DictionaryImportanceEventTypes:ImportantEvents@l##", "Important events", "Важные события");
            AddALV(list, "##l@DictionaryImportanceEventTypes:Internal@l##", "Own notes", "Собственные примечания");
            AddALV(list, "##l@DictionaryImportanceEventTypes:Message@l##", "Messages", "Сообщения");
            AddALV(list, "##l@DictionaryImportanceEventTypes:PaperMoovement@l##", "Paper movement", "Движение БН");
            AddALV(list, "##l@DictionarySendTypes:SendForConsideration@l##", "For consideration", "Для рассмотрения");
            AddALV(list, "##l@DictionarySendTypes:SendForControl@l##", "In control", "На контроль(отв.исп.)");
            AddALV(list, "##l@DictionarySendTypes:SendForExecution@l##", "For execution", "Соисполнителю");
            AddALV(list, "##l@DictionarySendTypes:SendForInformation@l##", "For information", "Для сведения");
            AddALV(list, "##l@DictionarySendTypes:SendForInformationExternal@l##", "For information external agents", "Для сведения внешнему агенту");
            AddALV(list, "##l@DictionarySendTypes:SendForResponsibleExecution@l##", "ResponsibleExecution", "Исполненителю(отв.исп.)");
            AddALV(list, "##l@DictionarySendTypes:SendForSigning@l##", "For signing", "На подпись");
            AddALV(list, "##l@DictionarySendTypes:SendForVisaing@l##", "For visaing", "На визирование");
            AddALV(list, "##l@DictionarySendTypes:SendForАgreement@l##", "For agreement", "На согласование");
            AddALV(list, "##l@DictionarySendTypes:SendForАpproval@l##", "For approval", "На утверждение");
            AddALV(list, "##l@DictionarySubordinationTypes:Execution@l##", "Execution", "Исполнение");
            AddALV(list, "##l@DictionarySubordinationTypes:Informing@l##", "Informing", "Информирование");
            AddALV(list, "##l@DictionarySubscriptionStates:No@l##", "No", "Нет");
            AddALV(list, "##l@DictionarySubscriptionStates:Sign@l##", "Sign", "Подпись");
            AddALV(list, "##l@DictionarySubscriptionStates:Violated@l##", "Violated", "Нарушена");
            AddALV(list, "##l@DictionarySubscriptionStates:Visa@l##", "Visa", "Виза");
            AddALV(list, "##l@DictionarySubscriptionStates:Аgreement@l##", "Аgreement", "Согласование");
            AddALV(list, "##l@DictionarySubscriptionStates:Аpproval@l##", "Аpproval", "Утверждение");

            AddALV(list, "##l@DmsExceptions:AccessIsDenied@l##", "Access is Denied! Object: {0} Action: {1}", "Отказано в доступе! Объект: {0} Действие: {1}");
            AddALV(list, "##l@DmsExceptions:CannotAccessToFile@l##", "Cannot access to user file!", "Файл пользователя не доступен!");
            AddALV(list, "##l@DmsExceptions:CannotSaveFile@l##", "Error when save user file!", "Ошибка при сохранения файла пользователя!");
            AddALV(list, "##l@DmsExceptions:ClientIsNotFound@l##", "Client not found", "Клиент не найден");
            AddALV(list, "##l@DmsExceptions:ClientNameAlreadyExists@l##", "Client Name already exists", "Имя клиента уже существует");
            AddALV(list, "##l@DmsExceptions:ClientCodeAlreadyExists@l##", "Domain \"{0}\" already exists", "Доменное имя \"{0}\" уже занято");
            AddALV(list, "##l@DmsExceptions:ClientVerificationCodeIncorrect@l##", "Verification code is invalid", "Проверочный код неверен");
            AddALV(list, "##l@DmsExceptions:CommandNotDefinedError@l##", "The desired command for \"{0}\" not found", "Команда для \"{0}\" не найдена");
            AddALV(list, "##l@DmsExceptions:CouldNotChangeAttributeLaunchPlan@l##", "Couldn\"t change attribute LaunchPlan", "Невозможно изменить атрибут LaunchPlan");
            AddALV(list, "##l@DmsExceptions:CouldNotChangeFavourite@l##", "Couldn\"t change attribute Favourite", "Невозможно изменить атрибут Favourite");
            AddALV(list, "##l@DmsExceptions:CouldNotChangeIsInWork@l##", "Couldn\"t change attribute IsInWork", "Невозможно изменить атрибут IsInWork");
            AddALV(list, "##l@DmsExceptions:CouldNotPerformThisOperation@l##", "Could Not Perform This Operation!", "Операция не выполнена!");
            AddALV(list, "##l@DmsExceptions:CryptographicError@l##", "Encryption Error", "Ошибка шифрования");
            AddALV(list, "##l@DmsExceptions:DatabaseError@l##", "An error occurred while accessing the database!", "Ошибка при обращении к базе данных!");
            AddALV(list, "##l@DmsExceptions:DatabaseIsNotFound@l##", "Database not found", "База данных не найдена");
            AddALV(list, "##l@DmsExceptions:DatabaseIsNotSet@l##", "The database is not set", "База данных не установлена");

            AddALV(list, "##l@DmsExceptions:AdminRecordNotUnique@l##", "Setting record should be unique!", "Настроечная запись должена быть уникальна!");
            AddALV(list, "##l@DmsExceptions:AdminRecordCouldNotBeAdded@l##", "You could not add this setting data!", "Вы не можете добавить настроечные данные");
            AddALV(list, "##l@DmsExceptions:AdminRecordCouldNotBeDeleted@l##", "You could not delete from this dictionary data!", "Вы не можете удалить настроечные данные");
            AddALV(list, "##l@DmsExceptions:AdminRecordWasNotFound@l##", "Dictionary record was not found!", "Элемент справочника не найден!");

            AddALV(list, "##l@DmsExceptions:DictionaryAddressTypeCodeNotUnique@l##", "", "");
            AddALV(list, "##l@DmsExceptions:DictionaryAddressTypeNameNotUnique@l##", "", "");
            AddALV(list, "##l@DmsExceptions:DictionaryContactTypeCodeNotUnique@l##", "", "");
            AddALV(list, "##l@DmsExceptions:DictionaryContactTypeNameNotUnique@l##", "", "");

            AddALV(list, "##l@DmsExceptions:DictionaryAgentBankMFOCodeNotUnique@l##", "Bank \"{0}\" MFO \"{1}\" сode should be unique!", "Банк \"{0}\" c МФО \"{1}\" уже есть в справочнике");
            AddALV(list, "##l@DmsExceptions:DictionaryAgentCompanyOKPOCodeNotUnique@l##", "Company \"{0}\" OKPO сode should be unique!", "Юридическое лицо с указанным ОКПО уже есть в справочнике");
            AddALV(list, "##l@DmsExceptions:DictionaryAgentCompanyTaxCodeNotUnique@l##", "Company \"{0}\" tax сode should be unique!", "Юридическое лицо с указанным ИНН уже есть в справочнике");
            AddALV(list, "##l@DmsExceptions:DictionaryAgentCompanyVATCodeNotUnique@l##", "Company \"{0}\" VAT сode should be unique!", "Юридическое лицо с указанным номером свидетельства НДС уже есть в справочнике");
            AddALV(list, "##l@DmsExceptions:DictionaryAgentEmployeePassportNotUnique@l##", "Employee \"{0}\" passport should be unique!", "Сотрудник с указанными паспортными данными уже есть в справочнике");
            AddALV(list, "##l@DmsExceptions:DictionaryAgentEmployeePersonnelNumberNotUnique@l##", "Employee \"{0}\" personnel number should be unique!", "Сотрудник с указанным табельным номером уже есть в справочнике");
            AddALV(list, "##l@DmsExceptions:DictionaryAgentEmployeeTaxCodeNotUnique@l##", "Employee \"{0}\" tax code should be unique!", "Сотрудник с указанным ИНН уже есть в справочнике");
            AddALV(list, "##l@DmsExceptions:DictionaryAgentNameNotUnique@l##", "Agent name \"{0}\" should be unique!", "Агент \"{0}\" уже есть в справочнике");
            AddALV(list, "##l@DmsExceptions:DictionaryAgentPersonPassportNotUnique@l##", "Person \"{0}\" passport should be unique!", "Физлицо с указанными паспортными данными уже есть в справочнике");
            AddALV(list, "##l@DmsExceptions:DictionaryAgentPersonTaxCodeNotUnique@l##", "Person \"{0}\" tax code should be unique!", "Физлицо с указанным ИНН уже есть в справочнике");
            AddALV(list, "##l@DmsExceptions:DictionaryAgentContactTypeNotUnique@l##", "Agent contact type should be unique!", "Контакт с указанным типом уже есть у этого агента");
            AddALV(list, "##l@DmsExceptions:DictionaryAgentContactNotUnique@l##", "Agent contact should be unique!", "Указанный контакт уже есть у этого агента");
            AddALV(list, "##l@DmsExceptions:DictionaryAgentAddressNameNotUnique@l##", "", "");
            AddALV(list, "##l@DmsExceptions:DictionaryAgentAddressTypeNotUnique@l##", "", "");
            AddALV(list, "##l@DmsExceptions:DictionaryAgentAccountNumberNotUnique@l##", "", "");

            AddALV(list, "##l@DmsExceptions:DictionaryCostomDictionaryNotUnique@l##", "", "");
            AddALV(list, "##l@DmsExceptions:DictionaryCostomDictionaryTypeNotUnique@l##", "", "");
            AddALV(list, "##l@DmsExceptions:DictionarysdDepartmentNotBeSubordinated@l##", "", "");
            AddALV(list, "##l@DmsExceptions:DictionaryDepartmentNameNotUnique@l##", "", "");
            AddALV(list, "##l@DmsExceptions:DictionaryDocumentSubjectNameNotUnique@l##", "", "");
            AddALV(list, "##l@DmsExceptions:DictionaryDocumentTypeNameNotUnique@l##", "", "");
            AddALV(list, "##l@DmsExceptions:DictionaryPositionExecutorIsInvalidPeriod@l##", "", "Период исполнения задан неверно!");
            AddALV(list, "##l@DmsExceptions:DictionaryPositionExecutorNotUnique@l##", "", "Сотрудник \"{1}\" не может быть назначен на должность повторно \"{0}\" c {2} по {3}");
            AddALV(list, "##l@DmsExceptions:DictionaryPositionExecutorPersonalNotUnique@l##", "", "На должность \"{0}\" штатно назначен \"{1}\" c {2} по {3}");
            AddALV(list, "##l@DmsExceptions:DictionaryPositionExecutorIONotUnique@l##", "", "На должность \"{0}\" назначен исполняющий обязанности \"{1}\" c {2} по {3}");
            AddALV(list, "##l@DmsExceptions:DictionaryPositionExecutorReferentNotUnique@l##", "", "На должность \"{0}\" назначен референт \"{1}\" c {2} по {3}");
            AddALV(list, "##l@DmsExceptions:DictionaryRegistrationJournalNotUnique@l##", "", "");
            AddALV(list, "##l@DmsExceptions:DictionaryStandartSendListNotUnique@l##", "", "");
            AddALV(list, "##l@DmsExceptions:DictionaryStandartSendListContentNotUnique@l##", "", "");
            AddALV(list, "##l@DmsExceptions:DictionaryTagNotUnique@l##", "", "");



            AddALV(list, "##l@DmsExceptions:DictionaryRecordCouldNotBeAdded@l##", "You could not add this dictionary data!", "Вы не можете добавить данные в этот справочник");
            AddALV(list, "##l@DmsExceptions:DictionaryRecordCouldNotBeDeleted@l##", "You could not delete from this dictionary data!", "Вы не можете удалить данные из этого справочника");
            AddALV(list, "##l@DmsExceptions:DictionaryRecordNotUnique@l##", "Dictionary record should be unique!", "Элемент справочника должен быть уникален!");
            AddALV(list, "##l@DmsExceptions:DictionaryRecordWasNotFound@l##", "Dictionary record was not found!", "Элемент справочника не найден!");
            AddALV(list, "##l@DmsExceptions:DictionarySystemRecordCouldNotBeDeleted@l##", "Dictionary system record was not deleted!", "Невозможно удалить предустановленные записи справочника");
            AddALV(list, "##l@DmsExceptions:DictionaryTagNotFoundOrUserHasNoAccess@l##", "User could not access this tag!", "Пользователь не имеет доступа к этому тегу!");
            AddALV(list, "##l@DmsExceptions:DocumentCannotBeModifiedOrDeleted@l##", "Document cannot be Modified or Deleted!", "Документ не может быть изменен или удален!");
            AddALV(list, "##l@DmsExceptions:DocumentCouldNotBeRegistered@l##", "Document registration has non been successfull! Try again!", "Регистрационный документ не была успешной! Попробуй еще раз!");
            AddALV(list, "##l@DmsExceptions:DocumentFileWasChangedExternally@l##", "The document file has been modified from the outside", "Файл документа был изменен извне");
            AddALV(list, "##l@DmsExceptions:DocumentHasAlreadyHasLink@l##", "Document has already has link!", "Документ уже имеет ссылку!");
            AddALV(list, "##l@DmsExceptions:DocumentHasAlredyBeenRegistered@l##", "Document has already been registered!", "Документ уже зарегистрирован!");
            AddALV(list, "##l@DmsExceptions:DocumentNotFoundOrUserHasNoAccess@l##", "User could not access this document!", "Документ не доступен!");
            AddALV(list, "##l@DmsExceptions:DocumentRestrictedSendListDoesNotMatchTheTemplate@l##", "Document Restricted SendList does not match the template", "Разрешающий список рассылок для документа не соответствует шаблону");
            AddALV(list, "##l@DmsExceptions:DocumentRestrictedSendListDuplication@l##", "Duplicate Entry DocumentRestrictSendList", "Дублирование записей в разрешающем списке рассылке для документа");
            AddALV(list, "##l@DmsExceptions:DocumentSendListDoesNotMatchTheTemplate@l##", "Document SendList does not match the template", "Список рассылок для документа не соответствует шаблону");
            AddALV(list, "##l@DmsExceptions:DocumentSendListNotFoundInDocumentRestrictedSendList@l##", "DocumentSendList not found in DocumentRestrictedSendList", "Получатель не найден в разрешающем списке рассылок для документа");
            AddALV(list, "##l@DmsExceptions:EncryptionCertificatePrivateKeyСanNotBeExported@l##", "The private key can not be exported", "Приватный ключ нельзя экспортировать");
            AddALV(list, "##l@DmsExceptions:EncryptionCertificateWasNotFound@l##", "The certificate was not found", "Сертификат не был найден");
            AddALV(list, "##l@DmsExceptions:EventNotFoundOrUserHasNoAccess@l##", "User could not access this event!", "Пользователь не имеет доступа к этому событию!");
            AddALV(list, "##l@DmsExceptions:ExecutorAgentForPositionIsNotDefined@l##", "Executor agent for position is not defined!", "Исполнитель для должности не определен!");
            AddALV(list, "##l@DmsExceptions:LicenceExceededNumberOfConnectedUsers@l##", "You have exceeded the allowed number of connected users", "Превышено разрешенное количество подключенных пользователей");
            AddALV(list, "##l@DmsExceptions:LicenceExceededNumberOfRegisteredUsers@l##", "You have exceeded the allowed number of registered users", "Превышено разрешенное количество зарегистрированных пользователей");
            AddALV(list, "##l@DmsExceptions:LicenceExpired@l##", "Licence expired", "Срок лицензии истек");
            AddALV(list, "##l@DmsExceptions:LicenceInformationError@l##", "The licence is not valid", "Лицензия недействительна");
            AddALV(list, "##l@DmsExceptions:NeedInformationAboutCorrespondent@l##", "Need information about correspondent!", "Нужна информация о корреспонденте!");
            AddALV(list, "##l@DmsExceptions:NotFilledWithAdditionalRequiredAttributes@l##", "Not filled with additional required attributes!", "Не заполнены обязательные дополнительные атрибуты!");
            AddALV(list, "##l@DmsExceptions:PaperListNotFoundOrUserHasNoAccess@l##", "Paper list not found or user has no access", "Список бумага не найдена или пользователь не имеет доступа");
            AddALV(list, "##l@DmsExceptions:PaperNotFoundOrUserHasNoAccess@l##", "Paper not found or user has no access", "Бумага не найдена или пользователь не имеет доступа");
            AddALV(list, "##l@DmsExceptions:PlanPointHasAlredyBeenLaunched@l##", "Plan Point has already been Launched!", "Пункт плана уже запущен!");
            AddALV(list, "##l@DmsExceptions:TaskNotFoundOrUserHasNoAccess@l##", "Task not found", "Task не найден");
            AddALV(list, "##l@DmsExceptions:TemplateDocumentIsNotValid@l##", "The document template is not valid", "Шаблон документа не корректен");
            AddALV(list, "##l@DmsExceptions:TemplateDocumentNotFoundOrUserHasNoAccess@l##", "User could not access this template document!", "Пользователь не имеет доступ к этот шаблону документа!");
            AddALV(list, "##l@DmsExceptions:UnknownDocumentFile@l##", "Could not find appropriate document file!", "Не удалось найти соответствующий файл документа!");
            AddALV(list, "##l@DmsExceptions:UserFileNotExists@l##", "User file does not exists on Filestore!", "Пользовательский файл не существует в файловом хранилище");
            AddALV(list, "##l@DmsExceptions:UserHasNoAccessToDocument@l##", "User could not access this document!", "Пользователь не может получить доступ к этот документ!");
            AddALV(list, "##l@DmsExceptions:UserNameAlreadyExists@l##", "User Name already exists", "Имя пользователя уже существует");
            AddALV(list, "##l@DmsExceptions:UserNameIsNotDefined@l##", "Employee for the current user could not be defined!", "Сотрудник для текущего пользователя не может быть определен!");
            AddALV(list, "##l@DmsExceptions:UserPositionIsNotDefined@l##", "Position for the current user could not be defined!", "Позиция для текущего пользователя не может быть определена!");
            AddALV(list, "##l@DmsExceptions:WaitHasAlreadyClosed@l##", "Wait has already closed!", "Ожидание уже закрыто!");
            AddALV(list, "##l@DmsExceptions:WaitNotFoundOrUserHasNoAccess@l##", "User could not access this wait!", "Пользователь не имеет доступа к этим ожиданиям!");
            AddALV(list, "##l@DmsExceptions:WrongDocumentSendListEntry@l##", "Plan item is wrong.", "Некорректный пункт плана");



            //pss 23.09.2016 Выявил DmsExceptions которые не имели перевода 
            //TODO Требуется локализация (перевод ошибок)
            AddALV(list, "##l@DmsExceptions:ControlerHasAlreadyBeenDefined@l##", "Controler Has Already Been Defined", "Контролер уже определен");
            AddALV(list, "##l@DmsExceptions:CouldNotModifyTemplateDocument@l##", "", "");
            AddALV(list, "##l@DmsExceptions:CouldNotPerformOperationWithPaper@l##", "Could Not Perform Operation With Paper", "Невозможно осуществить операцию с бумажными носителями");
            AddALV(list, "##l@DmsExceptions:EncryptionCertificateHasExpired@l##", "Encryption Certificate Has Been Expired", "Сертификат просрочен");
            AddALV(list, "##l@DmsExceptions:NobodyIsChosen@l##", "NobodyIsChosen", "Никто не выбран");
            AddALV(list, "##l@DmsExceptions:ResponsibleExecutorHasAlreadyBeenDefined@l##", "Responsible Executor Has Already Been Defined", "Ответственный исполнитель уже определен");
            AddALV(list, "##l@DmsExceptions:ResponsibleExecutorIsNotDefined@l##", "Responsible Executor Is Not Defined", "Ответственный исполнитель не определен");
            AddALV(list, "##l@DmsExceptions:SigningTypeNotAllowed@l##", "Signing Type Is Not Allowed", "Недопустимый тип подписи");
            AddALV(list, "##l@DmsExceptions:SubordinationHasBeenViolated@l##", "Subordination Has Been Violated", "Нарушена субординация");
            AddALV(list, "##l@DmsExceptions:TargetIsNotDefined@l##", "Target Is Not Defined", "Получатель не определен");
            AddALV(list, "##l@DmsExceptions:TaskIsNotDefined@l##", "Task Is Not Defined", "Задача не определена");
            AddALV(list, "##l@DmsExceptions:ContriolHasNotBeenChanged@l##", "Contriol Has Not Been Changed", "Параметры контроля не изменены");

            // после добавления переводов можно обновить их в базе api/v2/Languages/RefreshLanguageValues

            return list;
        }
        public static List<AdminLanguageValues> GetAdminLanguageValuesForObjects()
        {
            var list = new List<AdminLanguageValues>();

            AddALV(list, "##l@Objects:Documents@l##", "Documents", "Документы");
            AddALV(list, "##l@Objects:DocumentAccesses@l##", "Document accesses", "Документы - доступы");
            AddALV(list, "##l@Objects:DocumentRestrictedSendLists@l##", "Document restricted send lists", "Документы - ограничения рассылки");
            AddALV(list, "##l@Objects:DocumentSendLists@l##", "Document send lists", "Документы - план работы");
            AddALV(list, "##l@Objects:DocumentFiles@l##", "Document files", "Документы - файлы");
            AddALV(list, "##l@Objects:DocumentLinks@l##", "Document links", "Документы - связи");
            AddALV(list, "##l@Objects:DocumentSendListStages@l##", "Document send list stages", "Документы - этапы плана работ");
            AddALV(list, "##l@Objects:DocumentEvents@l##", "Document events", "Документы - события");
            AddALV(list, "##l@Objects:DocumentWaits@l##", "Document waits", "Документы - ожидания");
            AddALV(list, "##l@Objects:DocumentSubscriptions@l##", "Document subscriptions", "Документы - подписи");
            AddALV(list, "##l@Objects:DocumentTasks@l##", "Document tasks", "Документы - задачи");
            AddALV(list, "##l@Objects:DocumentPapers@l##", "Document papers", "Документы - бумажные носители");
            AddALV(list, "##l@Objects:DocumentPaperEvents@l##", "Document paper events", "Документы - события по бумажным носителям");
            AddALV(list, "##l@Objects:DocumentPaperLists@l##", "Document paper lists", "Документы - реестры передачи бумажных носителей");
            AddALV(list, "##l@Objects:DocumentSavedFilters@l##", "Document saved filters", "Документы - сохраненные фильтры");
            AddALV(list, "##l@Objects:DocumentTags@l##", "Document tags", "Документы - тэги");
            AddALV(list, "##l@Objects:DictionaryDocumentType@l##", "Dictionary document type", "Типы документов");
            AddALV(list, "##l@Objects:DictionaryAddressType@l##", "Dictionary address type", "Типы адресов");
            AddALV(list, "##l@Objects:DictionaryDocumentSubjects@l##", "Dictionary document subjects", "Тематики документов");
            AddALV(list, "##l@Objects:DictionaryRegistrationJournals@l##", "Dictionary registration journals", "Журналы регистрации");
            AddALV(list, "##l@Objects:DictionaryContactType@l##", "Dictionary contact type", "Типы контактов");
            AddALV(list, "##l@Objects:DictionaryAgents@l##", "Dictionary agents", "Контрагенты");
            AddALV(list, "##l@Objects:DictionaryContacts@l##", "Dictionary contacts", "Контакты");
            AddALV(list, "##l@Objects:DictionaryAgentAddresses@l##", "Dictionary agent addresses", "Адреса");
            AddALV(list, "##l@Objects:DictionaryAgentPersons@l##", "Dictionary agent persons", "Физические лица");
            AddALV(list, "##l@Objects:DictionaryDepartments@l##", "Dictionary departments", "Структура предприятия");
            AddALV(list, "##l@Objects:DictionaryPositions@l##", "Dictionary positions", "Штатное расписание");
            AddALV(list, "##l@Objects:DictionaryAgentEmployees@l##", "Dictionary agent employees", "Сотрудники");
            AddALV(list, "##l@Objects:DictionaryAgentCompanies@l##", "Dictionary agent companies", "Юридические лица");
            AddALV(list, "##l@Objects:DictionaryAgentBanks@l##", "Dictionary agent banks", "Контрагенты - банки");
            AddALV(list, "##l@Objects:DictionaryAgentAccounts@l##", "Dictionary agent accounts", "Расчетные счета");
            AddALV(list, "##l@Objects:DictionaryStandartSendListContent@l##", "Dictionary standart send list content", "Типовые списки рассылки (содержание)");
            AddALV(list, "##l@Objects:DictionaryStandartSendLists@l##", "Dictionary standart send lists", "Типовые списки рассылки");
            AddALV(list, "##l@Objects:DictionaryAgentClientCompanies@l##", "Dictionary agent client companies", "Компании");
            AddALV(list, "##l@Objects:DictionaryPositionExecutorTypes@l##", "Dictionary position executor types", "Типы исполнителей");
            AddALV(list, "##l@Objects:DictionaryPositionExecutors@l##", "Dictionary position executors", "Исполнители должности");
            AddALV(list, "##l@Objects:TemplateDocument@l##", "Template document", "Шаблоны документов");
            AddALV(list, "##l@Objects:TemplateDocumentSendList@l##", "Template document send list", "Списки рассылки в шаблонах");
            AddALV(list, "##l@Objects:TemplateDocumentRestrictedSendList@l##", "Template document restricted send list", "Ограничительные списки рассылки в шаблонах");
            AddALV(list, "##l@Objects:TemplateDocumentTask@l##", "Template document task", "Задачи в шаблонах");
            AddALV(list, "##l@Objects:TemplateDocumentAttachedFiles@l##", "Template document attached files", "Прикрепленные к шаблонам файлы");
            AddALV(list, "##l@Objects:DictionaryTag@l##", "Dictionary tag", "Теги");
            AddALV(list, "##l@Objects:CustomDictionaryTypes@l##", "Custom dictionary types", "Типы пользовательских словарей");
            AddALV(list, "##l@Objects:CustomDictionaries@l##", "Custom dictionaries", "Пользовательские словари");
            AddALV(list, "##l@Objects:Properties@l##", "Properties", "Динамические аттрибуты");
            AddALV(list, "##l@Objects:PropertyLinks@l##", "Property links", "Связи динамических аттрибутов с объектами системы");
            AddALV(list, "##l@Objects:PropertyValues@l##", "Property values", "Значения динамических аттрибутов");

            // Спасибо за то, что добавил перевод! Удачных идей и быстрого кода.

            return list;
        }

        public static List<AdminLanguageValues> GetAdminLanguageValuesForActions()
        {
            var list = new List<AdminLanguageValues>();

            AddALV(list, "##l@DocumentActions:AddDocument@l##", "Add document", "Создать документ по шаблону");
            AddALV(list, "##l@DocumentActions:CopyDocument@l##", "Copy document", "Создать документ копированием");
            AddALV(list, "##l@DocumentActions:ModifyDocument@l##", "Modify document", "Изменить документ");
            AddALV(list, "##l@DocumentActions:DeleteDocument@l##", "Delete document", "Удалить проект");
            AddALV(list, "##l@DocumentActions:LaunchPlan@l##", "Launch plan", "Запустить выполнение плана");
            AddALV(list, "##l@DocumentActions:AddDocumentSendListItem@l##", "Add document send list item", "Добавить пункт плана");
            AddALV(list, "##l@DocumentActions:StopPlan@l##", "Stop plan", "Остановить выполнение плана");
            AddALV(list, "##l@DocumentActions:ChangeExecutor@l##", "Change executor", "Передать управление");
            AddALV(list, "##l@DocumentActions:RegisterDocument@l##", "Register document", "Зарегистрировать проект");
            AddALV(list, "##l@DocumentActions:MarkDocumentEventAsRead@l##", "Mark document event as read", "Отметить прочтение событий по документу");
            AddALV(list, "##l@DocumentActions:SendForInformation@l##", "Send for information", "Направить для сведения");
            AddALV(list, "##l@DocumentActions:SendForConsideration@l##", "Send for consideration", "Направить для рассмотрения");
            AddALV(list, "##l@DocumentActions:SendForInformationExternal@l##", "Send for information external", "Направить для сведения внешнему агенту");
            AddALV(list, "##l@DocumentActions:SendForControl@l##", "Send for control", "Направить для контроля");
            AddALV(list, "##l@DocumentActions:SendForResponsibleExecution@l##", "Send for responsible execution", "Направить для отв.исполнения");
            AddALV(list, "##l@DocumentActions:SendForExecution@l##", "Send for execution", "Направить для исполнения");
            AddALV(list, "##l@DocumentActions:SendForVisaing@l##", "Send for visaing", "Направить для визирования");
            AddALV(list, "##l@DocumentActions:SendForАgreement@l##", "Send for аgreement", "Направить для согласование");
            AddALV(list, "##l@DocumentActions:SendForАpproval@l##", "Send for аpproval", "Направить для утверждения");
            AddALV(list, "##l@DocumentActions:SendForSigning@l##", "Send for signing", "Направить для подписи");
            AddALV(list, "##l@DocumentActions:ReportRegistrationCardDocument@l##", "Report registration card document", "Регистрационная карточка");
            AddALV(list, "##l@DocumentActions:AddFavourite@l##", "Add favourite", "Добавить в избранное");
            AddALV(list, "##l@DocumentActions:DeleteFavourite@l##", "Delete favourite", "Удалить из избранного");
            AddALV(list, "##l@DocumentActions:FinishWork@l##", "Finish work", "Закончить работу с документом");
            AddALV(list, "##l@DocumentActions:StartWork@l##", "Start work", "Возобновить работу с документом");
            AddALV(list, "##l@DocumentActions:ChangePosition@l##", "Change position", "Поменять должность в документе");
            AddALV(list, "##l@DocumentActions:AddDocumentRestrictedSendList@l##", "Add document restricted send list", "Добавить ограничение рассылки");
            AddALV(list, "##l@DocumentActions:AddByStandartSendListDocumentRestrictedSendList@l##", "Add by standart send list document restricted send list", "Добавить ограничения рассылки по стандартному списку");
            AddALV(list, "##l@DocumentActions:DeleteDocumentRestrictedSendList@l##", "Delete document restricted send list", "Удалить ограничение рассылки");
            AddALV(list, "##l@DocumentActions:ModifyDocumentSendList@l##", "Modify document send list", "Изменить пункт плана");
            AddALV(list, "##l@DocumentActions:DeleteDocumentSendList@l##", "Delete document send list", "Удалить пункт плана");
            AddALV(list, "##l@DocumentActions:LaunchDocumentSendListItem@l##", "Launch document send list item", "Запустить пункт плана на исполнение");
            AddALV(list, "##l@DocumentActions:AddDocumentFile@l##", "Add document file", "Добавить файл");
            AddALV(list, "##l@DocumentActions:ModifyDocumentFile@l##", "Modify document file", "Изменить файл");
            AddALV(list, "##l@DocumentActions:DeleteDocumentFile@l##", "Delete document file", "Удалить файл");
            AddALV(list, "##l@DocumentActions:AddDocumentFileUseMainNameFile@l##", "Add document file use main name file", "Добавить версию файла к файлу");
            AddALV(list, "##l@DocumentActions:AcceptDocumentFile@l##", "Accept document file", "Файл принят");
            AddALV(list, "##l@DocumentActions:RejectDocumentFile@l##", "Reject document file", "Файл не принят");
            AddALV(list, "##l@DocumentActions:RenameDocumentFile@l##", "Rename document file", "Переименовать файл");
            AddALV(list, "##l@DocumentActions:DeleteDocumentFileVersion@l##", "Delete document file version", "Удалить версию файл");
            AddALV(list, "##l@DocumentActions:DeleteDocumentFileVersionRecord@l##", "Delete document file version record", "Удалить запись о версим файла");
            AddALV(list, "##l@DocumentActions:AcceptMainVersionDocumentFile@l##", "Accept main version document file", "Сделать основной версией");
            AddALV(list, "##l@DocumentActions:AddDocumentLink@l##", "Add document link", "Добавить связь между документами");
            AddALV(list, "##l@DocumentActions:DeleteDocumentLink@l##", "Delete document link", "Удалить связь между документами");
            AddALV(list, "##l@DocumentActions:AddDocumentSendList@l##", "Add document send list", "Добавить пункт плана");
            AddALV(list, "##l@DocumentActions:AddByStandartSendListDocumentSendList@l##", "Add by standart send list document send list", "Добавить план работы по стандартному списку");
            AddALV(list, "##l@DocumentActions:AddDocumentSendListStage@l##", "Add document send list stage", "Добавить этап плана");
            AddALV(list, "##l@DocumentActions:DeleteDocumentSendListStage@l##", "Delete document send list stage", "Удалить этап плана");
            AddALV(list, "##l@DocumentActions:SendMessage@l##", "Send message", "Направить сообщение участникам рабочей группы");
            AddALV(list, "##l@DocumentActions:AddNote@l##", "Add note", "Добавить примечание");
            AddALV(list, "##l@DocumentActions:ControlOn@l##", "Control on", "Взять на контроль");
            AddALV(list, "##l@DocumentActions:ControlChange@l##", "Control change", "Изменить параметры контроля");
            AddALV(list, "##l@DocumentActions:SendForExecutionChange@l##", "Send for execution change", "Изменить параметры направлен для исполнения");
            AddALV(list, "##l@DocumentActions:SendForResponsibleExecutionChange@l##", "Send for responsible execution change", "Изменить параметры направлен для отв.исполнения");
            AddALV(list, "##l@DocumentActions:ControlTargetChange@l##", "Control target change", "Изменить параметры контроля для исполнителя");
            AddALV(list, "##l@DocumentActions:ControlOff@l##", "Control off", "Снять с контроля");
            AddALV(list, "##l@DocumentActions:MarkExecution@l##", "Mark execution", "Отметить исполнение");
            AddALV(list, "##l@DocumentActions:AcceptResult@l##", "Accept result", "Принять результат");
            AddALV(list, "##l@DocumentActions:RejectResult@l##", "Reject result", "Отклонить результат");
            AddALV(list, "##l@DocumentActions:WithdrawVisaing@l##", "Withdraw visaing", "Отозвать с визирования");
            AddALV(list, "##l@DocumentActions:WithdrawАgreement@l##", "Withdraw аgreement", "Отозвать с согласования");
            AddALV(list, "##l@DocumentActions:WithdrawАpproval@l##", "Withdraw аpproval", "Отозвать с утверждения");
            AddALV(list, "##l@DocumentActions:WithdrawSigning@l##", "Withdraw signing", "Отозвать с подписи");
            AddALV(list, "##l@DocumentActions:AffixVisaing@l##", "Affix visaing", "Завизировать");
            AddALV(list, "##l@DocumentActions:AffixАgreement@l##", "Affix аgreement", "Согласовать");
            AddALV(list, "##l@DocumentActions:AffixАpproval@l##", "Affix аpproval", "Утвердить");
            AddALV(list, "##l@DocumentActions:AffixSigning@l##", "Affix signing", "Подписать");
            AddALV(list, "##l@DocumentActions:SelfAffixSigning@l##", "Self affix signing", "Самоподписание");
            AddALV(list, "##l@DocumentActions:RejectVisaing@l##", "Reject visaing", "Отказать в визирования");
            AddALV(list, "##l@DocumentActions:RejectАgreement@l##", "Reject аgreement", "Отказать в согласование");
            AddALV(list, "##l@DocumentActions:RejectАpproval@l##", "Reject аpproval", "Отказать в утверждения");
            AddALV(list, "##l@DocumentActions:RejectSigning@l##", "Reject signing", "Отказать в подписи");
            AddALV(list, "##l@DocumentActions:AddDocumentTask@l##", "Add document task", "Добавить задачу");
            AddALV(list, "##l@DocumentActions:ModifyDocumentTask@l##", "Modify document task", "Изменить задачу");
            AddALV(list, "##l@DocumentActions:DeleteDocumentTask@l##", "Delete document task", "Удалить задачу");
            AddALV(list, "##l@DocumentActions:AddDocumentPaper@l##", "Add document paper", "Добавить бумажный носитель");
            AddALV(list, "##l@DocumentActions:ModifyDocumentPaper@l##", "Modify document paper", "Изменить бумажный носитель");
            AddALV(list, "##l@DocumentActions:MarkOwnerDocumentPaper@l##", "Mark owner document paper", "Отметить нахождение бумажного носителя у себя");
            AddALV(list, "##l@DocumentActions:MarkСorruptionDocumentPaper@l##", "Mark сorruption document paper", "Отметить порчу бумажного носителя");
            AddALV(list, "##l@DocumentActions:DeleteDocumentPaper@l##", "Delete document paper", "Удалить бумажный носитель");
            AddALV(list, "##l@DocumentActions:PlanDocumentPaperEvent@l##", "Plan document paper event", "Планировать движение бумажного носителя");
            AddALV(list, "##l@DocumentActions:CancelPlanDocumentPaperEvent@l##", "Cancel plan document paper event", "Отменить планирование движения бумажного носителя");
            AddALV(list, "##l@DocumentActions:SendDocumentPaperEvent@l##", "Send document paper event", "Отметить передачу бумажного носителя");
            AddALV(list, "##l@DocumentActions:CancelSendDocumentPaperEvent@l##", "Cancel send document paper event", "Отменить передачу бумажного носителя");
            AddALV(list, "##l@DocumentActions:RecieveDocumentPaperEvent@l##", "Recieve document paper event", "Отметить прием бумажного носителя");
            AddALV(list, "##l@DocumentActions:AddDocumentPaperList@l##", "Add document paper list", "Добавить реестр бумажных носителей");
            AddALV(list, "##l@DocumentActions:ModifyDocumentPaperList@l##", "Modify document paper list", "Изменить реестр бумажных носителей");
            AddALV(list, "##l@DocumentActions:DeleteDocumentPaperList@l##", "Delete document paper list", "Удалить реестр бумажных носителей");
            AddALV(list, "##l@DocumentActions:AddSavedFilter@l##", "Add saved filter", "Добавить сохраненный фильтр");
            AddALV(list, "##l@DocumentActions:ModifySavedFilter@l##", "Modify saved filter", "Изменить сохраненный фильтр");
            AddALV(list, "##l@DocumentActions:DeleteSavedFilter@l##", "Delete saved filter", "Удалить сохраненный фильтр");
            AddALV(list, "##l@DocumentActions:ModifyDocumentTags@l##", "Modify document tags", "Изменить тэги по документу");
            AddALV(list, "##l@DocumentActions:AddTemplateDocument@l##", "Add template document", "Добавить шаблон документа");
            AddALV(list, "##l@DocumentActions:ModifyTemplateDocument@l##", "Modify template document", "Изменить шаблон документа");
            AddALV(list, "##l@DocumentActions:DeleteTemplateDocument@l##", "Delete template document", "Удалить шаблон документа");
            AddALV(list, "##l@DocumentActions:AddTemplateDocumentSendList@l##", "Add template document send list", "Добавить список рассылки в шаблон");
            AddALV(list, "##l@DocumentActions:ModifyTemplateDocumentSendList@l##", "Modify template document send list", "Изменить список рассылки в шаблоне");
            AddALV(list, "##l@DocumentActions:DeleteTemplateDocumentSendList@l##", "Delete template document send list", "Удалить список рассылки в шаблоне");
            AddALV(list, "##l@DocumentActions:AddTemplateDocumentRestrictedSendList@l##", "Add template document restricted send list", "Добавить ограничительный список рассылки в шаблон");
            AddALV(list, "##l@DocumentActions:ModifyTemplateDocumentRestrictedSendList@l##", "Modify template document restricted send list", "Изменить ограничительный список рассылки в шаблоне");
            AddALV(list, "##l@DocumentActions:DeleteTemplateDocumentRestrictedSendList@l##", "Delete template document restricted send list", "Удалить ограничительный список рассылки в шаблоне");
            AddALV(list, "##l@DocumentActions:AddTemplateDocumentTask@l##", "Add template document task", "Добавить задачу в шаблон");
            AddALV(list, "##l@DocumentActions:ModifyTemplateDocumentTask@l##", "Modify template document task", "Изменить задачу в шаблоне");
            AddALV(list, "##l@DocumentActions:DeleteTemplateDocumentTask@l##", "Delete template document task", "Удалить задачу в шаблоне");
            AddALV(list, "##l@DocumentActions:AddTemplateAttachedFile@l##", "Add template attached file", "Добавить файл в шаблон");
            AddALV(list, "##l@DocumentActions:ModifyTemplateAttachedFile@l##", "Modify template attached file", "Изменить файл в шаблоне");
            AddALV(list, "##l@DocumentActions:DeleteTemplateAttachedFile@l##", "Delete template attached file", "Удалить файл в шаблоне");
            AddALV(list, "##l@DictionaryActions:AddDocumentType@l##", "Add document type", "Добавить тип документа");
            AddALV(list, "##l@DictionaryActions:ModifyDocumentType@l##", "Modify document type", "Изменить тип документа");
            AddALV(list, "##l@DictionaryActions:DeleteDocumentType@l##", "Delete document type", "Удалить тип документа");
            AddALV(list, "##l@DictionaryActions:AddAddressType@l##", "Add address type", "Добавить тип адреса");
            AddALV(list, "##l@DictionaryActions:ModifyAddressType@l##", "Modify address type", "Изменить тип адреса");
            AddALV(list, "##l@DictionaryActions:DeleteAddressType@l##", "Delete address type", "Удалить тип адреса");
            AddALV(list, "##l@DictionaryActions:AddDocumentSubject@l##", "Add document subject", "Добавить тематику");
            AddALV(list, "##l@DictionaryActions:ModifyDocumentSubject@l##", "Modify document subject", "Изменить тематику");
            AddALV(list, "##l@DictionaryActions:DeleteDocumentSubject@l##", "Delete document subject", "Удалить тематику");
            AddALV(list, "##l@DictionaryActions:AddRegistrationJournal@l##", "Add registration journal", "Добавить журнал регистрации");
            AddALV(list, "##l@DictionaryActions:ModifyRegistrationJournal@l##", "Modify registration journal", "Изменить журнал регистрации");
            AddALV(list, "##l@DictionaryActions:DeleteRegistrationJournal@l##", "Delete registration journal", "Удалить журнал регистрации");
            AddALV(list, "##l@DictionaryActions:AddContactType@l##", "Add contact type", "Добавить тип контакта");
            AddALV(list, "##l@DictionaryActions:ModifyContactType@l##", "Modify contact type", "Изменить тип контакта");
            AddALV(list, "##l@DictionaryActions:DeleteContactType@l##", "Delete contact type", "Удалить тип контакта");
            AddALV(list, "##l@DictionaryActions:AddAgent@l##", "Add agent", "Добавить контрагента");
            AddALV(list, "##l@DictionaryActions:ModifyAgent@l##", "Modify agent", "Изменить контрагента");
            AddALV(list, "##l@DictionaryActions:DeleteAgent@l##", "Delete agent", "Удалить контрагента");
            AddALV(list, "##l@DictionaryActions:AddAgentContact@l##", "Add agent contact", "Добавить контакт");
            AddALV(list, "##l@DictionaryActions:ModifyAgentContact@l##", "Modify agent contact", "Изменить контакт");
            AddALV(list, "##l@DictionaryActions:DeleteAgentContact@l##", "Delete agent contact", "Удалить контакт");
            AddALV(list, "##l@DictionaryActions:AddAgentAddress@l##", "Add agent address", "Добавить адрес");
            AddALV(list, "##l@DictionaryActions:ModifyAgentAddress@l##", "Modify agent address", "Изменить адрес");
            AddALV(list, "##l@DictionaryActions:DeleteAgentAddress@l##", "Delete agent address", "Удалить адрес");
            AddALV(list, "##l@DictionaryActions:AddAgentPerson@l##", "Add agent person", "Добавить физическое лицо");
            AddALV(list, "##l@DictionaryActions:ModifyAgentPerson@l##", "Modify agent person", "Изменить физическое лицо");
            AddALV(list, "##l@DictionaryActions:DeleteAgentPerson@l##", "Delete agent person", "Удалить физическое лицо");
            AddALV(list, "##l@DictionaryActions:AddDepartment@l##", "Add department", "Добавить подразделение");
            AddALV(list, "##l@DictionaryActions:ModifyDepartment@l##", "Modify department", "Изменить подразделение");
            AddALV(list, "##l@DictionaryActions:DeleteDepartment@l##", "Delete department", "Удалить подразделение");
            AddALV(list, "##l@DictionaryActions:AddPosition@l##", "Add position", "Добавить должность");
            AddALV(list, "##l@DictionaryActions:ModifyPosition@l##", "Modify position", "Изменить должность");
            AddALV(list, "##l@DictionaryActions:DeletePosition@l##", "Delete position", "Удалить должность");
            AddALV(list, "##l@DictionaryActions:AddAgentEmployee@l##", "Add agent employee", "Добавить сотрудника");
            AddALV(list, "##l@DictionaryActions:ModifyAgentEmployee@l##", "Modify agent employee", "Изменить сотрудника");
            AddALV(list, "##l@DictionaryActions:DeleteAgentEmployee@l##", "Delete agent employee", "Удалить сотрудника");
            AddALV(list, "##l@DictionaryActions:AddAgentCompany@l##", "Add agent company", "Добавить юридическое лицо");
            AddALV(list, "##l@DictionaryActions:ModifyAgentCompany@l##", "Modify agent company", "Изменить юридическое лицо");
            AddALV(list, "##l@DictionaryActions:DeleteAgentCompany@l##", "Delete agent company", "Удалить юридическое лицо");
            AddALV(list, "##l@DictionaryActions:AddAgentBank@l##", "Add agent bank", "Добавить банк");
            AddALV(list, "##l@DictionaryActions:ModifyAgentBank@l##", "Modify agent bank", "Изменить банк");
            AddALV(list, "##l@DictionaryActions:DeleteAgentBank@l##", "Delete agent bank", "Удалить банк");
            AddALV(list, "##l@DictionaryActions:AddAgentAccount@l##", "Add agent account", "Добавить расчетный счет");
            AddALV(list, "##l@DictionaryActions:ModifyAgentAccount@l##", "Modify agent account", "Изменить расчетный счет");
            AddALV(list, "##l@DictionaryActions:DeleteAgentAccount@l##", "Delete agent account", "Удалить расчетный счет");
            AddALV(list, "##l@DictionaryActions:AddStandartSendListContent@l##", "Add standart send list content", "Добавить содержание типового списка рассылки");
            AddALV(list, "##l@DictionaryActions:ModifyStandartSendListContent@l##", "Modify standart send list content", "Изменить содержание типового списка рассылки");
            AddALV(list, "##l@DictionaryActions:DeleteStandartSendListContent@l##", "Delete standart send list content", "Удалить содержание типового списка рассылки");
            AddALV(list, "##l@DictionaryActions:AddStandartSendList@l##", "Add standart send list", "Добавить типовой список рассылки");
            AddALV(list, "##l@DictionaryActions:ModifyStandartSendList@l##", "Modify standart send list", "Изменить типовой список рассылки");
            AddALV(list, "##l@DictionaryActions:DeleteStandartSendList@l##", "Delete standart send list", "Удалить типовой список рассылки");
            AddALV(list, "##l@DictionaryActions:AddAgentClientCompany@l##", "Add agent client company", "Добавить компанию");
            AddALV(list, "##l@DictionaryActions:ModifyAgentClientCompany@l##", "Modify agent client company", "Изменить компанию");
            AddALV(list, "##l@DictionaryActions:DeleteAgentClientCompany@l##", "Delete agent client company", "Удалить компанию");
            AddALV(list, "##l@DictionaryActions:AddExecutorType@l##", "Add executor type", "Добавить тип исполнителя");
            AddALV(list, "##l@DictionaryActions:ModifyExecutorType@l##", "Modify executor type", "Изменить тип исполнителя");
            AddALV(list, "##l@DictionaryActions:DeleteExecutorType@l##", "Delete executor type", "Удалить тип исполнителя");
            AddALV(list, "##l@DictionaryActions:AddExecutor@l##", "Add executor", "Добавить исполнителя");
            AddALV(list, "##l@DictionaryActions:ModifyExecutor@l##", "Modify executor", "Изменить исполнителя");
            AddALV(list, "##l@DictionaryActions:DeleteExecutor@l##", "Delete executor", "Удалить исполнителя");
            AddALV(list, "##l@DictionaryActions:AddTag@l##", "Add tag", "Добавить тэг");
            AddALV(list, "##l@DictionaryActions:ModifyTag@l##", "Modify tag", "Изменить тэг");
            AddALV(list, "##l@DictionaryActions:DeleteTag@l##", "Delete tag", "Удалить тэг");
            AddALV(list, "##l@DictionaryActions:AddCustomDictionaryType@l##", "Add custom dictionary type", "Добавить тип пользовательского словаря");
            AddALV(list, "##l@DictionaryActions:ModifyCustomDictionaryType@l##", "Modify custom dictionary type", "Изменить тип пользовательского словаря");
            AddALV(list, "##l@DictionaryActions:DeleteCustomDictionaryType@l##", "Delete custom dictionary type", "Удалить тип пользовательского словаря");
            AddALV(list, "##l@DictionaryActions:AddCustomDictionary@l##", "Add custom dictionary", "Добавить запись пользовательского словаря");
            AddALV(list, "##l@DictionaryActions:ModifyCustomDictionary@l##", "Modify custom dictionary", "Изменить запись пользовательского словаря");
            AddALV(list, "##l@DictionaryActions:DeleteCustomDictionary@l##", "Delete custom dictionary", "Удалить запись пользовательского словаря");
            AddALV(list, "##l@PropertyAction:AddProperty@l##", "Add property", "Добавить динамический аттрибут");
            AddALV(list, "##l@PropertyAction:ModifyProperty@l##", "Modify property", "Изменить динамический аттрибут");
            AddALV(list, "##l@PropertyAction:DeleteProperty@l##", "Delete property", "Удалить динамический аттрибут");
            AddALV(list, "##l@PropertyAction:AddPropertyLink@l##", "Add property link", "Добавить связь динамических аттрибутов");
            AddALV(list, "##l@PropertyAction:ModifyPropertyLink@l##", "Modify property link", "Изменить связь динамических аттрибутов");
            AddALV(list, "##l@PropertyAction:DeletePropertyLink@l##", "Delete property link", "Удалить связь динамических аттрибутов");
            AddALV(list, "##l@PropertyAction:ModifyPropertyValues@l##", "Modify property values", "Изменить значение динамических аттрибутов");
            AddALV(list, "##l@EncryptionActions:AddEncryptionCertificate@l##", "Add encryption certificate", "Добавить сертификат");
            AddALV(list, "##l@EncryptionActions:ModifyEncryptionCertificate@l##", "Modify encryption certificate", "Изменить сертификат");
            AddALV(list, "##l@EncryptionActions:VerifyPdf@l##", "Verify pdf", "Проверка Pdf");
            AddALV(list, "##l@EncryptionActions:DeleteEncryptionCertificate@l##", "Delete encryption certificate", "Удалить сертификат");
            AddALV(list, "##l@AdminActions:AddRole@l##", "Add role", "Добавить роль");
            AddALV(list, "##l@AdminActions:ModifyRole@l##", "Modify role", "Изменить роль");
            AddALV(list, "##l@AdminActions:DeleteRole@l##", "Delete role", "Удалить роль");
            AddALV(list, "##l@AdminActions:AddRoleAction@l##", "Add role action", "Добавить действие для роли");
            AddALV(list, "##l@AdminActions:DeleteRoleAction@l##", "Delete role action", "Удалить действие для роли");
            AddALV(list, "##l@AdminActions:AddPositionRole@l##", "Add position role", "Добавить роль для должности");
            AddALV(list, "##l@AdminActions:ModifyPositionRole@l##", "Modify position role", "Изменить роль для должности");
            AddALV(list, "##l@AdminActions:DeletePositionRole@l##", "Delete position role", "Удалить роль для должности");
            AddALV(list, "##l@AdminActions:DuplicatePositionRoles@l##", "Duplicate position roles", "Дублировать роли должности");
            AddALV(list, "##l@AdminActions:AddUserRole@l##", "Add user role", "Добавить роль для пользователя");
            AddALV(list, "##l@AdminActions:ModifyUserRole@l##", "Modify user role", "Изменить роль для пользователя");
            AddALV(list, "##l@AdminActions:DeleteUserRole@l##", "Delete user role", "Удалить роль для пользователя");
            AddALV(list, "##l@AdminActions:SetSubordination@l##", "Set subordination", "Управление правилами рассылки");
            AddALV(list, "##l@AdminActions:SetSubordinationByCompany@l##", "Set subordination by company", "Управление правилами рассылки");
            AddALV(list, "##l@AdminActions:SetSubordinationByDepartment@l##", "Set subordination by department", "Управление правилами рассылки");
            AddALV(list, "##l@AdminActions:SetDefaultSubordination@l##", "Set default subordination", "Управление правилами рассылки");
            AddALV(list, "##l@AdminActions:DuplicateSubordinations@l##", "Duplicate subordinations", "Управление правилами рассылки");
            AddALV(list, "##l@AdminActions:SetAllSubordination@l##", "Allow all subordinations", "Разрешить рассылку без ограничений");

            AddALV(list, "##l@AdminActions:AddDepartmentAdmin@l##", "Add department admin", "Добавить администратора подразделения");
            AddALV(list, "##l@AdminActions:DeleteDepartmentAdmin@l##", "Delete department admin", "Удалить администратора подразделения");


            AddALV(list, "##l@SystemActions:SetSetting@l##", "Add setting", "Добавить настройку");

            // Спасибо за то, что добавил перевод! Удачных идей и быстрого кода.

            return list;
        }

        #endregion


        public static List<AspNetLicences> GetAspNetLicences()
        {
            var items = new List<AspNetLicences>();

            //items.Add(new AspNetLicences { Id = 0, Name = "", Description = "", NamedNumberOfConnections = null, ConcurenteNumberOfConnections = null, DurationDay = null, Functionals = null, IsActive = true });
            items.Add(new AspNetLicences { Id = 1, Name = "Base licence", Description = "", NamedNumberOfConnections = null, ConcurenteNumberOfConnections = 10, DurationDay = 365, Functionals = null, IsActive = true });
            items.Add(new AspNetLicences { Id = 2, Name = "Small business licence", Description = "", NamedNumberOfConnections = null, ConcurenteNumberOfConnections = 50, DurationDay = 365, Functionals = null, IsActive = true });
            items.Add(new AspNetLicences { Id = 3, Name = "Fixed Name business", Description = "", NamedNumberOfConnections = null, ConcurenteNumberOfConnections = null, DurationDay = 365, Functionals = null, IsActive = true });
            items.Add(new AspNetLicences { Id = 4, Name = "Unlimited", Description = "", NamedNumberOfConnections = 50, ConcurenteNumberOfConnections = null, DurationDay = null, Functionals = null, IsActive = true });

            return items;
        }

    }
}
