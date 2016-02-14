using BL.CrossCutting.DependencyInjection;
using BL.Model.DocumentCore;
using BL.Model.SystemCore;
using DMS_WebAPI.Results;
using DMS_WebAPI.Utilities;
using System.Collections.Generic;
using System.Web.Http;
using BL.Logic.DocumentCore.Interfaces;
using BL.Model.Enums;
using System.Web.Http.Description;

namespace DMS_WebAPI.Controllers.Documents
{
    [Authorize]
    public class DocumentsController : ApiController
    {
        /// <summary>
        /// Получение списка документов
        /// </summary>
        /// <param name="filter">модель фильтра документов</param>
        /// <param name="paging">paging</param>
        /// <returns></returns>
        [ResponseType(typeof(JsonResult))]
        public IHttpActionResult Get([FromUri] FilterDocument filter, [FromUri]UIPaging paging)
        {
            var cxt = DmsResolver.Current.Get<UserContext>().Get();
            var docProc = DmsResolver.Current.Get<IDocumentService>();
            var docs = docProc.GetDocuments(cxt, filter, paging);
            var res = new JsonResult(docs, this);
            res.Paging = paging;
            return res;
        }

        /// <summary>
        /// Получение документа по ИД
        /// </summary>
        /// <param name="id">ИД Документа</param>
        /// <returns>Документ</returns>
        public IHttpActionResult Get(int id)
        {
            var cxt = DmsResolver.Current.Get<UserContext>().Get();
            var docProc = DmsResolver.Current.Get<IDocumentService>();

            var doc = docProc.GetDocument(cxt, id);
            var metaData = docProc.GetModifyMetaData(cxt, doc);
            /* metaData = new List<BaseSystemUIElement>()
            {
                new BaseSystemUIElement()
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
                new BaseSystemUIElement()
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
                new BaseSystemUIElement()
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
                new BaseSystemUIElement()
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
                new BaseSystemUIElement()
                {
                    ObjectCode = "Documents",
                    ActionCode = "Modify",
                    Code = "SenderAgentPerson",
                    TypeCode = "select",
                    Description = "Контактное лицо в организации",
                    Label = "Контакт",
                    Hint = "Контактное лицо в организации, от которой получен документ",
                    ValueTypeCode = "number",
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
                new BaseSystemUIElement()
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
                new BaseSystemUIElement()
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
                new BaseSystemUIElement()
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
                new BaseSystemUIElement()
                {
                    ObjectCode = "Documents",
                    ActionCode = "Modify",
                    Code = "AccessLevel",
                    TypeCode = "select",
                    Description = "Описание что это",
                    Label = "Контакт",
                    Hint = "Контактное лицо в организации, от которой получен документ",
                    ValueTypeCode = "number",
                    IsMandatory = true,
                    IsReadOnly = false,
                    IsVisible = true,
                    SelectAPI = "AdminAccessLevels",
                    SelectFieldCode = "Id",
                    SelectDescriptionFieldCode = "Name",
                    ValueFieldCode = "AccessLevel",
                    ValueDescriptionFieldCode = "AccessLevelName",
                    Format = ""
                },
            };*/
            return new JsonResult(doc, metaData, this);
        }

        /// <summary>
        /// Добавление документа по шаблону
        /// </summary>
        /// <param name="model"></param>
        /// <returns>Добавленный документ</returns>
        public IHttpActionResult Post([FromBody]AddDocumentByTemplateDocument model)
        {
            var cxt = DmsResolver.Current.Get<UserContext>().Get(model.CurrentPositionId);
            var docProc = DmsResolver.Current.Get<IDocumentService>();
            return Get(docProc.AddDocumentByTemplateDocument(cxt, model));
            //return new JsonResult(null,this);
        }

        /// <summary>
        /// Модификация документа 
        /// </summary>
        /// <param name="model">Модель для обновления документа</param>
        /// <returns>Обновленный документ</returns>
        public IHttpActionResult Put([FromBody]ModifyDocument model)
        {
            var cxt = DmsResolver.Current.Get<UserContext>().Get();
            var docProc = DmsResolver.Current.Get<IDocumentService>();
            return Get(docProc.ModifyDocument(cxt, model));
            //return new JsonResult(null, this);
        }

        /// <summary>
        /// Удаление документа 
        /// </summary>
        /// <param name="id">ИД документа</param>
        /// <returns></returns>
        public IHttpActionResult Delete(int id)
        {
            var cxt = DmsResolver.Current.Get<UserContext>().Get();
            var docProc = DmsResolver.Current.Get<IDocumentService>();
            docProc.DeleteDocument(cxt, id);
            return new JsonResult(null, this);
        }

    }
}
