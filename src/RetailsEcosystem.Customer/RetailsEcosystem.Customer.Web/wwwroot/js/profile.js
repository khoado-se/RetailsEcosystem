$(function () {

    // ── Avatar upload ──────────────────────────────────────────────────
    $('#avatar-upload-btn').on('click', function () {
        $('#avatar-file-input').trigger('click');
    });

    $('#avatar-file-input').on('change', function () {
        var file = this.files[0];
        if (!file) return;

        // Show local preview immediately for instant feedback
        var previewUrl = URL.createObjectURL(file);
        $('#avatar-preview-container').html(
            '<img src="' + previewUrl + '" alt="Preview"'
            + ' style="width:100%;height:100%;object-fit:cover;" />'
        );

        // Disable button and show spinner during upload
        $('#avatar-upload-btn').prop('disabled', true)
            .html('<span class="spinner-border spinner-border-sm me-1" role="status"'
                + ' aria-hidden="true"></span>Uploading…');

        var formData = new FormData();
        formData.append('file', file);
        formData.append('__RequestVerificationToken',
            $('input[name="__RequestVerificationToken"]').val());

        $.ajax({
            url: '/account/profile/avatar',
            type: 'POST',
            data: formData,
            processData: false,
            contentType: false,
            success: function (res) {
                // Update the hidden AvatarUrl field so the next profile save preserves it
                $('#AvatarUrl').val(res.url);

                // Update the header avatar without a full page reload
                var $headerAvatar = $('#header-user-avatar');
                $headerAvatar.html(
                    '<img src="' + res.url + '" alt="Profile photo" />'
                );

                $('#avatar-upload-btn').prop('disabled', false)
                    .html('<i class="lni lni-camera me-1"></i>Change Photo');
            },
            error: function () {
                // Revert preview to original avatar / initials
                location.reload();
            }
        });
    });

    // ── Mirror full name into identity panel as user types ────────────
    $('#FullName').on('input', function () {
        var val = $(this).val().trim();
        var initial = val.length > 0 ? val[0].toUpperCase() : '?';
        $('#identity-name').text(val || 'Your Name');
        var $initials = $('#avatar-initials');
        if ($initials.length) $initials.text(initial);
    });

    // ── Save button loading state ─────────────────────────────────────
    $('#profile-form').on('submit', function () {
        if (!$(this).valid()) return;
        $('#save-btn')
            .prop('disabled', true)
            .html('<span class="spinner-border spinner-border-sm me-2"'
                + ' role="status" aria-hidden="true"></span>Saving…');
    });

});
