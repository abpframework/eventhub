$(function () {
    $("#Event_CountryId").prepend("<option value='' selected='selected'></option>");

    $('#Event_IsOnline').click(function() {
        if ($(this).is(':checked')) {
            $("#event-link-group").show();
            $("#event-location-group").hide();
        }else{
            $("#event-link-group").hide();
            $("#event-location-group").show();
        }
    });
});