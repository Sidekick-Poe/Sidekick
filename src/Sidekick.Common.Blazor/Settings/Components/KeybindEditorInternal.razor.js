export const removeFocus = (elementId) => {
    const element = document.getElementById(elementId);
    if (element) {
        element.blur();
    }
};
