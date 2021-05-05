(function () {
    var l = abp.localization.getResource('EventHub');
    abp.widgets.RegistrationArea = function ($wrapper) {

        var widgetManager = $wrapper.data('abp-widget-manager');
        var eventId = $wrapper.find('[data-event-id]').attr('data-event-id');

        function getFilters() {
            return {
                eventId: eventId
            };
        }

        function init() {
            var $registerButton = $wrapper.find('#EventRegisterButton');
            $registerButton.click(function (e) {
                e.preventDefault();
                $registerButton.buttonBusy(true);
                abp.ajax({
                    url: $registerButton.attr('data-url')
                }).then(function (){
                    widgetManager.refresh();
                    abp.event.trigger('EventHub.Event.RegistrationStatusChanged');
                    abp.message.success(l('EventRegisterSuccessMessage'));
                }).always(function (){
                    $registerButton.buttonBusy(false);
                });
            });

            var $cancelRegistrationButton = $wrapper.find('#EventCancelRegistrationButton');
            $cancelRegistrationButton.click(function (e) {
                e.preventDefault();
                $cancelRegistrationButton.buttonBusy(true);
                abp.ajax({
                    url: $cancelRegistrationButton.attr('data-url')
                }).then(function (){
                    widgetManager.refresh();
                    abp.event.trigger('EventHub.Event.RegistrationStatusChanged');
                    abp.notify.info(l('EventRegistrationCancelledMessage'));
                }).always(function (){
                    $cancelRegistrationButton.buttonBusy(false);
                });
            });
        }

        return {
            getFilters: getFilters,
            init: init
        };
    };
})();
