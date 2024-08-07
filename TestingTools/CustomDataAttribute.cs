using AutoFixture;
using AutoFixture.Xunit2;

namespace TestingTools;

public class CustomDataAttribute<T> : AutoDataAttribute
    where T : ICustomization, new()
{
    public CustomDataAttribute()
        : base(static () => new Fixture().Customize(new T())) { }
}

public class InlineCustomDataAttribute<T>(params object?[] args)
    : InlineAutoDataAttribute(new CustomDataAttribute<T>(), args)
    where T : ICustomization, new();
