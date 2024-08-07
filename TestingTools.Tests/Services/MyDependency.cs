namespace TestingTools.Tests.Services;

public class MyDependency : IMyDependency
{
    public int GetValue(long id) => 123;
}
