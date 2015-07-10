; (function ($) {

    if (window.registerTrigger) {
        var notifyTypes = ["alert", "warning", "error", "success", "information"];

        window.registerTrigger("message", function(evt, m) {
            alert(m.message);
        });

        window.registerTrigger("notify", function(evt, m) {
            alert(m.message);

            //$.pnotify({
            //    title: m.title || "通知",
            //    text: m.message,
            //    type: notifyTypes[m.type]
            //});

            //var config = {
            //        theme: "bootstrapTheme",
            //        layout: 'topCenter',
            //        dismissQueue: true,
            //        type: notifyTypes[m.type],
            //        //title: m.title || "通知",
            //        text: m.message,
            //        timeout: m.timeout
            //        //animation: {
            //        //    open: 'animated bounceInLeft',
            //        //    close: 'animated bounceOutLeft'
            //        //}
            //    },
            //    n = noty(config);

        });
    }

})(jQuery);