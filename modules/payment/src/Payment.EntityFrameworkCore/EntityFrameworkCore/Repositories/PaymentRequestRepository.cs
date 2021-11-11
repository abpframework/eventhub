using System;
using System.Linq;
using System.Collections.Generic;
using System.Linq.Dynamic.Core;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Payment.PaymentRequests;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace Payment.EntityFrameworkCore.Repositories
{
    public class PaymentRequestRepository : EfCoreRepository<IPaymentDbContext, PaymentRequest, Guid>, IPaymentRequestRepository
    {
        public PaymentRequestRepository(IDbContextProvider<IPaymentDbContext> dbContextProvider) 
            : base(dbContextProvider)
        {
        }

        public async Task<List<PaymentRequest>> GetListAsync(
            int skipCount, 
            int maxResultCount, 
            string sorting, 
            string productName = null, 
            DateTime? maxCreationTime = null,
            DateTime? minCreationTime = null, 
            PaymentRequestState? state = null, 
            CancellationToken cancellationToken = default
        )
        {
            return await (await GetQueryableAsync())
                .WhereIf(!string.IsNullOrWhiteSpace(productName), x => x.ProductName.Contains(productName))
                .WhereIf(maxCreationTime != null, p => p.CreationTime <= maxCreationTime)
                .WhereIf(minCreationTime != null, p => p.CreationTime >= minCreationTime)
                .WhereIf(state != null, p => p.State == state)
                .OrderBy(string.IsNullOrWhiteSpace(sorting) ? nameof(PaymentRequest.ProductName) : sorting)
                .PageBy(skipCount, maxResultCount)
                .ToListAsync(GetCancellationToken(cancellationToken));
        }

        public async Task<int> GetCountAsync(
            string productName = null, 
            DateTime? maxCreationTime = null, 
            DateTime? minCreationTime = null,
            PaymentRequestState? state = null, 
            CancellationToken cancellationToken = default
        )
        {
            return await (await GetQueryableAsync())
                .WhereIf(!string.IsNullOrWhiteSpace(productName), x => x.ProductName.Contains(productName))
                .WhereIf(maxCreationTime != null, p => p.CreationTime <= maxCreationTime)
                .WhereIf(minCreationTime != null, p => p.CreationTime >= minCreationTime)
                .WhereIf(state != null, p => p.State == state)
                .CountAsync(GetCancellationToken(cancellationToken));
        }
    }
}