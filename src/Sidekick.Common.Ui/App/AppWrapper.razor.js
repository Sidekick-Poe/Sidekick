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
