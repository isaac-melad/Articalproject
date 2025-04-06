/**
 * Publishers page JavaScript functionality
 */

document.addEventListener('DOMContentLoaded', function() {
    // Initialize any interactive elements
    initializePublishersPage();
});

/**
 * Initialize the publishers page functionality
 */
function initializePublishersPage() {
    // Add smooth hover effects for publisher cards
    const publisherCards = document.querySelectorAll('.publisher-card');
    
    if (publisherCards.length > 0) {
        publisherCards.forEach(card => {
            // Add hover animation if needed
            card.addEventListener('mouseenter', function() {
                this.style.transform = 'translateY(-5px)';
            });
            
            card.addEventListener('mouseleave', function() {
                this.style.transform = 'translateY(0)';
            });
        });
    }
    
    // Initialize social media link interactions
    const socialLinks = document.querySelectorAll('.social-link');
    if (socialLinks.length > 0) {
        socialLinks.forEach(link => {
            link.addEventListener('click', function(e) {
                // Prevent default if the link is not fully configured
                const href = this.getAttribute('href');
                if (!href || href === '#') {
                    e.preventDefault();
                }
            });
        });
    }
}