$(function () {

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