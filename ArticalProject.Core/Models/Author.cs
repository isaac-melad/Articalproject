using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Http;

namespace ArticalProject.Core.Models;

/// <summary>
/// يمثل بيانات المؤلف
/// </summary>
public class Author
{
    [Key]
    [Display(Name = "المعرف")]
    public int Id { get; set; }

    [Required(ErrorMessage = "User ID is required.")]
    [StringLength(100),Display(Name = "معرف المستخدم")]
    public string? UserId { get; set; }

    [Required(ErrorMessage = "User Name is required.")]
    [Display(Name = "اسم المستخدم")]
    [StringLength(50, ErrorMessage = "User Name cannot be longer than 50 characters.")]
    public string? UserName { get; set; }

    [Required(ErrorMessage = "Full Name is required.")]
    [Display(Name = "الاسم الكامل")]
    [StringLength(100, ErrorMessage = "Full Name cannot be longer than 100 characters.")]
    public string? FullName { get; set; }

    [Display(Name = "رابط الصورة")]
    [Url(ErrorMessage = "Profile Image URL must be a valid URL.")]
    public string? ProfileImageUrl { get; set; }

    [Display(Name = "الوصف")]
    [StringLength(500, ErrorMessage = "Bio cannot be longer than 500 characters.")]
    public string? Bio { get; set; }

    [Display(Name = "رابط الفيسبوك")]
    [StringLength(250),Url(ErrorMessage = "Facebook URL must be a valid URL.")]
    public string? FacebookUrl { get; set; }

    [Display(Name = "رابط الانستجرام")]
    [StringLength(250),Url(ErrorMessage = "Instagram URL must be a valid URL.")]
    public string? InstagramUrl { get; set; }

    [Display(Name = "رابط تويتر")]
    [StringLength(250),Url(ErrorMessage = "Twitter URL must be a valid URL.")]
    public string? TwitterUrl { get; set; }

    [NotMapped]
    public IFormFile? FileImg { get; set; }

    // Navigation property
    public ICollection<AuthorPost>? AuthorPosts { get; set; }
}
