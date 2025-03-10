export const init = (elementId, dotNetRef) => {

    document.getElementById(elementId).addEventListener("input", function () {
        dotNetRef.invokeMethodAsync("Update", document.getElementById(elementId).innerHTML);
    });

    // Select all text when the contenteditable element is focused
    document.getElementById(elementId).addEventListener("focus", function () {
        const range = document.createRange();
        const selection = window.getSelection();
        range.selectNodeContents(this); // Select all the content
        selection.removeAllRanges();    // Clear any existing selection
        selection.addRange(range);      // Apply the new range as the selection
    });

}

export const setValue = (elementId, value) => {
    document.getElementById(elementId).innerHTML = value;
}
