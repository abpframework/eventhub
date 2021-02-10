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
    
    $("#Event_Timing_Form").submit(function (e) {
        e.preventDefault();
        
        if(!$(this).valid()) {
            return false;
        }
        
        var data = {
            id: $('#EventTiming_Id').val(),
            startTime: $('#EventTiming_StartTime').val(),
            endTime: $('#EventTiming_EndTime').val()
        };
        
        $.ajax({
            url: abp.appPath + `api/event/update-timing`,
            type: 'POST',
            dataType: 'json',
            data: data,
            success: function (data) {
                abp.message
                    .success("Timing changed successfully!") //TODO: localize it!
                    .then(data => window.location.href = "/events/" + $("#event-detail-url").val());
            },
            error: function (data) {
                abp.message.error(data.responseJSON.error.message);
            }
        });
    })
});