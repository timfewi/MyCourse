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

    const courseDatePickerElement = document.querySelector('#courseDatePicker input');
    if (courseDatePickerElement) {
        flatpickr(courseDatePickerElement, {
            dateFormat: "Y-m-d", // ISO 8601 Format für das Model Binding
            altInput: true,
            altFormat: "d.m.Y", // Anzeigeformat
            allowInput: true,
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
    }
});
