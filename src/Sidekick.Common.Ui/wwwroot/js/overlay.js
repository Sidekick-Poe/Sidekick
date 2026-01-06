(() => {
  const overlay = {
    dotnet: null,
    dragging: null,
    dragOffsetX: 0,
    dragOffsetY: 0,
    lastRects: new Map(),
    pendingFrame: false,

    init(dotnetRef) {
      this.dotnet = dotnetRef;
      document.body.classList.add("overlay-mode");
      document.documentElement.classList.add("overlay-mode");
      this.attachEvents();
      this.sendViewport();
      setInterval(() => this.sendBounds(), 250);
      window.addEventListener("resize", () => this.sendViewport());
    },

    attachEvents() {
      document.addEventListener("mousedown", (event) => {
        const header = event.target.closest(".overlay-widget-header");
        if (!header) {
          return;
        }

        const widget = header.closest(".overlay-widget");
        if (!widget) {
          return;
        }

        const rect = widget.getBoundingClientRect();
        this.dragging = widget;
        this.dragOffsetX = event.clientX - rect.left;
        this.dragOffsetY = event.clientY - rect.top;
        event.preventDefault();
      });

      document.addEventListener("mousemove", (event) => {
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
        this.dragging = null;
        this.queueBounds();
      });
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
  };

  window.sidekickOverlay = overlay;
})();
