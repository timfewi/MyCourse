document.addEventListener('DOMContentLoaded', function () {

    // Hover effect
    const images = document.querySelectorAll('.hover-image');
    images.forEach(img => {
        const hoverSrc = img.getAttribute('data-hover');
        const originalSrc = img.getAttribute('src');
        img.addEventListener('mouseover', () => {
            img.src = hoverSrc;
        });
        img.addEventListener('mouseout', () => {
            img.src = originalSrc;
        });
    });

    // TinyMCE 
    if (typeof tinymce !== 'undefined') {
        tinymce.init({
            selector: 'textarea.tinymce',
            plugins: 'lists table',
            toolbar: 'undo redo | formatselect | bold italic underline | ' +
                'alignleft aligncenter alignright alignjustify | ' +
                'bullist numlist outdent indent | link image | code',
            menubar: false,
            height: 300,
            width: '100%',
            license_key: 'gpl',
            branding: false,
            statusbar: false,
            setup: function (editor) {
                editor.on('change', function () {
                    editor.save();
                });
            }
        });
    } else {
        console.error('TinyMCE ist nicht geladen.');
    }
});