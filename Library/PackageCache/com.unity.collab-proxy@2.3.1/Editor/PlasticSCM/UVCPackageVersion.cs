using System;
using System.Linq;

using Codice.LogWrapper;
using Unity.PlasticSCM.Editor.UI;

using PackageManager = UnityEditor.PackageManager;

namespace Unity.PlasticSCM.Editor
{
    internal static class UVCPackageVersion
    {
        internal static string Value { get; private set; }

        internal static void AsyncGetVersion()
        {
            AsyncFindPackage(
                UVCS_PACKAGE_NAME,
                PackageManager.Client.List(true),
                OnFindPackageCompleted);
        }

        static void OnFindPackageCompleted(
            PackageManager.PackageInfo packageInfo)
        {
            if (packageInfo == null)
                return;

            Value = packageInfo.version;

            mLog.DebugFormat("OnFindPackageCompleted - Package version: {0}", Value);
        }

        static void AsyncFindPackage(
            string packageName,
            PackageManager.Requests.ListRequest listRequest,
            Action<PackageManager.PackageInfo> onCompleted)
        {
            EditorDispatcher.Dispatch(delegate
            {
                if (!listRequest.IsCompleted)
                {
                    AsyncFindPackage(
                        packageName, listRequest,
                        onCompleted);
                    return;
                }

                if (listRequest.Status == PackageManager.StatusCode.Success &&
                    listRequest.Result != null)
                {
                    PackageManager.PackageInfo packageInfo =
                        listRequest.Result.FirstOrDefault(
                            package => package.name == packageName);

                    onCompleted(packageInfo);
                }
            });
        }

        const string UVCS_PACKAGE_NAME = "com.unity.collab-proxy";

        static readonly ILog mLog = LogManager.GetLogger("UVCPackageVersion");
    }
}
