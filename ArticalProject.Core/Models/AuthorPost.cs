using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ArticalProject.Core.Models;

public class AuthorPost
{
    #region الخصائص الأساسية
    [Key]
    [Display(Name = "المعرف")]
    public int Id { get; init; }

    [Required(ErrorMessage = "معرف المستخدم مطلوب")]
    [Display(Name = "معرف المستخدم")]
    public string? UserId { get; init; }

    [DataType(DataType.Text)]
    [Required(ErrorMessage = "اسم المستخدم مطلوب")]
    [Display(Name = "اسم المستخدم")]
    [StringLength(50, ErrorMessage = "اسم المستخدم لا يمكن أن يتجاوز 50 حرفاً")]
    public string? UserName { get; init; }

    [DataType(DataType.Text)]
    [Required(ErrorMessage = "الاسم الكامل مطلوب")]
    [Display(Name = "الاسم الكامل")]
    [StringLength(100, ErrorMessage = "الاسم الكامل لا يمكن أن يتجاوز 100 حرف")]
    public string? FullName { get; init; }
    #endregion

    #region خصائص المنشور
    [DataType(DataType.Text)]
    [Required(ErrorMessage = "عنوان المنشور مطلوب")]
    [Display(Name = "عنوان المنشور")]
    [StringLength(200, ErrorMessage = "عنوان المنشور لا يمكن أن يتجاوز 200 حرف")]
    public string? PostTitle { get; set; }

    [DataType(DataType.MultilineText)]
    [Required(ErrorMessage = "محتوى المنشور مطلوب")]
    [Display(Name = "محتوى المنشور")]
    public string? Content { get; set; }

    [DataType(DataType.DateTime)]
    [Display(Name = "تاريخ الإنشاء")]
    public DateTime CreatedAt { get; set; } = DateTime.Now;

    [Required(ErrorMessage = "التصنيف مطلوب")]
    [Display(Name = "التصنيف")]
    public int CategoryId { get; set; }    // Changed from string PostCategory

    [DataType(DataType.Upload)]
    [Display(Name = "رابط الصورة")]
    [Url(ErrorMessage = "يجب أن يكون رابط الصورة صالحاً")]
    public string? PostImageUrl { get; set; }
    #endregion

    #region خصائص العلاقات
    public int AuthorId { get; set; }
    [ForeignKey(nameof(AuthorId))]
    public Author? Author { get; set; }

    [ForeignKey(nameof(CategoryId))]
    public Category? Category { get; set; }
    #endregion
}
