using BL.Logic.DictionaryCore.Interfaces;
using BL.Model.DictionaryCore.IncomingModel;
using BL.Model.DictionaryCore.FrontModel;
using DMS_WebAPI.Results;
using DMS_WebAPI.Utilities;
using System.Web.Http;
using BL.Model.Enums;
using BL.Model.DictionaryCore.FilterModel;
using BL.CrossCutting.DependencyInjection;


namespace DMS_WebAPI.Controllers.Dictionaries
{
    /// <summary>
    /// Справочник типов контактов
    /// </summary>
    [Authorize]
    public class DictionaryContactTypesController : ApiController
    {
        /// <summary>
        /// Возвращает список типов контактов
        /// </summary>
        /// <param name="filter">Параметры для фильтрации типов контактов</param>
        /// <returns>Cписок типов контактов</returns>
        // GET: api/DictionaryContactTypes
        public IHttpActionResult Get([FromUri] FilterDictionaryContactType filter)
        {
            var ctx = DmsResolver.Current.Get<UserContext>().Get();
            var tmpDictProc = DmsResolver.Current.Get<IDictionaryService>();
            var tmpDicts = tmpDictProc.GetDictionaryContactTypes(ctx, filter);
            return new JsonResult(tmpDicts, this);
        }

        // GET: api/DictionaryDocumentTypes/5
        public IHttpActionResult Get(int id)
        {
            
            var ctx = DmsResolver.Current.Get<UserContext>().Get();
            var tmpDictProc = DmsResolver.Current.Get<IDictionaryService>();
            var tmpDict = tmpDictProc.GetDictionaryContactType(ctx, id);
            return new JsonResult(tmpDict, this);
        }

        /// <summary>
        /// Добавление типа контакта
        /// </summary>
        /// <param name="model"></param>
        /// <returns>Измененный запись словаря типа документа</returns>
        public IHttpActionResult Post([FromBody]ModifyDictionaryContactType model)
        {
            var ctx = DmsResolver.Current.Get<UserContext>().Get();
            var tmpDict = DmsResolver.Current.Get<IDictionaryService>();
            return Get((int)tmpDict.ExecuteAction(EnumDictionaryActions.AddContactType, ctx, model));
        }

        /// <summary>
        /// Изменение записи тип контакта
        /// </summary>
        /// <param name="id"></param>
        /// <param name="model"></param>
        /// <returns>Измененный запись словаря тип контакта</returns>
        [HttpPut]
        public IHttpActionResult Put(int id, [FromBody]ModifyDictionaryContactType model)
        {
            model.Id = id;
            var ctx = DmsResolver.Current.Get<UserContext>().Get();
            var tmpDict = DmsResolver.Current.Get<IDictionaryService>();
            tmpDict.ExecuteAction(EnumDictionaryActions.ModifyContactType, ctx, model);
            return Get(model.Id);
        }

        /// <summary>
        /// Удаляет из справочника запись
        /// </summary>
        /// <returns>Возвращает id удаленного документа</returns> 
        public IHttpActionResult Delete([FromUri] int id)
        {
            var ctx = DmsResolver.Current.Get<UserContext>().Get();
            var tmpDict = DmsResolver.Current.Get<IDictionaryService>();

            tmpDict.ExecuteAction(EnumDictionaryActions.DeleteContactType, ctx, id);
            FrontDictionaryContactType tmp = new FrontDictionaryContactType();
            tmp.Id = id;

            return new JsonResult(tmp, this);

        }
    }
}
