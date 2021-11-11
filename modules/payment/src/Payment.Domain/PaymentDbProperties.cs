namespace Payment
{
    public static class PaymentDbProperties
    {
        public static string DbTablePrefix { get; set; } = "Pay";

        public static string DbSchema { get; set; } = null;

        public const string ConnectionStringName = "Payment";
    }
}
