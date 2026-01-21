(() => {
  const overlay = {
    dotnet: null,
    dragging: null,
    dragOffsetX: 0,
    dragOffsetY: 0,
    resizing: null,
    resizeStartX: 0,
    resizeStartY: 0,
    resizeStartWidth: 0,
    resizeStartHeight: 0,
    restoreDragging: null,
    restoreOffsetX: 0,
    restoreOffsetY: 0,
    restoreStartX: 0,
    restoreStartY: 0,
    restoreMoved: false,
    suppressRestoreClick: false,
    pendingRestore: false,
    lastRects: new Map(),
    pendingFrame: false,

    init(dotnetRef) {
      this.dotnet = dotnetRef;
      document.body.classList.add("overlay-mode");
      document.documentElement.classList.add("overlay-mode");
      this.attachEvents();
      this.attachMessages();
      this.sendViewport();
      setInterval(() => this.sendBounds(), 250);
      window.addEventListener("resize", () => this.sendViewport());
      window.addEventListener("blur", () => {
        if (this.dragging) {
          this.dragging = null;
          this.setDragState(false);
        }
        if (this.resizing) {
          this.resizing = null;
          this.setResizeState(false);
        }
      });
    },

    attachMessages() {
      window.addEventListener("message", (event) => {
        const data = event.data;
        if (!data || data.type !== "sidekick:close") {
          return;
        }

        const widgetId = typeof data.widgetId === "string" ? data.widgetId : null;
        if (widgetId && this.dotnet) {
          this.dotnet.invokeMethodAsync("CloseWidgetFromJs", widgetId);
          return;
        }

        const frame = Array.from(document.querySelectorAll(".overlay-widget-frame")).find(
          (node) => node.contentWindow === event.source
        );
        if (!frame) {
          return;
        }

        const widget = frame.closest(".overlay-widget");
        if (!widget || !widget.dataset.widgetId) {
          return;
        }

        if (!this.dotnet) {
          return;
        }

        this.dotnet.invokeMethodAsync("CloseWidgetFromJs", widget.dataset.widgetId);
      });
    },

    attachEvents() {
      document.addEventListener("mousedown", (event) => {
        const restoreControls = event.target.closest(".overlay-home-controls");
        if (restoreControls) {
          const rect = restoreControls.getBoundingClientRect();
          this.restoreDragging = restoreControls;
          this.restoreOffsetX = event.clientX - rect.left;
          this.restoreOffsetY = event.clientY - rect.top;
          this.restoreStartX = event.clientX;
          this.restoreStartY = event.clientY;
          this.restoreMoved = false;
          return;
        }

        const resizeHandle = event.target.closest(".overlay-widget-resize-handle");
        if (resizeHandle) {
          const widget = resizeHandle.closest(".overlay-widget");
          if (!widget) {
            return;
          }

          const rect = widget.getBoundingClientRect();
          this.resizing = widget;
          this.resizeStartX = event.clientX;
          this.resizeStartY = event.clientY;
          this.resizeStartWidth = rect.width;
          this.resizeStartHeight = rect.height;
          this.setResizeState(true);
          event.preventDefault();
          return;
        }

        const widget = event.target.closest(".overlay-widget");
        if (!widget) {
          return;
        }

        const dragHandle = event.target.closest(".overlay-widget-drag-handle");
        if (!dragHandle && !event.altKey) {
          return;
        }

        const rect = widget.getBoundingClientRect();
        this.dragging = widget;
        this.dragOffsetX = event.clientX - rect.left;
        this.dragOffsetY = event.clientY - rect.top;
        this.setDragState(true);
        event.preventDefault();
      });

      document.addEventListener("mousemove", (event) => {
        if (this.restoreDragging) {
          const distanceX = event.clientX - this.restoreStartX;
          const distanceY = event.clientY - this.restoreStartY;
          if (!this.restoreMoved) {
            const threshold = 6;
            if (Math.hypot(distanceX, distanceY) < threshold) {
              return;
            }

            this.restoreMoved = true;
            this.suppressRestoreClick = true;
          }

          const button = this.restoreDragging;
          const bounds = document.documentElement.getBoundingClientRect();
          let left = event.clientX - this.restoreOffsetX;
          let top = event.clientY - this.restoreOffsetY;

          left = Math.max(0, Math.min(left, bounds.width - button.offsetWidth));
          top = Math.max(0, Math.min(top, bounds.height - button.offsetHeight));

          button.style.left = `${Math.round(left)}px`;
          button.style.top = `${Math.round(top)}px`;
          this.queueRestoreUpdate(Math.round(left), Math.round(top));
          event.preventDefault();
          return;
        }

        if (this.resizing) {
          const widget = this.resizing;
          const rect = widget.getBoundingClientRect();
          const deltaX = event.clientX - this.resizeStartX;
          const deltaY = event.clientY - this.resizeStartY;
          const minWidth = 240;
          const minHeight = 180;
          const maxWidth = Math.max(minWidth, window.innerWidth - rect.left);
          const maxHeight = Math.max(minHeight, window.innerHeight - rect.top);
          const width = Math.min(maxWidth, Math.max(minWidth, this.resizeStartWidth + deltaX));
          const height = Math.min(maxHeight, Math.max(minHeight, this.resizeStartHeight + deltaY));

          widget.style.width = `${Math.round(width)}px`;
          widget.style.height = `${Math.round(height)}px`;
          this.queueBounds();
          return;
        }

        if (!this.dragging) {
          return;
        }

        const widget = this.dragging;
        const parentRect = document.documentElement.getBoundingClientRect();
        let left = event.clientX - this.dragOffsetX;
        let top = event.clientY - this.dragOffsetY;

        left = Math.max(0, Math.min(left, parentRect.width - widget.offsetWidth));
        top = Math.max(0, Math.min(top, parentRect.height - widget.offsetHeight));

        widget.style.left = `${Math.round(left)}px`;
        widget.style.top = `${Math.round(top)}px`;
        this.queueBounds();
      });

      document.addEventListener("mouseup", () => {
        if (this.dragging) {
          this.dragging = null;
          this.queueBounds();
          this.setDragState(false);
        }
        if (this.resizing) {
          this.resizing = null;
          this.queueBounds();
          this.setResizeState(false);
        }
        if (this.restoreDragging) {
          if (this.restoreMoved) {
            setTimeout(() => {
              this.suppressRestoreClick = false;
            }, 0);
          }
          this.restoreDragging = null;
        }
      });

      document.addEventListener(
        "click",
        (event) => {
          const restoreControls = event.target.closest(".overlay-home-controls");
          if (!restoreControls) {
            return;
          }

          if (this.suppressRestoreClick) {
            this.suppressRestoreClick = false;
            event.preventDefault();
            event.stopPropagation();
          }
        },
        true
      );
    },

    sendViewport() {
      if (!this.dotnet) {
        return;
      }

      this.dotnet.invokeMethodAsync("UpdateViewport", window.innerWidth, window.innerHeight);
    },

    sendBounds() {
      if (!this.dotnet) {
        return;
      }

      const widgets = document.querySelectorAll(".overlay-widget");
      const rects = [];
      widgets.forEach((widget) => {
        const id = widget.dataset.widgetId;
        if (!id) {
          return;
        }

        const rect = widget.getBoundingClientRect();
        const next = {
          id,
          x: Math.round(rect.left),
          y: Math.round(rect.top),
          width: Math.round(rect.width),
          height: Math.round(rect.height),
        };

        const last = this.lastRects.get(id);
        if (
          last &&
          last.x === next.x &&
          last.y === next.y &&
          last.width === next.width &&
          last.height === next.height
        ) {
          return;
        }

        this.lastRects.set(id, next);
        rects.push(next);
      });

      if (rects.length === 0) {
        return;
      }

      this.dotnet.invokeMethodAsync("UpdateWidgetBounds", rects);
    },

    queueBounds() {
      if (this.pendingFrame) {
        return;
      }

      this.pendingFrame = true;
      requestAnimationFrame(() => {
        this.pendingFrame = false;
        this.sendBounds();
      });
    },

    setDragState(active) {
      const root = document.documentElement;
      if (active) {
        root.classList.add("overlay-dragging");
        document.body?.classList.add("overlay-dragging");
      } else {
        root.classList.remove("overlay-dragging");
        document.body?.classList.remove("overlay-dragging");
      }
    },

    setResizeState(active) {
      const root = document.documentElement;
      if (active) {
        root.classList.add("overlay-resizing");
        document.body?.classList.add("overlay-resizing");
      } else {
        root.classList.remove("overlay-resizing");
        document.body?.classList.remove("overlay-resizing");
      }
    },


    queueRestoreUpdate(x, y) {
      if (!this.dotnet) {
        return;
      }

      if (this.pendingRestore) {
        return;
      }

      this.pendingRestore = true;
      requestAnimationFrame(() => {
        this.pendingRestore = false;
        this.dotnet.invokeMethodAsync("UpdateRestoreButtonPosition", x, y);
      });
    },
  };

  window.sidekickOverlay = overlay;
})();
