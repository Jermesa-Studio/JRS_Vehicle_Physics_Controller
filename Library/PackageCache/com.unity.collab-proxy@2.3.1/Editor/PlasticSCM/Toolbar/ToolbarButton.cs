using UnityEditor;
using UnityEngine;

using PlasticGui;
using Unity.PlasticSCM.Editor;

namespace Unity.Cloud.Collaborate
{
    [InitializeOnLoad]
    internal static class ToolbarBootstrap
    {
        static ToolbarBootstrap()
        {
            ToolbarButton.InitializeIfNeeded();
        }
    }

    internal class ToolbarButton : SubToolbar
    {
        internal static void InitializeIfNeeded()
        {
            if (CollabPlugin.IsEnabled())
                return;

            ToolbarButton toolbar = new ToolbarButton { Width = 32f };
            Toolbar.AddSubToolbar(toolbar);
        }

        ToolbarButton()
        {
            PlasticPlugin.OnNotificationUpdated += OnPlasticNotificationUpdated;
        }

        ~ToolbarButton()
        {
            PlasticPlugin.OnNotificationUpdated -= OnPlasticNotificationUpdated;
        }

        void OnPlasticNotificationUpdated()
        {
            Toolbar.RepaintToolbar();
        }

        public override void OnGUI(Rect rect)
        {
            Texture icon = PlasticPlugin.GetPluginStatusIcon();
            EditorGUIUtility.SetIconSize(new Vector2(16, 16));

            mButtonGUIContent.image = icon;

            if (GUI.Button(rect, mButtonGUIContent, "AppCommand"))
            {
                PlasticPlugin.OpenPlasticWindowDisablingOfflineModeIfNeeded();
            }

            EditorGUIUtility.SetIconSize(Vector2.zero);
        }

        static GUIContent mButtonGUIContent = new GUIContent(
            string.Empty, PlasticLocalization.GetString(
                PlasticLocalization.Name.UnityVersionControl));
    }
}
