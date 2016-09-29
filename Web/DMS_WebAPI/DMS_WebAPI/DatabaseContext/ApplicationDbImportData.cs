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

        // этот метод-подпорка пока не решены вопросы локализации полностью
        // достаю переводы из функции инициализации переводов при первом старте
        public static string ReplaceLanguageLabel(string userLanguage, string text)
        {
            string errorMessage = text;

            try
            {
                var labelsInText = new List<string>();
                foreach (Match label in Regex.Matches(errorMessage, "##l@(.*?)@l##"))
                {
                    labelsInText.Add(label.Value);
                }

                if (labelsInText.Count > 0)
                {
                    int LanguageId = 0;

                    if (string.IsNullOrEmpty(userLanguage)) userLanguage = string.Empty;

                    //var lang =  GetAdminLanguages().Where(x => x.Name == userLanguage).FirstOrDefault();

                    if (userLanguage == string.Empty )
                    {
                        LanguageId = GetAdminLanguages().Where(x => x.IsDefault == true).FirstOrDefault().Id;
                    }

                    var labels = GetAdminLanguageValues().Where( x => x.LanguageId == LanguageId && labelsInText.Contains( x.Label)).ToArray();

                    for (int i = 0, l = labels.Length; i < l; i++)
                    {
                        string val = labels[i].Value;
                        if (string.IsNullOrEmpty(val)) val = "Empty translation for label: " + labels[i].Label;
                        errorMessage = errorMessage.Replace(labels[i].Label, val);
                    }
                }

                return errorMessage;
            }
            catch (Exception ex) { }
            return text;
        }


        #region [+] Languages ...

        public static List<AdminLanguages> GetAdminLanguages()
        {
            var items = new List<AdminLanguages>();

            items.Add(new AdminLanguages { Id = 1, Code = "ru", Name = "Русский", IsDefault = true });
            items.Add(new AdminLanguages { Id = 2, Code = "en", Name = "English", IsDefault = false });

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

            AddALV(list, "##l@DmsExceptions:IncomingModelIsNotValid@l##", "Incoming Model is not valid! {0}", "Входящая модель недействительна! {0}");
            AddALV(list, "##l@DmsExceptions:WrongParameterTypeError@l##", "Parameter type commands is incorrect!", "Тип параметра комманды указан неверно!");
            AddALV(list, "##l@DmsExceptions:WrongParameterValueError@l##", "Parameters commands incorrect!", "Параметры комманды неверные!");
            AddALV(list, "##l@DmsExceptions:UserUnauthorized@l##", "Authorization has been denied for this request.", "Пользователь не авторизован");
            AddALV(list, "##l@DmsExceptions:RecordNotUnique@l##", "Record is not Unique", "Запись не уникальна");

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
            AddALV(list, "##l@DmsExceptions:ControlerHasAlreadyBeenDefined@l##", "", "");
            AddALV(list, "##l@DmsExceptions:CouldNotModifyTemplateDocument@l##", "", "");
            AddALV(list, "##l@DmsExceptions:CouldNotPerformOperationWithPaper@l##", "", "");
            AddALV(list, "##l@DmsExceptions:EncryptionCertificateHasExpired@l##", "", "");
            AddALV(list, "##l@DmsExceptions:NobodyIsChosen@l##", "", "");
            AddALV(list, "##l@DmsExceptions:ResponsibleExecutorHasAlreadyBeenDefined@l##", "", "");
            AddALV(list, "##l@DmsExceptions:ResponsibleExecutorIsNotDefined@l##", "", "");
            AddALV(list, "##l@DmsExceptions:SigningTypeNotAllowed@l##", "", "");
            AddALV(list, "##l@DmsExceptions:SubordinationHasBeenViolated@l##", "", "");
            AddALV(list, "##l@DmsExceptions:TargetIsNotDefined@l##", "", "");
            AddALV(list, "##l@DmsExceptions:TaskIsNotDefined@l##", "", "");

            // после добавления переводов можно обновить их в базе api/v2/Languages/RefreshLanguageValues

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
