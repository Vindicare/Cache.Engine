using System;
using BasicFodyAddin.Fody;
using Fody;
using Xunit;
#pragma warning disable 618

public class WeaverTests
{
    static TestResult testResult;

    static WeaverTests()
    {
        var weavingTask = new ModuleWeaver();
        testResult = weavingTask.ExecuteTestRun("AssemblyToProcess.dll");
    }

    [Fact]
    public void CacheWeaver_WhenCallingMethodWithBasicDataTypes_ValidateCachingInstructionAdded()
    {
        // Arrange
        var testClassType = testResult.Assembly.GetType("AssemblyToProcess.TestClass");
        var testClass = (dynamic)Activator.CreateInstance(testClassType);

        // Act
        var result = testClass.GetResult(2);
        var cachedResult = testClass.GetResult(2);

        // Assert
        Assert.Equal(0, result);
        Assert.Equal(1, cachedResult);
    }
}