using System;
using Volo.Abp.DependencyInjection;

namespace Payment
{
    public class PaymentTestData : ISingletonDependency
    {
        public Guid PaymentRequest1Id { get; } = Guid.NewGuid();

        public string Customer1Id { get; } = Guid.NewGuid().ToString("N");

        public string Product1Id { get; } = Guid.NewGuid().ToString("N");
    }
}