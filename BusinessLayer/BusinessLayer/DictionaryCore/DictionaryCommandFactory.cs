using BL.CrossCutting.DependencyInjection;
using BL.CrossCutting.Interfaces;
using BL.Logic.DependencyInjection;
using BL.Logic.DictionaryCore.CustomDictionary;
using BL.Logic.DictionaryCore.DocumentType;
using BL.Logic.DictionaryCore.Contact;
using BL.Logic.DictionaryCore.ContactType;
using BL.Logic.DictionaryCore.AgentAdresses;
using BL.Logic.DictionaryCore.AgentPerson;
using BL.Logic.DictionaryCore.AgentBank;
using BL.Logic.DictionaryCore.AgentCompany;
using BL.Logic.DictionaryCore.AgentEmployee;
using BL.Logic.DictionaryCore.Agent;
using BL.Logic.DictionaryCore.AgentAccount;
using BL.Logic.DictionaryCore.StandartSendList;
using BL.Logic.DictionaryCore.Tag;
using BL.Model.Enums;
using BL.Model.Exception;

namespace BL.Logic.DictionaryCore
{
    public static class DictionaryCommandFactory
    {
        public static IDictionaryCommand GetDictionaryCommand(EnumDictionaryActions act, IContext ctx, object param)
        {
            IDictionaryCommand cmd;
            switch (act)
            {
                // Типы документов
                #region DictionaryDocumentTypes
                case EnumDictionaryActions.AddDocumentType:
                    cmd = DmsResolver.Current.Get<AddDictionaryDocumentTypeCommand>();
                    break;
                case EnumDictionaryActions.ModifyDocumentType:
                    cmd = DmsResolver.Current.Get<ModifyDictionaryDocumentTypeCommand>();
                    break;
                case EnumDictionaryActions.DeleteDocumentType:
                    cmd = DmsResolver.Current.Get<DeleteDictionaryDocumentTypeCommand>();
                    break;
                #endregion DictionaryDocumentTypes

                // Агенты
                #region DictionaryAgents
                case EnumDictionaryActions.AddAgent:
                    cmd = DmsResolver.Current.Get<AddDictionaryAgentCommand>();
                    break;
                case EnumDictionaryActions.ModifyAgent:
                    cmd = DmsResolver.Current.Get<ModifyDictionaryAgentCommand>();
                    break;
                case EnumDictionaryActions.DeleteAgent:
                    cmd = DmsResolver.Current.Get<DeleteDictionaryAgentCommand>();
                    break;
                #endregion DictionaryAgents

                // Сотрудники
                #region DictionaryAgentPersons
                case EnumDictionaryActions.AddAgentPerson:
                    cmd = DmsResolver.Current.Get<AddDictionaryAgentPersonCommand>();
                    break;
                case EnumDictionaryActions.ModifyAgentPerson:
                    cmd = DmsResolver.Current.Get<ModifyDictionaryAgentPersonCommand>();
                    break;
                case EnumDictionaryActions.DeleteAgentPerson:
                    cmd = DmsResolver.Current.Get<DeleteDictionaryAgentPersonCommand>();
                    break;
                #endregion DictionaryAgentPersons

                case EnumDictionaryActions.ModifyAgentEmployee:
                    cmd = DmsResolver.Current.Get<ModifyDictionaryAgentEmployeeCommand>();
                    break;
                case EnumDictionaryActions.AddAgentEmployee:
                    cmd = DmsResolver.Current.Get<AddDictionaryAgentEmployeeCommand>();
                    break;
                case EnumDictionaryActions.DeleteAgentEmployee:
                    cmd = DmsResolver.Current.Get<DeleteDictionaryAgentEmployeeCommand>();
                    break;

                case EnumDictionaryActions.ModifyAgentCompany:
                    cmd = DmsResolver.Current.Get<ModifyDictionaryAgentCompanyCommand>();
                    break;
                case EnumDictionaryActions.AddAgentCompany:
                    cmd = DmsResolver.Current.Get<AddDictionaryAgentCompanyCommand>();
                    break;
                case EnumDictionaryActions.DeleteAgentCompany:
                    cmd = DmsResolver.Current.Get<DeleteDictionaryAgentCompanyCommand>();
                    break;

                case EnumDictionaryActions.ModifyAgentBank:
                    cmd = DmsResolver.Current.Get<ModifyDictionaryAgentBankCommand>();
                    break;
                case EnumDictionaryActions.AddAgentBank:
                    cmd = DmsResolver.Current.Get<AddDictionaryAgentBankCommand>();
                    break;
                case EnumDictionaryActions.DeleteAgentBank:
                    cmd = DmsResolver.Current.Get<DeleteDictionaryAgentBankCommand>();
                    break;

                case EnumDictionaryActions.ModifyAgentAccount:
                    cmd = DmsResolver.Current.Get<ModifyDictionaryAgentAccountCommand>();
                    break;
                case EnumDictionaryActions.AddAgentAccount:
                    cmd = DmsResolver.Current.Get<AddDictionaryAgentAccountCommand>();
                    break;
                case EnumDictionaryActions.DeleteAgentAccount:
                    cmd = DmsResolver.Current.Get<DeleteDictionaryAgentAccountCommand>();
                    break;

                // Адреса
                #region DictionaryAgentAddress
                case EnumDictionaryActions.AddAgentAddress:
                    cmd = DmsResolver.Current.Get<AddDictionaryAgentAddressCommand>();
                    break;
                case EnumDictionaryActions.ModifyAgentAddress:
                    cmd = DmsResolver.Current.Get<ModifyDictionaryAgentAddressCommand>();
                    break;
                case EnumDictionaryActions.DeleteAgentAddress:
                    cmd = DmsResolver.Current.Get<DeleteDictionaryAgentAddressCommand>();
                    break;
                #endregion DictionaryAgentAddress

                // Типы адресов
                #region DictionaryAddressType
                case EnumDictionaryActions.AddAddressType:
                    cmd = DmsResolver.Current.Get<AddDictionaryAddressTypeCommand>();
                    break;
                case EnumDictionaryActions.ModifyAddressType:
                    cmd = DmsResolver.Current.Get<ModifyDictionaryAddressTypeCommand>();
                    break;
                case EnumDictionaryActions.DeleteAddressType:
                    cmd = DmsResolver.Current.Get<DeleteDictionaryAddressTypeCommand>();
                    break;
                #endregion DictionaryAddressType

                // Контакты
                #region DictionaryContacts
                case EnumDictionaryActions.AddContact:
                    cmd = DmsResolver.Current.Get<AddDictionaryContactCommand>();
                    break;
                case EnumDictionaryActions.ModifyContact:
                    cmd = DmsResolver.Current.Get<ModifyDictionaryContactCommand>();
                    break;
                case EnumDictionaryActions.DeleteContact:
                    cmd = DmsResolver.Current.Get<DeleteDictionaryContactCommand>();
                    break;
                #endregion DictionaryContacts

                // Типы контактов
                #region DictionaryContactTypes
                case EnumDictionaryActions.AddContactType:
                    cmd = DmsResolver.Current.Get<AddDictionaryContactTypeCommand>();
                    break;
                case EnumDictionaryActions.ModifyContactType:
                    cmd = DmsResolver.Current.Get<ModifyDictionaryContactTypeCommand>();
                    break;
                case EnumDictionaryActions.DeleteContactType:
                    cmd = DmsResolver.Current.Get<DeleteDictionaryContactTypeCommand>();
                    break;
                #endregion DictionaryContactTypes

                // Теги
                #region DictionaryTags
                case EnumDictionaryActions.AddTag:
                    cmd = DmsResolver.Current.Get<AddDictionaryTagCommand>();
                    break;
                case EnumDictionaryActions.ModifyTag:
                    cmd = DmsResolver.Current.Get<ModifyDictionaryTagCommand>();
                    break;
                case EnumDictionaryActions.DeleteTag:
                    cmd = DmsResolver.Current.Get<DeleteDictionaryTagCommand>();
                    break;
                #endregion DictionaryTags

                // CustomDictionaryTypes
                #region CustomDictionaryType
                case EnumDictionaryActions.AddCustomDictionaryType:
                    cmd = DmsResolver.Current.Get<AddCustomDictionaryTypeCommand>();
                    break;
                case EnumDictionaryActions.ModifyCustomDictionaryType:
                    cmd = DmsResolver.Current.Get<ModifyCustomDictionaryTypeCommand>();
                    break;
                case EnumDictionaryActions.DeleteCustomDictionaryType:
                    cmd = DmsResolver.Current.Get<DeleteCustomDictionaryTypeCommand>();
                    break;
                #endregion CustomDictionaryType

                // CustomDictionarys
                #region CustomDictionarys
                case EnumDictionaryActions.AddCustomDictionary:
                    cmd = DmsResolver.Current.Get<AddCustomDictionaryCommand>();
                    break;
                case EnumDictionaryActions.ModifyCustomDictionary:
                    cmd = DmsResolver.Current.Get<ModifyCustomDictionaryCommand>();
                    break;
                case EnumDictionaryActions.DeleteCustomDictionary:
                    cmd = DmsResolver.Current.Get<DeleteCustomDictionaryCommand>();
                    break;
                #endregion CustomDictionarys


                // Тематики документов
                #region DictionaryDocumentSubjects
                case EnumDictionaryActions.AddDocumentSubject:
                    cmd = DmsResolver.Current.Get<AddDictionaryDocumentSubjectCommand>();
                    break;
                case EnumDictionaryActions.ModifyDocumentSubject:
                    cmd = DmsResolver.Current.Get<ModifyDictionaryDocumentSubjectCommand>();
                    break;
                case EnumDictionaryActions.DeleteDocumentSubject:
                    cmd = DmsResolver.Current.Get<DeleteDictionaryDocumentSubjectCommand>();
                    break;
                #endregion DictionaryDocumentSubjects

                // Журналы регистрации
                #region DictionaryRegistrationJournals
                case EnumDictionaryActions.AddRegistrationJournal:
                    cmd = DmsResolver.Current.Get<AddDictionaryRegistrationJournalCommand>();
                    break;
                case EnumDictionaryActions.ModifyRegistrationJournal:
                    cmd = DmsResolver.Current.Get<ModifyDictionaryRegistrationJournalCommand>();
                    break;
                case EnumDictionaryActions.DeleteRegistrationJournal:
                    cmd = DmsResolver.Current.Get<DeleteDictionaryRegistrationJournalCommand>();
                    break;
                #endregion DictionaryRegistrationJournals
                   
                // Структура предприятия
                #region DictionaryDepartmentss
                case EnumDictionaryActions.AddDepartment:
                    cmd = DmsResolver.Current.Get<AddDictionaryDepartmentCommand>();
                    break;
                case EnumDictionaryActions.ModifyDepartment:
                    cmd = DmsResolver.Current.Get<ModifyDictionaryDepartmentCommand>();
                    break;
                case EnumDictionaryActions.DeleteDepartment:
                    cmd = DmsResolver.Current.Get<DeleteDictionaryDepartmentCommand>();
                    break;
                #endregion DictionaryDepartmentss

                // Штатное расписание
                #region DictionaryPositionss
                case EnumDictionaryActions.AddPosition:
                    cmd = DmsResolver.Current.Get<AddDictionaryPositionCommand>();
                    break;
                case EnumDictionaryActions.ModifyPosition:
                    cmd = DmsResolver.Current.Get<ModifyDictionaryPositionCommand>();
                    break;
                case EnumDictionaryActions.DeletePosition:
                    cmd = DmsResolver.Current.Get<DeleteDictionaryPositionCommand>();
                    break;
                #endregion DictionaryPositionss

                // Компании
                #region DictionaryCompanies
                case EnumDictionaryActions.AddCompany:
                    cmd = DmsResolver.Current.Get<AddDictionaryCompanyCommand>();
                    break;
                case EnumDictionaryActions.ModifyCompany:
                    cmd = DmsResolver.Current.Get<ModifyDictionaryCompanyCommand>();
                    break;
                case EnumDictionaryActions.DeleteCompany:
                    cmd = DmsResolver.Current.Get<DeleteDictionaryCompanyCommand>();
                    break;
                #endregion DictionaryCompanies

                // Исполнители
                #region DictionaryPositionExecutors
                case EnumDictionaryActions.AddExecutor:
                    cmd = DmsResolver.Current.Get<AddDictionaryPositionExecutorCommand>();
                    break;
                case EnumDictionaryActions.ModifyExecutor:
                    cmd = DmsResolver.Current.Get<ModifyDictionaryPositionExecutorCommand>();
                    break;
                case EnumDictionaryActions.DeleteExecutor:
                    cmd = DmsResolver.Current.Get<DeleteDictionaryPositionExecutorCommand>();
                    break;
                #endregion DictionaryPositionExecutors


                #region DictionaryStandartSendListContent
                case EnumDictionaryActions.AddStandartSendListContent:
                    cmd = DmsResolver.Current.Get<AddDictionaryStandartSendListContentCommand>();
                    break;
                case EnumDictionaryActions.ModifyStandartSendListContent:
                    cmd = DmsResolver.Current.Get<ModifyDictionaryStandartSendListContentCommand>();
                    break;
                case EnumDictionaryActions.DeleteStandartSendListContent:
                    cmd = DmsResolver.Current.Get<DeleteDictionaryStandartSendListContentCommand>();
                    break;
                #endregion DictionaryStandartSendListContent


                default:
                    throw new CommandNotDefinedError();
            }
            cmd.InitializeCommand(act, ctx, param);
            return cmd;
        }
    }
}