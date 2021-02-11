$(function () {
    var l = abp.localization.getResource('EventHub');

    $("#Event_CountryId").prepend("<option value='' selected='selected'></option>");
    $("#Event_Language").prepend("<option value='' selected='selected'></option>");

    $('#Event_IsOnline').click(function() {
        if ($(this).is(':checked')) {
            $("#event-link-group").show();
            $("#event-location-group").hide();
        }else{
            $("#event-link-group").hide();
            $("#event-location-group").show();
        }
    });

    var fileInput = document.getElementById('Event_CoverImageFile');
    var file;

    fileInput.addEventListener('change', function () {
        var showModal = true;

        file = fileInput.files[0];

        var permittedExtensions = ["jpg", "jpeg", "png"]
        var fileExtension = $(this).val().split('.').pop();
        if (permittedExtensions.indexOf(fileExtension) === -1) {
            showModal = false;
            abp.message.error(l('EventCoverImageExtensionNotAllowed'))
                .then(() =>  {
                    $('#Event_CoverImageFile').val('');
                    file = null;
                });
        }
        else if(file.size > 1024*1024) {
            showModal = false;
            abp.message.error(l('EventCoverImageSizeExceedMessage'))
                .then(() =>  {
                    $('#Event_CoverImageFile').val('');
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
});