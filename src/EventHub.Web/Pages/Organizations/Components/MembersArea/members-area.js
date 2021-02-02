(function () {
    abp.widgets.MembersArea = function ($wrapper) {
        var organizationId = $wrapper.find('[data-organization-id]').attr('data-organization-id');
        return {
            getFilters: function () {
                return {
                    organizationId: organizationId
                };
            }
        };
    };

    abp.event.on("EventHub.Organization.JoinStatusChanged", function(){
        $('[data-widget-name="MembersArea"]')
            .data('abp-widget-manager')
            .refresh();
    });
})();
