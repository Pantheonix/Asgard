using EnkiProblems.Localization;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Localization;

namespace EnkiProblems.Permissions;

public class EnkiProblemsPermissionDefinitionProvider : PermissionDefinitionProvider
{
    public override void Define(IPermissionDefinitionContext context)
    {
        var myGroup = context.AddGroup(EnkiProblemsPermissions.GroupName);
        //Define your own permissions here. Example:
        //myGroup.AddPermission(EnkiProblemsPermissions.MyPermission1, L("Permission:MyPermission1"));
    }

    private static LocalizableString L(string name)
    {
        return LocalizableString.Create<EnkiProblemsResource>(name);
    }
}
