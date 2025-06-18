// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

// Animation d'apparition des cartes au chargement
window.addEventListener('DOMContentLoaded', function() {
    document.body.classList.add('page-loaded');
    const cards = document.querySelectorAll('.card');
    cards.forEach((card, i) => {
        setTimeout(() => {
            card.classList.add('visible');
        }, 200 + i * 150);
    });

    // Carousel pour chaque card
    document.querySelectorAll('.card').forEach(function(card) {
        const carousel = card.querySelector('.card-carousel');
        const images = carousel.querySelectorAll('img');
        let current = 0;
        const prevBtn = card.querySelector('.card-carousel-button.prev');
        const nextBtn = card.querySelector('.card-carousel-button.next');

        function updateCarousel() {
            carousel.style.transform = `translateX(-${current * 100}%)`;
        }
        if (prevBtn && nextBtn) {
            prevBtn.addEventListener('click', function() {
                current = (current - 1 + images.length) % images.length;
                updateCarousel();
            });
            nextBtn.addEventListener('click', function() {
                current = (current + 1) % images.length;
                updateCarousel();
            });
        }
    });

    // Navigation horizontale des cartes
    const grid = document.querySelector('.product-grid');
    const leftBtn = document.querySelector('.product-scroll-arrow.left');
    const rightBtn = document.querySelector('.product-scroll-arrow.right');
    if (leftBtn && rightBtn && grid) {
        leftBtn.addEventListener('click', function() {
            grid.scrollBy({ left: -360, behavior: 'smooth' });
        });
        rightBtn.addEventListener('click', function() {
            grid.scrollBy({ left: 360, behavior: 'smooth' });
        });
    }
});

// Animation d'ouverture/fermeture de la description longue
document.querySelectorAll('.dropdown-toggle').forEach(function(btn) {
    btn.addEventListener('click', function() {
        const desc = this.nextElementSibling;
        desc.classList.toggle('show');
        this.textContent = desc.classList.contains('show') ? 'Voir moins' : 'Voir plus';
    });
});
