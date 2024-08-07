using TestingTools.Tests.Services;

namespace TestingTools.Tests;

public class HarnessTests
{
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
