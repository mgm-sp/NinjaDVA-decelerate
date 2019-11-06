"use strict";

(function () {
    /* Poll function: */
    function poll(scheduleNextPoll) {
        $.ajax($('#pollUrl').attr('href'), {
            cache: false,
            dataType: 'json',
            complete: function (jqXHR, textStatus) {
                /* Schedule the next call of poll(): */
                if (scheduleNextPoll) {
                    window.setTimeout(poll, 5000, true);
                }
            },
            error: function (jqXHR, textStatus, errorThrown) {
                console.log(jqXHR, textStatus, errorThrown);
            },
            success: function (data, textStatus, jqXHR) {
                /* Clear all user entries: */
                $('#users').empty();
                /* Add new user entries: */
                data.room.users.forEach(function (entry) {
                    /* Get template content: */
                    var templateId = (entry.transformedSpeedChoice === null) ? '#userTemplateNotVoted' : '#userTemplateVoted';
                    var html = $(templateId).html();
                    /* Replace placeholders: */
                    html = html.replace('[name]', entry.name);
                    /* Create new element: */
                    var tr = $(html);
                    /* Add width: */
                    if (entry.transformedSpeedChoice !== null) {
                        tr.find('.innerUserSlider').css('width', entry.transformedSpeedChoice + '%');
                    }
                    /* Append it: */
                    $('#users').append(tr);
                });
                /* Update status message: */
                $('#statusDiv').removeClass('alert-danger alert-warning alert-success');
                $('#statusDiv').addClass('alert-' + data.color);
                $('#statusText').text(data.text);
                /* Update slider: */
                $('#statusSlider').css('width', data.transformedAverageSpeedChoice + '%');
            },
            timeout: 10000
        });
    }

    /* SignalR hub: */
    var hub = new signalR.HubConnectionBuilder().withUrl($('#hubUrl').attr('href')).build();
    hub.on('Notify', function (user, type, arg) {
        console.log(user, type, arg);
        /* Something changed, poll new data (but only once): */
        poll(false);
        /* TODO: Remove polling and only use the data provided by the notification. */
    });
    hub.onclose(function (err) {
        if (err !== undefined) {
            /* Fall back to polling at a regular interval: */
            console.log("SignalR error, falling back to regular polling.")
            poll(true);
            return console.error(err.toString());
        }
    })
    hub.start().catch(function (err) {
        /* Fall back to polling at a regular interval: */
        console.log("SignalR error, falling back to regular polling.")
        poll(true);
        return console.error(err.toString());
    });
})();