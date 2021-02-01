using System;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Repositories;

namespace EventHub.Events
{
    public class EventUrlCodeGenerator : ITransientDependency
    {
        private static string UrlCodeChars = "abcdefghijklmnopqrstuvwxyz0123456789";

        private readonly IRepository<Event, Guid> _eventRepository;

        public EventUrlCodeGenerator(IRepository<Event, Guid> eventRepository)
        {
            _eventRepository = eventRepository;
        }

        public async Task<string> GenerateAsync()
        {
            string urlCode;

            do
            {
                urlCode = GenerateRandomCodeAsync();
            } while (await _eventRepository.AnyAsync(x => x.UrlCode == urlCode));

            return urlCode;
        }

        private string GenerateRandomCodeAsync()
        {
            var sb = new StringBuilder();

            for (var i = 0; i < EventConsts.UrlCodeLength; i++)
            {
                sb.Append(UrlCodeChars[RandomHelper.GetRandom(0, UrlCodeChars.Length)]);
            }

            return sb.ToString();
        }
    }
}
