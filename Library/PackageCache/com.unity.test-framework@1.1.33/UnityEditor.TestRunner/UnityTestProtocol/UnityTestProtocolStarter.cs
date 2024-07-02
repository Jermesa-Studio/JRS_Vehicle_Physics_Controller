using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.TestTools.TestRunner.Api;
using UnityEngine;

namespace UnityEditor.TestTools.TestRunner.UnityTestProtocol
{
    [InitializeOnLoad]
    internal static class UnityTestProtocolStarter
    {
        static UnityTestProtocolStarter()
        {
            var commandLineArgs = Environment.GetCommandLineArgs();
            if (commandLineArgs.Contains("-automated") && commandLineArgs.Contains("-runTests")) // wanna have it only for utr run
            {
                var api = ScriptableObject.CreateInstance<TestRunnerApi>();
                var listener = new UnityTestProtocolListener(GetRepositoryPath(commandLineArgs));
                api.RegisterCallbacks(listener);
            }
        }

        private static string GetRepositoryPath(IReadOnlyList<string> commandLineArgs)
        {
            for (var i = 0; i < commandLineArgs.Count; i++)
            {
                if (commandLineArgs[i].Equals("-projectRepositoryPath"))
                {
                    return commandLineArgs[i + 1];
                }
            }
            return string.Empty;
        }
    }
}
