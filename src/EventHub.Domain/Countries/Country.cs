using System;
using Volo.Abp;
using Volo.Abp.Domain.Entities;

namespace EventHub.Countries
{
    public class Country : Entity<Guid>
    {
        public string Name { get; private set; }

        private Country()
        {
            
        }

        internal Country(
            Guid id, 
            string name)
        : base(id)
        {
            Name = Check.NotNullOrWhiteSpace(name, nameof(name), CountryConsts.MaxNameLength);
        }
    }
}