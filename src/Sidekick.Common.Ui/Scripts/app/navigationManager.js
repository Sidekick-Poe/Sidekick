let dotNetRef = null;

export default {
    register(ref){
        dotNetRef = ref;
    },
    navigateTo(url) {
        dotNetRef.invokeMethodAsync("NavigateTo", url);
    }
};
