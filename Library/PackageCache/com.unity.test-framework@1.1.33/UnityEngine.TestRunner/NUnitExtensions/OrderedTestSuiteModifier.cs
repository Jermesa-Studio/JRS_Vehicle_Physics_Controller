using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;

namespace UnityEngine.TestRunner.NUnitExtensions
{
    internal class OrderedTestSuiteModifier : ITestSuiteModifier
    {
        internal const string suiteIsReorderedProperty = "suiteIsReordered";
        private string[] m_OrderedTestNames;
        
        public OrderedTestSuiteModifier(string[] orderedTestNames)
        {
            m_OrderedTestNames = orderedTestNames;
        }

        public TestSuite ModifySuite(TestSuite root)
        {
            var suite = new TestSuite(root.Name);
            suite.Properties.Set(suiteIsReorderedProperty, true);
            var workingStack = new List<ITest> { suite };

            foreach (var fullName in m_OrderedTestNames)
            {
                var test = FindTest(root, fullName);
                if (test == null)
                {
                    continue;
                }

                var ancestorList = GetAncestorList(test);

                for (int i = 0; i < ancestorList.Count; i++)
                {
                    if (i >= workingStack.Count || ancestorList[i].Name != workingStack[i].Name || !ancestorList[i].HasChildren)
                    {
                        // The ancestor list diverges from the current working set. We need to insert a new element
                        var commonParent = workingStack[i - 1];
                        var nodeToClone = ancestorList[i];

                        var newNode = CloneNode(nodeToClone);
                        CloneProperties(newNode, nodeToClone);
                        newNode.Properties.Set(suiteIsReorderedProperty, true);
                        (commonParent as TestSuite).Add(newNode);
                        if (i < workingStack.Count)
                        {
                            workingStack = workingStack.Take(i).ToList();
                        }

                        workingStack.Add(newNode);
                        
                    }
                }
                
            }
            
            return suite;
        }

        private static void CloneProperties(ITest target, ITest source)
        {
            if (target == source)
            {
                return;
            }

            foreach (var key in source.Properties.Keys)
            {
                foreach (var value in source.Properties[key])
                {
                    target.Properties.Set(key, value);
                }
            }
        }

        private static Test CloneNode(ITest test)
        {
            var type = test.GetType();
            if (type == typeof(TestSuite))
            {
                return new TestSuite(test.Name);
            }

            if (type == typeof(TestAssembly))
            {
                var testAssembly = test as TestAssembly;
                return new TestAssembly(testAssembly.Assembly, testAssembly.Name);
            }
            
            if (type == typeof(TestFixture))
            {
                return new TestFixture(test.TypeInfo);
            }
            
            if (type == typeof(TestMethod))
            {
                return test as Test;
            }

            if (type == typeof(ParameterizedMethodSuite))
            {
                return new ParameterizedMethodSuite(test.Method);
            }

            throw new NotImplementedException(type.FullName);
        }

        private static List<ITest> GetAncestorList(ITest test)
        {
            var list = new List<ITest>();
            while (test != null)
            {
                list.Insert(0, test);
                test = test.Parent;
            }

            return list;
        }

        private static ITest FindTest(ITest node, string fullName)
        {
            if (node.HasChildren)
            {
                return node.Tests
                    .Select(test => FindTest(test, fullName))
                    .FirstOrDefault(match => match != null);
            }

            return TestExtensions.GetFullName(node.FullName, node.HasChildIndex() ? node.GetChildIndex() : -1) == fullName ? node : null;
        }
    }
}
