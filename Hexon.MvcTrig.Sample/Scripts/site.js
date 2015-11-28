function openFancybox(url, options) {
    if (!url || url.match(/^\s*javascript/)) {
        return;
    }

    var defaults = {
        type: "iframe",
        href: url,
        autoSize: true,
        padding: 5,
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
    
    // 全站通用開啟燈箱
    $("body")
        .on("click", "a[data-fancybox]", function () {
            var $this = $(this),
                sizes = $this.data("fancybox").match(/^(\d+%?)(x(\d+%?))?(\s|$)/),
                opts = { autoSize: true };

            console.log(sizes);

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

    // 全站通用重新載入列表
    $("body")
        .on("reload-table", function (data) {
            var $lists = $("[data-table-url]");

            $lists.each(function (i, el) {
                var $container = $(el),
                    url = $container.data("table-url");

                $.get(url, null, function (result, status, xhr) {
                    $container.html(result);
                });
            });
        });

});