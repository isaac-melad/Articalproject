/**
 * صفحة تحرير معلومات المؤلف
 * معالجة معاينة الصور وحذفها
 */

/**
 * معاينة الصورة المحددة
 * @param {HTMLInputElement} input - عنصر إدخال الملف
 */
function previewImage(input) {
    // العناصر المستخدمة في معاينة الصورة
    const preview = document.getElementById('imagePreview');
    const existingImages = document.querySelectorAll('.current-profile-image');

    if (input.files && input.files[0]) {
        // قراءة وعرض الصورة المحددة
        const reader = new FileReader();
        reader.onload = function (e) {
            preview.src = e.target.result;
            preview.style.display = 'block';
            // إخفاء الصور الحالية
            existingImages.forEach(img => img.parentElement.style.display = 'none');
        }
        reader.readAsDataURL(input.files[0]);
    } else {
        // إذا لم يتم تحديد صورة، إظهار الصور الحالية
        preview.style.display = 'none';
        existingImages.forEach(img => img.parentElement.style.display = 'block');
    }
}

/**
 * حذف صورة الملف الشخصي الحالية
 * @param {HTMLButtonElement} button - زر الحذف
 */
function deleteExistingImage(button) {
    // العثور على العناصر المطلوبة للحذف
    const container = button.closest('.position-relative');
    const hiddenInput = document.querySelector('input[name="ProfileImageUrl"]');

    // تأكيد الحذف من المستخدم
    if (confirm('هل أنت متأكد من حذف الصورة الحالية؟')) {
        container.style.display = 'none';
        hiddenInput.value = ''; // مسح رابط الصورة
    }
}

