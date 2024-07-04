window.getBoundingClientRect = (element) => {
    if (element) {
        const rect = element.getBoundingClientRect();
        return {
            left: rect.left,
            right: rect.right,
            top: rect.top,
            bottom: rect.bottom
        };
    }
    return null;
};