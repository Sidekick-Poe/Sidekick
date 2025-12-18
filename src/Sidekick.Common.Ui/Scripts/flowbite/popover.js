import {Popover} from 'flowbite';

export default (triggerId, parameters) => {
    try {
        const targetId = parameters[0];
        const placement = parameters[1] || 'top';
        const trigger = parameters[2] || 'click';

        const targetElement = document.getElementById(targetId);
        const triggerElement = document.getElementById(triggerId);

        if (targetElement === null || triggerElement === null) {
            console.warn(`[Sidekick] Flowbite popover: target element #${targetId} or trigger element #${triggerId} not found.`);
            return {
                destroy() {
                },
            };
        }

        console.log(`[Sidekick] Flowbite popover: initializing for target element #${targetId} and trigger element #${triggerId}`);
        const options = {
            placement: placement,
            triggerType: trigger,
        };

        return new Popover(targetElement, triggerElement, options);
    } catch (e) {
        console.error(e);
    }
};
