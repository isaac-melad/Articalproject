using System;

namespace ArticalProject.Data.Repo;

public interface IGetPostsByUser<T> where T : class
{
    IEnumerable<T> GetPostsByUser(string userId);
}
