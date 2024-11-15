// blog-detail.js

// Bildvorschau Modal initialisieren
window.showImagePreview = function (selectedIndex) {
    console.log('showImagePreview called with selectedIndex:', selectedIndex);
    const medias = window.blogMedias;

    if (!medias || !Array.isArray(medias)) {
        console.error('Medias-Daten sind nicht definiert oder kein Array.');
        return;
    }

    console.log('Medias:', medias);

    const carouselIndicators = document.getElementById('carousel-indicators');
    const carouselInner = document.getElementById('carousel-inner');

    if (!carouselIndicators || !carouselInner) {
        console.error('Carousel-Elemente konnten nicht gefunden werden.');
        return;
    }

    // Leeren der bisherigen Inhalte
    while (carouselIndicators.firstChild) {
        carouselIndicators.removeChild(carouselIndicators.firstChild);
    }
    while (carouselInner.firstChild) {
        carouselInner.removeChild(carouselInner.firstChild);
    }

    // Durchlaufe alle Medias und erstelle die Indikatoren und Carousel Items
    medias.forEach((url, index) => {
        console.log('Creating carousel item for index:', index, 'URL:', url);

        // Erstellen der Indikatoren
        const indicator = document.createElement('button');
        indicator.type = 'button';
        indicator.setAttribute('data-bs-target', '#courseCarousel');
        indicator.setAttribute('data-bs-slide-to', index);
        indicator.setAttribute('aria-label', 'Slide ' + (index + 1));

        if (index === selectedIndex) { // Setzen des ausgewählten Indikators
            indicator.classList.add('active');
            indicator.setAttribute('aria-current', 'true');
        }

        carouselIndicators.appendChild(indicator);

        // Erstellen der Carousel Items
        const carouselItem = document.createElement('div');
        carouselItem.classList.add('carousel-item');

        if (index === selectedIndex) { // Setzen des ausgewählten Carousel-Items
            carouselItem.classList.add('active');
        }

        const img = document.createElement('img');
        img.src = url;
        img.classList.add('d-block', 'w-100', 'modal-img');
        img.alt = 'Bildvorschau';

        carouselItem.appendChild(img);
        carouselInner.appendChild(carouselItem);
    });

    // Initialisiere und zeige das Modal
    const imagePreviewModal = new bootstrap.Modal(document.getElementById('imagePreviewModal'), {
        keyboard: true
    });
    imagePreviewModal.show();

    // Setze das Carousel auf das ausgewählte Bild
    const courseCarousel = document.getElementById('courseCarousel');
    if (courseCarousel) {
        const carousel = bootstrap.Carousel.getInstance(courseCarousel);
        if (carousel) {
            carousel.to(selectedIndex);
        } else {
            new bootstrap.Carousel(courseCarousel, {
                interval: 5000
            });
            bootstrap.Carousel.getInstance(courseCarousel).to(selectedIndex);
        }
    }
};
