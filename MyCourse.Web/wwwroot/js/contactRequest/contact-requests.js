$(document).ready(function () {
    // Initialisiere DataTable für aktive Kontaktanfragen
    var activeTable = $('#activeContactRequestsTable').DataTable({
        "paging": true,
        "searching": true,
        "info": false,
        "lengthChange": false,
        "pageLength": 5,
        "order": [[2, "desc"]],
        "language": {
            "url": "//cdn.datatables.net/plug-ins/1.13.4/i18n/de-DE.json"
        }
    });

    // Initialisiere DataTable für inaktive Kontaktanfragen
    var inactiveTable = $('#inactiveContactRequestsTable').DataTable({
        "paging": true,
        "searching": true,
        "info": false,
        "lengthChange": false,
        "pageLength": 5,
        "order": [[2, "desc"]],
        "language": {
            "url": "//cdn.datatables.net/plug-ins/1.13.4/i18n/de-DE.json"
        }
    });

    // Funktion zur Behandlung von Click-Events für beide Tabellen
    function handleRowClick(table) {
        table.on('click', 'tr', function () {
            var url = $(this).data('url');

            if (!url) return;

            // Entferne die aktive Klasse von allen Zeilen in beiden Tabellen
            activeTable.$('tr.table-active').removeClass('table-active');
            inactiveTable.$('tr.table-active').removeClass('table-active');

            // Füge die aktive Klasse zur angeklickten Zeile hinzu
            $(this).addClass('table-active');

            // Zeige den Ladeindikator und verstecke die Details
            $('#loadingIndicator').show();
            $('#contactDetails').hide();

            // AJAX-Anfrage senden
            $.get(url, function (data) {
                $('#contactDetails').html(data);
            })
                .always(function () {
                    $('#loadingIndicator').hide();
                    $('#contactDetails').show();
                })
                .fail(function () {
                    toastr.error('Fehler beim Laden der Kontaktanfrage.');
                    $('#loadingIndicator').hide();
                });
        });
    }

    // Setze Click-Handler für beide Tabellen
    handleRowClick(activeTable);
    handleRowClick(inactiveTable);

    // Behandlung des Formular-Submit via AJAX
    $(document).on('submit', '#responseForm', function (event) {
        event.preventDefault();

        var form = $(this);
        var url = form.attr('action');
        var formData = form.serialize();

        $.post(url, formData, function (data) {
            // Bei Erfolg die Liste aktualisieren oder eine Erfolgsmeldung anzeigen
            window.location.reload();
        }).fail(function (jqXHR, textStatus, errorThrown) {
            // Bei Fehler eine Fehlermeldung anzeigen
            toastr.error('Fehler beim Senden der Antwort.');
        });
    });

    // Behandlung des Abbrechen-Buttons
    $(document).on('click', '#cancelButton', function (event) {
        event.preventDefault();
        // Entferne die aktive Klasse von allen Zeilen
        activeTable.$('tr.table-active').removeClass('table-active');
        inactiveTable.$('tr.table-active').removeClass('table-active');
        $('#contactDetails').html('<p>Wählen Sie eine Kontaktanfrage aus der Liste aus, um sie anzuzeigen und zu beantworten.</p>');
    });
});