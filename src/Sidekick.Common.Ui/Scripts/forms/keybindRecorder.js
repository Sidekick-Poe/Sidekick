/**
 * Runs while a hotkey is being recorded, until disposed. The key itself is read by the keyboard hook in .NET.
 * Calls a .NET method when the mouse is pressed outside the element, instead of relying on the field's focus and blur,
 * which the web view on Linux does not fire.
 * Blocks the default action of keys, like scrolling with Space, since the hook can't block keys on Linux.
 * Also calls the .NET method when the window loses focus or is hidden, for the web version.
 * The desktop app relies on the window's Deactivated event instead, since these don't fire on Linux either.
 */
export default (elementId, dotNetReference, methodName) => {
    const cancel = () => {
        dotNetReference.invokeMethodAsync(methodName);
    };

    const mousedown = (event) => {
        const element = document.getElementById(elementId);
        if (element && element.contains(event.target)) return;

        cancel();
    };

    const visibilitychange = () => {
        if (document.hidden) cancel();
    };

    const keydown = (event) => {
        event.preventDefault();
    };

    document.addEventListener("mousedown", mousedown, true);
    document.addEventListener("keydown", keydown, true);
    document.addEventListener("visibilitychange", visibilitychange);
    window.addEventListener("blur", cancel);

    const dispose = () => {
        document.removeEventListener("mousedown", mousedown, true);
        document.removeEventListener("keydown", keydown, true);
        document.removeEventListener("visibilitychange", visibilitychange);
        window.removeEventListener("blur", cancel);
    };

    return {
        dispose,
    };
};
