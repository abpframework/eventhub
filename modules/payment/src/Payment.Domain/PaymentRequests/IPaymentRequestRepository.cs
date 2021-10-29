using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace Payment.PaymentRequests
{
    public interface IPaymentRequestRepository : IRepository<PaymentRequest, Guid>
    {
        Task<List<PaymentRequest>> GetListAsync(
            int skipCount,
            int maxResultCount,
            string sorting,
            string filter,
            DateTime? creationDateMax = null,
            DateTime? creationDateMin = null,
            PaymentRequestState? state = null,
            CancellationToken cancellationToken = default
        );

        Task<int> GetCountAsync(
            string filter,
            DateTime? creationDateMax = null,
            DateTime? creationDateMin = null,
            PaymentRequestState? state = null,
            CancellationToken cancellationToken = default
        );
    }
}