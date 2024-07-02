using UnityEditor;
using UnityEngine;

namespace Unity.PlasticSCM.Editor.UI
{
    internal static class DrawActionButton
    {
        internal static bool For(string buttonText)
        {
            GUIContent buttonContent = new GUIContent(buttonText);

            return ForRegularButton(buttonContent);
        }

        internal static bool For(string buttonText, string buttonTooltip)
        {
            GUIContent buttonContent = new GUIContent(buttonText, buttonTooltip);

            return ForRegularButton(buttonContent);
        }

        internal static bool ForCommentSection(string buttonText)
        {
            GUIContent buttonContent = new GUIContent(buttonText);

            GUIStyle buttonStyle = new GUIStyle(EditorStyles.miniButton);

            buttonStyle.stretchWidth = false;

            float width = MeasureMaxWidth.ForTexts(buttonStyle, buttonText);

            Rect rt = GUILayoutUtility.GetRect(
                buttonContent,
                buttonStyle,
                GUILayout.MinWidth(width),
                GUILayout.MaxWidth(width));

            return GUI.Button(rt, buttonContent, buttonStyle);
        }

        static bool ForRegularButton(GUIContent buttonContent)
        {
            GUIStyle buttonStyle = new GUIStyle(EditorStyles.miniButton);

            buttonStyle.stretchWidth = false;

            Rect rt = GUILayoutUtility.GetRect(
                buttonContent,
                buttonStyle,
                GUILayout.MinWidth(UnityConstants.REGULAR_BUTTON_WIDTH));

            return GUI.Button(rt, buttonContent, buttonStyle);
        }
    }
}
