$(function () {
    $("#Event_CountryId").prepend("<option value=''></option>");
    $("#Event_Language").prepend("<option value=''></option>");
    
    if($("#Event_IsOnline").is(":checked")) {
        $("#event-link-group").show();
        $("#event-location-group").hide(); 
    }

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