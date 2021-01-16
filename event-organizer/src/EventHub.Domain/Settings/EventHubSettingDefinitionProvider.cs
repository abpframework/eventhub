using Volo.Abp.Settings;

namespace EventHub.Settings
{
    public class EventHubSettingDefinitionProvider : SettingDefinitionProvider
    {
        public override void Define(ISettingDefinitionContext context)
        {
            //Define your own settings here. Example:
            //context.Add(new SettingDefinition(EventHubSettings.MySetting1));
        }
    }
}
