(function () {
    var stepType = {
        "NewEvent": "NewEvent",
        "NewTrack": "NewTrack",
        "NewSession": "NewSession"
    }

    abp.widgets.CreateEventArea = function ($wrapper) {
        var widgetManager = $wrapper.data('abp-widget-manager');
        var eventApiService = eventHub.controllers.events.event;
        var eventIdInput = $('#EventId');
        var eventUrlCodeInput = $('#EventUrlCode');

        var filter = {
            eventUrlCode: "",
            stepType: stepType.NewEvent
        }
        
        var getFilters = function () {
            return filter;
        }

        var init = function () {
            console.log(filter)
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
                    eventApiService.update(eventIdInput.val(), input).then(function () {
                        abp.notify.success('Updated event');
                        SwitchToTrackCreation()
                    });
                } else {
                    eventApiService.create(input).then(function (eventCreatedResponse) {
                        abp.notify.success('Created event as a draft');
                        eventIdInput.val(eventCreatedResponse.id);
                        eventUrlCodeInput.val(eventCreatedResponse.urlCode);
                        SwitchToTrackCreation()
                    });
                }
            });

            function SwitchToTrackCreation() {
                $('#CreateEventContainer').css('display', 'none');
                $('#CreateTrackContainer').css('display', '');
                ScrollToWrapperBegin();
            }

            AddNewTrackButtonClickEventHandler();
        };

        function AddNewTrackButtonClickEventHandler() {
            var addNewTrackButton = $('#AddNewTrackButton');
            addNewTrackButton.click(function (e) {
                e.preventDefault();
                var trackName = $('#TrackName').val().trim();
                eventApiService.addTrack(eventIdInput.val(), {name: trackName}).then(function () {
                    $('#AddTrackModal').modal('hide');
                    abp.notify.success('Added the track');
                    FillFilter(stepType.NewTrack)
                    widgetManager.refresh();
                });
            });
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

        return {
            getFilters: getFilters,
            init: init,
        };
    }
})();
