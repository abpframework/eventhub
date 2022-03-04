namespace EventHub
{
    public static class EventHubErrorCodes
    {
        public const string OrganizationNameAlreadyExists = "EventHub:OrganizationNameAlreadyExists";
        public const string NotAuthorizedToCreateEventInThisOrganization = "EventHub:NotAuthorizedToCreateEventInThisOrganization";
        public const string EndTimeCantBeEarlierThanStartTime = "EventHub:EndTimeCantBeEarlierThanStartTime";
        public const string SessionEndTimeCantBeEarlierThanStartTime = "EventHub:SessionEndTimeCantBeEarlierThanStartTime";
        public const string CantRegisterOrUnregisterForAPastEvent = "EventHub:CantRegisterOrUnregisterForAPastEvent";
        public const string NotAuthorizedToUpdateOrganizationProfile = "EventHub:NotAuthorizedToUpdateOrganizationProfile";
        public const string CapacityOfEventFull = "EventHub:CapacityOfEventFull";
        public const string CapacityCanNotBeLowerThanRegisteredUserCount = "EventHub:CapacityCantBeLowerThanRegisteredUserCount";
        public const string NotAuthorizedToUpdateEvent = "EventHub:NotAuthorizedToUpdateEvent";
        public const string CantChangeEventTiming = "EventHub:CantChangeEventTiming";
        public const string SessionTimeShouldBeInTheEventTime = "EventHub:SessionTimeShouldBeInTheEventTime";
        public const string SessionTimeConflictsWithAnExistingSession = "EventHub:SessionTimeConflictsWithAnExistingSession";
        public const string UserNotFound = "EventHub:UserNotFound";
        public const string OrganizationNotFound = "EventHub:OrganizationNotFound";
        public const string TrackNameAlreadyExist = "EventHub:TrackNameAlreadyExist";
        public const string TrackNotFound = "EventHub:TrackNotFound";
        public const string SessionNotFound = "EventHub:SessionNotFound";
        public const string SessionTitleAlreadyExist = "EventHub:SessionTitleAlreadyExist";
        public const string CannotCreateNewEvent = "EventHub:CannotCreateNewEvent";
        public const string CannotAddNewTrack = "EventHub:CannotAddNewTrack";
        public const string CannotRegisterToEvent = "EventHub:CannotRegisterToEvent";
    }
}
