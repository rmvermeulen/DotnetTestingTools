namespace TestingTools.Tests;

public static class Harness
{
    public interface IDependencies<T>
        where T : class
    {
        public IDependencies<T> UseMock<TDep>()
            where TDep : class;
        public IDependencies<T> UseMock<TDep>(Action<Mock<TDep>> setup)
            where TDep : class;
        public IDependencies<T> UseDefault<TDep, TService>()
            where TService : class, TDep;
        public IDependencies<T> UseDefault<TDep, TService>(Action<IDependencies<TService>> setup)
            where TService : class, TDep;
    }

    public class Dependencies<T>(IFixture fixture) : IDependencies<T>
        where T : class
    {
        public IDependencies<T> UseMock<TDep>()
            where TDep : class
        {
            _ = fixture.Freeze<Mock<TDep>>();
            return this;
        }

        public IDependencies<T> UseMock<TDep>(Action<Mock<TDep>> setup)
            where TDep : class
        {
            Mock<TDep> m = fixture.Freeze<Mock<TDep>>();
            setup(m);
            return this;
        }

        public IDependencies<T> UseDefault<TDep, TService>()
            where TService : class, TDep
        {
            fixture.Inject<TDep>(fixture.Freeze<TService>());
            return this;
        }

        public IDependencies<T> UseDefault<TDep, TService>(Action<IDependencies<TService>> setup)
            where TService : class, TDep
        {
            setup(new Dependencies<TService>(fixture));
            fixture.Inject<TDep>(fixture.Freeze<TService>());
            return this;
        }
    }

    public static T Build<T>(Action<IDependencies<T>> setup)
        where T : class
    {
        IFixture fixture = new Fixture().Customize(new AutoMoqCustomization());
        return Build(fixture, setup);
    }

    public static T Build<T>(IFixture fixture, Action<IDependencies<T>> setup)
        where T : class
    {
        setup(new Dependencies<T>(fixture));
        return fixture.Create<T>();
    }
}
