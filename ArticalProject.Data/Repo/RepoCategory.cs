using ArticalProject.Core.Models;
using ArticalProject.Data.DataEf;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace ArticalProject.Data.Repo;

public class RepoCategory : IRepo<Category>
{
    private readonly DataContextEf context;

    public RepoCategory(DataContextEf context)
    {
        this.context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public Category GetById(int id)
    {
        var category = context.Categories.Find(id);
        if (category == null)
            throw new KeyNotFoundException($"Category with id {id} not found");
        return category;
    }

    public IEnumerable<Category> GetAll()
    {
        return context.Categories.ToList();
    }

    public void Add(Category entity)
    {
        context.Categories.Add(entity);
        context.SaveChanges();
    }

    public void Update(Category entity)
    {
        context.Categories.Update(entity);
        context.SaveChanges();
    }

    public void Delete(int id)
    {
        var category = GetById(id);
        context.Categories.Remove(category);
        context.SaveChanges();
    }

    public IEnumerable<Category> SearchByName(string name)
    {
        return context.Categories
            .Where(c => c.Name != null && c.Name.Contains(name))
            .ToList();
    }

    public Category GetDataByUser(string userId)
    {
        throw new NotImplementedException();
    }
}
