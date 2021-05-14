$(document).ready(function () {
    $('.datepicker').datepicker({
        changeMonth: true,
        changeYear: true,
        dateFormat: 'yy/mm/dd'
    });

    $('input.emp-check').on('focusout', function () {
        if ($(this).val() != '') {
            $(this).removeClass('is-invalid');
            $(this).addClass('is-valid');
        } else {
            $(this).removeClass('is-valid');
            $(this).addClass('is-invalid');
        }
    });
});