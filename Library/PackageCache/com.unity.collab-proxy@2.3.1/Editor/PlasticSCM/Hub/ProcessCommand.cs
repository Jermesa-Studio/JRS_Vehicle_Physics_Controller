using System;
using System.Collections.Generic;

using UnityEditor;

using Codice.LogWrapper;
using Unity.PlasticSCM.Editor.Hub.Operations;

namespace Unity.PlasticSCM.Editor.Hub
{
    internal static class ProcessCommand
    {
        internal const string IS_PROCESS_COMMAND_ALREADY_EXECUTED_KEY =
            "PlasticSCM.ProcessCommand.IsAlreadyExecuted";
        internal const string IS_PLASTIC_COMMAND_KEY =
            "PlasticSCM.ProcessCommand.IsPlasticCommand";

        internal static void Initialize()
        {
            EditorApplication.update += RunOnceWhenAccessTokenIsInitialized;
        }

        static void RunOnceWhenAccessTokenIsInitialized()
        {
            if (string.IsNullOrEmpty(CloudProjectSettings.accessToken))
                return;

            EditorApplication.update -= RunOnceWhenAccessTokenIsInitialized;

            Execute(CloudProjectSettings.accessToken);
        }

        static void Execute(string unityAccessToken)
        {
            if (SessionState.GetBool(
                    IS_PROCESS_COMMAND_ALREADY_EXECUTED_KEY, false))
            {
                return;
            }

            ProcessCommandFromArgs(
                Environment.GetCommandLineArgs(),
                unityAccessToken);

            SessionState.SetBool(
                IS_PROCESS_COMMAND_ALREADY_EXECUTED_KEY, true);
        }

        internal static void ProcessCommandFromArgs(
            string[] commandLineArgs,
            string unityAccessToken)
        {
            Dictionary<string, string> args =
                CommandLineArguments.Build(commandLineArgs);

            mLog.DebugFormat(
                "Processing Unity arguments: {0}",
                string.Join(" ", commandLineArgs));

            ParseArguments.Command command =
                ParseArguments.GetCommand(args);

            if (!command.IsValid())
                return;

            SessionState.SetBool(
                IS_PLASTIC_COMMAND_KEY, true);

            OperationParams parameters = OperationParams.
                BuildFromCommand(command, unityAccessToken);

            switch (command.OperationType)
            {
                case ParseArguments.Command.Operation.CreateWorkspace:
                    CreateWorkspace.LaunchOperation(parameters);
                    return;
                case ParseArguments.Command.Operation.DownloadRepository:
                    DownloadRepository.LaunchOperation(parameters);
                    return;
            }
        }

        static readonly ILog mLog = LogManager.GetLogger("ProcessCommand");
    }
}
