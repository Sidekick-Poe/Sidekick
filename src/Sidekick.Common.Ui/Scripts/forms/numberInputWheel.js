/**
 * Prevent scrolling the page when using the mouse wheel.
 * Allows the user to increment or decrement the value of a number input using the mouse wheel.
 */
export default (elementId) => {
    const element = document.getElementById(elementId);
    if (!element) return;

    element.addEventListener("wheel", (event) => {
        event.preventDefault();

        if (event.deltaY < 0)
            element.stepUp();
        else
            element.stepDown();
    }, { passive: false });
};
