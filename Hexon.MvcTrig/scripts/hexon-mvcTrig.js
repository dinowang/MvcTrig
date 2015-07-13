; (function ($) {

    if (!window.registerTrigger) {
        var triggers = {
            "reload": function (evt) {
                location.reload();
            }
        };

        window.registerTrigger = function(name, callback) {
            triggers[name] = callback;
        };

        window.getTrigger = function(name) {
            //return triggers[name] || function () { };
            return triggers[name];
        };
    }

    function takeHeaderData(xhr, key, callback) {
        var content = xhr.getResponseHeader(key);
        if (content) {
            callback(content);
        }
    }

    function executeEachDirective(m) {
        var targets = [ window, parent || window, top || parent || window ];

        takeHeaderData(m.xhr, "X-Triggers", function (n) {
            for (var i = 0; i < n; i++) {
                takeHeaderData(m.xhr, "X-Trigger-" + i, function (data) {
                    var pack = JSON.parse(decodeURIComponent(data)),
                        action = window.getTrigger(pack.trigger);

                    action.apply(targets[pack.scope].document.body, [m.evt, pack.data, m.xhr]);
                });
            }
        });

        targets = null;
    }
    
    // AJAX 透過 HTTP response header 攜帶 trigger 命令
    $(document)
        .ajaxSuccess(function (evt, xhr, options, result) {
            executeEachDirective({ evt: evt, xhr: xhr, options: options, result: result });
        });

})(jQuery);