using BL.CrossCutting.DependencyInjection;
using BL.CrossCutting.Interfaces;
using BL.Logic.DictionaryCore.AgentEmployee;
using BL.Logic.DictionaryCore.Positions;
using BL.Model.Enums;
using BL.Model.Exception;

namespace BL.Logic.DictionaryCore
{
    public static class DictionaryCommandFactory
    {
        public static IDictionaryCommand GetDictionaryCommand(EnumActions act, IContext ctx, object param)
        {
            var dmsExceptions = ctx.ClientLicence?.LicenceError as DmsExceptions;
            if (dmsExceptions != null)
                throw dmsExceptions;

            IDictionaryCommand cmd;
            switch (act)
            {
                // Типы документов
                #region DictionaryDocumentTypes
                case EnumActions.AddDocumentType:
                    cmd = DmsResolver.Current.Get<AddDictionaryDocumentTypeCommand>();
                    break;
                case EnumActions.ModifyDocumentType:
                    cmd = DmsResolver.Current.Get<ModifyDictionaryDocumentTypeCommand>();
                    break;
                case EnumActions.DeleteDocumentType:
                    cmd = DmsResolver.Current.Get<DeleteDictionaryDocumentTypeCommand>();
                    break;
                #endregion DictionaryDocumentTypes

                // Агенты
                #region DictionaryAgents
                //case EnumDictionaryActions.AddAgent:
                //    cmd = DmsResolver.Current.Get<AddDictionaryAgentCommand>();
                //    break;
                //case EnumDictionaryActions.ModifyAgent:
                //    cmd = DmsResolver.Current.Get<ModifyDictionaryAgentCommand>();
                //    break;
                //case EnumDictionaryActions.DeleteAgent:
                //    cmd = DmsResolver.Current.Get<DeleteDictionaryAgentCommand>();
                //    break;
                case EnumActions.SetAgentImage:
                    cmd = DmsResolver.Current.Get<SetDictionaryAgentImageCommand>();
                    break;
                case EnumActions.DeleteAgentImage:
                    cmd = DmsResolver.Current.Get<DeleteDictionaryAgentImageCommand>();
                    break;
                #endregion DictionaryAgents

                case EnumActions.ModifyAgentPeoplePassport:
                    cmd = DmsResolver.Current.Get<ModifyAgentPeoplePassportCommand>();
                    break;

                // Сотрудники
                #region DictionaryAgentPersons
                case EnumActions.AddAgentPerson:
                    cmd = DmsResolver.Current.Get<AddDictionaryAgentPersonCommand>();
                    break;
                case EnumActions.AddAgentPersonExisting:
                    cmd = DmsResolver.Current.Get<AddDictionaryAgentPersonExistingCommand>();
                    break;
                case EnumActions.ModifyAgentPerson:
                    cmd = DmsResolver.Current.Get<ModifyDictionaryAgentPersonCommand>();
                    break;
                case EnumActions.DeleteAgentPerson:
                    cmd = DmsResolver.Current.Get<DeleteDictionaryAgentPersonCommand>();
                    break;
                #endregion DictionaryAgentPersons
                case EnumActions.ModifyAgentEmployee:
                    cmd = DmsResolver.Current.Get<ModifyDictionaryAgentEmployeeCommand>();
                    break;
                case EnumActions.AddAgentEmployee:
                    cmd = DmsResolver.Current.Get<AddDictionaryAgentEmployeeCommand>();
                    break;
                case EnumActions.DeleteAgentEmployee:
                    cmd = DmsResolver.Current.Get<DeleteDictionaryAgentEmployeeCommand>();
                    break;
                //case EnumDictionaryActions.ModifyAgentEmployeeLanguage:
                //cmd = DmsResolver.Current.Get<ModifyDictionaryAgentUserLanguageCommand>();
                //break;
                case EnumActions.ModifyAgentCompany:
                    cmd = DmsResolver.Current.Get<ModifyDictionaryAgentCompanyCommand>();
                    break;
                case EnumActions.AddAgentCompany:
                    cmd = DmsResolver.Current.Get<AddDictionaryAgentCompanyCommand>();
                    break;
                case EnumActions.DeleteAgentCompany:
                    cmd = DmsResolver.Current.Get<DeleteDictionaryAgentCompanyCommand>();
                    break;

                case EnumActions.ModifyAgentBank:
                    cmd = DmsResolver.Current.Get<ModifyDictionaryAgentBankCommand>();
                    break;
                case EnumActions.AddAgentBank:
                    cmd = DmsResolver.Current.Get<AddDictionaryAgentBankCommand>();
                    break;
                case EnumActions.DeleteAgentBank:
                    cmd = DmsResolver.Current.Get<DeleteDictionaryAgentBankCommand>();
                    break;

                case EnumActions.ModifyAgentAccount:
                    cmd = DmsResolver.Current.Get<ModifyDictionaryAgentAccountCommand>();
                    break;
                case EnumActions.AddAgentAccount:
                    cmd = DmsResolver.Current.Get<AddDictionaryAgentAccountCommand>();
                    break;
                case EnumActions.DeleteAgentAccount:
                    cmd = DmsResolver.Current.Get<DeleteDictionaryAgentAccountCommand>();
                    break;

                // Адреса
                #region DictionaryAgentAddress
                case EnumActions.AddAgentAddress:
                case EnumActions.AddBankAddress:
                case EnumActions.AddCompanyAddress:
                case EnumActions.AddClientCompanyAddress:
                case EnumActions.AddEmployeeAddress:
                case EnumActions.AddPersonAddress:
                    cmd = DmsResolver.Current.Get<AddDictionaryAgentAddressCommand>();
                    break;
                case EnumActions.ModifyAgentAddress:
                case EnumActions.ModifyBankAddress:
                case EnumActions.ModifyCompanyAddress:
                case EnumActions.ModifyClientCompanyAddress:
                case EnumActions.ModifyEmployeeAddress:
                case EnumActions.ModifyPersonAddress:
                    cmd = DmsResolver.Current.Get<ModifyDictionaryAgentAddressCommand>();
                    break;
                case EnumActions.DeleteAgentAddress:
                case EnumActions.DeleteBankAddress:
                case EnumActions.DeleteCompanyAddress:
                case EnumActions.DeleteClientCompanyAddress:
                case EnumActions.DeleteEmployeeAddress:
                case EnumActions.DeletePersonAddress:
                    cmd = DmsResolver.Current.Get<DeleteDictionaryAgentAddressCommand>();
                    break;
                #endregion DictionaryAgentAddress

                // Типы адресов
                #region DictionaryAddressType
                case EnumActions.AddAddressType:
                    cmd = DmsResolver.Current.Get<AddDictionaryAddressTypeCommand>();
                    break;
                case EnumActions.ModifyAddressType:
                    cmd = DmsResolver.Current.Get<ModifyDictionaryAddressTypeCommand>();
                    break;
                case EnumActions.DeleteAddressType:
                    cmd = DmsResolver.Current.Get<DeleteDictionaryAddressTypeCommand>();
                    break;
                #endregion DictionaryAddressType

                // Контакты
                #region DictionaryContacts
                case EnumActions.AddAgentContact:
                case EnumActions.AddBankContact:
                case EnumActions.AddCompanyContact:
                case EnumActions.AddClientCompanyContact:
                case EnumActions.AddEmployeeContact:
                case EnumActions.AddPersonContact:
                    cmd = DmsResolver.Current.Get<AddDictionaryContactCommand>();
                    break;
                case EnumActions.ModifyAgentContact:
                case EnumActions.ModifyBankContact:
                case EnumActions.ModifyCompanyContact:
                case EnumActions.ModifyClientCompanyContact:
                case EnumActions.ModifyEmployeeContact:
                case EnumActions.ModifyPersonContact:
                    cmd = DmsResolver.Current.Get<ModifyDictionaryContactCommand>();
                    break;
                case EnumActions.DeleteAgentContact:
                case EnumActions.DeleteBankContact:
                case EnumActions.DeleteCompanyContact:
                case EnumActions.DeleteClientCompanyContact:
                case EnumActions.DeleteEmployeeContact:
                case EnumActions.DeletePersonContact:
                    cmd = DmsResolver.Current.Get<DeleteDictionaryContactCommand>();
                    break;
                #endregion DictionaryContacts

                // Типы контактов
                #region DictionaryContactTypes
                case EnumActions.AddContactType:
                    cmd = DmsResolver.Current.Get<AddDictionaryContactTypeCommand>();
                    break;
                case EnumActions.ModifyContactType:
                    cmd = DmsResolver.Current.Get<ModifyDictionaryContactTypeCommand>();
                    break;
                case EnumActions.DeleteContactType:
                    cmd = DmsResolver.Current.Get<DeleteDictionaryContactTypeCommand>();
                    break;
                #endregion DictionaryContactTypes

                // Теги
                #region DictionaryTags
                case EnumActions.AddTag:
                    cmd = DmsResolver.Current.Get<AddDictionaryTagCommand>();
                    break;
                case EnumActions.ModifyTag:
                    cmd = DmsResolver.Current.Get<ModifyDictionaryTagCommand>();
                    break;
                case EnumActions.DeleteTag:
                    cmd = DmsResolver.Current.Get<DeleteDictionaryTagCommand>();
                    break;
                #endregion DictionaryTags

                // CustomDictionaryTypes
                #region CustomDictionaryType
                case EnumActions.AddCustomDictionaryType:
                    cmd = DmsResolver.Current.Get<AddCustomDictionaryTypeCommand>();
                    break;
                case EnumActions.ModifyCustomDictionaryType:
                    cmd = DmsResolver.Current.Get<ModifyCustomDictionaryTypeCommand>();
                    break;
                case EnumActions.DeleteCustomDictionaryType:
                    cmd = DmsResolver.Current.Get<DeleteCustomDictionaryTypeCommand>();
                    break;
                #endregion CustomDictionaryType

                // CustomDictionarys
                #region CustomDictionarys
                case EnumActions.AddCustomDictionary:
                    cmd = DmsResolver.Current.Get<AddCustomDictionaryCommand>();
                    break;
                case EnumActions.ModifyCustomDictionary:
                    cmd = DmsResolver.Current.Get<ModifyCustomDictionaryCommand>();
                    break;
                case EnumActions.DeleteCustomDictionary:
                    cmd = DmsResolver.Current.Get<DeleteCustomDictionaryCommand>();
                    break;
                #endregion CustomDictionarys


                // Журналы регистрации
                #region DictionaryRegistrationJournals
                case EnumActions.AddRegistrationJournal:
                    cmd = DmsResolver.Current.Get<AddDictionaryRegistrationJournalCommand>();
                    break;
                case EnumActions.ModifyRegistrationJournal:
                    cmd = DmsResolver.Current.Get<ModifyDictionaryRegistrationJournalCommand>();
                    break;
                case EnumActions.DeleteRegistrationJournal:
                    cmd = DmsResolver.Current.Get<DeleteDictionaryRegistrationJournalCommand>();
                    break;
                #endregion DictionaryRegistrationJournals

                // Структура предприятия
                #region DictionaryDepartmentss
                case EnumActions.AddDepartment:
                    cmd = DmsResolver.Current.Get<AddDictionaryDepartmentCommand>();
                    break;
                case EnumActions.ModifyDepartment:
                    cmd = DmsResolver.Current.Get<ModifyDictionaryDepartmentCommand>();
                    break;
                case EnumActions.DeleteDepartment:
                    cmd = DmsResolver.Current.Get<DeleteDictionaryDepartmentCommand>();
                    break;
                #endregion DictionaryDepartmentss

                // Штатное расписание
                #region DictionaryPositionss
                case EnumActions.AddPosition:
                    cmd = DmsResolver.Current.Get<AddDictionaryPositionCommand>();
                    break;
                case EnumActions.ModifyPosition:
                    cmd = DmsResolver.Current.Get<ModifyDictionaryPositionCommand>();
                    break;
                case EnumActions.DeletePosition:
                    cmd = DmsResolver.Current.Get<DeleteDictionaryPositionCommand>();
                    break;
                #endregion DictionaryPositionss

                // Компании
                #region DictionaryCompanies
                case EnumActions.AddOrg:
                    cmd = DmsResolver.Current.Get<AddDictionaryAgentClientCompanyCommand>();
                    break;
                case EnumActions.ModifyOrg:
                    cmd = DmsResolver.Current.Get<ModifyDictionaryAgentClientCompanyCommand>();
                    break;
                case EnumActions.DeleteOrg:
                    cmd = DmsResolver.Current.Get<DeleteDictionaryAgentClientCompanyCommand>();
                    break;
                #endregion DictionaryCompanies

                // Исполнители
                #region DictionaryPositionExecutors
                case EnumActions.AddExecutor:
                    cmd = DmsResolver.Current.Get<AddPositionExecutorCommand>();
                    break;
                case EnumActions.ModifyExecutor:
                    cmd = DmsResolver.Current.Get<ModifyPositionExecutorCommand>();
                    break;
                case EnumActions.DeleteExecutor:
                    cmd = DmsResolver.Current.Get<DeleteDictionaryPositionExecutorCommand>();
                    break;

                // Делегирование полномочий
                case EnumActions.AddUserPositionExecutor:
                    cmd = DmsResolver.Current.Get<AddUserPositionExecutorCommand>();
                    break;
                case EnumActions.ModifyUserPositionExecutor:
                    cmd = DmsResolver.Current.Get<ModifyUserPositionExecutorCommand>();
                    break;
                case EnumActions.DeleteUserPositionExecutor:
                    cmd = DmsResolver.Current.Get<DeleteUserPositionExecutorCommand>();
                    break;
                #endregion 


                #region DictionaryStandartSendList
                case EnumActions.AddStandartSendList:
                    cmd = DmsResolver.Current.Get<AddDictionaryStandartSendListCommand>();
                    break;
                case EnumActions.ModifyStandartSendList:
                    cmd = DmsResolver.Current.Get<ModifyDictionaryStandartSendListCommand>();
                    break;
                case EnumActions.DeleteStandartSendList:
                    cmd = DmsResolver.Current.Get<DeleteDictionaryStandartSendListCommand>();
                    break;
                #endregion DictionaryStandartSendList

                #region DictionaryStandartSendListContent
                case EnumActions.AddStandartSendListContent:
                    cmd = DmsResolver.Current.Get<AddDictionaryStandartSendListContentCommand>();
                    break;
                case EnumActions.ModifyStandartSendListContent:
                    cmd = DmsResolver.Current.Get<ModifyDictionaryStandartSendListContentCommand>();
                    break;
                case EnumActions.DeleteStandartSendListContent:
                    cmd = DmsResolver.Current.Get<DeleteDictionaryStandartSendListContentCommand>();
                    break;
                #endregion DictionaryStandartSendListContent


                default:
                    throw new CommandNotDefinedError(act.ToString());
            }
            cmd.InitializeCommand(act, ctx, param);
            return cmd;
        }
    }
}