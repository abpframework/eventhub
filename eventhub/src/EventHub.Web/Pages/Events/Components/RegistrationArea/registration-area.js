(function () {
    abp.widgets.RegistrationArea = function ($wrapper) {

        function init() {
            $wrapper.find('#EventRegisterButton').click(function(e){
                e.preventDefault();
                alert("TODO: register");
            });
        }

        return {
            init: init
        };
    };
})();
