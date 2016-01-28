using BL.Model.DocumentCore;

namespace BL.Database.DatabaseContext
{
    public interface IDatabaseAdapter
    {
        void AddDocument(DmsContext dbContext, BaseDocument document);
        void UpdateDocument(DmsContext dbContext, BaseDocument document);
        int AddTemplate(DmsContext dbContext, BaseTemplateDocument template);
    }
}