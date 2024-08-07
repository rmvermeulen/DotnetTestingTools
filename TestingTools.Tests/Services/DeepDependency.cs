namespace TestingTools.Tests.Services;

public class DeepDependency : IDeepDependency
{
    public int GetNestedValue() => 666;
}
