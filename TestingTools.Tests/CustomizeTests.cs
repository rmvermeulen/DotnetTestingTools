using TestingTools.Tests.Services;

namespace TestingTools.Tests;

public class CustomizeTests
{
    public class AllLongsAre12 : ICustomization
    {
        public void Customize(IFixture fixture) => fixture.Inject(12L);
    }

    [Theory]
    [InlineAutoData(false)]
    [InlineCustomData<AllLongsAre12>(true)]
    public void EasyCustomization(bool shouldBeEqual, long id, List<long> someValues)
    {
        someValues.Add(id);

        _ = shouldBeEqual
            ? someValues.Should().BeEquivalentTo([12, 12, 12, 12])
            : someValues.Should().NotBeEquivalentTo([12, 12, 12, 12]);
    }

    public class SpecificSetup : ICustomization
    {
        public void Customize(IFixture fixture) =>
            fixture
                .UseAutoMoq()
                .Apply<AllLongsAre12>()
                //.UseTracing()
                .UseOmitOnRecursion();
    }

    [Theory, InlineCustomData<SpecificSetup>]
    public void SpecificCustomization(long id, MyService sut)
    {
        _ = id.Should().Be(12);
        _ = sut.GetValue(id).Should().Be(It.IsAny<int>());
    }
}
