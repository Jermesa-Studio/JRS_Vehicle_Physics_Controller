namespace UnityEditor.TestTools.CodeCoverage
{
    [System.Serializable]
    internal class JsonFile
    {
        public string[] assembliesInclude;
        public string[] assembliesExclude;
        public string[] pathsInclude;
        public string[] pathsExclude;
    }
}
