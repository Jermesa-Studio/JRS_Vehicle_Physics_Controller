using UnityEngine;

public class JSToggleScripts : MonoBehaviour
{
    [System.Serializable]
    public class ScriptToggle
    {
        public MonoBehaviour script;
        public KeyCode toggleKey;
    }

    public ScriptToggle[] scriptToggles;

    void Update()
    {
        foreach (var scriptToggle in scriptToggles)
        {
            if (Input.GetKeyDown(scriptToggle.toggleKey))
            {
                // Enable the selected script and disable all others
                foreach (var otherScriptToggle in scriptToggles)
                {
                    otherScriptToggle.script.enabled = otherScriptToggle == scriptToggle;
                }
            }
        }
    }
}
