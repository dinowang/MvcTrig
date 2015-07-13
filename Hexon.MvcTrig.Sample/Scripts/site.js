function openFancybox(url, options) {
    if (!url || url.match(/^\s*javascript/)) {
        return;
    }

    var defaults = {
        type: "iframe",
        href: url,
        autoSize: true,
        topRatio: 0,
        scrolling: "no",
        helpers: {
            overlay: {
                css: {
                    "background": "rgba(0, 0, 0, .5)",
                    "overflow": "auto"
                }
            }
        }
    };

    if (url && url.indexOf("#") > -1) {
        defaults.type = "inline";
    }

    options = options || {};

    if ($.isPlainObject(options)) {
        options.autoSize = (!options.width && !options.height);
        $.extend(defaults, options);
    }

    if (options.autoSize) {
        defaults.helpers.overlay.css.overflow = "hidden";
    }
    if (defaults.width === "100%") {
        defaults.margin[1] = defaults.margin[3] = 0;
    }
    if (defaults.height === "100%") {
        defaults.margin[0] = defaults.margin[2] = 0;
    }

    console.log(defaults);

    $.fancybox(defaults);
}


$(document).ready(function () {
    
    $("body")
        .on("click", "a[data-fancybox]", function () {
            var $this = $(this),
                sizes = $this.data("fancybox").match(/^(\d+%?)(x(\d+%?))?(\s|$)/),
                opts = { autoSize: true };

            if (sizes) {
                var w = sizes[1],
                    h = sizes[3] || w;

                if (w.indexOf("%") == -1) {
                    w = parseInt(w);
                }
                if (h.indexOf("%") == -1) {
                    h = parseInt(h);
                }

                opts.height = h;
                opts.width = w;
                opts.autoSize = false;
            }

            openFancybox(this.href, opts);

            return false;
        });

});