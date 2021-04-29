$(function () {
    var l = abp.localization.getResource('EventHub');

    var fileInput = document.getElementById('Organization_ProfilePictureFile');
    var file;

    fileInput.addEventListener('change', function () {

        file = fileInput.files[0];

        var permittedExtensions = ["jpg", "jpeg", "png"]
        var fileExtension = $(this).val().split('.').pop();
        if (permittedExtensions.indexOf(fileExtension) === -1) {
            abp.message.error(l('OrganizationCoverImageExtensionNotAllowed'))
                .then(() => {
                    $('#Organization_ProfilePictureFile').val('');
                    file = null;
                });
        } else if (file.size > 1024 * 1024) {
            abp.message.error(l('OrganizationCoverImageSizeExceedMessage'))
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

            if (imageError) {
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
            }

            reader.readAsDataURL(input);
        }
    }
});