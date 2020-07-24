window.blginterop = (function () {

    return {

        getElementScrollTop : function (elementRef) {
            return elementRef.scrollTop ;
        },

        getBoundingClientRect: function (elementRef) {
            return elementRef.getBoundingClientRect();
        },

        getElementScrollWidth: function (elementRef) {
            return elementRef.scrollWidth;
        }
    };
})();