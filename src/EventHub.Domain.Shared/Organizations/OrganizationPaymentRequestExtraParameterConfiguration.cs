namespace EventHub.Organizations
{
    public class OrganizationPaymentRequestExtraParameterConfiguration
    {
        public string OrganizationName { get; set; }
        
        public bool IsExtend { get; set; }

        public int? PremiumPeriodAsMonth { get; set; }

        public OrganizationPlanType TargetPlanType { get; set; }
    }
}
