using System;
using System.Reflection;

using UnityEditor;

namespace Unity.PlasticSCM.Editor
{
    public static class CollabPlugin
    {
        public static bool IsEnabled()
        {
            return IsCollabInstanceEnabled();
        }

        internal static void Disable()
        {
            SetCollabEnabledInstanceAs(false);

            SetCollabEnabledInProjectSettingsAs(false);
        }

        internal static void Enable()
        {
            SetCollabEnabledInstanceAs(true);

            SetCollabEnabledInProjectSettingsAs(true);
        }

        static void SetCollabEnabledInstanceAs(bool value)
        {
            object collabInstance = GetCollabInstance();

            if (collabInstance == null)
                return;

            // Invokes Collab.instance.SetCollabEnabledForCurrentProject(false)
            SetCollabEnabledForCurrentProject(collabInstance, value);
        }

        static void SetCollabEnabledInProjectSettingsAs(bool value)
        {
            // Invokes PlayerSettings.SetCloudServiceEnabled("Collab", false)
            SetCloudServiceEnabled("Collab", value);

            AssetDatabase.SaveAssets();
        }

        static bool IsCollabInstanceEnabled()
        {
            object collabInstance = GetCollabInstance();

            if (collabInstance == null)
                return false;

            // Invokes Collab.instance.IsCollabEnabledForCurrentProject()
            return IsCollabEnabledForCurrentProject(collabInstance);
        }

        static void SetCollabEnabledForCurrentProject(object collabInstance, bool enable)
        {
            MethodInfo InternalSetCollabEnabledForCurrentProject =
                CollabType.GetMethod("SetCollabEnabledForCurrentProject");

            if (InternalSetCollabEnabledForCurrentProject == null)
                return;

            InternalSetCollabEnabledForCurrentProject.
                Invoke(collabInstance, new object[] { enable });
        }

        static void SetCloudServiceEnabled(string setting, bool enable)
        {
            MethodInfo InternalSetCloudServiceEnabled = PlayerSettingsType.GetMethod(
               "SetCloudServiceEnabled",
               BindingFlags.NonPublic | BindingFlags.Static);

            if (InternalSetCloudServiceEnabled == null)
                return;

            InternalSetCloudServiceEnabled.
                Invoke(null, new object[] { setting, enable });
        }

        static object GetCollabInstance()
        {
            if (CollabType == null)
                return null;

            PropertyInfo InternalInstance =
                CollabType.GetProperty("instance");

            if (InternalInstance == null)
                return null;

            return InternalInstance.GetValue(null, null);
        }

        static bool IsCollabEnabledForCurrentProject(object collabInstance)
        {
            MethodInfo InternalIsCollabEnabledForCurrentProject =
                CollabType.GetMethod("IsCollabEnabledForCurrentProject");

            if (InternalIsCollabEnabledForCurrentProject == null)
                return false;

            return (bool)InternalIsCollabEnabledForCurrentProject.
                Invoke(collabInstance, null);
        }

        static readonly Type CollabType =
            typeof(UnityEditor.Editor).Assembly.
                GetType("UnityEditor.Collaboration.Collab");

        static readonly Type PlayerSettingsType =
            typeof(UnityEditor.PlayerSettings);
    }
}
