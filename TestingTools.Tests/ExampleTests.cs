using AutoFixture;
using AutoFixture.AutoMoq;
using AutoFixture.Xunit2;
using FluentAssertions;
using Moq;

namespace TestingTools.Tests;

public interface IMyDependency
{
    public int GetValue(long id);
}

public class MyDependency : IMyDependency
{
    public int GetValue(long id) => 123;
}

public interface IAnotherDependency
{
    public int GetNestedValue();
}

public class AnotherDependency(IDeepDependency deep) : IAnotherDependency
{
    public int GetNestedValue() => deep.GetNestedValue();
}

public interface IDeepDependency
{
    public int GetNestedValue();
}

public class DeepDependency : IDeepDependency
{
    public int GetNestedValue() => 666;
}

public class MyService(IMyDependency dep)
{
    public int GetValue(long id) => dep.GetValue(id);
}

public class ComplexService(IAnotherDependency dep)
{
    public int GetNestedValue() => dep.GetNestedValue();
}

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

public class ExampleTests
{
    // [Fact]
    // public void BuildATest() =>
    //     UnitTest
    //         .For<MyService>()
    //         .Arrange(f =>
    //         {
    //             var id = f.Create<long>();
    //             var expected = f.Create<int>();
    //             _ = f.Mock<IMyDependency>().Setup(x => x.GetValue(id)).Returns(expected);
    //             return new { id, expected };
    //         })
    //         .Act((sut, state) => sut.GetValue(state.id))
    //         .Assert((output, state) => output.Should().Be(state.expected))
    //         .RunSync();

    [Theory, InlineAutoData]
    public void BuildASimpleSystemaInline(long id) =>
        // Arrange
        Harness
            .Build<MyService>(static deps => { })
            // Act
            .GetValue(id)
            // Assert
            .Should()
            .Be(0);

    [Theory, InlineAutoData]
    public void BuildASystemaInline(long id, int expected) =>
        // Arrange
        Harness
            .Build<MyService>(deps =>
                deps.UseMock<IMyDependency>(m => _ = m.Setup(x => x.GetValue(id)).Returns(expected))
            )
            // Act
            .GetValue(id)
            // Assert
            .Should()
            .Be(expected);

    [Theory, InlineAutoData]
    public void BuildAComplexSystemInline(int expected) =>
        // Arrange
        Harness
            .Build<ComplexService>(deps =>
                deps.UseDefault<IAnotherDependency, AnotherDependency>(deps =>
                    deps.UseMock<IDeepDependency>(m =>
                        m.Setup(x => x.GetNestedValue()).Returns(expected)
                    )
                )
            )
            // Act
            .GetNestedValue()
            // Assert
            .Should()
            .Be(expected);

    [Theory, InlineAutoData]
    public void BuildAComplexSystem(int expected)
    {
        // Arrange
        var sut = Harness.Build<ComplexService>(deps =>
            deps.UseDefault<IAnotherDependency, AnotherDependency>(deps =>
                deps.UseMock<IDeepDependency>(m =>
                    m.Setup(x => x.GetNestedValue()).Returns(expected)
                )
            )
        );

        // Act
        var result = sut.GetNestedValue();

        // Assert
        _ = result.Should().Be(expected);
    }
}
