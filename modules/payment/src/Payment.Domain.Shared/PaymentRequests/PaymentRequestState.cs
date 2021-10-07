namespace Payment.PaymentRequests
{
    public enum PaymentRequestState : byte
    {
        Waiting = 0,
        Completed,
        Failed
    }
}