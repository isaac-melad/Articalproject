using ArticalProject.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace ArticalProject.Data.DataEf;

/// <summary>
/// هذا هو سياق قاعدة البيانات لتطبيق ArticalProject.
/// </summary>
public class DataContextEf(DbContextOptions<DataContextEf> options) : DbContext(options)
{
    /// <summary>
    /// جداول الفئات في قاعدة البيانات.
    /// </summary>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Category>().HasData(
            new Category { Id = 1, Name = "تطوير الويب" },
            new Category { Id = 2, Name = "تطبيقات الموبايل" },
            new Category { Id = 3, Name = "الذكاء الاصطناعي" },
            new Category { Id = 4, Name = "أمن المعلومات" },
            new Category { Id = 5, Name = "برمجة" },
            new Category { Id = 6, Name = "الحوسبة السحابية" },
            new Category { Id = 7, Name = "قواعد البيانات" },
            new Category { Id = 8, Name = "تجربة المستخدم" },
            new Category { Id = 9, Name = "تطوير الألعاب" },
            new Category { Id = 10, Name = "تقنيات حديثة" },
            new Category { Id = 11, Name = "إنترنت الأشياء" },
            new Category { Id = 12, Name = "تعلم الآلة" },
            new Category { Id = 13, Name = "تحليل البيانات" },
            new Category { Id = 14, Name = "أنظمة التشغيل" },
            new Category { Id = 15, Name = "شبكات الحاسوب" }
        );
    }
    public required DbSet<Category> Categories { get; set; }
    public required DbSet<Author> Authors { get; set; }
    public required DbSet<AuthorPost> AuthorPosts { get; set; }
}
