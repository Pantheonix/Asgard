using Volo.Abp.Settings;

namespace EnkiProblems.Settings;

public class EnkiProblemsSettingDefinitionProvider : SettingDefinitionProvider
{
    public override void Define(ISettingDefinitionContext context)
    {
        //Define your own settings here. Example:
        //context.Add(new SettingDefinition(EnkiProblemsSettings.MySetting1));
    }
}
