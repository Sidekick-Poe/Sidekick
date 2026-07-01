/**
 * Prevent scrolling the page when using the mouse wheel.
 * Allows the user to increment or decrement the value of a number input using the mouse wheel.
 */
export default (elementId) => {
    const element = document.getElementById(elementId);
    if (!element) return;

    var handler = (event) => {
        event.preventDefault();

        if (event.deltaY < 0)
            element.stepUp();
        else
            element.stepDown();
    };
    element.addEventListener("wheel", handler, {passive: false});

    const dispose = () => {
        if (!element) return;
        element.removeEventListener("wheel", handler);
    }

    return {
        dispose,
    };
};
