namespace ArticalProject.Data.Repo;

public interface IRepo<T> where T : class
{
    T GetById(int id);
    IEnumerable<T> GetAll();
    void Add(T entity);
    void Update(T entity);
    void Delete(int id);
    IEnumerable<T> SearchByName(string name);
    T GetDataByUser(string userId);
}
