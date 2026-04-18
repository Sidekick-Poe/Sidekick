import {Popover} from 'flowbite';

export default (triggerId, parameters) => {
    try {
        const targetId = parameters[0];
        const dotNetRef = parameters[1] || null;
        const placement = parameters[2] || 'top';
        const trigger = parameters[3] || 'click';

        const targetElement = document.getElementById(targetId);
        const triggerElement = document.getElementById(triggerId);

        if (targetElement === null || triggerElement === null) {
            console.warn(`[Sidekick] Flowbite popover: target element #${targetId} or trigger element #${triggerId} not found.`);
            return {
                destroy() {
                },
            };
        }

        let visible = false;

        const onShow = async () => {
            if (!visible && dotNetRef != null) dotNetRef.invokeMethodAsync('OnShow');
            visible = true;
        };

        const onHide = async () => {
            visible = false;
        };

        console.log(`[Sidekick] Flowbite popover: initializing for target element #${targetId} and trigger element #${triggerId}`);
        const options = {
            placement: placement,
            triggerType: trigger,
            onShow,
            onHide,
        };

        return new Popover(targetElement, triggerElement, options);
    } catch (e) {
        console.error(e);
    }
};
