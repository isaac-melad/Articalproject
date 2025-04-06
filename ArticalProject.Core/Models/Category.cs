using System.ComponentModel.DataAnnotations;

namespace ArticalProject.Core.Models;

// تعريف الفئة: تمثل فئة التصنيف
public class Category
{
    // معرف الفئة (المفتاح الأساسي)
    [Key]
    [Display(Name = "المعرف")]
    public int Id { get; set; }

    // اسم الفئة (مطلوب)
    [Display(Name = "الاسم")]
    [Required(ErrorMessage = "الاسم مطلوب")]
    [StringLength(50, ErrorMessage = "اسم التصنيف لا يمكن أن يتجاوز 50 حرف")]
    public string Name { get; set; } = string.Empty;

    // Navigation property for AuthorPost
    public ICollection<AuthorPost>? Posts { get; set; }
}