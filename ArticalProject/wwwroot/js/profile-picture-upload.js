/**
 * Profile Picture Upload Handler
 * Handles the profile picture upload functionality for the user profile page
 */

document.addEventListener('DOMContentLoaded', function() {
    const profilePictureContainer = document.querySelector('.profile-picture-container');
    const profilePictureInput = document.createElement('input');
    profilePictureInput.type = 'file';
    profilePictureInput.accept = 'image/*';
    profilePictureInput.style.display = 'none';
    profilePictureInput.id = 'profile-picture-input';
    document.body.appendChild(profilePictureInput);
    
    // Handle click on profile picture container
    if (profilePictureContainer) {
        profilePictureContainer.addEventListener('click', function() {
            profilePictureInput.click();
        });
    }
    
    // Handle file selection
    profilePictureInput.addEventListener('change', function(e) {
        if (e.target.files && e.target.files[0]) {
            const reader = new FileReader();
            
            reader.onload = function(event) {
                // For demo purposes, we'll just set the image source directly
                // In a real application, you would upload the image to a server
                const profilePicture = document.querySelector('.profile-picture') || document.createElement('img');
                profilePicture.src = event.target.result;
                profilePicture.classList.add('profile-picture');
                profilePicture.alt = 'صورة الملف الشخصي';
                
                const placeholder = document.querySelector('.profile-picture-placeholder');
                if (placeholder) {
                    placeholder.remove();
                }
                
                if (!document.querySelector('.profile-picture')) {
                    profilePictureContainer.prepend(profilePicture);
                }
                
                // Set the hidden input value to the base64 image data
                const profilePictureUrl = document.getElementById('profile-picture-url');
                if (profilePictureUrl) {
                    profilePictureUrl.value = event.target.result;
                }
                
                // Show a success message
                showNotification('تم تحميل الصورة بنجاح', 'success');
            };
            
            reader.readAsDataURL(e.target.files[0]);
        }
    });
    
    // Function to show notification
    function showNotification(message, type = 'info') {
        const notification = document.createElement('div');
        notification.className = `alert alert-${type} alert-dismissible fade show notification-toast`;
        notification.role = 'alert';
        notification.innerHTML = `
            ${message}
            <button type="button" class="btn-close" data-bs-dismiss="alert" aria-label="Close"></button>
        `;
        
        document.body.appendChild(notification);
        
        // Position the notification
        notification.style.position = 'fixed';
        notification.style.top = '20px';
        notification.style.right = '20px';
        notification.style.zIndex = '9999';
        notification.style.minWidth = '250px';
        notification.style.boxShadow = '0 0.5rem 1rem rgba(0,0,0,.15)';
        notification.style.animation = 'fadeInRight 0.5s';
        
        // Auto dismiss after 3 seconds
        setTimeout(() => {
            notification.style.animation = 'fadeOutRight 0.5s';
            setTimeout(() => {
                notification.remove();
            }, 500);
        }, 3000);
    }
    
    // Add animation styles
    const style = document.createElement('style');
    style.textContent = `
        @keyframes fadeInRight {
            from {
                opacity: 0;
                transform: translateX(50px);
            }
            to {
                opacity: 1;
                transform: translateX(0);
            }
        }
        
        @keyframes fadeOutRight {
            from {
                opacity: 1;
                transform: translateX(0);
            }
            to {
                opacity: 0;
                transform: translateX(50px);
            }
        }
    `;
    document.head.appendChild(style);
});