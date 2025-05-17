export const initializeIntersectionObserver = (elementId, dotNetRef) => {
    const element = document.getElementById(elementId);
    if (!element) {
        return;
    }

    const observer = new IntersectionObserver((entries) => {
        entries.forEach(entry => {
            if (entry.isIntersecting) {
                dotNetRef.invokeMethodAsync('OnIntersecting');
            }
        });
    }, { threshold: 1 });

    observer.observe(element);
};
