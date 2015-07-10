; (function ($) {
    if (window.registerTrigger) {

        window.registerTrigger("fancyOpen", function (evt, m) {
            var opt = {};
            if (m.width) opt.width = m.width;
            if (m.height) opt.height = m.height;

            openFancybox(m.url, opt);
        });

        window.registerTrigger("fancyClose", function (evt, m) {
            $.fancybox.close();
        });

        window.registerTrigger("fancyResize", function (evt, m) {
            $.fancybox.resize(m);
        });
    }
})(jQuery);