document.addEventListener('DOMContentLoaded', function () {
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
});