namespace TestingTools.Tests.Services;

public class AnotherDependency(IDeepDependency deep) : IAnotherDependency
{
    public int GetNestedValue() => deep.GetNestedValue();
}
