import intersectionObserver from "./app/intersectionObserver";
import navigationManager from "./app/navigationManager";
import scrollToBottom from "./app/scrollToBottom";
import zoomHandler from "./app/zoomHandler";

import contentEditable from "./forms/contentEditable";
import removeFocus from "./forms/removeFocus.js";
import setIndeterminate from "./forms/setIndeterminate.js";

import modal from "./flowbite/modal";
import popover from "./flowbite/popover";
import tooltip from "./flowbite/tooltip";

window.sidekick = {
    app: {
        intersectionObserver,
        navigationManager,
        scrollToBottom,
        zoomHandler,
    },
    flowbite: {
        modal,
        popover,
        tooltip,
    },
    forms: {
        contentEditable,
        removeFocus,
        setIndeterminate,
    },
};
