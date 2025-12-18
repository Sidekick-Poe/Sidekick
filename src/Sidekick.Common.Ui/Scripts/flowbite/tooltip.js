import {Tooltip} from 'flowbite';

export default (triggerId, parameters) => {
    try {
        const targetId = parameters[0];
        const placement = parameters[1] || 'top';

        const targetElement = document.getElementById(targetId);
        const triggerElement = document.getElementById(triggerId);

        if (targetElement === null || triggerElement === null) {
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
            onShow: () => {
                targetElement.classList.remove('hidden');
            }
        };

        targetElement.classList.add('hidden');

        return new Tooltip(targetElement, triggerElement, options);
    } catch (e) {
        console.error(e);
    }
};
