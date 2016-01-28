using BL.Model.DocumentCore;

namespace BL.Database.DatabaseContext
{
    public interface IDatabaseAdapter
    {
        void AddDocument(DmsContext dbContext, BaseDocument document);
        void UpdateBaseDocument(DmsContext dbContext, BaseDocument document);
        int AddTemplate(DmsContext dbContext, FullTemplateDocument template);
    }
}