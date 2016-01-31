using BL.Model.DocumentCore;

namespace BL.Database.DatabaseContext
{
    public interface IDatabaseAdapter
    {
        DBModel.Document.Documents AddDocument(DmsContext dbContext, FullDocument document);
        void UpdateDocument(DmsContext dbContext, FullDocument document);
        int AddTemplate(DmsContext dbContext, BaseTemplateDocument template);
    }
}