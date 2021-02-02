(function () {
    var l = abp.localization.getResource('EventHub');
    abp.widgets.JoinArea = function ($wrapper) {

        var widgetManager = $wrapper.data('abp-widget-manager');
        var organizationId = $wrapper.find('[data-organization-id]').attr('data-organization-id');

        function getFilters() {
            return {
                organizationId: organizationId
            };
        }

        function init() {
            var joinButton = $wrapper.find('#OrganizationJoinButton');
            joinButton.click(function (e) {
                e.preventDefault();
                joinButton.buttonBusy(true);
                abp.ajax({
                    url: joinButton.attr('data-url')
                }).then(function (){
                    widgetManager.refresh();
                    abp.event.trigger('EventHub.Organization.JoinStatusChanged');
                    abp.message.success(l('OrganizationJoinSuccessMessage'), l('OrganizationJoinSuccessMessageTitle'));
                }).always(function (){
                    joinButton.buttonBusy(false);
                });
            });

            var $leaveButton = $wrapper.find('#OrganizationLeaveButton');
            $leaveButton.click(function (e) {
                e.preventDefault();
                $leaveButton.buttonBusy(true);
                abp.ajax({
                    url: $leaveButton.attr('data-url')
                }).then(function (){
                    widgetManager.refresh();
                    abp.event.trigger('EventHub.Organization.JoinStatusChanged');
                    abp.notify.info(l('OrganizationMembershipLeaveMessage'));
                }).always(function (){
                    $leaveButton.buttonBusy(false);
                });
            });
        }

        return {
            getFilters: getFilters,
            init: init
        };
    };
})();
