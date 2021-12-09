(function () {
    abp.widgets.CreateEventArea = function ($wrapper) {
        var eventApiService = eventHub.controllers.events.event;
        var eventIdInput = $('#EventId');

        if (eventIdInput.val().length === 36) {
            // TODO: Add organization in UppdateEventDto
            $('#OrganizationId').prop("disabled", true).removeAttr('name');
        }

        $("#CreateEventForm").submit(function (e) {
            e.preventDefault();
            if (!$(this).valid()) {
                return false;
            }
            var input = $(this).serializeFormToObject();

            if (eventIdInput.val().length === 36) {
                eventApiService.update(eventIdInput.val(), input).then(function (eventUpdatedResponse) {
                    abp.notify.success('Updated event');
                    SwitchToTrackCreation()
                    ScrollToWrapperBegin();
                });
            } else {
                eventApiService.create(input).then(function (eventCreatedResponse) {
                    abp.notify.success('Created event as a draft');
                    eventIdInput.val(eventCreatedResponse.id);
                    SwitchToTrackCreation()
                    ScrollToWrapperBegin();
                });
            }
        });


        function ScrollToWrapperBegin() {
            $([document.documentElement, document.body]).animate({
                scrollTop: $wrapper.offset().top
            }, 100);
        }

        function SwitchToTrackCreation() {
            $('#CreateEventContainer').css('display', 'none');
            $('#CreateTrackContainer').css('display', '');
            AddNewTrackButtonClickEventHandler();
        }

        function AddNewTrackButtonClickEventHandler() {
            var addNewTrackButton = $('#AddNewTrackButton');
            addNewTrackButton.click(function (e) {
                e.preventDefault();
                var trackName = $('#TrackName').val().trim();
                eventApiService.addTrack(eventIdInput.val(), {name: trackName}).then(function () {
                    var url = addNewTrackButton.attr('data-url').replace("eventIdPlaceholder", eventIdInput.val());
                    abp.ajax({
                        type: 'GET', dataType: 'html', contentType: 'text/html; charset=utf-8', url: url
                    }).then(function (getTracksResponse) {
                        $('#AddTrackModal').modal('hide');
                        var createTrackContainer = $wrapper.find('#CreateTrackContainer');
                        createTrackContainer.text("");
                        createTrackContainer.append(getTracksResponse);
                        AddNewTrackButtonClickEventHandler();
                    });

                    abp.notify.success('Added the track');
                });
            });
        }
    }
})();
