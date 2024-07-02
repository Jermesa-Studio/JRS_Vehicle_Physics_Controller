using System;
using System.Linq;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal.Filters;
using UnityEngine;

namespace UnityEditor.TestTools.TestRunner.Api
{
    /// <summary>
    /// A set of execution settings defining how to run tests, using the <see cref="TestRunnerApi"/>.
    /// </summary>
    [Serializable]
    public class ExecutionSettings
    {
        /// <summary>
        /// Creates an instance with a given set of filters, if any.
        /// </summary>
        /// <param name="filtersToExecute">Set of filters</param>
        public ExecutionSettings(params Filter[] filtersToExecute)
        {
            filters = filtersToExecute;
        }

        [SerializeField]
        private BuildTarget m_TargetPlatform;
        [SerializeField]
        private bool m_HasTargetPlatform;

        /// <summary>
        /// An instance of <see cref="ITestRunSettings"/> to set up before running tests on a Player.
        /// </summary>
        // Note: Is not available after serialization
        public ITestRunSettings overloadTestRunSettings;

        [SerializeField]
        internal Filter filter;
        ///<summary>
        ///A collection of <see cref="Filter"/> to execute tests on.
        ///</summary>
        [SerializeField]
        public Filter[] filters;
        /// <summary>
        ///  Note that this is only supported for EditMode tests, and that tests which take multiple frames (i.e. [UnityTest] tests, or tests with [UnitySetUp] or [UnityTearDown] scaffolding) will be filtered out.
        /// </summary>
        /// <returns>If true, the call to Execute() will run tests synchronously, guaranteeing that all tests have finished running by the time the call returns.</returns>
        [SerializeField]
        public bool runSynchronously;
        /// <summary>
        /// The time, in seconds, the editor should wait for heartbeats after starting a test run on a player. This defaults to 10 minutes.
        /// </summary>
        [SerializeField]
        public int playerHeartbeatTimeout = 60 * 10;

        [SerializeField]
        internal string[] orderedTestNames;
        internal string playerSavePath { get; set; }

        internal bool EditModeIncluded()
        {
            return filters.Any(f => IncludesTestMode(f.testMode, TestMode.EditMode));
        }

        internal bool PlayModeInEditorIncluded()
        {
            return filters.Any(f => IncludesTestMode(f.testMode, TestMode.PlayMode) && targetPlatform == null);
        }

        internal bool PlayerIncluded()
        {
            return filters.Any(f => IncludesTestMode(f.testMode, TestMode.PlayMode) && targetPlatform != null);
        }

        private static bool IncludesTestMode(TestMode testMode, TestMode modeToCheckFor)
        {
            return (testMode & modeToCheckFor) == modeToCheckFor;
        }

        internal ITestFilter BuildNUnitFilter()
        {
            return new OrFilter(filters.Select(f => f.ToRuntimeTestRunnerFilter(runSynchronously).BuildNUnitFilter()).ToArray());
        }

        /// <returns>
        /// The <see cref="BuildTarget"/> platform to run the test on. If set to null, then the Editor is the target for the tests.
        /// </returns>
        internal BuildTarget? targetPlatform
        {
            get { return m_HasTargetPlatform ? (BuildTarget?)m_TargetPlatform : null; }
            set
            {
                {
                    if (value.HasValue)
                    {
                        m_HasTargetPlatform = true;
                        m_TargetPlatform = value.Value;
                    }
                    else
                    {
                        m_HasTargetPlatform = false;
                        m_TargetPlatform = default;
                    }
                }
            }
        }
    }
}
