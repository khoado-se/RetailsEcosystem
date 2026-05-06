$(function () {
    $(document).on('click', '.password-toggle', function () {
        const $input = $(this).closest('.input-group').find('input');
        const $icon = $(this).find('i');
        if ($input.attr('type') === 'password') {
            $input.attr('type', 'text');
            $icon.removeClass('opacity-50');
        } else {
            $input.attr('type', 'password');
            $icon.addClass('opacity-50');
        }
    });
});
