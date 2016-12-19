using BL.CrossCutting.Context;
using BL.CrossCutting.DependencyInjection;
using BL.Logic.AdminCore.Interfaces;
using BL.Logic.DictionaryCore.Interfaces;
using BL.Logic.SystemCore.Interfaces;
using BL.Logic.SystemServices.FullTextSearch;
using BL.Model.AdminCore;
using BL.Model.AdminCore.FilterModel;
using BL.Model.AdminCore.InternalModel;
using BL.Model.Database;
using BL.Model.DictionaryCore.FilterModel;
using BL.Model.DictionaryCore.FrontModel;
using BL.Model.SystemCore.Filters;
using BL.Model.SystemCore.FrontModel;
using BL.Model.WebAPI.FrontModel;
using DMS_WebAPI.Results;
using DMS_WebAPI.Utilities;
using System.Collections.Generic;
using System.Diagnostics;
using System.Web.Http;
using System.Web.Http.Description;

namespace DMS_WebAPI.ControllersV3.System
{
    /// <summary>
    /// Контекст пользователя (Все пользователя являются сотрудниками, но у сотрудника может быть выключена возможность авторизации)
    /// </summary>
    [Authorize]
    [RoutePrefix(ApiPrefix.V3 + "User")]
    public class UserController : ApiController
    {
        Stopwatch stopWatch = new Stopwatch();

    }
}