'use strict';

document.addEventListener('DOMContentLoaded', () => {

  // ── CategorySelectDropdown ────────────────────────────────────────────────
  class CategorySelectDropdown {
    constructor(mountEl) {
      this.mount       = mountEl;
      this.apiUrl      = mountEl.dataset.apiUrl;
      this.placeholder = mountEl.dataset.placeholder || 'All Categories';
      this.selectedId  = mountEl.dataset.selectedId   || '';
      this.selectedName= mountEl.dataset.selectedName || '';
      this.page        = 1;
      this.pageSize    = 10;
      this.loading     = false;
      this.done        = false;
      this._onResize = () => this._positionPanel();
      this._onScroll = (e) => {
        if (this.panel.contains(e.target)) return;
        this._close();
      };
      this._buildDOM();
      this._bindEvents();
      if (this.selectedId) {
        this._fetchNext(); // pre-load so selected item appears highlighted on open
      }
    }

    _buildDOM() {
      // Trigger button
      this.trigger = document.createElement('button');
      this.trigger.type = 'button';
      this.trigger.className = 'cs-trigger btn btn-outline-secondary btn-sm w-100';
      this.trigger.innerHTML = `<span class="cs-trigger-label">${this.selectedName || this.placeholder}</span><i class="lni lni-chevron-down cs-trigger-caret"></i>`;
      this.mount.appendChild(this.trigger);

      // Hidden input picked up by the surrounding <form>
      this.hiddenInput = document.createElement('input');
      this.hiddenInput.type  = 'hidden';
      this.hiddenInput.name  = 'categoryId';
      this.hiddenInput.value = this.selectedId;
      this.mount.appendChild(this.hiddenInput);

      // Panel (appended to body so it escapes any overflow:hidden ancestors)
      this.panel = document.createElement('div');
      this.panel.className = 'cs-panel';
      this.panel.hidden = true;
      document.body.appendChild(this.panel);

      // "All Categories" option at top
      this._prependAllOption();

      // Sentinel for IntersectionObserver-based infinite scroll
      this.sentinel = document.createElement('div');
      this.sentinel.className = 'cs-sentinel';
      this.panel.appendChild(this.sentinel);

      // Loading spinner
      this.loadingEl = document.createElement('div');
      this.loadingEl.className = 'cs-loading';
      this.loadingEl.innerHTML = '<div class="spinner-border spinner-border-sm text-primary" role="status"><span class="visually-hidden">Loading…</span></div>';
      this.loadingEl.hidden = true;
      this.panel.appendChild(this.loadingEl);

      // IntersectionObserver fires _fetchNext when sentinel scrolls into view
      this._observer = new IntersectionObserver(entries => {
        if (entries[0].isIntersecting) this._fetchNext();
      }, { root: this.panel, threshold: 0.1 });
    }

    _bindEvents() {
      this.trigger.addEventListener('click', () => {
        if (this.panel.hidden) this._openPanel();
        else this._close();
      });

      this._outsideHandler = e => {
        if (!this.panel.contains(e.target) && !this.mount.contains(e.target)) this._close();
      };
    }

    _positionPanel() {
      const rect = this.trigger.getBoundingClientRect();
      this.panel.style.top   = `${rect.bottom + 4}px`;
      this.panel.style.left  = `${rect.left}px`;
      this.panel.style.width = `${rect.width}px`;
    }

    _openPanel() {
      this._positionPanel();
      this.panel.hidden = false;
      this.trigger.setAttribute('aria-expanded', 'true');
      this._observer.observe(this.sentinel);
      document.addEventListener('mousedown', this._outsideHandler);
      window.addEventListener('resize', this._onResize);
      window.addEventListener('scroll', this._onScroll, true);
    }

    _close() {
      this.panel.hidden = true;
      this.trigger.setAttribute('aria-expanded', 'false');
      this._observer.unobserve(this.sentinel);
      document.removeEventListener('mousedown', this._outsideHandler);
      window.removeEventListener('resize', this._onResize);
      window.removeEventListener('scroll', this._onScroll, true);
    }

    _prependAllOption() {
      const opt = this._makeOption('', this.placeholder, this.selectedId === '');
      this.panel.insertBefore(opt, this.panel.firstChild);
    }

    _makeOption(id, name, isSelected) {
      const el = document.createElement('div');
      el.className = 'cs-option' + (isSelected ? ' cs-option--selected' : '');
      el.textContent = name;
      el.addEventListener('click', () => this._select(id, name));
      return el;
    }

    async _fetchNext() {
      if (this.loading || this.done) return;
      this.loading = true;
      this.loadingEl.hidden = false;
      try {
        const url = `${this.apiUrl}?pageNumber=${this.page}&pageSize=${this.pageSize}`;
        const res  = await fetch(url);
        if (!res.ok) throw new Error(`HTTP ${res.status}`);
        const data = await res.json();
        const items = data.items ?? data.Items ?? [];
        items.forEach(cat => {
          const isSelected = String(cat.id ?? cat.Id) === this.selectedId;
          const opt = this._makeOption(String(cat.id ?? cat.Id), cat.name ?? cat.Name, isSelected);
          this.panel.insertBefore(opt, this.sentinel);
        });
        if (items.length < this.pageSize) {
          this.done = true;
          this._observer.unobserve(this.sentinel);
        }
        this.page++;
      } catch (err) {
        console.error('[CategorySelectDropdown] fetch error', err);
      } finally {
        this.loading = false;
        this.loadingEl.hidden = true;
      }
    }

    _select(id, name) {
      this.selectedId   = id;
      this.hiddenInput.value = id;
      this.trigger.querySelector('.cs-trigger-label').textContent = name || this.placeholder;
      // Update selected highlight
      this.panel.querySelectorAll('.cs-option').forEach(el => {
        el.classList.toggle('cs-option--selected', el.textContent === (name || this.placeholder) && (id === '' ? el === this.panel.firstElementChild : true));
      });
      this.mount.dispatchEvent(new CustomEvent('cs:change', { bubbles: true, detail: { id, name } }));
      this._close();
    }

    reset() {
      this._select('', this.placeholder);
    }
  }

  document.querySelectorAll('[data-category-select]').forEach(el => {
    el.classList.add('cs-wrapper');
    new CategorySelectDropdown(el);
  });

  // ── Header search clear button ────────────────────────────────────────────
  const form        = document.getElementById('header-search-form');
  const searchInput = document.getElementById('header-search-input');
  const clearBtn    = document.getElementById('header-search-clear');
  if (form && searchInput && clearBtn) {
    function syncVisibility() {
      clearBtn.hidden = searchInput.value.trim().length === 0;
    }
    searchInput.addEventListener('input', syncVisibility);
    clearBtn.addEventListener('click', () => {
      searchInput.value = '';
      syncVisibility();
      form.submit();
    });
    syncVisibility();
  }

});
