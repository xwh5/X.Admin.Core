namespace X.Admin.Core.Permissions;

public static class CorePermissions
{
    public const string GroupName = "Core";


    
    //Add your own permission names. Example:
    //public const string MyPermission1 = GroupName + ".MyPermission1";


    public static class CapManagement
    {
        public const string Default = GroupName + ".Cap";
        public const string Query = Default + ".Query";
    }
}
