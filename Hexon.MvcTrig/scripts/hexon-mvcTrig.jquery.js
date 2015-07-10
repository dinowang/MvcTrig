; (function ($) {
    if (window.registerTrigger) {

        window.registerTrigger("event", function (evt, m) {
            $(m.selector).trigger(m.eventName, [m.data]);
        });

    }
})(jQuery);