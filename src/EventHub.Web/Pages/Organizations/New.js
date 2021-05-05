$(function () {
    var l = abp.localization.getResource('EventHub');

    var infoArea = document.getElementById( 'upload-label' );
    var fileInput = document.getElementById('Organization_ProfilePictureFile');
    var file;

    fileInput.addEventListener('change', function () {
        var showFile = true;
        file = fileInput.files[0];

        if (file === undefined){
            $('#Organization_ProfilePictureFile').val('');
            $('#imageResult').attr('src', '#');
            infoArea.textContent = 'Choose file'
            return;
        }

        var permittedExtensions = ["jpg", "jpeg", "png"]
        var fileExtension = $(this).val().split('.').pop();
        if (permittedExtensions.indexOf(fileExtension) === -1) {
            showFile = false;
            abp.message.error(l('OrganizationProfilePictureExtensionNotAllowed'))
                .then(() => {
                    $('#Organization_ProfilePictureFile').val('');
                    file = null;
                });
        } else if (file.size > 1024 * 1024) {
            showFile = false;
            abp.message.error(l('OrganizationProfilePictureSizeExceedMessage'))
                .then(() => {
                    $('#Organization_ProfilePictureFile').val('');
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

            if (showFile && imageError) {
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