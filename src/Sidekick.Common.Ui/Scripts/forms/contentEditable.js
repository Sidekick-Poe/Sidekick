export default (elementId, dotNetRef) => {

    const element = document.getElementById(elementId);

    const input = () => {
        let value = (element.innerHTML ?? '').trim();
        if (value === '<br>') value = '';

        dotNetRef.invokeMethodAsync("Update", value);
    };
    element.addEventListener("input", input);

    // Select all text when the contenteditable element is focused
    const focus = () => {
        if (element.innerHTML === element.dataset.placeholder) {
            element.innerHTML = '';
            element.classList.remove('text-xs');
        }

        const range = document.createRange();
        const selection = window.getSelection();
        range.selectNodeContents(element); // Select all the content
        selection.removeAllRanges();       // Clear any existing selection
        selection.addRange(range);         // Apply the new range as the selection
    };
    element.addEventListener("focus", focus);

    const blur = () => {
        const value = (element.innerHTML ?? '').trim();
        if (value === '' || value === '<br>') {
            element.innerHTML = element.dataset.placeholder;
            element.classList.add('text-xs');
        }
    };
    element.addEventListener("blur", blur);

    const setValue = (value) => {
        const element = document.getElementById(elementId);
        element.innerHTML = value;
        if (value === '' || value === null) {
            element.innerHTML = element.dataset.placeholder;
            element.classList.add('text-xs');
        } else {
            element.classList.remove('text-xs');
        }
    }

    /**
     * Prevent scrolling the page when using the mouse wheel.
     */
    const wheel = (event) => {
        event.preventDefault();
    };
    element.addEventListener("wheel", wheel, { passive: false });

    const dispose = () => {
        element.removeEventListener("input", input);
        element.removeEventListener("focus", focus);
        element.removeEventListener("blur", blur);
        element.removeEventListener("wheel", wheel);
    }

    return {
        setValue,
        dispose,
    };
}
