using Volo.Abp.GlobalFeatures;
using Volo.Abp.Threading;

namespace EventHub
{
    public static class EventHubGlobalFeatureConfigurator
    {
        private static readonly OneTimeRunner OneTimeRunner = new();

        public static void Configure()
        {
            OneTimeRunner.Run(() =>
            {
                GlobalFeatureManager.Instance.Modules.CmsKit(cmsKit =>
                {
                    cmsKit.Tags.Enable();
                    cmsKit.Comments.Enable();
                });
            });
        }
    }
}