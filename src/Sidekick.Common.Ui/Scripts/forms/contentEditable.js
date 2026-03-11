export default (elementId, dotNetRef) => {

    const element = document.getElementById(elementId);

    const input = () => {
        dotNetRef.invokeMethodAsync("Update", element.innerHTML);
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
        range.selectNodeContents(this); // Select all the content
        selection.removeAllRanges();    // Clear any existing selection
        selection.addRange(range);      // Apply the new range as the selection
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

    const dispose = () => {
        element.removeEventListener("input", input);
        element.removeEventListener("focus", focus);
        element.removeEventListener("blur", blur);
    }

    return {
        setValue,
        dispose,
    };
}
