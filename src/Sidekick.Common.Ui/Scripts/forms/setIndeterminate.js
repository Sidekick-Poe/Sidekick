export default (elementId, value) => {
    document.getElementById(elementId).checked = value === true;
    document.getElementById(elementId).indeterminate = value === null;
};
