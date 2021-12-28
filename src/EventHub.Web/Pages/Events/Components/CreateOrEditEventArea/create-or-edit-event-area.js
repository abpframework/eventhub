(function () {
    abp.widgets.CreateOrEditEventArea = function ($wrapper) {
        var l = abp.localization.getResource('EventHub');
        toastr.options.timeOut = 500;
        toastr.options.preventDuplicates = true;
        
        var stepType = {
            "Event": "Event",
            "Track": "Track",
            "Session": "Session",
            "Preview": "Preview"
        }

        var widgetManager = $wrapper.data('abp-widget-manager');
        var eventApiService = eventHub.controllers.events.event;
        var eventIdInput = $('#EventId');
        var eventUrlCodeInput = $('#EventUrlCode');

        var filter = {
            eventUrlCode: "",
            stepType: stepType.Event
        }

        var getFilters = function () {
            return filter;
        }

        var init = function () {
            $('#CreateEventButton').removeAttr('disabled')
            FocusTrackNameInputInModalEventHandler("AddTrackModal")
            FocusTrackNameInputInModalEventHandler("EditTrackModal")
            NewEventFormInputAreaChangeHandler();
            FilFileInputChangeEventHandler();
            
            if (eventIdInput.val().length === 36) {
                $('#OrganizationId').prop("disabled", true); // Once the Organization is selected, it cannot be changed again.
            }

            $("#CreateEventForm").submit(function (e) {
                e.preventDefault();
                if (!$(this).valid()) {
                    return false;
                }
                
                var formData = new FormData();
                formData = FillNewEventFormData(formData);

                var httpMetod = 'POST'
                if (eventIdInput.val().length === 36) {
                    httpMetod = 'PUT'
                }

                $.ajax({
                    xhr: function() {
                        var xhr = new window.XMLHttpRequest();
                        xhr.upload.addEventListener("progress", function(evt) {
                            if (evt.lengthComputable) {
                                var percentComplete = evt.loaded / evt.total;
                                percentComplete = parseInt(percentComplete * 100);
                                if(percentComplete !== 100){
                                    $('#CreateEventButton').prop( "disabled", true);
                                }
                            }
                        }, false);

                        return xhr;
                    },
                    url: $('#CreateEventButton').data('url'),
                    data: formData,
                    type: httpMetod,
                    contentType: false,
                    processData: false,
                    success: function(response){
                        if (eventIdInput.val().length === 36) {
                            abp.notify.success('Updated event');
                        }else{
                            abp.notify.success('Created event as a draft' , '', toastr.options.timeOut = 2500);
                            eventIdInput.val(response.id);
                            eventUrlCodeInput.val(response.urlCode);
                        }
                        SwitchToTrackCreation()
                    },
                    error: function (errorRaw) {
                        abp.notify.error(errorRaw.responseJSON.error.message, 'Error', 
                            toastr.options = {
                                timeOut: 2500,
                                progressBar: true,
                                positionClass: "toast-bottom-right"
                            }
                        );
                    }
                });
            });

            function SwitchToTrackCreation() {
                $('#EventContainer').css('display', 'none');
                $('#TrackContainer').css('display', '');
                ScrollToWrapperBegin();
            }
            
            AddNewTrackSubmitFormEventHandler();
            OpenEditTrackModalClickEventHandler();
            EditTrackSubmitFormEventHandler();
            DeleteTrackButtonClickEventHandler();

            OpenAddNewSessionModalClickEventHandler();
            AddNewSessionEventHandler();
            OpenEditSessionModalClickEventHandler();
            EditSessionEventHandler();
            DeleteSessionButtonClickEventHandler();

            PublishEventButtonClickEventHandler();

            PrevieousOrNextStepTransitionEventHandler()
        };
        
        function FillNewEventFormData(formData) {
            var fileInput = document.getElementById('Event_CoverImageFile');
            formData.append("OrganizationId", $('#OrganizationId').val().trim());
            formData.append("Title", $('#inputTitle').val().trim());
            formData.append("StartTime", $('#inputStartdate').val().trim());
            formData.append("EndTime", $('#inputEnddate').val().trim());
            formData.append("Description", $('#inputDescription').val().trim());
            formData.append("CoverImageStreamContent",  fileInput.files[0]);
            formData.append("IsOnline", $('#IsOnline').val().trim());
            formData.append("OnlineLink", $('#inputOnlineLink').val().trim());
            formData.append("CountryId", $('#CountryId').val().trim());
            formData.append("City", $('#inputCity').val().trim());
            formData.append("Language", $('#Language').val().trim());
            formData.append("Capacity", $('#inputCapacity').val().trim());
            return formData;
        }

        function AddNewTrackSubmitFormEventHandler() {
            var addNewTrackForm = $('#AddNewTrackForm');
            addNewTrackForm.submit(function (e) {
                e.preventDefault();
                if (!addNewTrackForm.valid()) {
                    return false;
                }
                var trackName = addNewTrackForm.find('#TrackNameInput').val().trim();
                eventApiService.addTrack(eventIdInput.val(), {name: trackName}).then(function () {
                    $('#AddTrackModal').modal('hide');
                    abp.notify.success('Added the track');
                    FillFilter(stepType.Track);
                    widgetManager.refresh();
                });
            });
        }

        function FocusTrackNameInputInModalEventHandler(elementId) {
            $('#' + elementId).on('shown.bs.modal', function () {
                $(this).find('input[type=text]').trigger('focus')
            });
        }

        function OpenEditTrackModalClickEventHandler() {
            $('.edit-track-button').click(function (e) {
                var editTrackModal = $('#EditTrackModal');
                var clickedTrackId = this.id;
                var trackName = $('#' + clickedTrackId).data('track-name');
                editTrackModal.find('#TrackNameInput').val(trackName);
                editTrackModal.find('#TrackId').val(clickedTrackId);
            });
        }

        function EditTrackSubmitFormEventHandler() {
            var editTrackForm = $('#EditTrackForm');
            editTrackForm.submit(function (e) {
                e.preventDefault();
                if (!editTrackForm.valid()) {
                    return false;
                }
                var trackId = editTrackForm.find('#TrackId').val();
                var oldTrackName = $('#' + trackId).data('track-name');
                var trackName = editTrackForm.find('#TrackNameInput').val().trim();
                if (trackName === oldTrackName) {
                    abp.notify.error('Track name must be different from the previous name.');
                    return;
                }
                eventApiService.updateTrack(eventIdInput.val(), trackId, {name: trackName}).then(function () {
                    $('#EditTrackModal').modal('hide');
                    abp.notify.success('Updated the track');
                    FillFilter(stepType.Track)
                    widgetManager.refresh();
                });
            });
        }

        function DeleteTrackButtonClickEventHandler() {
            $('.delete-track-button').click(function (e) {
                e.preventDefault();
                eventApiService.deleteTrack(eventIdInput.val(), this.id).then(function () {
                    abp.notify.success('Successfully deleted');
                    FillFilter(stepType.Track)
                    widgetManager.refresh();
                });
            });
        }

        function OpenAddNewSessionModalClickEventHandler() {
            $('.add-session-button').click(function (e) {
                var addSessionModal = $('#AddSessionModal');
                var clickedSessionId = this.id;
                var trackId = $('#' + clickedSessionId).data('track-id');
                addSessionModal.find('#TrackId').val(trackId);
                $('#StartTimeInNewSessionModal').val($('#inputStartdate').val());
                $('#EndTimeInNewSessionModal').val($('#inputEnddate').val());

                CreateAutoCompleteByElement("SpeakersInNewSessionModal");
            });
        }

        function AddNewSessionEventHandler() {
            var addNewSessionForm = $('#AddNewSessionForm');
            addNewSessionForm.submit(function (e) {
                e.preventDefault();
              
                if (!addNewSessionForm.valid()) {
                    return false;
                }

                var addSessionModal = $('#AddSessionModal');
                var trackId = addSessionModal.find('#TrackId').val();
                
                var userNameList = $('#SpeakersInNewSessionModal').val().split(/[ , ]+/);
                userNameList = userNameList.filter(v => v !== '');
                var input = $(this).serializeFormToObject();
                input.speakerUserNames = userNameList;
                eventApiService.addSession(eventIdInput.val(), trackId, input).then(function () {
                    $('#AddSessionModal').modal('hide');
                    abp.notify.success('Added the session');
                    FillFilter(stepType.Session);
                    widgetManager.refresh();
                });
            });
        }

        function OpenEditSessionModalClickEventHandler() {
            $('.edit-session-button').click(function (e) {
                e.preventDefault();
                var editSessionModal = $('#EditSessionModal');

                var clickedSessionId = this.id;
                var trackId = $('#' + clickedSessionId).data('track-id');

                editSessionModal.find('#TrackId').val(trackId);
                editSessionModal.find('#SessionId').val(clickedSessionId);

                var titleOfSession = $('#TitleOfSession-' + clickedSessionId).val();
                $('#EditModalTitle').html('Edit ' + abp.utils.truncateStringWithPostfix(titleOfSession, 10) + ' Session');
                editSessionModal.find('#TitleInEditSessionModal').val(titleOfSession);
                editSessionModal.find('#DescriptionInEditSessionModal').val($('#DescriptionOfSession-' + clickedSessionId).val());
                editSessionModal.find('#StartTimeInEditSessionModal').val($('#StartTimeOfSession-' + clickedSessionId).val());
                editSessionModal.find('#EndTimeInEditSessionModal').val($('#EndTimeOfSession-' + clickedSessionId).val());
                editSessionModal.find('#LanguageInEditSessionModal').val($('#LanguageOfSession-' + clickedSessionId).val());
                editSessionModal.find('#SpeakersInEditSessionModal').val($('#SpeakersOfSession-' + clickedSessionId).val());

                CreateAutoCompleteByElement("SpeakersInEditSessionModal");
            });
        }

        function EditSessionEventHandler() {
            var editSessionForm = $('#EditSessionForm');
            editSessionForm.submit(function (e) {
                e.preventDefault();

                if (!editSessionForm.valid()) {
                    return false;
                }

                var userNameList = $('#SpeakersInEditSessionModal').val().split(/[ , ]+/);
                userNameList = userNameList.filter(v => v !== '');
                var input = editSessionForm.serializeFormToObject();
                input.speakerUserNames = userNameList;
                eventApiService.updateSession(eventIdInput.val(), editSessionForm.find('#TrackId').val(), editSessionForm.find('#SessionId').val(), input).then(function () {
                    $('#EditSessionModal').modal('hide');
                    abp.notify.success('Updated the session');
                    FillFilter(stepType.Session);
                    widgetManager.refresh();
                });
            });
        }

        function DeleteSessionButtonClickEventHandler() {
            $('.delete-session-button').click(function (e) {
                e.preventDefault();
                var clickedSessionId = this.id;
                var trackId = $('#' + clickedSessionId).data('track-id');
                
                eventApiService.deleteSession(eventIdInput.val(), trackId, clickedSessionId).then(function () {
                    abp.notify.success('Successfully deleted');
                    FillFilter(stepType.Session)
                    widgetManager.refresh();
                });
            });
        }
        
        function PublishEventButtonClickEventHandler() {
            $('#PublishButton').click(function (e) {
                e.preventDefault();
                eventApiService.publish(eventIdInput.val()).then(function (res) {
                    e.prefe
                    abp.notify.success('Event Published');
                    window.location.replace('https://' + window.location.host + '/events/' + res);
                });
            });
        }
        
        function PrevieousOrNextStepTransitionEventHandler() {
            $('.step-transition').click(function (e) {
                var clickedButtonId = this.id;
                var targetStep = $('#' + clickedButtonId).data('target-step');
                FillFilter(targetStep);
                widgetManager.refresh();
                ScrollToWrapperBegin();
            });
        }

        function CreateAutoCompleteByElement(id) {
            var element = document.getElementById(id);
            var awesomplete = new Awesomplete(element, {
                filter: function (text, input) {
                    return Awesomplete.FILTER_CONTAINS(text, input.match(/[^,]*$/)[0]);
                },

                item: function (text, input) {
                    return Awesomplete.ITEM(text, input.match(/[^,]*$/)[0]);
                },

                replace: function (text) {
                    var before = this.input.value.match(/^.+,\s*|/)[0];
                    this.input.value = before + text + " , ";
                },
                list: []
            });

            function delay(callback, ms) {
                var timer = 0;
                return function () {
                    var context = this, args = arguments;
                    clearTimeout(timer);
                    timer = setTimeout(function () {
                        callback.apply(context, args);
                    }, ms || 0);
                };
            }

            $('#' + id).keyup(delay(function (e) {
                var filterText = $(this).val().split(/[ ,]+/);
                eventHub.controllers.users.user.getListByUserName(filterText[filterText.length-1]).then(function (res) {
                    var userNames = []
                    $.each(res, function (i, v) {
                        if (!$('#' + id).val().includes(v.userName)){
                            userNames.push(v.userName);
                        }
                    });

                    awesomplete.list = userNames
                    awesomplete.evaluate();
                });
            }, 500));
        }
        
        function FillFilter(stepType) {
            filter.stepType = stepType;
            filter.eventUrlCode = eventUrlCodeInput.val();
        }

        function ScrollToWrapperBegin() {
            $([document.documentElement, document.body]).animate({
                scrollTop: $wrapper.offset().top
            }, 100);
        }

        $(document).ajaxSend(function (event, jqxhr, settings, exception) {
            ButtonsBusy(true)
        });

        $(document).ajaxComplete(function (event, jqxhr, settings, exception) {
            ButtonsBusy(false)
        });

        abp.ajax.showError = function (error) {
            abp.notify.error(
                error.message,
                'Error',
                toastr.options = {
                    timeOut: 2500,
                    progressBar: true,
                    positionClass: "toast-bottom-right"
                }
            );
        }
        
        function ButtonsBusy(isBusy) {
            if (isBusy) {
                $('button[type=submit]').attr('disabled', 'disabled')
            } else {
                $('button[type=submit]').removeAttr('disabled')
            }
        }
        
        function NewEventFormInputAreaChangeHandler() {
            var isOnline = $("#IsOnline option:selected").val()
            if (isOnline === "True") {
                $("#CountryId").attr("required", false);
                $("#inputCity").attr("required", false);
                $(".event-link-group").show();
                $(".event-location-group").hide();
            } else {
                $("#CountryId").attr("required", true);
                $("#inputCity").attr("required", true);
                $(".event-link-group").hide();
                $(".event-location-group").show();
            }

            $('#IsOnline').on('change', '', function () {
                var isOnline = $("#IsOnline option:selected").val()
                if (isOnline === "True") {
                    $("#CountryId").attr("required", false);
                    $("#inputCity").attr("required", false);
                    $(".event-link-group").show();
                    $(".event-location-group").hide();
                } else {
                    $("#CountryId").attr("required", true);
                    $("#inputCity").attr("required", true);
                    $(".event-link-group").hide();
                    $(".event-location-group").show();
                }
            });
        }
        
        function FilFileInputChangeEventHandler() {
            var file;
            var infoArea = document.getElementById('upload-label');
            var fileInput = document.getElementById('Event_CoverImageFile');
            
            if (fileInput !== null) {
                fileInput.addEventListener('change', function () {
                    var showFile = true;

                    file = fileInput.files[0];

                    if (file === undefined) {
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
                    } else if (file.size > 1024 * 1024) {
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
                            if (eventIdInput.val().length === 36){
                                $('.image-area').css('display', 'none').removeClass('d-block');
                                $('.result-area').css('display', '');
                            }
                            $('#imageResult').attr('src', e.target.result);
                            infoArea.textContent = 'File name: ' + input.name;
                        }

                        reader.readAsDataURL(input);
                    }
                }
            }
        }

        return {
            getFilters: getFilters,
            init: init,
        };
    }
})();
