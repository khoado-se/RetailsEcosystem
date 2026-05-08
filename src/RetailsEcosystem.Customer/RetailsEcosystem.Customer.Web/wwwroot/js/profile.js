$(function () {

    // ── Avatar live preview ────────────────────────────────────────────
    $('#avatar-upload-btn').on('click', function () {
        $('#avatar-file-input').trigger('click');
    });

    $('#avatar-file-input').on('change', function () {
        var file = this.files[0];
        if (!file) return;
        var previewUrl = URL.createObjectURL(file);
        $('#avatar-preview-container').html(
            '<img src="' + previewUrl + '" alt="Preview"'
            + ' style="width:100%;height:100%;object-fit:cover;" />'
        );
    });

    // ── Mirror full name into identity panel as user types ────────────
    $('#FullName').on('input', function () {
        var val = $(this).val().trim();
        var initial = val.length > 0 ? val[0].toUpperCase() : '?';
        $('#identity-name').text(val || 'Your Name');
        var $initials = $('#avatar-initials');
        if ($initials.length) $initials.text(initial);
    });

    // ── Save button loading state (guard matches checkout.js pattern) ─
    $('#profile-form').on('submit', function () {
        if (!$(this).valid()) return;
        $('#save-btn')
            .prop('disabled', true)
            .html('<span class="spinner-border spinner-border-sm me-2"'
                + ' role="status" aria-hidden="true"></span>Saving…');
    });

});
