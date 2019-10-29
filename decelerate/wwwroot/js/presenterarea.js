(function () {
    /* Poll function: */
    function poll() {
        $.ajax($('#pollUrl').attr('href'), {
            cache: false,
            dataType: 'json',
            complete: function (jqXHR, textStatus) {
                window.setTimeout(poll, 5000);
            },
            error: function (jqXHR, textStatus, errorThrown) {
                console.log(jqXHR, textStatus, errorThrown);
            },
            success: function (data, textStatus, jqXHR) {
                /* Clear all user entries: */
                $('#users').empty();
                /* Add new user entries: */
                data.users.forEach(function (entry) {
                    /* Get template content: */
                    var html = $('#userTemplate').html();
                    /* Replace placeholders: */
                    html = html.replace('[name]', entry.name);
                    html = html.replace('[speedChoice]', entry.speedChoice);
                    /* Create new element: */
                    var tr = $(html);
                    /* Append it: */
                    $('#users').append(tr);
                });
            },
            timeout: 10000
        });
    }
    /* Register poll function: */
    window.addEventListener('load', function () {
        window.setTimeout(poll, 5000);
    }, false);
})();