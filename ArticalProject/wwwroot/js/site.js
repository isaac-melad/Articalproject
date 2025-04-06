// Wait for DOM to be fully loaded
document.addEventListener('DOMContentLoaded', () => {
  initializeScrollEffects();
  initializeTheme();
  initializeAnimations();
});

// Scroll Effects
function initializeScrollEffects() {
  // Scroll to Top Button
  const scrollTop = document.querySelector('.scroll-top');

  window.addEventListener('scroll', () => {
    // Show/hide scroll button with smooth transition
    scrollTop.style.display = window.scrollY > 300 ? 'flex' : 'none';

    // Add scroll effect to navbar
    const navbar = document.querySelector('.navbar');
    navbar.classList.toggle('scrolled', window.scrollY > 50);

    // Reveal animations when scrolling
    const revealElements = document.querySelectorAll('.reveal');
    revealElements.forEach(el => {
      const elementTop = el.getBoundingClientRect().top;
      const elementVisible = 150;

      if (elementTop < window.innerHeight - elementVisible) {
        el.classList.add('active');
      }
    });
  });

  scrollTop.addEventListener('click', () => {
    window.scrollTo({ top: 0, behavior: 'smooth' });
  });
}

// Enhanced Dark Mode Toggle with Smooth Transitions and Visual Feedback
function initializeTheme() {
  const themeToggle = document.getElementById('theme-toggle');
  if (!themeToggle) return;

  const sunIcon = themeToggle.querySelector('.sun-icon');
  const moonIcon = themeToggle.querySelector('.moon-icon');
  const ripple = themeToggle.querySelector('.toggle-ripple');

  // Toggle theme function with enhanced animations
  function setTheme(isDark) {
    // Apply transition class before changing theme
    document.body.classList.add('theme-transition');

    // Set color scheme and toggle dark mode class
    document.documentElement.style.setProperty('color-scheme', isDark ? 'dark' : 'light');
    document.body.classList.toggle('dark-mode', isDark);

    // Toggle button appearance with animation
    themeToggle.classList.toggle('dark', isDark);

    // Create ripple effect
    if (ripple) {
      ripple.classList.add('active');
      setTimeout(() => {
        ripple.classList.remove('active');
      }, 600);
    }

    // Save preference to localStorage
    localStorage.setItem('theme', isDark ? 'dark' : 'light');

    // Remove transition class after transition completes
    setTimeout(() => {
      document.body.classList.remove('theme-transition');
    }, 500); // Slightly longer than the CSS transition time
  }

  // Initialize theme based on saved preference or system preference
  function initializeThemeSettings() {
    const savedTheme = localStorage.getItem('theme');
    const prefersDark = window.matchMedia('(prefers-color-scheme: dark)').matches;
    const initialTheme = savedTheme || (prefersDark ? 'dark' : 'light');
    const isDark = initialTheme === 'dark';

    // Apply theme without transition on initial load
    document.body.classList.add('no-transition');
    setTheme(isDark);

    // Force a reflow before removing the class
    window.getComputedStyle(document.body).getPropertyValue('opacity');
    document.body.classList.remove('no-transition');

    // Set initial tooltip text based on current theme
    const tooltipEl = themeToggle.querySelector('.toggle-tooltip');
    if (tooltipEl) {
      tooltipEl.textContent = isDark ? 'الوضع الفاتح' : 'الوضع المظلم';
    }
  }

  // Initialize theme
  initializeThemeSettings();

  // Toggle theme on button click with enhanced feedback
  themeToggle.addEventListener('click', () => {
    // Add 3D press effect
    themeToggle.classList.add('pressed');
    setTimeout(() => {
      themeToggle.classList.remove('pressed');
    }, 300);

    // Try to provide haptic feedback on supported devices
    if (navigator.vibrate) {
      navigator.vibrate(5);
    }

    // Add pulse animation to the toggle button
    themeToggle.classList.add('pulse');
    setTimeout(() => {
      themeToggle.classList.remove('pulse');
    }, 300);

    // Toggle theme with animation
    setTheme(!document.body.classList.contains('dark-mode'));

    // Update tooltip text based on theme
    const tooltipEl = themeToggle.querySelector('.toggle-tooltip');
    if (tooltipEl) {
      tooltipEl.textContent = document.body.classList.contains('dark-mode') ? 'الوضع الفاتح' : 'الوضع المظلم';
    }
  });

  // Listen for system preference changes
  window.matchMedia('(prefers-color-scheme: dark)').addEventListener('change', e => {
    if (!localStorage.getItem('theme')) {
      setTheme(e.matches);
    }
  });

  // Add hover effect
  themeToggle.addEventListener('mouseenter', () => {
    themeToggle.classList.add('hover');
  });

  themeToggle.addEventListener('mouseleave', () => {
    themeToggle.classList.remove('hover');
  });
}

