// Скрипт для улучшения взаимодействия с формами авторизации
document.addEventListener('DOMContentLoaded', function () {
    // Добавление анимации для кнопок
    const buttons = document.querySelectorAll('.btn-auth');
    buttons.forEach(button => {
        button.addEventListener('mouseenter', function () {
            this.classList.add('pulse');
        });

        button.addEventListener('mouseleave', function () {
            this.classList.remove('pulse');
        });
    });

    // Улучшение валидации форм
    const forms = document.querySelectorAll('form');
    forms.forEach(form => {
        const inputs = form.querySelectorAll('input');

        inputs.forEach(input => {
            input.addEventListener('focus', function () {
                this.parentElement.classList.add('is-focused');
            });

            input.addEventListener('blur', function () {
                this.parentElement.classList.remove('is-focused');

                // Добавляем класс is-filled, если поле заполнено
                if (this.value.trim() !== '') {
                    this.parentElement.classList.add('is-filled');
                } else {
                    this.parentElement.classList.remove('is-filled');
                }
            });

            // Установить начальное состояние для заполненных полей
            if (input.value.trim() !== '') {
                input.parentElement.classList.add('is-filled');
            }
        });
    });

    // Добавляем визуальный эффект тряски для невалидных полей при отправке формы
    forms.forEach(form => {
        form.addEventListener('submit', function (e) {
            const invalidInputs = form.querySelectorAll('.input-validation-error');

            invalidInputs.forEach(input => {
                input.classList.add('shake');
                setTimeout(() => {
                    input.classList.remove('shake');
                }, 600);
            });
        });
    });

    // Добавляем эффект клика на кнопке
    buttons.forEach(button => {
        button.addEventListener('click', function () {
            this.classList.add('clicked');

            setTimeout(() => {
                this.classList.remove('clicked');
            }, 200);
        });
    });

    // Анимация для валидных полей при вводе
    const inputs = document.querySelectorAll('input');
    inputs.forEach(input => {
        input.addEventListener('input', function () {
            if (this.checkValidity()) {
                this.classList.add('is-valid');
            } else {
                this.classList.remove('is-valid');
            }
        });
    });
});

// Дополнительные анимации
@keyframes pulse {
    0 % { transform: scale(1); }
    50 % { transform: scale(1.05); }
    100 % { transform: scale(1); }
}

@keyframes shake {
    0 %, 100 % { transform: translateX(0); }
    10 %, 30 %, 50 %, 70 %, 90 % { transform: translateX(-5px); }
    20 %, 40 %, 60 %, 80 % { transform: translateX(5px); }
}

@keyframes clicked {
    0 % { transform: scale(1); }
    50 % { transform: scale(0.95); }
    100 % { transform: scale(1); }
}

.pulse {
    animation: pulse 0.6s ease -in -out;
}

.shake {
    animation: shake 0.6s ease -in -out;
}

.clicked {
    animation: clicked 0.2s ease -in -out;
}