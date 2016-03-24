using System.Collections.Generic;
using BL.CrossCutting.Interfaces;
using BL.Logic.SystemServices.MailWorker;
using BL.Model.DocumentCore.InternalModel;
using BL.Model.FullTextSerach;

namespace BL.Logic.SystemServices.FullTextSearch
{
    public interface IFullTextSearchService: ISystemWorkerService
    {
        void Dispose();
        IEnumerable<FullTextSearchResult> Search(IContext ctx, string text);
        IEnumerable<FullTextSearchResult> Search(IContext ctx, string text, EnumSearchObjectType objectType, int documentId);
    }
}