// Add additional animations and enhanced interactions
function initializeAnimations() {
  // Add animation classes to elements
  const headlines = document.querySelectorAll('h1, h2, h3');
  headlines.forEach((headline, index) => {
    headline.classList.add('reveal', 'reveal-delay-' + (index % 3));
  });

  // Add animation to cards with staggered delay
  const cards = document.querySelectorAll('.card');
  cards.forEach((card, index) => {
    card.classList.add('reveal', 'reveal-slide-up');
    card.style.transitionDelay = `${index * 0.1}s`;
  });

  // Add animation to feature cards
  const featureCards = document.querySelectorAll('.feature-card');
  featureCards.forEach((card, index) => {
    card.classList.add('reveal', 'reveal-slide-up');
    card.style.transitionDelay = `${index * 0.15}s`;
  });

  // Add animation to testimonial cards
  const testimonialCards = document.querySelectorAll('.testimonial-card');
  testimonialCards.forEach((card, index) => {
    card.classList.add('reveal', 'reveal-slide-up');
    card.style.transitionDelay = `${index * 0.15}s`;
  });

  // Add animation to stats
  const statItems = document.querySelectorAll('.stat-item');
  statItems.forEach((item, index) => {
    item.classList.add('reveal', 'reveal-delay-' + index);
  });

  // Add animation to CTA section
  const ctaSection = document.querySelector('.cta-section');
  if (ctaSection) {
    ctaSection.classList.add('reveal');
  }

  // Add animation to hero section
  const heroSection = document.querySelector('.hero-section');
  if (heroSection) {
    heroSection.classList.add('animate__animated', 'animate__fadeIn');
    heroSection.style.animationDuration = '1.5s';
  }

  // Add animations to footer elements
  const footerHeadings = document.querySelectorAll('.footer-heading');
  footerHeadings.forEach((heading, index) => {
    heading.classList.add('reveal', 'reveal-slide-up');
    heading.style.transitionDelay = `${0.2 + (index * 0.1)}s`;
  });

  const footerInfo = document.querySelector('.footer-info');
  if (footerInfo) {
    footerInfo.classList.add('reveal', 'reveal-slide-up');
    footerInfo.style.transitionDelay = '0.3s';
  }

  const socialIcons = document.querySelector('.social-icons-container');
  if (socialIcons) {
    socialIcons.classList.add('reveal', 'reveal-slide-up');
    socialIcons.style.transitionDelay = '0.4s';

    // Add staggered animation to individual social icons
    const icons = socialIcons.querySelectorAll('.btn-social');
    icons.forEach((icon, index) => {
      icon.style.opacity = '0';
      icon.style.transform = 'translateY(20px)';

      setTimeout(() => {
        icon.style.transition = 'all 0.3s ease';
        icon.style.opacity = '1';
        icon.style.transform = 'translateY(0)';
      }, 500 + (index * 100));
    });
  }

  const footerLinks = document.querySelector('.footer-links');
  if (footerLinks) {
    footerLinks.classList.add('reveal', 'reveal-slide-up');
    footerLinks.style.transitionDelay = '0.5s';
  }

  // Initialize tooltips if Bootstrap is available
  if (typeof bootstrap !== 'undefined' && bootstrap.Tooltip) {
    const tooltipTriggerList = [].slice.call(document.querySelectorAll('[data-bs-toggle="tooltip"]'));
    tooltipTriggerList.map(function (tooltipTriggerEl) {
      return new bootstrap.Tooltip(tooltipTriggerEl);
    });
  }

  // Initialize popovers if Bootstrap is available
  if (typeof bootstrap !== 'undefined' && bootstrap.Popover) {
    const popoverTriggerList = [].slice.call(document.querySelectorAll('[data-bs-toggle="popover"]'));
    popoverTriggerList.map(function (popoverTriggerEl) {
      return new bootstrap.Popover(popoverTriggerEl, {
        trigger: 'focus'
      });
    });
  }
}

// Handle navbar links active state
const navLinks = document.querySelectorAll('.navbar-nav .nav-link');
navLinks.forEach(link => {
  if (link.getAttribute('href') === window.location.pathname) {
    link.classList.add('active');
  }
});

// Improve form interactions
const formInputs = document.querySelectorAll('.form-control');
formInputs.forEach(input => {
  input.addEventListener('focus', () => {
    input.parentElement.classList.add('focused');
  });

  input.addEventListener('blur', () => {
    input.parentElement.classList.remove('focused');
    if (input.value.trim() !== '') {
      input.parentElement.classList.add('filled');
    } else {
      input.parentElement.classList.remove('filled');
    }
  });

  // Set initial state for filled inputs
  if (input.value.trim() !== '') {
    input.parentElement.classList.add('filled');
  }
});

// Ensure contact form fields are always visible
document.addEventListener('DOMContentLoaded', function() {
  // Make sure all form elements in contact form are visible
  const contactFormGroups = document.querySelectorAll('#contact .form-group');
  if (contactFormGroups.length > 0) {
    contactFormGroups.forEach(function(el) {
      el.style.opacity = '1';
      el.style.transform = 'translateY(0)';
      el.style.visibility = 'visible';
      el.style.display = 'block';
    });
  }
});