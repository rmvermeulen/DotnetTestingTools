namespace TestingTools.Tests.Services;

public class MyService(IMyDependency dep)
{
    public int GetValue(long id) => dep.GetValue(id);
}
