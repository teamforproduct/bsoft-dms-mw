using BL.CrossCutting.DependencyInjection;
using BL.Model.DocumentCore;
using BL.Model.SystemCore;
using DMS_WebAPI.Results;
using DMS_WebAPI.Utilities;
using System.Collections.Generic;
using System.Web.Http;
using BL.Logic.DocumentCore.Interfaces;
using BL.Model.Enums;

namespace DMS_WebAPI.Controllers.Documents
{
    [Authorize]
    [RoutePrefix("api/Documents")]
    public class DocumentsController : ApiController
    {
        //GET: api/Documents
        public IHttpActionResult Get([FromUri] FilterDocument filters)
        {
            var cxt = DmsResolver.Current.Get<UserContext>().Get();
            var docProc = DmsResolver.Current.Get<IDocumentService>();
            var paging = new UIPaging();
            var docs = docProc.GetDocuments(cxt, filters, paging);
            return new JsonResult(docs, this);
        }

        // GET: api/Documents/5
        public IHttpActionResult Get(int id)
        {
            var cxt = DmsResolver.Current.Get<UserContext>().Get();
            var docProc = DmsResolver.Current.Get<IDocumentService>();

            var doc = docProc.GetDocument(cxt, id);

            var metaData = new List<SystemUIElements>()
            {
                new SystemUIElements()
                {
                    ObjectCode = "Documents",
                    ActionCode = "Modify",
                    Code = "GeneralInfo",
                    TypeCode = "display_only_text",
                    Description = "Общая информация",
                    ValueTypeCode = "text",
                    IsMandatory = false,
                    IsReadOnly = true,
                    IsVisible = true,
                    ValueFieldCode = "GeneralInfo",
                    ValueDescriptionFieldCode = "GeneralInfo",
                    Format = ""
                },
                new SystemUIElements()
                {
                    ObjectCode = "Documents",
                    ActionCode = "Modify",
                    Code = "DocumentSubject",
                    TypeCode = "select",
                    Description = "Тематика документа",
                    Label = "Тематика документа",
                    Hint = "Выберите из словаря тематику документа",
                    ValueTypeCode = "number",
                    IsMandatory = false,
                    IsReadOnly = false,
                    IsVisible = true,
                    SelectAPI = "DictionaryDocumentSubjects",
                    SelectFieldCode = "Id",
                    SelectDescriptionFieldCode = "Name",
                    ValueFieldCode = "DocumentSubjectId",
                    ValueDescriptionFieldCode = "DocumentSubjectName",
                    Format = ""
                },
                new SystemUIElements()
                {
                    ObjectCode = "Documents",
                    ActionCode = "Modify",
                    Code = "Description",
                    TypeCode = "textarea",
                    Description = "Краткое содержание",
                    Label = "Краткое содержание",
                    Hint = "Введите краткое содержание",
                    ValueTypeCode = "text",
                    IsMandatory = true,
                    IsReadOnly = false,
                    IsVisible = true,
                    ValueFieldCode = "Description",
                    ValueDescriptionFieldCode = "Description",
                    Format = ""
                },
                new SystemUIElements()
                {
                    ObjectCode = "Documents",
                    ActionCode = "Modify",
                    Code = "SenderAgent",
                    TypeCode = "select",
                    Description = "Контрагент, от которого получен документ",
                    Label = "Организация",
                    Hint = "Контрагент, от которого получен документ",
                    ValueTypeCode = "number",
                    IsMandatory = true,
                    IsReadOnly = false,
                    IsVisible = doc.DocumentDirection == EnumDocumentDirections.External,
                    SelectAPI = "DictionaryAgents",
                    SelectFilter = "{\"IsIndividual\" : \"False\"}",
                    SelectFieldCode = "Id",
                    SelectDescriptionFieldCode = "Name",
                    ValueFieldCode = "SenderAgentId",
                    ValueDescriptionFieldCode = "SenderAgentName",
                    Format = ""
                },
                new SystemUIElements()
                {
                    ObjectCode = "Documents",
                    ActionCode = "Modify",
                    Code = "SenderAgentPerson",
                    TypeCode = "select",
                    Description = "Контактное лицо в организации",
                    Label = "Контакт",
                    Hint = "Контактное лицо в организации, от которой получен документ",
                    ValueTypeCode = "Number",
                    IsMandatory = true,
                    IsReadOnly = false,
                    IsVisible = doc.DocumentDirection == EnumDocumentDirections.External,
                    SelectAPI = "DictionaryAgentPersons",
                    SelectFilter = "{\"AgentId\" : \"@SenderAgentId\"}",
                    SelectFieldCode = "Id",
                    SelectDescriptionFieldCode = "Name",
                    ValueFieldCode = "SenderAgentPersonId",
                    ValueDescriptionFieldCode = "SenderAgentPersonName",
                    Format = ""
                },
                new SystemUIElements()
                {
                    ObjectCode = "Documents",
                    ActionCode = "Modify",
                    Code = "SenderNumber",
                    TypeCode = "input",
                    Description = "Входящий номер",
                    Label = "Входящий номер",
                    Hint = "Входящий номер",
                    ValueTypeCode = "text",
                    IsMandatory = true,
                    IsReadOnly = false,
                    IsVisible = doc.DocumentDirection == EnumDocumentDirections.External,
                    ValueFieldCode = "SenderNumber",
                    ValueDescriptionFieldCode = "SenderNumber",
                    Format = ""
                },
                new SystemUIElements()
                {
                    ObjectCode = "Documents",
                    ActionCode = "Modify",
                    Code = "SenderDate",
                    TypeCode = "input",
                    Description = "Дата входящего документа",
                    Label = "Дата входящего документа",
                    Hint = "Дата входящего документа",
                    ValueTypeCode = "date",
                    IsMandatory = true,
                    IsReadOnly = false,
                    IsVisible = doc.DocumentDirection == EnumDocumentDirections.External,
                    ValueFieldCode = "SenderDate",
                    ValueDescriptionFieldCode = "SenderDate",
                    Format = ""
                },
                new SystemUIElements()
                {
                    ObjectCode = "Documents",
                    ActionCode = "Modify",
                    Code = "Addressee",
                    TypeCode = "input",
                    Description = "Кому адресован документ",
                    Label = "Кому адресован документ",
                    Hint = "Кому адресован документ",
                    ValueTypeCode = "text",
                    IsMandatory = true,
                    IsReadOnly = false,
                    IsVisible = doc.DocumentDirection == EnumDocumentDirections.External,
                    ValueFieldCode = "Addressee",
                    ValueDescriptionFieldCode = "Addressee",
                    Format = ""
                },
                new SystemUIElements()
                {
                    ObjectCode = "Documents",
                    ActionCode = "Modify",
                    Code = "AccessLevel",
                    TypeCode = "select",
                    Description = "Описание что это",
                    Label = "Контакт",
                    Hint = "Контактное лицо в организации, от которой получен документ",
                    ValueTypeCode = "Number",
                    IsMandatory = true,
                    IsReadOnly = false,
                    IsVisible = doc.DocumentDirection == EnumDocumentDirections.External,
                    SelectAPI = "AdminAccessLevels",
                    SelectFieldCode = "Id",
                    SelectDescriptionFieldCode = "Name",
                    ValueFieldCode = "AccessLevel",
                    ValueDescriptionFieldCode = "AccessLevelName",
                    Format = ""
                },
            };
            return new JsonResult(doc, metaData, this);
        }

        // POST: api/Documents
        public IHttpActionResult Post([FromBody]int templateId)
        {
            var cxt = DmsResolver.Current.Get<UserContext>().Get();
            var docProc = DmsResolver.Current.Get<IDocumentService>();
            return Get(docProc.AddDocumentByTemplateDocument(cxt, templateId));
            //return new JsonResult(null,this);
        }

        // PUT: api/Documents/5
        public IHttpActionResult Put([FromBody]ModifyDocument model)
        {
            var cxt = DmsResolver.Current.Get<UserContext>().Get();
            var docProc = DmsResolver.Current.Get<IDocumentService>();
            return Get(docProc.ModifyDocument(cxt, model));
            //return new JsonResult(null, this);
        }

        // DELETE: api/Documents/5
        public void Delete(int id)
        {
        }
    }
}
