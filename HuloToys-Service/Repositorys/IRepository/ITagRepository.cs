namespace HuloToys_Service.Repro.IRepository
{
    public interface ITagRepository
    {
        public Task<List<string>> GetAllTagByArticleID(long articleID);
    }
}
