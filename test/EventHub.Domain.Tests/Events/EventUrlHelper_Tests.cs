using Shouldly;
using Xunit;

namespace EventHub.Events
{
    public class EventUrlHelper_Tests
    {
        [Theory]
        [InlineData("Introducing ABP Framework!", "introducing-abp-framework")]
        [InlineData("Blazor: UI Messages", "blazor-ui-messages")]
        [InlineData("What's new in .NET 6", "whats-new-in-net-6")]
        public void Should_Convert_Title_To_Proper_Urls(string title, string url)
        {
            var result = EventUrlHelper.ConvertTitleToUrlPart(title);
            result.ShouldBe(url);
        }
    }
}