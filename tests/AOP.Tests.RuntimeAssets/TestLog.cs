using System;
using System.Collections.Generic;
using System.Text;

namespace AOP.Tests.Assets
{
    public static class TestLog
    {
        private static readonly List<string> _log = new List<string>();
        public static IReadOnlyList<string> Log => _log;

        public static void Write(string @event)
        {
            _log.Add(@event);
        }

        public static void Reset()
        {
            _log.Clear();
        }
    }
}
