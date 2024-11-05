document.addEventListener('DOMContentLoaded', function () {
    const hoverImages = document.querySelectorAll('.hover-image');
    const imagePreviewModal = new bootstrap.Modal(document.getElementById('imagePreviewModal'), {
        keyboard: true
    });

    hoverImages.forEach(img => {
        // Hover-Effekt: Bild wechseln beim Überfahren
        img.addEventListener('mouseenter', function () {
            const hoverUrl = this.getAttribute('data-hover');
            if (hoverUrl) {
                this.src = hoverUrl;
            }
        });

        img.addEventListener('mouseleave', function () {
            const defaultUrl = this.getAttribute('data-default');
            if (defaultUrl) {
                this.src = defaultUrl;
            }
        });

        // Klick-Effekt: Modal mit Karussell öffnen und Event-Propagation stoppen
        img.addEventListener('click', function (e) {
            e.stopPropagation(); // Verhindert, dass das Klicken auf das Bild den Klick-Handler der Karte auslöst
            const courseCard = this.closest('.course-card');
            const courseId = courseCard.getAttribute('data-course-id');
            if (courseId) {
                fetchCourseImages(courseId);
            }
        });
    });

    // Klick auf die gesamte Karte navigiert zu den Details, außer auf die Buttons
    const courseCards = document.querySelectorAll('.course-card');

    courseCards.forEach(card => {
        card.addEventListener('click', function (e) {
            // Verhindere, dass Klicks auf die Buttons die Karte navigieren
            if (e.target.closest('a') || e.target.closest('button')) {
                return;
            }

            const detailsLink = this.querySelector('a.btn-primary');
            if (detailsLink) {
                window.location.href = detailsLink.href;
            }
        });
    });

    // Funktion zum Abrufen der Kursbilder
    async function fetchCourseImages(courseId) {
        try {
            // Zeige den Ladeindikator und verstecke das Karussell
            const loadingIndicator = document.getElementById('loadingIndicator');
            const courseCarousel = document.getElementById('courseCarousel');
            loadingIndicator.classList.remove('d-none');
            courseCarousel.classList.add('d-none');

            const response = await fetch(`/Course/GetCourseImages/${courseId}`);

            if (!response.ok) {
                throw new Error('Fehler beim Laden der Kursbilder.');
            }
            const images = await response.json();
            populateCarousel(images);

            // Verstecke den Ladeindikator und zeige das Karussell
            loadingIndicator.classList.add('d-none');
            courseCarousel.classList.remove('d-none');

            imagePreviewModal.show();
        } catch (error) {
            console.error(error);
            toastr.error('Es gab ein Problem beim Laden der Kursbilder.');
            // Verstecke den Ladeindikator, falls ein Fehler auftritt
            document.getElementById('loadingIndicator').classList.add('d-none');
        }
    }

    // Funktion zum Befüllen des Karussells mit den geladenen Bildern
    function populateCarousel(images) {
        const carouselInner = document.getElementById('carousel-inner');
        const carouselIndicators = document.getElementById('carousel-indicators');

        // Sicheres Leeren des Karussells ohne innerHTML
        clearElement(carouselInner);
        clearElement(carouselIndicators);

        images.forEach((image, index) => {
            console.log('Image URL:', image.imageUrl); // Debugging-Ausgabe
            console.log('Alt Text:', image.altText);   // Debugging-Ausgabe

            // Erstelle Carousel-Indikatoren
            const indicator = document.createElement('button');
            indicator.type = 'button';
            indicator.setAttribute('data-bs-target', '#courseCarousel');
            indicator.setAttribute('data-bs-slide-to', index.toString());
            indicator.setAttribute('aria-label', `Slide ${index + 1}`);
            if (index === 0) {
                indicator.classList.add('active');
                indicator.setAttribute('aria-current', 'true');
            }
            carouselIndicators.appendChild(indicator);

            // Erstelle Carousel-Items
            const carouselItem = document.createElement('div');
            carouselItem.classList.add('carousel-item');
            if (index === 0) {
                carouselItem.classList.add('active');
            }

            const imgElement = document.createElement('img');
            imgElement.src = image.imageUrl; // Verwende 'imageUrl' statt 'ImageUrl'
            imgElement.classList.add('d-block', 'w-100');
            imgElement.alt = image.altText; // Verwende 'altText' statt 'AltText'

            carouselItem.appendChild(imgElement);
            carouselInner.appendChild(carouselItem);
        });
    }

    // Hilfsfunktion zum sicheren Leeren eines Elements
    function clearElement(element) {
        while (element.firstChild) {
            element.removeChild(element.firstChild);
        }
    }
});
