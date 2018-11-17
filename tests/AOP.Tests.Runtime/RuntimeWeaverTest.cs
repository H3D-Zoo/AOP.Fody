using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using AOP.Fody;
using Fody;
namespace AOP.Tests.Runtime
{
    public class RuntimeWeaverTest
    {
        [Fact]
        void RuntimeWeaver()
        {
            var weaving = new ModuleWeaver();
            weaving.ExecuteTestRun(System.IO.Path.Combine(System.IO.Directory.GetCurrentDirectory(),"AssemblyToProcess.dll"));
        }
    }
}
