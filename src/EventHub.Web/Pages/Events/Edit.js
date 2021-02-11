$(function () {
    var l = abp.localization.getResource('EventHub');

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
    
    $("#btn-event-timing").click(function () {
        $("#event-timing-preview-modal").modal("show");
    });
    
    $("#event-timing-confirm").click(function () {
        $("#Event_Timing_Form").submit();
        $("#event-timing-preview-modal").modal("hide");
    })
    
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
                    .success(l("EventTimingChangeSuccessMessage"))
                    .then(data => window.location.href = "/events/" + $("#event-detail-url").val());
            },
            error: function (data) {
                abp.message.error(data.responseJSON.error.message);
            }
        });
    })

    // Cover Image Section
    $('#event-cover-image-btnSubmit').prop( "disabled", true );
    var fileInput = document.getElementById('event-cover-image-input');
    var file;

    fileInput.addEventListener('change', function () {
        var showModal = true;

        file = fileInput.files[0];

        if (file === undefined){
            $('#event-cover-image-input').val('');
            $("#choose-cover-image").html(l("ChooseEventCoverImage"));
            $('#event-cover-image-btnSubmit').prop( "disabled", true );
            return;
        }else{
            document.getElementById("choose-cover-image").innerHTML = file.name;
        }

        var permittedExtensions = ["jpg", "jpeg", "png"]
        var fileExtension = $(this).val().split('.').pop();
        if (permittedExtensions.indexOf(fileExtension) === -1) {
            showModal = false;
            abp.message.error(l('EventCoverImageExtensionNotAllowed'))
                .then(() =>  {
                    $('#event-cover-image-file').val('');
                    $("#choose-cover-image").html(l("ChooseEventCoverImage"));
                    file = null;
                });
        }
        else if(file.size > 1024*1024) {
            showModal = false;
            abp.message.error(l('EventCoverImageSizeExceedMessage'))
                .then(() => {
                    $('#event-cover-image-file').val('');
                    $("#choose-cover-image").html(l("ChooseEventCoverImage"));
                    file = null;
                });
        }

        var img = new Image();
        img.onload = function() {
            var imageError = true;
            var sizes = {
                width: this.width,
                height: this.height
            };
            URL.revokeObjectURL(this.src);

            if(showModal && imageError) {
                readURL(file);
                $('#cover-image-preview-modal').modal('show');
            }
        }

        var objectURL = URL.createObjectURL(file);
        img.src = objectURL;
        if (file.size > 0 && file.size < 1024*1024){
            $('#event-cover-image-btnSubmit').prop( "disabled", false);
        }
    });

    function readURL(input) {
        if (input) {
            var reader = new FileReader();

            reader.onload = function (e) {
                $('#img-upload').attr('src', e.target.result);
            }

            reader.readAsDataURL(input);
        }
    }

    $('form#cover-image-form').submit(function (e) {
        e.preventDefault();

        var eventId = document.getElementById("event-id").value;

        if(!$(this).valid()) {
            return false;
        }

        var formData = new FormData();
        formData.append("EventId", eventId);
        formData.append("CoverImageFile", file);

        $.ajax({
            xhr: function() {
                var xhr = new window.XMLHttpRequest();
                xhr.upload.addEventListener("progress", function(evt) {
                    if (evt.lengthComputable) {
                        var percentComplete = evt.loaded / evt.total;
                        percentComplete = parseInt(percentComplete * 100);
                        if(percentComplete !== 100){
                            fileInput.prop( "disabled", true );
                            $('#event-cover-image-btnSubmit').prop( "disabled", true );
                        }
                    }
                }, false);

                return xhr;
            },
            url: abp.appPath + `api/event/save-cover-image`,
            data: formData,
            type: 'POST',
            contentType: false,
            processData: false,
            success: function(data){
                abp.message
                    .success(l('EventCoverImageEditSuccessMessage'))
                    .then(data => window.location.href = "/events/" + $("#event-detail-url").val());
            },
            error: function (data) {
                abp.message.error(data.responseJSON.error.message);
            }
        });
    });
});