(function () {
    abp.widgets.RegistrationArea = function ($wrapper) {

        var widgetManager = $wrapper.data('abp-widget-manager');

        function init() {
            $wrapper.find('#EventRegisterButton').click(function(e){
                e.preventDefault();
                widgetManager.refresh();
            });
        }

        return {
            init: init
        };
    };
})();
