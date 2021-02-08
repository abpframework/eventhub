$(function () {
    var l = abp.localization.getResource('EventHub');

    var fileInput = document.getElementById("organization_id");
    var file;
    
    fileInput.addEventListener('change', function () {
        var showModal = true;
        
        file = fileInput.files[0];
        
        if(file == undefined) {
            $("#choose-cover-image").html("Choose a cover image...");
            return;
        }
        else {
            $("#choose-cover-image").html(file.name);
        }

        var permittedExtensions = ["jpg", "jpeg", "png"]
        var fileExtension = $(this).val().split('.').pop();

        if (permittedExtensions.indexOf(fileExtension) === -1) {
            showModal = false;
            abp.message.error('This Extension Is Not Allowed')
                .then(() =>  {
                    $("#choose-cover-image").html("Choose a cover image...");
                    file = null;
                });
        }
        //1mb
        else if(file.size > 1024 * 1024) {
            showModal = false;
            abp.message.error('The File Is Too Large')
                .then(() => {
                    $("#choose-cover-image").html("Choose a cover image...");
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
            
            if(showModal && imageError) {
                readURL(file);
                $("#picturePreviewModal").modal('show');
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
    
    $("form#EditOrganizationProfileForm").submit(function (e) {
        e.preventDefault();

        var id = document.getElementById("my-id").value;

        var formData = new FormData();
        formData.append("OrganizationId", id);
        formData.append("ProfilePictureFile", file);

        $.ajax({
            xhr: function() {
                var xhr = new window.XMLHttpRequest();
                xhr.upload.addEventListener("progress", function(evt) {
                    if (evt.lengthComputable) {
                        var percentComplete = evt.loaded / evt.total;
                        percentComplete = parseInt(percentComplete * 100);
                        if(percentComplete !== 100){
                            $('#btnSubmit').prop( "disabled", true );
                        }
                    }
                }, false);

                return xhr;
            },
            url: `/api/organization`,
            data: formData,
            type: 'POST',
            contentType: false,
            processData: false,
            success: function(data){
                abp.message
                    .success("Organization Profile Picture successfully added.")
                    .then(data => window.location.href = "/");
            },
            error: function (data) {
                abp.message.error(data.responseJSON.error.message);
            }
        });

    });
});