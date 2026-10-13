// Zvýraznění kódu, tlačítko "kopírovat" a lišta průběhu čtení.
// window.blogEnhance() volá i Blazor komponenta v OnAfterRenderAsync.
// highlight.js (~130 kB) se stahuje líně – jen když stránka opravdu obsahuje bloky kódu.

(function () {
  var isEn = (document.documentElement.lang || '').toLowerCase().indexOf('en') === 0;
  var T = {
    copy: isEn ? 'Copy' : 'Kopírovat',
    copied: isEn ? 'Copied ✓' : 'Zkopírováno ✓'
  };

  var hljsLoading = null;
  function loadHljs() {
    if (window.hljs) return Promise.resolve();
    if (hljsLoading) return hljsLoading;
    hljsLoading = new Promise(function (resolve, reject) {
      var s = document.createElement('script');
      s.src = '/lib/hljs/highlight.min.js';
      s.onload = resolve;
      s.onerror = reject;
      document.head.appendChild(s);
    });
    return hljsLoading;
  }

  function highlight() {
    var blocks = document.querySelectorAll('article pre code:not([data-highlighted])');
    if (!blocks.length) return;
    loadHljs().then(function () {
      blocks.forEach(function (block) {
        try { window.hljs.highlightElement(block); } catch (e) { /* ignore */ }
      });
    }).catch(function () { /* bez zvýraznění je kód pořád čitelný */ });
  }

  function addCopyButtons() {
    document.querySelectorAll('article pre').forEach(function (pre) {
      if (pre.querySelector('.copy-btn')) return;
      var btn = document.createElement('button');
      btn.type = 'button';
      btn.className = 'copy-btn';
      btn.textContent = T.copy;
      btn.addEventListener('click', function () {
        var code = pre.querySelector('code');
        navigator.clipboard.writeText(code ? code.innerText : pre.innerText).then(function () {
          btn.textContent = T.copied;
          setTimeout(function () { btn.textContent = T.copy; }, 1500);
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
