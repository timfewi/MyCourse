document.addEventListener('DOMContentLoaded', function () {

    // Sidebar Toggle
    const menuToggleButton = document.getElementById("menu-toggle");
    if (menuToggleButton) {
        menuToggleButton.addEventListener('click', function (e) {
            e.preventDefault();
            document.getElementById("wrapper").classList.toggle("toggled");
            document.getElementById("sidebar").classList.toggle("active");
        });
    }

    // Tooltip-Initialisierung (Bootstrap 5)
    const tooltipTriggerList = document.querySelectorAll('[data-bs-toggle="tooltip"]');
    tooltipTriggerList.forEach(function (tooltipTriggerEl) {
        new bootstrap.Tooltip(tooltipTriggerEl);
    });


    // Switches
    const switches = document.querySelectorAll('input.form-check-input');
    console.log(`Gefundene Switches: ${switches.length}`);

    switches.forEach(function (switchElement) {
        console.log(`Initialisiere Switch mit ID: ${switchElement.id}`);
        updateSwitchState(switchElement);
        switchElement.addEventListener('change', function (event) {
            console.log(`Switch geändert: ${event.target.id}`);
            updateSwitchState(event.target);
        });
    });

    /**
    * Aktualisiert den Zustandstext basierend auf dem Switch-Zustand.
    * @param {HTMLElement} switchElement - Das Switch-Input-Element.
    */
    function updateSwitchState(switchElement) {
        const switchStateSpan = switchElement.parentElement.querySelector('.switch-state');
        if (!switchStateSpan) {
            console.warn('Kein Element mit der Klasse "switch-state" gefunden innerhalb des Elternteils des Switches.');
            return;
        }

        if (switchElement.checked) {
            switchStateSpan.textContent = 'Ja';
            switchStateSpan.classList.remove('bg-secondary');
            switchStateSpan.classList.add('bg-success');
            console.log(`Switch "${switchElement.id}" ist aktiviert (Ja).`);
        } else {
            switchStateSpan.textContent = 'Nein';
            switchStateSpan.classList.remove('bg-success');
            switchStateSpan.classList.add('bg-secondary');
            console.log(`Switch "${switchElement.id}" ist deaktiviert (Nein).`);
        }
    }


    // TinyMCE 
    if (typeof tinymce !== 'undefined') {
        tinymce.init({
            selector: 'textarea.tinymce',
            plugins: 'lists link table',
            toolbar: 'undo redo | formatselect | bold italic underline | ' +
                'alignleft aligncenter alignright alignjustify | ' +
                'bullist numlist outdent indent | link image | code',
            menubar: false,
            height: 300,
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

    // Toastr Optionen konfigurieren
    toastr.options = {
        "closeButton": true,
        "debug": false,
        "newestOnTop": false,
        "progressBar": true,
        "positionClass": "toast-top-right",
        "preventDuplicates": false,
        "onclick": null,
        "showDuration": "300",
        "hideDuration": "1000",
        "timeOut": "5000",
        "extendedTimeOut": "1000",
        "showEasing": "swing",
        "hideEasing": "linear",
        "showMethod": "fadeIn",
        "hideMethod": "fadeOut"
    };

    // Universelle Flatpickr-Initialisierung für alle Inputs mit der Klasse 'flatpickr'
    const flatpickrElements = document.querySelectorAll('.flatpickr');
    flatpickrElements.forEach(function (element) {
        flatpickr(element, {
            enableTime: true, // Zeit auswählen erlauben
            noCalendar: false, // Kalender anzeigen
            dateFormat: "Y-m-d H:i", // ISO 8601 Format für das Model Binding inklusive Zeit
            altInput: true,
            altFormat: "d.m.Y H:i", // Anzeigeformat inklusive Zeit im 24-Stunden-Format
            allowInput: true,
            time_24hr: true, // 24-Stunden-Format aktivieren
            locale: {
                firstDayOfWeek: 1, // Montag als erster Tag der Woche
                weekdays: {
                    shorthand: ["So", "Mo", "Di", "Mi", "Do", "Fr", "Sa"],
                    longhand: ["Sonntag", "Montag", "Dienstag", "Mittwoch", "Donnerstag", "Freitag", "Samstag"],
                },
                months: {
                    shorthand: ["Jan", "Feb", "Mär", "Apr", "Mai", "Jun", "Jul", "Aug", "Sep", "Okt", "Nov", "Dez"],
                    longhand: ["Januar", "Februar", "März", "April", "Mai", "Juni", "Juli", "August", "September", "Oktober", "November", "Dezember"],
                }
            }
        });
    });

});
