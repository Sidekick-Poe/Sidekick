export const initializeZoomHandling = () => {
    let zoom = 1;

    const updateVh = () => {
        const vh = parseInt(window.innerHeight / zoom);
        document.documentElement.style.setProperty('--sidekick-vh', `${vh}px`);
    }

    window.addEventListener("resize", updateVh);

    return {
        update: (value) => {
            zoom = value;
            document.documentElement.style.setProperty('--sidekick-zoom', zoom);
            updateVh();
        },
    };
};


export const initializeEmbedHandling = () => {
    let embedded = false;
    try {
        embedded = window.self !== window.top;
    } catch {
        embedded = true;
    }

    if (embedded) {
        document.documentElement.classList.add('sidekick-embedded');
        if (document.body) {
            document.body.classList.add('sidekick-embedded');
        }
    }
};


export const notifyOverlayClose = () => {
    try {
        let widgetId = null;
        try {
            const url = new URL(window.location.href);
            widgetId = url.searchParams.get("overlayWidgetId");
        } catch {
            // ignore
        }

        if (!widgetId) {
            try {
                const frame = window.frameElement;
                if (frame && frame.dataset && frame.dataset.widgetId) {
                    widgetId = frame.dataset.widgetId;
                }
            } catch {
                // ignore
            }
        }
        if (window.parent && window.parent !== window) {
            window.parent.postMessage({ type: 'sidekick:close', widgetId }, '*');
        }
    } catch {
        // ignore
    }
};
