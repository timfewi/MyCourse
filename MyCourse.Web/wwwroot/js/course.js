document.addEventListener('DOMContentLoaded', () => {

    // Funktion zum Setzen des Modal-Inhalts
    const setModalContent = (modalElement, htmlContent) => {
        const modalContent = modalElement.querySelector('.modal-content');
        const sanitizedContent = htmlContent.replace(/<script[^>]*>([\s\S]*?)<\/script>/gi, '');
        modalContent.innerHTML = sanitizedContent;

        // Event Listener für den "Anmelden"-Button hinzufügen
        const anmeldenButton = modalContent.querySelector('.open-registration-modal');
        if (anmeldenButton) {
            anmeldenButton.addEventListener('click', (e) => {
                const courseId = anmeldenButton.getAttribute('data-course-id');
                // Schließe das Kursdetails-Modal
                const detailsModalInstance = bootstrap.Modal.getInstance(modalElement);
                detailsModalInstance.hide();

                // Öffne das Registrierungsformular-Modul
                openRegistrationModal(courseId);
            });
        }
    };


    // Handle Details Modal
    const detailsModal = document.getElementById('detailsModal');
    detailsModal.addEventListener('show.bs.modal', (event) => {
        const button = event.relatedTarget;
        const detailsUrl = button.getAttribute('data-details-url');

        const modal = detailsModal;
        setModalContent(modal, `
            <div class="modal-body text-center">
                <span class="spinner-border" role="status" aria-hidden="true"></span> Lädt...
            </div>`);

        // Fetch request to get the partial view
        fetch(detailsUrl, {
            headers: {
                'X-Requested-With': 'XMLHttpRequest'
            }
        })
            .then(response => {
                if (response.ok) {
                    return response.text();
                } else {
                    throw new Error('Netzwerkantwort war nicht ok.');
                }
            })
            .then(data => {
                setModalContent(modal, data);
            })
            .catch(error => {
                setModalContent(modal, `
                    <div class="modal-header">
                        <h5 class="modal-title">Fehler</h5>
                        <button type="button" class="btn-close" data-bs-dismiss="modal"></button>
                    </div>
                    <div class="modal-body">
                        <p>Es ist ein Fehler aufgetreten. Bitte versuche es später erneut.</p>
                    </div>`);
            });
    });

    // Handle Register Modal
    const registerModal = document.getElementById('registerModal');
    registerModal.addEventListener('show.bs.modal', (event) => {
        const button = event.relatedTarget;
        const registerUrl = button.getAttribute('data-register-url');

        const modal = registerModal;
        setModalContent(modal, `
            <div class="modal-body text-center">
                <span class="spinner-border" role="status" aria-hidden="true"></span> Lädt...
            </div>`);

        // Fetch request to get the partial view
        fetch(registerUrl, {
            headers: {
                'X-Requested-With': 'XMLHttpRequest'
            }
        })
            .then(response => {
                if (response.ok) {
                    return response.text();
                } else {
                    throw new Error('Netzwerkantwort war nicht ok.');
                }
            })
            .then(data => {
                setRegisterModalContent(registerModal, data);
            })
            .catch(error => {
                setModalContent(registerModal, `
                    <div class="modal-header">
                        <h5 class="modal-title">Fehler</h5>
                        <button type="button" class="btn-close" data-bs-dismiss="modal"></button>
                    </div>
                    <div class="modal-body">
                        <p>Es ist ein Fehler aufgetreten. Bitte versuche es später erneut.</p>
                    </div>`);
            });
    });

    // Funktion zum Setzen des Inhalts und Hinzufügen des Event Listeners für das Registrierungsformular
    const setRegisterModalContent = (modalElement, htmlContent) => {
        const modalContent = modalElement.querySelector('.modal-content');
        const sanitizedContent = htmlContent.replace(/<script[^>]*>([\s\S]*?)<\/script>/gi, '');
        modalContent.innerHTML = sanitizedContent;

        // Event Listener für das Formular hinzufügen
        const form = modalContent.querySelector('#registration-form');
        if (form) {
            form.addEventListener('submit', (e) => {
                e.preventDefault();
                const formData = new FormData(form);
                const actionUrl = form.getAttribute('action');

                fetch(actionUrl, {
                    method: 'POST',
                    body: formData,
                    headers: {
                        'X-Requested-With': 'XMLHttpRequest',
                    }
                })
                    .then(response => {
                        if (response.ok) {
                            const contentType = response.headers.get('content-type');
                            if (contentType && contentType.includes('application/json')) {
                                return response.json();
                            } else {
                                return response.text();
                            }
                        } else {
                            throw new Error('Netzwerkantwort war nicht ok.');
                        }
                    })
                    .then(data => {
                        if (typeof data === 'object' && data.success !== undefined) {
                            if (data.success) {
                                // Schließe das Modal und zeige eine Erfolgsmeldung
                                const modalInstance = bootstrap.Modal.getInstance(registerModal);
                                modalInstance.hide();
                                toastr.success(data.message, "Erfolg");
                            } else {
                                // Zeige eine Fehlermeldung
                                toastr.error(data.message, "Fehler");
                            }
                        } else {
                            // Aktualisiere das Modal mit der erhaltenen HTML (Fehleranzeige)
                            setRegisterModalContent(registerModal, data);
                        }
                    })
                    .catch(error => {
                        toastr.error("Es ist ein Fehler aufgetreten. Bitte versuche es später erneut.", "Fehler");
                    });
            });
        }
    };

    const openRegistrationModal = (courseId) => {
        const registerModal = document.getElementById('registerModal');
        const registerUrl = `Course/RegisterPartial?id=${courseId}`; // Passe die URL entsprechend an

        setModalContent(registerModal, `
        <div class="modal-body text-center">
            <span class="spinner-border" role="status" aria-hidden="true"></span> Lädt...
        </div>`);

        fetch(registerUrl, {
            headers: {
                'X-Requested-With': 'XMLHttpRequest'
            }
        })
            .then(response => {
                if (response.ok) {
                    return response.text();
                } else {
                    throw new Error('Netzwerkantwort war nicht ok.');
                }
            })
            .then(data => {
                setRegisterModalContent(registerModal, data);
                // Öffne das Registrierungsformular-Modul
                const registerModalInstance = new bootstrap.Modal(registerModal);
                registerModalInstance.show();
            })
            .catch(error => {
                setModalContent(registerModal, `
                <div class="modal-header">
                    <h5 class="modal-title">Fehler</h5>
                    <button type="button" class="btn-close" data-bs-dismiss="modal"></button>
                </div>
                <div class="modal-body">
                    <p>Es ist ein Fehler aufgetreten. Bitte versuche es später erneut.</p>
                </div>`);
            });
    };


});
