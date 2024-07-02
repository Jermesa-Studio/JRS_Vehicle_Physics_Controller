using UnityEditor;

namespace Unity.PlasticSCM.Editor.CollabMigration
{
    static class CloudProjectId
    {
        internal static bool HasValue()
        {
            if (PlasticPlugin.IsUnitTesting)
                return false;
            
            return !string.IsNullOrEmpty(GetValue());
        }

        internal static string GetValue()
        {
            //disable Warning CS0618  'PlayerSettings.cloudProjectId' is obsolete: 'cloudProjectId is deprecated
#pragma warning disable 0618
            return PlayerSettings.cloudProjectId;
        }
    }
}