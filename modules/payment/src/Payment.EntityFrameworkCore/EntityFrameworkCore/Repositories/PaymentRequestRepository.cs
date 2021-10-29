using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Payment.PaymentRequests;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace Payment.EntityFrameworkCore.Repositories
{
    public class PaymentRequestRepository : EfCoreRepository<IPaymentDbContext, PaymentRequest,Guid>, IPaymentRequestRepository
    {
        public PaymentRequestRepository(IDbContextProvider<IPaymentDbContext> dbContextProvider) 
            : base(dbContextProvider)
        {
        }

        public async Task<List<PaymentRequest>> GetListAsync(
            int skipCount, 
            int maxResultCount, 
            string sorting, 
            string filter, 
            DateTime? creationDateMax = null,
            DateTime? creationDateMin = null, 
            PaymentRequestState? state = null, 
            CancellationToken cancellationToken = default
        )
        {
            return await (await GetQueryableAsync())
                .WhereIf(!string.IsNullOrWhiteSpace(filter),
                    x => x.Currency.Contains(filter) || x.ProductName.Contains(filter))
                .WhereIf(creationDateMax != null, p => p.CreationTime <= creationDateMax)
                .WhereIf(creationDateMin != null, p => p.CreationTime >= creationDateMin)
                .WhereIf(state != null, p => p.State == state)
                .PageBy(skipCount, maxResultCount)
                .ToListAsync(GetCancellationToken(cancellationToken));
        }

        public async Task<int> GetCountAsync(
            string filter, 
            DateTime? creationDateMax = null, 
            DateTime? creationDateMin = null,
            PaymentRequestState? state = null, 
            CancellationToken cancellationToken = default
        )
        {
            return await (await GetQueryableAsync())
                .WhereIf(!string.IsNullOrWhiteSpace(filter),
                    x => x.Currency.Contains(filter) || x.ProductName.Contains(filter))
                .WhereIf(creationDateMax != null, p => p.CreationTime <= creationDateMax)
                .WhereIf(creationDateMin != null, p => p.CreationTime >= creationDateMin)
                .WhereIf(state != null, p => p.State == state)
                .CountAsync(GetCancellationToken(cancellationToken));
        }
    }
}