$(function () {
    $('#Event_IsOnline').click(function() {
        if ($(this).is(':checked')) {
            $("#event-link-form").show();
        }else{
            $("#event-link-form").hide();
        }
    });
});