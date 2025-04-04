document.addEventListener('DOMContentLoaded', function () {
    const openRegistrationModalLink = document.getElementById('openRegistrationModalLink');
    const openLoginModalLink = document.getElementById('openLoginModalLink');
    const registrationForm = document.getElementById('registrationForm');  //получение формы регистрации (внутри модального окна)
    openRegistrationModalLink.addEventListener('click', function (event) {
        event.preventDefault(); // Предотвращаем переход по ссылке

        // Получаем экземпляры модальных окон Bootstrap
        const loginModal = bootstrap.Modal.getInstance(document.getElementById('loginModal'));
        const registrationModal = new bootstrap.Modal(document.getElementById('registrationModal'));
        // Скрываем текущий модальный (входа)
        if (loginModal) {
            loginModal.hide();
        }
        // Отображаем модальное окно регистрации
        registrationModal.show();
    });
    openLoginModalLink.addEventListener('click', function (event) {
        event.preventDefault(); // Предотвращаем переход по ссылке

        // Получаем экземпляры модальных окон Bootstrap
        const registrationModal = bootstrap.Modal.getInstance(document.getElementById('registrationModal'));
        const loginModal = new bootstrap.Modal(document.getElementById('loginModal'));
        // Скрываем текущий модальный (входа)
        if (registrationModal) {
            registrationModal.hide();
        }
        // Отображаем модальное окно регистрации
        loginModal.show();
    });

    $('#loginModal').on('hidden.bs.modal', function (e) {
        var form = $(this).find('form');
        form[0].reset();                        // очистка формы
        $(this).find('.text-danger').text('');  // очистка сообщений об ошибках
        $(this).find('.is-invalid').removeClass('is-invalid'); 

        form.removeData('validator');
        form.removeData('unobtrusiveValidation');
        $.validator.unobtrusive.parse(form);
    });

    $('#registrationModal').on('hidden.bs.modal', function (e) {
        var form = $(this).find('form');
        form[0].reset();                        // очистка формы
        $(this).find('.text-danger').text('');  // очистка сообщений об ошибках
        $(this).find('.is-invalid').removeClass('is-invalid');

        form.removeData('validator');
        form.removeData('unobtrusiveValidation');
        $.validator.unobtrusive.parse(form);
    });
});



