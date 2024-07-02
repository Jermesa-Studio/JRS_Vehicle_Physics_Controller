using System;
using System.Linq;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal.Filters;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools.TestRunner.GUI;

namespace UnityEngine.TestTools.TestRunner
{
    [Serializable]
    internal class PlaymodeTestsControllerSettings
    {
        [SerializeField]
        public RuntimeTestRunnerFilter[] filters;
        public bool sceneBased;
        public string originalScene;
        public string bootstrapScene;
        public bool runInBackgroundValue;
        public bool consoleErrorPaused;
        public string[] orderedTestNames;


        public static PlaymodeTestsControllerSettings CreateRunnerSettings(RuntimeTestRunnerFilter[] filters, string[] orderedTestNames)
        {
            var settings = new PlaymodeTestsControllerSettings
            {
                filters = filters,
                sceneBased = false,
                originalScene = SceneManager.GetActiveScene().path,
                bootstrapScene = null,
                orderedTestNames = orderedTestNames
            };
            return settings;
        }

        internal ITestFilter BuildNUnitFilter()
        {
            return new OrFilter(filters.Select(f => f.BuildNUnitFilter()).ToArray());
        }
    }
}
