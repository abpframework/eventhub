$(function () {
    var l = abp.localization.getResource('EventHub');
    
    var isOnline = $("#Event_IsOnline option:selected").val()
    if (isOnline === "True") {
        $(".event-link-group").show();
        $(".event-location-group").hide();
    }else{
        $(".event-link-group").hide();
        $(".event-location-group").show();
    }
    
    $('#Event_IsOnline').on('change', '', function() {
        var isOnline = $("#Event_IsOnline option:selected").val()
        if (isOnline === "True") {
            $(".event-link-group").show();
            $(".event-location-group").hide();
        }else{
            $(".event-link-group").hide();
            $(".event-location-group").show();
        }
    });

    var infoArea = document.getElementById( 'upload-label' );
    var fileInput = document.getElementById('Event_CoverImageFile');
    var file;

    fileInput.addEventListener('change', function () {
        var showFile = true;

        file = fileInput.files[0];

        if (file === undefined){
            $('#Event_CoverImageFile').val('');
            $('#imageResult').attr('src', '#');
            infoArea.textContent = 'Choose file'
            return;
        }

        var permittedExtensions = ["jpg", "jpeg", "png"]
        var fileExtension = $(this).val().split('.').pop();
        if (permittedExtensions.indexOf(fileExtension) === -1) {
            showFile = false;
            abp.message.error(l('EventCoverImageExtensionNotAllowed'))
                .then(() => {
                    $('#Event_CoverImageFile').val('');
                    file = null;
                });
        }
        else if(file.size > 1024*1024) {
            showFile = false;
            abp.message.error(l('EventCoverImageSizeExceedMessage'))
                .then(() => {
                    $('#Event_CoverImageFile').val('');
                    file = null;
                });
        }

        var img = new Image();
        img.onload = function () {
            var imageError = true;
            var sizes = {
                width: this.width,
                height: this.height
            };
            URL.revokeObjectURL(this.src);

            if(showFile && imageError) {
                readURL(file);
            }
        }

        var objectURL = URL.createObjectURL(file);
        img.src = objectURL;
    });
    
    function readURL(input) {
        if (input) {
            var reader = new FileReader();

            reader.onload = function (e) {
                $('#imageResult').attr('src', e.target.result);
                infoArea.textContent = 'File name: ' + input.name;
            }

            reader.readAsDataURL(input);
        }
    }
});