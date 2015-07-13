; (function ($) {
    if (window.registerTrigger) {

        window.registerTrigger("modalOpen", function (evt, m, xhr) {
            console.log(arguments);
        });

        window.registerTrigger("modalClose", function (evt, m, xhr) {
            //TODO:
        });
    }
})(jQuery);