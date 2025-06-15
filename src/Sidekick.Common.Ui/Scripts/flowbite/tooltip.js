import {Tooltip} from 'flowbite';

export default (triggerId, parameters) => {
    const targetId = parameters[0];
    const placement = parameters[1] || 'top';

    const $targetEl = document.getElementById(targetId);
    const $triggerEl = document.getElementById(triggerId);

    if ($targetEl === null || $triggerEl === null) {
        console.warn(`[Sidekick] Flowbite tooltip: target element #${targetId} or trigger element #${triggerId} not found.`);
        return {
            destroy() {
            },
        };
    }

    console.log(`[Sidekick] Flowbite tooltip: initializing for target element #${targetId} and trigger element #${triggerId}`);
    const options = {
        placement: placement,
        triggerType: 'hover',
    };

    return new Tooltip($targetEl, $triggerEl, options);
};
