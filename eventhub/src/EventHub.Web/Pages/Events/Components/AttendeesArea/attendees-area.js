(function () {
    abp.widgets.AttendeesArea = function ($wrapper) {
        var eventId = $wrapper.find('[data-event-id]').attr('data-event-id');
        return {
            getFilters: function () {
                return {
                    eventId: eventId
                };
            }
        };
    };

    abp.event.on("EventHub.Event.RegistrationStatusChanged", function(){
       $('[data-widget-name="AttendeesArea"]')
           .data('abp-widget-manager')
           .refresh();
    });
})();
