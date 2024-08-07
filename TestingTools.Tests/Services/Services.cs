namespace TestingTools.Tests.Services;

public class ComplexService(IAnotherDependency dep)
{
    public int GetNestedValue() => dep.GetNestedValue();
}
