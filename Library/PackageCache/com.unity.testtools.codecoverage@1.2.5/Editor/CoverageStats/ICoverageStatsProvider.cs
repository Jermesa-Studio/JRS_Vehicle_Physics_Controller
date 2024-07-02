using System;
using System.Reflection;
using UnityEngine.TestTools;

namespace UnityEditor.TestTools.CodeCoverage
{
    internal interface ICoverageStatsProvider
    {
        CoveredSequencePoint[] GetSequencePointsFor(MethodBase method);
        CoveredMethodStats[] GetStatsFor(Type type);
        CoveredMethodStats GetStatsFor(MethodBase methodBase);
        CoveredMethodStats[] GetStatsFor(MethodBase[] methodBases);
        CoveredMethodStats[] GetStatsForAllCoveredMethods();
        void ResetAll();
        void ResetFor(MethodBase methodBase);
    }
}