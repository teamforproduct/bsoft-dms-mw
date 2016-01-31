using BL.CrossCutting.Context;
using BL.CrossCutting.DependencyInjection;
using BL.CrossCutting.Interfaces;
using BL.Logic.DocumentCore;
using BL.Model.DocumentCore;
using BL.Model.SystemCore;
using DMS_WebAPI.Results;
using DMS_WebAPI.Utilities;
using System.Collections.Generic;
using System.Web;
using System.Web.Http;
using BL.Logic.DocumentCore.Interfaces;

namespace DMS_WebAPI.Controllers
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
            var docs = docProc.GetDocuments(cxt, filters);
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
                    Code = "DocumentSubject",
                    Description = "Описание что это",
                    Label = "Тематика документа",
                    Hint = "Выберите из словаря тематику документа",
                    ValueTypeCode = "Number",
                    IsMandatory = false,
                    IsReadOnly = false,
                    IsVisible = true,
                    SelectAPI = "api/DictionaryDocumentSubjects",
                    SelectFiler = "",
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
                    Description = "Описание что это",
                    Label = "Краткое содержание",
                    Hint = "Введите краткое содержание",
                    ValueTypeCode = "String",
                    IsMandatory = true,
                    IsReadOnly = false,
                    IsVisible = true,
                    SelectAPI = "",
                    SelectFiler = "",
                    SelectFieldCode = "",
                    SelectDescriptionFieldCode = "",
                    ValueFieldCode = "Description",
                    ValueDescriptionFieldCode = "Description",
                    Format = ""
                },
                new SystemUIElements()
                {
                    ObjectCode = "Documents",
                    ActionCode = "Modify",
                    Code = "GeneralInfo",
                    Description = "Описание что это",
                    Label = "",
                    Hint = "",
                    ValueTypeCode = "String",
                    IsMandatory = false,
                    IsReadOnly = true,
                    IsVisible = true,
                    SelectAPI = "",
                    SelectFiler = "",
                    SelectFieldCode = "",
                    SelectDescriptionFieldCode = "",
                    ValueFieldCode = "GeneralInfo",
                    ValueDescriptionFieldCode = "GeneralInfo",
                    Format = ""
                },
                new SystemUIElements()
                {
                    ObjectCode = "Documents",
                    ActionCode = "Modify",
                    Code = "SenderAgent",
                    Description = "Описание что это",
                    Label = "Организация",
                    Hint = "Контрагент, от которого получен документ",
                    ValueTypeCode = "Number",
                    IsMandatory = true,
                    IsReadOnly = false,
                    IsVisible = doc.DocumentDirectionId == 1?true:false,
                    SelectAPI = "api/DictionaryAgents",
                    SelectFiler = "IsInner:False",
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
                    Code = "SenderPerson",
                    Description = "Описание что это",
                    Label = "Контакт",
                    Hint = "Контактное лицо в организации, от которой получен документ",
                    ValueTypeCode = "Number",
                    IsMandatory = true,
                    IsReadOnly = false,
                    IsVisible = doc.DocumentDirectionId == 1?true:false,
                    SelectAPI = "api/DictionaryContactPerson",
                    SelectFiler = "AgentId:@SenderAgent.SenderAgentId",
                    SelectFieldCode = "Id",
                    SelectDescriptionFieldCode = "Name",
                    ValueFieldCode = "SenderPersonId",
                    ValueDescriptionFieldCode = "SenderPersonName",
                    Format = ""
                }

            };

            return new JsonResult(doc, metaData, this);
        }

        // POST: api/Documents
        public IHttpActionResult Post([FromBody]int templateId)
        {
            var cxt = DmsResolver.Current.Get<UserContext>().Get();
            var docProc = DmsResolver.Current.Get<IDocumentService>();
            docProc.AddDocumentByTemplateDocument(cxt, templateId);
            return new JsonResult(null,this);
        }

        // PUT: api/Documents/5
        public IHttpActionResult Put([FromBody]ModifyDocument model)
        {
            var cxt = DmsResolver.Current.Get<UserContext>().Get();
            var docProc = DmsResolver.Current.Get<IDocumentService>();
            docProc.ModifyDocument(cxt, model);
            return new JsonResult(null, this);
        }

        // DELETE: api/Documents/5
        public void Delete(int id)
        {
        }
    }
}
