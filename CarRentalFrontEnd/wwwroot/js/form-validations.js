// Form validation and common functionality for add/edit forms

// Initialize date pickers with consistent settings
function initializeDatePickers() {
    $('input[type="datetime-local"]').each(function() {
        const $input = $(this);
        if (!$input.val()) {
            const now = new Date();
            const localDateTime = now.toISOString().slice(0, 16);
            $input.val(localDateTime);
        }
    });
}

// Initialize file upload previews
function initializeFilePreviews() {
    $('input[type="file"]').on('change', function(e) {
        const file = e.target.files[0];
        const $previewContainer = $(this).siblings('.file-preview');
        
        if (!file) {
            $previewContainer.hide();
            return;
        }

        // File type validation
        const validTypes = ['image/jpeg', 'image/png', 'image/gif', 'application/pdf'];
        if (!validTypes.includes(file.type)) {
            alert('Please select a valid file type (JPEG, PNG, GIF, or PDF)');
            $(this).val('');
            return;
        }

        // File size validation (5MB max)
        const maxSize = 5 * 1024 * 1024; // 5MB
        if (file.size > maxSize) {
            alert('File size must be less than 5MB');
            $(this).val('');
            return;
        }

        // Show preview for images
        if (file.type.startsWith('image/')) {
            const reader = new FileReader();
            reader.onload = function(e) {
                const $preview = $previewContainer.find('img');
                if ($preview.length === 0) {
                    $previewContainer.html(`
                        <div class="mt-2">
                            <small class="text-white-50">Preview:</small>
                            <img class="img-thumbnail mt-2" style="max-width: 200px; max-height: 200px;" />
                        </div>
                    `);
                }
                $previewContainer.find('img').attr('src', e.target.result);
                $previewContainer.show();
            };
            reader.readAsDataURL(file);
        } else {
            $previewContainer.hide();
        }
    });
}

// Initialize numeric input validation
function initializeNumericInputs() {
    $('input[type="number"]').on('input', function() {
        const $input = $(this);
        let value = $input.val();
        
        // Remove any non-numeric characters except decimal point
        value = value.replace(/[^0-9.]/g, '');
        
        // Ensure only one decimal point
        const decimalCount = (value.match(/\./g) || []).length;
        if (decimalCount > 1) {
            value = value.replace(/\.+$/, '');
        }
        
        $input.val(value);
    });
}

// Initialize form validation
function initializeFormValidation() {
    // Client-side validation using jQuery Validation
    $('form').validate({
        errorClass: 'is-invalid',
        validClass: 'is-valid',
        errorElement: 'div',
        errorPlacement: function(error, element) {
            error.addClass('invalid-feedback');
            element.closest('.form-group').append(error);
        },
        highlight: function(element, errorClass, validClass) {
            $(element).addClass(errorClass).removeClass(validClass);
            $(element).closest('.form-group').find('.input-group-text').addClass('border-danger');
        },
        unhighlight: function(element, errorClass, validClass) {
            $(element).removeClass(errorClass).addClass(validClass);
            $(element).closest('.form-group').find('.input-group-text').removeClass('border-danger');
        }
    });
}

// Initialize all form functionality
document.addEventListener('DOMContentLoaded', function() {
    initializeDatePickers();
    initializeFilePreviews();
    initializeNumericInputs();
    initializeFormValidation();
});
