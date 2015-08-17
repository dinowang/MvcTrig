; (function ($) {
    if (window.registerTrigger) {

        window.registerTrigger("modalOpen", function (evt, m, xhr) {
            $(m).modal();
        });

        window.registerTrigger("modalClose", function (evt, m, xhr) {
            //TODO:
        });
    }
})(jQuery);