(function () {
    abp.widgets.CreateEventArea = function ($wrapper) {
        toastr.options.timeOut = 500;
        toastr.options.preventDuplicates = true;
        
        var stepType = {
            "NewEvent": "NewEvent",
            "NewTrack": "NewTrack",
            "NewSession": "NewSession"
        }

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
            $('#CreateEventButton').removeAttr('disabled')
            FocusTrackNameInputInModalEventHandler("AddTrackModal")
            FocusTrackNameInputInModalEventHandler("EditTrackModal")

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

            AddNewTrackSubmitFormEventHandler();
            OpenEditTrackModalClickEventHandler();
            EditTrackSubmitFormEventHandler();
            DeleteTrackButtonClickEventHandler();

            OpenAddNewSessionModalClickEventHandler();
            AddNewSessionEventHandler();
            OpenEditSessionModalClickEventHandler();
            EditSessionEventHandler();
            DeleteSessionButtonClickEventHandler();

            PrevieousOrNextStepTransitionEventHandler()
        };

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
                    FillFilter(stepType.NewTrack);
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
                    FillFilter(stepType.NewTrack)
                    widgetManager.refresh();
                });
            });
        }

        function DeleteTrackButtonClickEventHandler() {
            $('.delete-track-button').click(function (e) {
                eventApiService.deleteTrack(eventIdInput.val(), this.id).then(function () {
                    abp.notify.success('Successfully deleted');
                    FillFilter(stepType.NewTrack)
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

                var userNameList = $('#SpeakersInNewSessionModal').val().split(/[ , ]+/);
                userNameList = userNameList.filter(v => v !== '');
                var input = $(this).serializeFormToObject();
                input.speakerUserNames = userNameList;
                eventApiService.addSession(eventIdInput.val(), input).then(function () {
                    $('#AddSessionModal').modal('hide');
                    abp.notify.success('Added the session');
                    FillFilter(stepType.NewSession);
                    widgetManager.refresh();
                });
            });
        }

        function OpenEditSessionModalClickEventHandler() {
            $('.edit-session-button').click(function (e) {
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
                    FillFilter(stepType.NewSession);
                    widgetManager.refresh();
                });
            });
        }

        function DeleteSessionButtonClickEventHandler() {
            $('.delete-session-button').click(function (e) {
                var clickedSessionId = this.id;
                var trackId = $('#' + clickedSessionId).data('track-id');
                
                eventApiService.deleteSession(eventIdInput.val(), trackId, clickedSessionId).then(function () {
                    abp.notify.success('Successfully deleted');
                    FillFilter(stepType.NewSession)
                    widgetManager.refresh();
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

        return {
            getFilters: getFilters,
            init: init,
        };
    }
})();
