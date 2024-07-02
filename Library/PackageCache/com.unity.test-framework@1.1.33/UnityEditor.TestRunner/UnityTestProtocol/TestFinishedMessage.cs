namespace UnityEditor.TestTools.TestRunner.UnityTestProtocol
{
    internal class TestFinishedMessage : Message
    {
        public string name;
        public TestState state;
        public string message;
        public ulong duration; // milliseconds
        public ulong durationMicroseconds;
        public string stackTrace;
        public string fileName;
        public int lineNumber;

        public TestFinishedMessage()
        {
            type = "TestStatus";
            phase = "End";
        }
    }
}
