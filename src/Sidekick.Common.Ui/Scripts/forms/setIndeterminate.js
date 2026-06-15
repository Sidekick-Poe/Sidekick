export default (elementId, value) => {
    const element = document.getElementById(elementId);
    if (!element) return;

    element.checked = value === true;
    element.indeterminate = value === null;
};