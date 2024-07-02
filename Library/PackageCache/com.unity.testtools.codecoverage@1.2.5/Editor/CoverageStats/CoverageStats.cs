using System;
using System.Reflection;
using UnityEngine.TestTools;

namespace UnityEditor.TestTools.CodeCoverage
{
    internal class CoverageStats : ICoverageStatsProvider
    {
        public CoveredSequencePoint[] GetSequencePointsFor(MethodBase method)
        {
            return Coverage.GetSequencePointsFor(method);
        }

        public CoveredMethodStats[] GetStatsFor(Type type)
        {
            return Coverage.GetStatsFor(type);
        }

        public CoveredMethodStats GetStatsFor(MethodBase methodBase)
        {
            return Coverage.GetStatsFor(methodBase);
        }

        public CoveredMethodStats[] GetStatsFor(MethodBase[] methodBases)
        {
            return Coverage.GetStatsFor(methodBases);
        }

        public CoveredMethodStats[] GetStatsForAllCoveredMethods()
        {
            return Coverage.GetStatsForAllCoveredMethods();
        }

        public void ResetAll()
        {
            Coverage.ResetAll();
        }

        public void ResetFor(MethodBase methodBase)
        {
            Coverage.ResetFor(methodBase);
        }
    }
}