export const init = (elementId, dotNetRef) => {

    const element = document.getElementById(elementId);

    element.addEventListener("input", function () {
        dotNetRef.invokeMethodAsync("Update", element.innerHTML);
    });

    // Select all text when the contenteditable element is focused
    element.addEventListener("focus", function () {
        if (element.innerHTML === element.dataset.placeholder) {
            element.innerHTML = '';
            element.classList.remove('text-xs');
        }

        const range = document.createRange();
        const selection = window.getSelection();
        range.selectNodeContents(this); // Select all the content
        selection.removeAllRanges();    // Clear any existing selection
        selection.addRange(range);      // Apply the new range as the selection
    });

    element.addEventListener("blur", function () {
        const value = (element.innerHTML ?? '').trim();
        if (value === '' || value === '<br>') {
            element.innerHTML = element.dataset.placeholder;
            element.classList.add('text-xs');
        }
    });

}

export const setValue = (elementId, value) => {
    const element = document.getElementById(elementId);
    element.innerHTML = value;
    if (value === '' || value === null) {
        element.innerHTML = element.dataset.placeholder;
        element.classList.add('text-xs');
    } else {
        element.classList.remove('text-xs');
    }
}
