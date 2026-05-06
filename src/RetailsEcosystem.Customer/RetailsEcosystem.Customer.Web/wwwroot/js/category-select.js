'use strict';

document.addEventListener('DOMContentLoaded', () => {

  // ── CategorySelectDropdown ──────────────────────────────────────────────────
  // Panel is appended to <body> and positioned with getBoundingClientRect() so
  // no ancestor overflow or stacking context can clip it.

  class CategorySelectDropdown {
    constructor(mountEl) {
      this.el           = mountEl;
      this.apiUrl       = mountEl.dataset.apiUrl || '/categories/paged';
      this.placeholder  = mountEl.dataset.placeholder || 'All Categories';
      this.selectedId   = mountEl.dataset.selectedId || '';
      this.selectedName = mountEl.dataset.selectedName || '';
      this.page    = 1;
      this.hasMore = true;
      this.loading = false;
      this.items   = [];
      this.open    = false;
      this._scrollHandler = null;

      this._buildDOM();
      this._bindEvents();
      mountEl._csInstance = this;
    }

    _buildDOM() {
      // Trigger button — looks like a form-select, stays in the mount element
      this.trigger = document.createElement('button');
      this.trigger.type = 'button';
      this.trigger.className = 'cs-trigger form-select text-start';
      this.trigger.textContent = (this.selectedId && this.selectedName)
        ? this.selectedName
        : this.placeholder;

      // Panel appended to <body> so no ancestor clips it
      this.panel = document.createElement('div');
      this.panel.className = 'cs-panel';
      this.panel.hidden = true;
      document.body.appendChild(this.panel);

      // Sentinel div — IntersectionObserver target for infinite scroll
      this.sentinel = document.createElement('div');
      this.sentinel.className = 'cs-sentinel';

      // Loading spinner
      this.spinner = document.createElement('div');
      this.spinner.className = 'cs-loading';
      this.spinner.hidden = true;
      this.spinner.innerHTML =
        '<span class="spinner-border spinner-border-sm text-primary" role="status" aria-hidden="true"></span>';

      this.panel.appendChild(this.sentinel);
      this.panel.appendChild(this.spinner);

      // Hidden input carries the value on form submit — stays in mount element
      this.hiddenInput = document.createElement('input');
      this.hiddenInput.type  = 'hidden';
      this.hiddenInput.name  = 'categoryId';
      this.hiddenInput.value = this.selectedId;

      this.el.appendChild(this.trigger);
      this.el.appendChild(this.hiddenInput);

      // IntersectionObserver on the panel's sentinel (panel is in body, always unclipped)
      this._observer = new IntersectionObserver((entries) => {
        if (entries[0].isIntersecting && this.hasMore && !this.loading) {
          this._fetchNext();
        }
      }, { threshold: 0.1 });
      this._observer.observe(this.sentinel);
    }

    _bindEvents() {
      this.trigger.addEventListener('click', () => {
        this.open ? this._close() : this._openPanel();
      });

      // Close when user clicks outside both the mount element and the body panel
      document.addEventListener('mousedown', (e) => {
        if (!this.el.contains(e.target) && !this.panel.contains(e.target)) {
          this._close();
        }
      });
    }

    _positionPanel() {
      const rect = this.trigger.getBoundingClientRect();
      this.panel.style.position = 'fixed';
      this.panel.style.top      = `${rect.bottom + 2}px`;
      this.panel.style.left     = `${rect.left}px`;
      this.panel.style.width    = `${rect.width}px`;
      // this.panel.style.zIndex   = '9999';
    }

    _openPanel() {
      this.open = true;
      this._positionPanel();
      this.panel.hidden = false;

      if (this.items.length === 0) {
        this._prependAllOption();
        this._fetchNext();
      }

      // Keep panel aligned while page scrolls or window resizes
      this._scrollHandler = () => this._positionPanel();
      window.addEventListener('scroll', this._scrollHandler, { passive: true });
      window.addEventListener('resize', this._scrollHandler, { passive: true });
    }

    _close() {
      this.open = false;
      this.panel.hidden = true;
      if (this._scrollHandler) {
        window.removeEventListener('scroll', this._scrollHandler);
        window.removeEventListener('resize', this._scrollHandler);
        this._scrollHandler = null;
      }
    }

    _prependAllOption() {
      const opt = this._makeOption('', this.placeholder, this.selectedId === '');
      this.panel.insertBefore(opt, this.sentinel);
    }

    _makeOption(id, name, isSelected) {
      const opt = document.createElement('div');
      opt.className = 'cs-option' + (isSelected ? ' cs-option--selected' : '');
      opt.dataset.value = id;
      opt.textContent = name;
      opt.addEventListener('click', () => this._select(id, name));
      return opt;
    }

    async _fetchNext() {
      if (this.loading || !this.hasMore) return;
      this.loading = true;
      this.spinner.hidden = false;
      try {
        const res  = await fetch(`${this.apiUrl}?pageNumber=${this.page}&pageSize=10`);
        const data = await res.json();
        const fetched = data.items || [];
        fetched.forEach(item => {
          const opt = this._makeOption(
            String(item.id),
            item.name,
            String(item.id) === String(this.selectedId)
          );
          this.panel.insertBefore(opt, this.sentinel);
          this.items.push(item);
        });
        this.hasMore = this.page < data.totalPage;
        this.page++;
      } catch (err) {
        console.error('[CategorySelect] fetch error:', err);
      } finally {
        this.loading = false;
        this.spinner.hidden = true;
      }
    }

    _select(id, name) {
      this.selectedId = id;
      this.hiddenInput.value = id;
      this.trigger.textContent = id ? name : this.placeholder;

      this.panel.querySelectorAll('.cs-option').forEach(o => {
        o.classList.toggle('cs-option--selected', o.dataset.value === id);
      });

      this._close();
      this.el.dispatchEvent(new CustomEvent('cs:change', { bubbles: true }));
    }

    reset() {
      this._select('', '');
    }
  }

  document.querySelectorAll('[data-category-select]').forEach(el => {
    el.classList.add('cs-wrapper');
    new CategorySelectDropdown(el);
  });


  // ── MegaMenuInfiniteScroll ──────────────────────────────────────────────────
  // Fetches categories lazily on hover; IntersectionObserver uses the list as
  // root so it only fires when the user actually scrolls inside the visible menu.
  // Observer is attached on mouseenter and detached on mouseleave to prevent
  // phantom triggers while the menu is CSS-hidden.

  class MegaMenuInfiniteScroll {
    constructor(wrapperEl) {
      this.wrapper      = wrapperEl;
      this.list         = wrapperEl.querySelector('#mega-cat-list');
      this.apiUrl       = wrapperEl.dataset.apiUrl || '/categories/paged';
      this.page         = 1;
      this.hasMore      = true;
      this.loading      = false;
      this.loaded       = false;
      this.menuVisible  = false;
      this._sentinel    = null;
      this._observer    = null;
      this._bindHover();
    }

    _bindHover() {
      this.wrapper.addEventListener('mouseenter', () => {
        this.menuVisible = true;
        if (!this.loaded && !this.loading) this._fetchNext();
        this._attachObserver();
      });

      this.wrapper.addEventListener('mouseleave', () => {
        this.menuVisible = false;
        this._detachObserver();
      });
    }

    _attachObserver() {
      if (this._observer || !this._sentinel) return;
      this._observer = new IntersectionObserver((entries) => {
        if (entries[0].isIntersecting && this.hasMore && !this.loading && this.menuVisible) {
          this._fetchNext();
        }
      }, { root: this.list, threshold: 0.1 });
      this._observer.observe(this._sentinel);
    }

    _detachObserver() {
      if (!this._observer) return;
      this._observer.disconnect();
      this._observer = null;
    }

    _ensureSentinel() {
      if (this._sentinel) return;
      this._sentinel = document.createElement('li');
      this._sentinel.style.height = '1px';
      this.list.appendChild(this._sentinel);
      // Attach observer now if menu is still visible
      if (this.menuVisible) this._attachObserver();
    }

    async _fetchNext() {
      if (this.loading || !this.hasMore) return;
      this.loading = true;
      try {
        const res  = await fetch(`${this.apiUrl}?pageNumber=${this.page}&pageSize=12`);
        const data = await res.json();
        const items = data.items || [];
        items.forEach(cat => {
          const li = document.createElement('li');
          const a  = document.createElement('a');
          a.href        = `/Products/ProductIndex?categoryId=${cat.id}`;
          a.textContent = cat.name;
          li.appendChild(a);
          if (this._sentinel) {
            this.list.insertBefore(li, this._sentinel);
          } else {
            this.list.appendChild(li);
          }
        });
        this.hasMore = this.page < (data.totalPage || 1);
        this.page++;
        this.loaded = true;
        this._ensureSentinel();
      } catch (err) {
        console.error('[MegaMenu] fetch error:', err);
      } finally {
        this.loading = false;
      }
    }
  }

  const megaWrapper = document.querySelector('.mega-category-menu[data-api-url]');
  if (megaWrapper) new MegaMenuInfiniteScroll(megaWrapper);


  // ── Header search clear button ────────────────────────────────────────────────
  const form        = document.getElementById('header-search-form');
  const searchInput = document.getElementById('header-search-input');
  const clearBtn    = document.getElementById('header-search-clear');
  if (form && searchInput && clearBtn) {
    function categoryInput() {
      return form.querySelector('input[name="categoryId"]');
    }

    function syncVisibility() {
      const hasText = searchInput.value.trim().length > 0;
      const hasCat  = (categoryInput()?.value ?? '') !== '';
      clearBtn.hidden = !(hasText || hasCat);
    }

    searchInput.addEventListener('input', syncVisibility);
    document.addEventListener('cs:change', syncVisibility);

    clearBtn.addEventListener('click', () => {
      searchInput.value = '';
      const mountEl = form.querySelector('[data-category-select]');
      if (mountEl && mountEl._csInstance) mountEl._csInstance.reset();
      syncVisibility();
      form.submit();
    });

    syncVisibility();
  }

});
