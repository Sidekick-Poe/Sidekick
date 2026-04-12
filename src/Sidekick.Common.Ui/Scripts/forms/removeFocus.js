export default (elementId) => {
    const element = document.getElementById(elementId);
    if (element) {
        element.blur();
    }
};
