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
