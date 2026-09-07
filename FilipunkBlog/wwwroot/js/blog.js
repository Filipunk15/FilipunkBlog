// Zvýraznění kódu, tlačítko "kopírovat" a lišta průběhu čtení.
// window.blogEnhance() volá i Blazor komponenta v OnAfterRenderAsync.

(function () {
  function highlight() {
    if (!window.hljs) return;
    document.querySelectorAll('article pre code:not([data-highlighted])').forEach(function (block) {
      try { window.hljs.highlightElement(block); } catch (e) { /* ignore */ }
    });
  }

  function addCopyButtons() {
    document.querySelectorAll('article pre').forEach(function (pre) {
      if (pre.querySelector('.copy-btn')) return;
      var btn = document.createElement('button');
      btn.type = 'button';
      btn.className = 'copy-btn';
      btn.textContent = 'Kopírovat';
      btn.addEventListener('click', function () {
        var code = pre.querySelector('code');
        navigator.clipboard.writeText(code ? code.innerText : pre.innerText).then(function () {
          btn.textContent = 'Zkopírováno ✓';
          setTimeout(function () { btn.textContent = 'Kopírovat'; }, 1500);
        });
      });
      pre.appendChild(btn);
    });
  }

  function readingProgress() {
    var bar = document.getElementById('read-progress');
    if (!bar) return;
    var article = document.querySelector('article');
    if (!article) { bar.style.width = '0'; return; }
    var height = article.offsetHeight - window.innerHeight;
    var scrolled = window.scrollY - article.offsetTop;
    var pct = height > 0 ? Math.min(100, Math.max(0, (scrolled / height) * 100)) : 0;
    bar.style.width = pct + '%';
  }

  function enhance() {
    highlight();
    addCopyButtons();
    readingProgress();
  }

  window.blogEnhance = enhance;

  if (document.readyState === 'loading') {
    document.addEventListener('DOMContentLoaded', enhance);
  } else {
    enhance();
  }
  // Blazor enhanced navigace (statické SSR stránky)
  document.addEventListener('enhancedload', enhance);
  window.addEventListener('scroll', readingProgress, { passive: true });
  window.addEventListener('resize', readingProgress, { passive: true });
})();
