document.addEventListener('DOMContentLoaded', function () {

    // Cookie Policy
    if (!getCookie("cookieConsent")) {
        document.getElementById("cookie-banner").style.display = "block";
    }

    document.getElementById("accept-cookies").addEventListener("click", function () {
        setCookie("cookieConsent", "accepted", 365);
        document.getElementById("cookie-banner").style.display = "none";
    });

    function setCookie(name, value, days) {
        let expires = "";
        if (days) {
            const date = new Date();
            date.setTime(date.getTime() + (days * 24 * 60 * 60 * 1000));
            expires = "; expires=" + date.toUTCString();
        }
        document.cookie = name + "=" + (value || "") + expires + "; path=/";
    }

    function getCookie(name) {
        const nameEQ = name + "=";
        const ca = document.cookie.split(';');
        for (let i = 0; i < ca.length; i++) {
            let c = ca[i];
            while (c.charAt(0) === ' ') c = c.substring(1, c.length);
            if (c.indexOf(nameEQ) === 0) return c.substring(nameEQ.length, c.length);
        }
        return null;
    }

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