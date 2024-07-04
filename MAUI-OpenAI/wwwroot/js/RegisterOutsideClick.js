window.registerOutsideClick = (dotNetHelper) => {
    document.addEventListener('click', (event) => {
        dotNetHelper.invokeMethodAsync('OnOutsideClick', event.clientX, event.clientY);
    });
};