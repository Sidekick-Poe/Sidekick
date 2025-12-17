import {Modal} from 'flowbite';

export default (triggerId, targetId) => {
    try {
        const targetElement = document.getElementById(targetId);
        const triggerElement = document.getElementById(triggerId);

        if (targetElement === null || triggerElement === null) {
            console.warn(`[Sidekick] Flowbite modal: target element #${targetId} or trigger element #${triggerId} not found.`);
            return {
                destroy() {
                },
            };
        }

        console.log(`[Sidekick] Flowbite modal: initializing for target element #${targetId}`);
        const options = {
            placement: 'top-left',
            backdrop: 'dynamic',
            backdropClasses: 'bg-stone-500/30 fixed inset-0 z-40',
            closable: false,
        };

        const instanceOptions = {
            id: `${targetId}-modal`,
            override: true
        };

        return new Modal(targetElement, options, instanceOptions);
    } catch (e) {
        console.error(e);
    }
};
