using ArticalProject.Core.Models;
using ArticalProject.Data.DataEf;


namespace ArticalProject.Data.Repo
{
    public class RepoAuthor(DataContextEf db) : IRepo<Author>
    {
        private readonly DataContextEf _db = db ?? throw new ArgumentNullException(nameof(db));

        public void Add(Author item)
        {
            _db.Authors.Add(item);
            _db.SaveChanges();
        }

        public void Update(Author item)
        {
            _db.Update(item);
            _db.SaveChanges();
        }

        public void Delete(Author item)
        {
            _db.Authors.Remove(item);
            _db.SaveChanges();
        }

        public List<Author> Search(string searchItem)
        {
            if (string.IsNullOrEmpty(searchItem))
                return new List<Author>();
                
            return _db.Authors.Where(x =>
                x.FullName != null && x.FullName.Contains(searchItem)).ToList();
        }

        public Author GetById(int id)
        {
            var author = _db.Authors.Find(id);
            return author ?? throw new KeyNotFoundException($"Author with ID {id} not found.");
        }

        public IEnumerable<Author> GetAll()
        {
            return _db.Authors.ToList();
        }

        public void Delete(int id)
        {
            var author = _db.Authors.Find(id);
            if (author != null)
            {
                _db.Authors.Remove(author);
                _db.SaveChanges();
            }
        }

        public IEnumerable<Author> SearchByName(string name)
        {
            return _db.Authors.Where(x => x.FullName != null && x.FullName.Contains(name)).ToList();
        }

        public Author GetDataByUser(string userId)
        {

            if (string.IsNullOrEmpty(userId))
                throw new ArgumentException("userId cannot be null or empty");

            return _db.Authors
                .FirstOrDefault(x => x.UserId == userId) ?? throw new KeyNotFoundException($"No author found for user {userId}");
        }
    }
}