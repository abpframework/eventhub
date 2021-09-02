using System;
using Volo.Abp.Application.Dtos;

namespace EventHub.Admin.Events
{
    public class CountryLookupDto : EntityDto<Guid>
    {
        public string Name { get; set; }
    }
}
