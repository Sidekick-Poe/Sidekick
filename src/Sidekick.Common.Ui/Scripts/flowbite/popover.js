import {Popover} from 'flowbite';

export default (triggerId, parameters) => {
    const targetId = parameters[0];
    const placement = parameters[1] || 'top';
    const trigger = parameters[2] || 'click';

    const $targetEl = document.getElementById(targetId);
    const $triggerEl = document.getElementById(triggerId);

    if ($targetEl === null || $triggerEl === null) {
        console.warn(`[Sidekick] Flowbite tooltip: target element #${targetId} or trigger element #${triggerId} not found.`);
        return {
            destroy() {
            },
        };
    }

    console.log(`[Sidekick] Flowbite tooltip: initializing tooltip for target element #${targetId} and trigger element #${triggerId}`);
    const options = {
        placement: placement,
        triggerType: trigger,
    };

    return new Popover($targetEl, $triggerEl, options);
};
