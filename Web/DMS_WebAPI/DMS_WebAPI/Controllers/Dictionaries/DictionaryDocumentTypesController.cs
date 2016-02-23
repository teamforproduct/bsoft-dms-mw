using BL.CrossCutting.DependencyInjection;
using BL.Logic.DictionaryCore.Interfaces;
using BL.Model.DictionaryCore;
using BL.Model.DictionaryCore.IncomingModel;
using DMS_WebAPI.Results;
using DMS_WebAPI.Utilities;
using System.Web.Http;
using BL.Model.Enums;

namespace DMS_WebAPI.Controllers.Dictionaries
{
    [Authorize]
    public class DictionaryDocumentTypesController : ApiController
    {
        /// <summary>
        /// Возвращает список типов документов
        /// </summary>
        /// <param name="filter">Параметры для фильтрации типов документа</param>
        /// <returns>Cписок типов документов</returns>
        // GET: api/DictionaryDocumentTypes
        public IHttpActionResult Get([FromUri] FilterDictionaryDocumentType filter)
        {
            var cxt = DmsResolver.Current.Get<UserContext>().Get();
            var tmpDictProc = DmsResolver.Current.Get<IDictionaryService>();
            var tmpDicts = tmpDictProc.GetDictionaryDocumentTypes(cxt, filter);
            return new JsonResult(tmpDicts, this);
        }

        // GET: api/DictionaryDocumentTypes/5
        public IHttpActionResult Get(int id)
        {
            var cxt = DmsResolver.Current.Get<UserContext>().Get();
            var tmpDictProc = DmsResolver.Current.Get<IDictionaryService>();
            var tmpDict = tmpDictProc.GetDictionaryDocumentType(cxt, id);
            return new JsonResult(tmpDict, this);
        }

        /// <summary>
        /// Добавление словаря тип документа
        /// </summary>
        /// <param name="model"></param>
        /// <returns>Измененный запись словаря типа документа</returns>
        public IHttpActionResult Post([FromBody]ModifyDictionaryDocumentType model)
        {
            var cxt = DmsResolver.Current.Get<UserContext>().Get();
            var tmpDict = DmsResolver.Current.Get<IDictionaryService>();
            return Get((int)tmpDict.ExecuteAction(EnumDictionaryAction.AddDocumentType,  cxt, model));
        }

        /// <summary>
        /// Изменение словаря тип документа 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="model"></param>
        /// <returns>Измененный запись словаря типа документа</returns>
        public IHttpActionResult Put(int id, [FromBody]ModifyDictionaryDocumentType model)
        {
            model.Id = id;
            var cxt = DmsResolver.Current.Get<UserContext>().Get();
            var tmpDict = DmsResolver.Current.Get<IDictionaryService>();
            tmpDict.ExecuteAction(EnumDictionaryAction.ModifyDocumentType, cxt, model);
            return Get(model.Id);
        }

    }
}