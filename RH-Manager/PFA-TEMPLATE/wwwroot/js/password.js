document.getElementById('togglePassword').addEventListener('click', function () {
    const passwordInput = document.getElementById('passwordInput');
    const icon = this.querySelector('i');

    if (passwordInput.type === 'password') {
        passwordInput.type = 'text';
        icon.classList.replace('fa-eye', 'fa-eye-slash');
    } else {
        passwordInput.type = 'password';
        icon.classList.replace('fa-eye-slash', 'fa-eye');
    }
});

// Generate strong password
document.getElementById('generatePassword').addEventListener('click', function () {
    const chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789!@#$%^&*()_+";
    let password = "";

    // Ensure we meet all requirements
    password += getRandomChar("ABCDEFGHIJKLMNOPQRSTUVWXYZ"); // At least one uppercase
    password += getRandomChar("0123456789"); // At least one number
    password += getRandomChar("!@#$%^&*()_+"); // At least one special char

    // Fill remaining characters
    for (let i = 0; i < 5; i++) { // 5 + 3 = 8 characters minimum
        password += getRandomChar(chars);
    }

    // Shuffle the password
    password = password.split('').sort(() => 0.5 - Math.random()).join('');

    // Set the password and show strength
    const passwordInput = document.getElementById('passwordInput');
    passwordInput.value = password;
    passwordInput.type = 'text'; // Show the generated password
    document.getElementById('togglePassword').querySelector('i')
        .classList.replace('fa-eye', 'fa-eye-slash');

    checkPasswordStrength(password);
    document.querySelector('.password-strength').classList.remove('d-none');
});

function getRandomChar(characters) {
    return characters.charAt(Math.floor(Math.random() * characters.length));
}

// Password strength indicator
function checkPasswordStrength(password) {
    const strengthBar = document.getElementById('passwordStrengthBar');
    const strengthText = document.getElementById('passwordStrengthText');

    // Reset
    strengthBar.style.width = '0%';
    strengthBar.className = 'progress-bar';

    if (!password) {
        document.querySelector('.password-strength').classList.add('d-none');
        return;
    }

    let strength = 0;
    // Length check
    strength += Math.min(25, (password.length / 12) * 25);
    // Uppercase check
    if (/[A-Z]/.test(password)) strength += 15;
    // Lowercase check
    if (/[a-z]/.test(password)) strength += 15;
    // Number check
    if (/[0-9]/.test(password)) strength += 15;
    // Special char check
    if (/[^A-Za-z0-9]/.test(password)) strength += 15;
    // Deduplication
    const uniqueChars = new Set(password).size;
    strength += (uniqueChars / password.length) * 15;

    // Cap at 100%
    strength = Math.min(100, Math.round(strength));

    // Update UI
    strengthBar.style.width = strength + '%';

    if (strength < 40) {
        strengthBar.classList.add('bg-danger');
        strengthText.textContent = 'Faible';
    } else if (strength < 70) {
        strengthBar.classList.add('bg-warning');
        strengthText.textContent = 'Moyen';
    } else {
        strengthBar.classList.add('bg-success');
        strengthText.textContent = 'Fort';
    }
}

// Check strength on input
document.getElementById('passwordInput').addEventListener('input', function () {
    checkPasswordStrength(this.value);
    document.querySelector('.password-strength').classList.remove('d-none');
});


$(document).ready(function () {
    // Setup real-time validation for all unique fields
    ['Email', 'Login', 'CIN'].forEach(function (field) {
        $('#' + field).on('blur', function () {
            var value = $(this).val();
            if (value) {
                $.get('/Utilisateur/Check' + field + '?' + field.toLowerCase() + '=' + value, function (data) {
                    var isValid = data;
                    var $field = $('#' + field);
                    var $errorSpan = $field.next('.text-danger');

                    if (!isValid) {
                        $field.addClass('is-invalid');
                        $errorSpan.text('Cette valeur est déjà utilisée');
                    } else {
                        $field.removeClass('is-invalid');
                        $errorSpan.text('');
                    }
                });
            }
        });
    });
});