namespace EventHub.Emailing
{
    public static class EmailTemplates
    {
        public const string Layout = "EventHub.Event.Layout";
        
        public const string EventRegistration = "EventHub.Event.Registration";
        
        public const string EventReminder = "EventHub.Event.Reminder";
        
        public const string NewEventCreated = "EventHub.Event.NewEventCreated";

        public const string EventTimingChanged = "EventHub.Event.EventTimingChanged";
        
        public const string PaymentRequestCompleted = "EventHub.Organization.PaymentRequestCompleted";
        
        public const string PaymentRequestFailed = "EventHub.Organization.PaymentRequestFailed";
        
        public const string PaidEnrollmentEndDateReminder = "EventHub.Organization.PaidEnrollmentEndDateReminder";
    }
}
