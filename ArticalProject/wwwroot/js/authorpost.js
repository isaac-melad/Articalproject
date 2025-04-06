// Form handling for author post creation and editing
document.addEventListener('DOMContentLoaded', function () {
    // Get the form element
    const form = document.querySelector('form[asp-action="Create"], form[asp-action="Edit"]');

    if (form) {
        // Handle image preview functionality
        setupImagePreview();

        // Setup form validation
        setupFormValidation(form);

        // Focus the first input field for better UX
        const firstInput = form.querySelector('input[type="text"], textarea');
        if (firstInput) {
            firstInput.focus();
        }
    }

    // Initialize tooltips for action buttons
    initializeTooltips();

    // Add table row hover effects
    setupTableInteractions();

    // Add animation to alerts
    animateAlerts();
});

/**
 * Setup image preview functionality with drag and drop
 */
function setupImagePreview() {
    const imageInput = document.getElementById('imageFile');
    const imagePreview = document.getElementById('imagePreview');
    const previewContainer = document.getElementById('imagePreviewContainer');
    const removeButton = document.getElementById('removeImage');
    const uploadArea = document.querySelector('.image-upload-area');

    if (imageInput && imagePreview && previewContainer) {
        // Process selected file function
        const processFile = (file) => {
            // Validate file type
            const validTypes = ['image/jpeg', 'image/png', 'image/gif', 'image/jpg'];
            if (!validTypes.includes(file.type)) {
                showError('يرجى اختيار صورة بتنسيق صالح (JPG, PNG, GIF)');
                imageInput.value = '';
                return false;
            }

            // Validate file size (max 5MB)
            if (file.size > 5 * 1024 * 1024) {
                showError('حجم الملف يتجاوز الحد المسموح به (5 ميجابايت)');
                imageInput.value = '';
                return false;
            }

            // Create a preview
            const reader = new FileReader();
            reader.onload = function(e) {
                imagePreview.src = e.target.result;
                previewContainer.classList.remove('d-none');

                // Add success indicator to upload area
                if (uploadArea) {
                    uploadArea.classList.add('border-success');
                    uploadArea.classList.remove('border-danger');

                    // Update upload area text
                    const uploadText = uploadArea.querySelector('.upload-text');
                    if (uploadText) {
                        uploadText.innerHTML = `<span class="text-success">تم اختيار الصورة:</span> ${file.name}`;
                    }
                }
            };
            reader.readAsDataURL(file);
            return true;
        };

        // When a new image is selected via input
        imageInput.addEventListener('change', function() {
            // Check if a file is selected
            if (imageInput.files && imageInput.files[0]) {
                processFile(imageInput.files[0]);
            } else {
                // No file selected, hide preview
                previewContainer.classList.add('d-none');

                // Reset upload area
                if (uploadArea) {
                    uploadArea.classList.remove('border-success', 'border-danger');
                    const uploadText = uploadArea.querySelector('.upload-text');
                    if (uploadText) {
                        uploadText.textContent = 'اختر صورة أو اسحبها وأفلتها هنا';
                    }
                }
            }
        });

        // Remove image button
        if (removeButton) {
            removeButton.addEventListener('click', function() {
                imageInput.value = '';
                previewContainer.classList.add('d-none');

                // Reset upload area
                if (uploadArea) {
                    uploadArea.classList.remove('border-success', 'border-danger');
                    const uploadText = uploadArea.querySelector('.upload-text');
                    if (uploadText) {
                        uploadText.textContent = 'اختر صورة أو اسحبها وأفلتها هنا';
                    }
                }
            });
        }

        // Setup drag and drop if upload area exists
        if (uploadArea) {
            // Prevent default drag behaviors
            ['dragenter', 'dragover', 'dragleave', 'drop'].forEach(eventName => {
                uploadArea.addEventListener(eventName, preventDefaults, false);
            });

            function preventDefaults(e) {
                e.preventDefault();
                e.stopPropagation();
            }

            // Highlight drop area when item is dragged over it
            ['dragenter', 'dragover'].forEach(eventName => {
                uploadArea.addEventListener(eventName, highlight, false);
            });

            ['dragleave', 'drop'].forEach(eventName => {
                uploadArea.addEventListener(eventName, unhighlight, false);
            });

            function highlight() {
                uploadArea.classList.add('drag-over');
            }

            function unhighlight() {
                uploadArea.classList.remove('drag-over');
            }

            // Handle dropped files
            uploadArea.addEventListener('drop', handleDrop, false);

            function handleDrop(e) {
                const dt = e.dataTransfer;
                const files = dt.files;

                if (files.length > 0) {
                    // Update the file input value
                    const fileList = new DataTransfer();
                    fileList.items.add(files[0]);
                    imageInput.files = fileList.files;

                    // Process the file
                    processFile(files[0]);
                }
            }
        }
    }
}

/**
 * Setup form validation and submission
 * @param {HTMLFormElement} form - The form element
 */
