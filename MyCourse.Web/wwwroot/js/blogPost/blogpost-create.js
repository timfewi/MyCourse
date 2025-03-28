﻿// Datei: wwwroot/js/blogpost/blogpost-create.js

// Datei-Upload-Verwaltung
const dataTransfer = new DataTransfer();
const MAX_FILES = 10; // Maximale Anzahl der Dateien
const MAX_FILE_SIZE = 10 * 1024 * 1024; // 10 MB

function updateFileList() {
    const input = document.getElementById('Images');
    const output = document.getElementById('fileList');

    if (!input) {
        console.error('Element mit ID "Images" nicht gefunden.');
        return;
    }

    // Dateien aus dem Input-Feld abrufen
    const files = Array.from(input.files);

    // Neue Dateien hinzufügen, die noch nicht im DataTransfer-Objekt sind
    files.forEach(file => {
        if (!Array.from(dataTransfer.files).some(f => f.name === file.name && f.size === file.size && f.lastModified === file.lastModified)) {
            if (dataTransfer.files.length < MAX_FILES) {
                if (file.size <= MAX_FILE_SIZE) {
                    dataTransfer.items.add(file);
                } else {
                    toastr.error(`Die Datei ${file.name} überschreitet die maximale Größe von ${MAX_FILE_SIZE / (1024 * 1024)} MB.`);
                }
            } else {
                toastr.error(`Du kannst maximal ${MAX_FILES} Dateien hochladen.`);
            }
        }
    });

    // Setze die Dateien im Input-Feld auf das DataTransfer-Objekt
    input.files = dataTransfer.files;

    // Ausgabebereich leeren
    while (output.firstChild) {
        output.removeChild(output.firstChild);
    }

    if (dataTransfer.files.length > 0) {
        const list = document.createElement('ul');
        list.style.listStyleType = 'none';
        list.style.paddingLeft = '0';

        Array.from(dataTransfer.files).forEach((file, index) => {
            const listItem = document.createElement('li');
            listItem.style.background = '#f8f9fa';
            listItem.style.marginBottom = '5px';
            listItem.style.padding = '10px';
            listItem.style.borderRadius = '4px';
            listItem.style.display = 'flex';
            listItem.style.alignItems = 'center';
            listItem.style.justifyContent = 'space-between';
            listItem.style.cursor = 'default';
            listItem.style.position = 'relative';

            // Dateiname und Vorschau
            const fileInfo = document.createElement('span');
            fileInfo.textContent = file.name;
            fileInfo.title = 'Vorschau anzeigen';
            fileInfo.style.flex = '1';
            fileInfo.style.marginRight = '10px';
            fileInfo.style.position = 'relative';

            // Vorschau beim Hover
            fileInfo.addEventListener('mouseenter', function (event) {
                showPreview(event, file);
            });
            fileInfo.addEventListener('mouseleave', function () {
                hidePreview();
            });

            // Entfernen-Button
            const removeButton = document.createElement('button');
            removeButton.type = 'button';
            removeButton.className = 'btn btn-danger btn-sm';
            removeButton.textContent = 'Entfernen';
            removeButton.style.marginLeft = '10px';
            removeButton.addEventListener('click', function () {
                removeFile(index);
            });

            listItem.appendChild(fileInfo);
            listItem.appendChild(removeButton);
            list.appendChild(listItem);
        });

        output.appendChild(list);
    }
}

function removeFile(index) {
    const dt = new DataTransfer();
    Array.from(dataTransfer.files).forEach((file, i) => {
        if (i !== index) {
            dt.items.add(file);
        }
    });
    dataTransfer.items.clear();
    Array.from(dt.files).forEach(file => {
        dataTransfer.items.add(file);
    });

    const input = document.getElementById('Images');
    if (input) {
        input.files = dataTransfer.files;
    }

    updateFileList();
}

function showPreview(event, file) {
    // Tooltip-Element erstellen
    const tooltip = document.createElement('div');
    tooltip.id = 'previewTooltip';
    tooltip.style.position = 'absolute';
    tooltip.style.border = '1px solid #ccc';
    tooltip.style.background = '#fff';
    tooltip.style.padding = '5px';
    tooltip.style.borderRadius = '4px';
    tooltip.style.boxShadow = '0 0 10px rgba(0,0,0,0.1)';
    tooltip.style.zIndex = '1000';
    tooltip.style.maxWidth = '200px';
    tooltip.style.pointerEvents = 'none';

    const img = document.createElement('img');
    img.src = URL.createObjectURL(file);
    img.style.width = '100%';
    img.style.height = 'auto';
    img.onload = function () {
        URL.revokeObjectURL(this.src);
    }

    tooltip.appendChild(img);
    document.body.appendChild(tooltip);

    const rect = event.target.getBoundingClientRect();
    tooltip.style.left = rect.left + window.scrollX + 'px';
    tooltip.style.top = rect.bottom + window.scrollY + 'px';
}

function hidePreview() {
    const tooltip = document.getElementById('previewTooltip');
    if (tooltip) {
        tooltip.remove();
    }
}




document.addEventListener('DOMContentLoaded', () => {
    const imagesElement = document.getElementById('Images');
    if (imagesElement) {
        imagesElement.addEventListener('change', updateFileList);
    } else {
        console.error('Element mit ID "Images" nicht gefunden.');
    }

    // Bildvorschau Modal initialisieren
    window.showImagePreview = function (imageUrl) {
        console.log("showImagePreview aufgerufen mit URL:", imageUrl);
        const previewImage = document.getElementById('previewImage');
        if (previewImage) {
            previewImage.src = imageUrl;
        }

        const imagePreviewModal = new bootstrap.Modal(document.getElementById('imagePreviewModal'), {
            keyboard: true
        });
        imagePreviewModal.show();
    };
});

