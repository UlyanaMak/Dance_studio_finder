/*function openModal(modalId) {
    var myModal = new bootstrap.Modal(document.getElementById(modalId));
    myModal.show();
}
$(document).ready(function () {
    $('#openLoginModal').on('click', function () {
        $.ajax({
            url: '/Home/Login',
            type: 'GET',
            success: function (data) {
                $('#loginModal .modal-content').html(data);
                openModal('loginModal');
            },
            error: function (xhr, status, error) {
                console.error("Ошибка при загрузке формы входа: " + error);
            }
        });
    });

    $('#openRegisterModal').on('click', function () {
        $.ajax({
            url: '/Home/Register',
            type: 'GET',
            success: function (data) {
                $('#registrationModal .modal-content').html(data);
                openModal('registrationModal');
            },
            error: function (xhr, status, error) {
                console.error("Ошибка при загрузке формы регистрации: " + error);
            }
        });
    });
    $('#registerForm').submit(function (e) {
        e.preventDefault();
        var form = $(this);

        $.ajax({
            url: form.attr('action'),
            type: 'POST',
            data: form.serialize(),
            success: function (response) {
                if (response.success) {
                    // Если регистрация успешна, перенаправляем
                    window.location.href = response.redirectUrl;
                } else {
                    // Если есть ошибки, обновляем содержимое модального окна
                    $('#registrationModal .modal-content').html(response);
                    // Повторно инициализируем валидацию
                    $.validator.unobtrusive.parse('#registerForm');
                }
            },
            error: function (xhr, status, error) {
                console.error("Ошибка при регистрации: " + error);
            }
        });
    });
});*/