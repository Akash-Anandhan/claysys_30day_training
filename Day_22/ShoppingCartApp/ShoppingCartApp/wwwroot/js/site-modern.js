/* ============================================
   SHOPIEE - Modern Global JS Enhancements
   ============================================ */

document.addEventListener('DOMContentLoaded', () => {

  // --- Navbar scroll effect ---
  const navbar = document.querySelector('.navbar');
  if (navbar) {
    const onScroll = () => {
      navbar.classList.toggle('scrolled', window.scrollY > 30);
    };
    window.addEventListener('scroll', onScroll, { passive: true });
    onScroll();
  }

  // --- Staggered card reveal on scroll ---
  const cards = document.querySelectorAll('.card');
  if (cards.length && 'IntersectionObserver' in window) {
    cards.forEach(card => {
      card.style.opacity = '0';
      card.style.transform = 'translateY(30px)';
      card.style.transition = 'opacity .5s ease, transform .5s ease';
    });

    const observer = new IntersectionObserver((entries) => {
      entries.forEach((entry, i) => {
        if (entry.isIntersecting) {
          const card = entry.target;
          const idx = Array.from(cards).indexOf(card);
          setTimeout(() => {
            card.style.opacity = '1';
            card.style.transform = 'translateY(0)';
          }, idx * 80);
          observer.unobserve(card);
        }
      });
    }, { threshold: 0.1, rootMargin: '0px 0px -40px 0px' });

    cards.forEach(card => observer.observe(card));
  }

  // --- Hero text reveal ---
  const hero = document.querySelector('.hero-section, .bg-dark.text-white.text-center.py-5');
  if (hero) {
    const heroChildren = hero.querySelectorAll('h1, p, a, .btn');
    heroChildren.forEach((el, i) => {
      el.style.opacity = '0';
      el.style.transform = 'translateY(20px)';
      el.style.transition = `opacity .6s ease ${i * .15}s, transform .6s ease ${i * .15}s`;
      requestAnimationFrame(() => {
        el.style.opacity = '1';
        el.style.transform = 'translateY(0)';
      });
    });
  }

  // --- Smooth scroll for anchor links ---
  document.querySelectorAll('a[href^="#"]').forEach(anchor => {
    anchor.addEventListener('click', function (e) {
      const target = document.querySelector(this.getAttribute('href'));
      if (target) {
        e.preventDefault();
        target.scrollIntoView({ behavior: 'smooth', block: 'start' });
      }
    });
  });

  // --- Add to cart button micro-interaction ---
  document.querySelectorAll('form[action*="AddToCart"] button[type="submit"]').forEach(btn => {
    btn.addEventListener('click', function () {
      const original = this.innerHTML;
      this.innerHTML = '<i class="bi bi-check-lg"></i> Added!';
      this.style.background = '#16a34a';
      setTimeout(() => {
        this.innerHTML = original;
        this.style.background = '';
      }, 1200);
    });
  });

});
