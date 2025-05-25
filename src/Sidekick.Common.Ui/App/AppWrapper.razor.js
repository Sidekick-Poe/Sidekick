export const initializeZoomHandling = () => {
    let zoom = 1;

    const updateVh = () => {
        const vh = parseInt(window.innerHeight / zoom);
        document.documentElement.style.setProperty("--sidekick-vh", `${vh}px`);
    };

    window.addEventListener("resize", updateVh);

    return {
        update: (value) => {
            zoom = value;
            document.documentElement.style.setProperty("--sidekick-zoom", zoom);
            updateVh();
        },
    };
};

export const initializeResizeHandling = (elementId, dotNetRef) => {
    const element = document.getElementById(elementId);

    let lastResizeTime = 0;

    interact(element).resizable({
        edges: { bottom: true, right: true },
        listeners: {
            move: function (event) {
                let { x, y } = event.target.dataset;

                x = (parseFloat(x) || 0) + event.deltaRect.left;
                y = (parseFloat(y) || 0) + event.deltaRect.top;

                Object.assign(event.target.style, {
                    width: `${event.rect.width}px`,
                    height: `${event.rect.height}px`,
                });

                Object.assign(event.target.dataset, { x, y });

                const now = Date.now();
                if (now - lastResizeTime >= 50) {
                    lastResizeTime = now;
                    dotNetRef.invokeMethodAsync("OnResize", event.rect.width, event.rect.height);
                }
            },
        },
    });
};
