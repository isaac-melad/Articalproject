using ArticalProject.Core.Models;
using ArticalProject.Data.DataEf;

namespace ArticalProject.Data.Repo
{
    public class RepoAuthorPost(DataContextEf db)
        : IRepo<AuthorPost>, ISearchByAnyProperties<AuthorPost>, IGetPostsByUser<AuthorPost>
    {
        private readonly DataContextEf _db = db ?? throw new ArgumentNullException(nameof(db));

        // الشرح: المُنشئ الذي يقوم بتهيئة قاعدة البيانات والتحقق من صحة المدخلات.

        // الشرح: دالة لإضافة AuthorPost جديد إلى قاعدة البيانات.
        public void Add(AuthorPost item)
        {
            _db.AuthorPosts.Add(item);
            _db.SaveChanges();
        }

        // الشرح: دالة لتحديث بيانات AuthorPost موجود في قاعدة البيانات.
        public void Update(AuthorPost item)
        {
            var existingPost = _db.AuthorPosts.Find(item.Id);
            if (existingPost == null)
                throw new KeyNotFoundException($"AuthorPost with ID {item.Id} not found.");

            // الشرح: تحديث جميع الخصائص من العنصر المدخل.
            _db.Entry(existingPost).CurrentValues.SetValues(item);
            _db.SaveChanges();
        }

        // الشرح: دالة لحذف AuthorPost من قاعدة البيانات باستخدام الكائن نفسه.
        public void Delete(AuthorPost item)
        {
            _db.AuthorPosts.Remove(item);
            _db.SaveChanges();
        }

        // الشرح: دالة لحذف AuthorPost من قاعدة البيانات باستخدام المعرف.
        public void Delete(int id)
        {
            var post = _db.AuthorPosts.Find(id);
            if (post != null)
            {
                _db.AuthorPosts.Remove(post);
                _db.SaveChanges();
            }
        }

        // الشرح: دالة لجلب AuthorPost باستخدام المعرف.
        public AuthorPost GetById(int id)
        {
            var post = _db.AuthorPosts.Find(id);
            return post ?? throw new KeyNotFoundException($"AuthorPost with ID {id} not found.");
        }

        // الشرح: دالة لجلب كافة AuthorPosts من قاعدة البيانات.
        public IEnumerable<AuthorPost> GetAll()
        {
            return _db.AuthorPosts.ToList();
        }

        // الشرح: دالة للبحث عن AuthorPosts باستخدام نص البحث.
        public List<AuthorPost> Search(string searchItem)
        {
            if (string.IsNullOrEmpty(searchItem))
                return new List<AuthorPost>();

            return _db.AuthorPosts
                .Where(x => (x.PostTitle != null && x.PostTitle.Contains(searchItem)) ||
                           (x.Content != null && x.Content.Contains(searchItem)))
                .ToList();
        }

        // الشرح: دالة لجلب كافة AuthorPosts الخاصة بمؤلف معين.
        public IEnumerable<AuthorPost> GetPostsByAuthor(int authorId)
        {
            return _db.AuthorPosts.Where(x => x.AuthorId == authorId).ToList();
        }

        // الشرح: دالة للبحث عن AuthorPosts باستخدام الاسم.
        public IEnumerable<AuthorPost> SearchByName(string name)
        {
            if (string.IsNullOrEmpty(name))
                return Enumerable.Empty<AuthorPost>();

            return _db.AuthorPosts
                .Where(x => x.PostTitle != null && x.PostTitle.Contains(name))
                .ToList();
        }

        

        public IEnumerable<AuthorPost> SearchByProperty(string property)
        {
            if (string.IsNullOrEmpty(property))
            return Enumerable.Empty<AuthorPost>();

            return _db.AuthorPosts
            .Where(x => 
                (x.PostTitle != null && x.PostTitle.Contains(property)) ||
                (x.Content != null && x.Content.Contains(property)) ||
                (x.PostImageUrl != null && x.PostImageUrl.Contains(property))
            )
            .ToList();
        }


        public AuthorPost GetDataByUser(string userId)
        {
            if (string.IsNullOrEmpty(userId))
                throw new ArgumentException("userId cannot be null or empty");

            return _db.AuthorPosts
                .FirstOrDefault(x => x.UserId == userId) ?? throw new KeyNotFoundException($"No post found for user {userId}");
        }

        public IEnumerable<AuthorPost> GetPostsByUser(string userId)
        {
            if (string.IsNullOrEmpty(userId))
                throw new ArgumentException("userId cannot be null or empty");
            else 
                return _db.AuthorPosts.Where(x => x.UserId == userId).ToList();
        }
    }
}