function setupFormValidation(form) {
    if (!form) return;

    // Listen for form submission
    form.addEventListener('submit', function(event) {
        // Get the required form fields
        const postTitleField = document.getElementById('PostTitle');
        const contentField = document.getElementById('Content');
        const categoryIdField = document.getElementById('CategoryId');
        const submitButton = document.getElementById('submitButton');
        const spinner = submitButton ? submitButton.querySelector('.spinner-border') : null;

        // Ensure required fields are filled before submitting
        let isValid = true;

        // Validate title
        if (postTitleField && !postTitleField.value.trim()) {
            isValid = false;
            showFieldError(postTitleField, 'يرجى إدخال عنوان المنشور');
        }

        // Fallback if TinyMCE is not loaded
        if (contentField && !contentField.value.trim()) {
            isValid = false;
            showFieldError(contentField, 'يرجى إدخال محتوى المنشور');
        }

        // Validate category
        if (categoryIdField && !categoryIdField.value) {
            isValid = false;
            showFieldError(categoryIdField, 'يرجى اختيار التصنيف');
        }

        // If the form is not valid, prevent submission
        if (!isValid) {
            event.preventDefault();
            return false;
        }

        // Show loading spinner
        if (submitButton && spinner) {
            submitButton.disabled = true;
            spinner.classList.remove('d-none');
            submitButton.querySelector('span:not(.spinner-border)').textContent = 'جاري الحفظ...';
        }

        // Debug log to ensure form submission is working
        console.log('Form is valid, submitting...');
        return true;
    });

    // Clear validation messages when user starts typing
    form.querySelectorAll('input, textarea, select').forEach(element => {
        element.addEventListener('input', function() {
            clearFieldError(this);
        });

        // Also clear on focus
        element.addEventListener('focus', function() {
            clearFieldError(this);
        });
    });
}

/**
 * Show error message for a specific field
 * @param {HTMLElement} field - The field element
 * @param {string} message - The error message
 */
function showFieldError(field, message) {
    if (!field) return;

    // Find the error span for this field
    const fieldName = field.getAttribute('name');
    const errorSpan = document.querySelector(`[data-valmsg-for="${fieldName}"]`);

    if (errorSpan) {
        errorSpan.textContent = message;
    }

    // Add error class to the field
    field.classList.add('is-invalid');
}

/**
 * Clear error message for a field
 * @param {HTMLElement} field - The field element
 */
function clearFieldError(field) {
    if (!field) return;

    // Find the error span for this field
    const fieldName = field.getAttribute('name');
    const errorSpan = document.querySelector(`[data-valmsg-for="${fieldName}"]`);

    if (errorSpan) {
        errorSpan.textContent = '';
    }

    // Remove error class
    field.classList.remove('is-invalid');
}

/**
 * Show general error message
 * @param {string} message - The error message
 * @param {HTMLElement} container - Optional container for the error message
 */
function showError(message, container = null) {
    if (container) {
        container.textContent = message;
        return;
    }

    // Create a toast notification if no container is provided
    const toast = document.createElement('div');
    toast.classList.add('toast', 'position-fixed', 'top-0', 'end-0', 'm-3', 'bg-danger', 'text-white', 'animate__animated', 'animate__fadeInRight');
    toast.setAttribute('role', 'alert');
    toast.setAttribute('aria-live', 'assertive');
    toast.setAttribute('aria-atomic', 'true');

    toast.innerHTML = `
        <div class="toast-header bg-danger text-white">
            <i class="bi bi-exclamation-triangle-fill me-2"></i>
            <strong class="me-auto">تنبيه</strong>
            <button type="button" class="btn-close" data-bs-dismiss="toast" aria-label="Close"></button>
        </div>
        <div class="toast-body">
            ${message}
        </div>
    `;

    document.body.appendChild(toast);

    // Initialize and show the toast
    const bsToast = new bootstrap.Toast(toast, {
        delay: 5000
    });
    bsToast.show();

    // Remove after it's hidden
    toast.addEventListener('hidden.bs.toast', function() {
        document.body.removeChild(toast);
    });
}

/**
 * Initialize tooltips for action buttons
 */
function initializeTooltips() {
    const tooltipTriggerList = [].slice.call(document.querySelectorAll('[data-bs-toggle="tooltip"]'));
    tooltipTriggerList.map(function (tooltipTriggerEl) {
        return new bootstrap.Tooltip(tooltipTriggerEl, {
            boundary: document.body,
            delay: { show: 500, hide: 100 }
        });
    });
}

/**
 * Setup table row interactions
 */
function setupTableInteractions() {
    // Add hover effect to table rows
    const postRows = document.querySelectorAll('.post-row');
    postRows.forEach(row => {
        row.addEventListener('mouseenter', function() {
            this.classList.add('row-highlight');
        });
        row.addEventListener('mouseleave', function() {
            this.classList.remove('row-highlight');
        });
    });

    // Add click effect to action buttons
    const actionButtons = document.querySelectorAll('.action-btn');
    actionButtons.forEach(btn => {
        btn.addEventListener('mousedown', function() {
            this.style.transform = 'translateY(0)';
        });
        btn.addEventListener('mouseup', function() {
            this.style.transform = '';
        });
        btn.addEventListener('mouseleave', function() {
            this.style.transform = '';
        });
    });
}

/**
 * Animate alerts on page load
 */
function animateAlerts() {
    const alerts = document.querySelectorAll('.alert');
    alerts.forEach((alert, index) => {
        alert.classList.add('animate__animated', 'animate__fadeInDown');
        alert.style.animationDelay = `${index * 0.2}s`;

        // Auto-dismiss alerts after 5 seconds
        setTimeout(() => {
            if (alert && alert.parentNode) {
                alert.classList.replace('animate__fadeInDown', 'animate__fadeOutUp');
                alert.addEventListener('animationend', () => {
                    if (alert.parentNode) {
                        alert.parentNode.removeChild(alert);
                    }
                });
            }
        }, 5000 + (index * 200));
    });
}