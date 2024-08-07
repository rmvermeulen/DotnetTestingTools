using AutoFixture;
using AutoFixture.AutoMoq;
using AutoFixture.Kernel;

namespace TestingTools;

public static class FixtureExtensions
{
    /// Apply a ICustomization to the fixture directly
    public static IFixture Apply<T>(this IFixture fixture)
        where T : ICustomization, new() => fixture.Customize(new T());

    /// Apply a Behavior to the fixture directly
    public static IFixture AddBehavior<T>(this IFixture fixture)
        where T : ISpecimenBuilderTransformation, new()
    {
        if (!fixture.Behaviors.OfType<T>().Any())
        {
            fixture.Behaviors.Add(new T());
        }
        return fixture;
    }

    /// Remove a Behavior from the fixture
    public static IFixture RemoveBehavior<T>(this IFixture fixture)
        where T : ISpecimenBuilderTransformation, new()
    {
        foreach (T b in fixture.Behaviors.OfType<T>().ToArray())
        {
            _ = fixture.Behaviors.Remove(b);
        }
        return fixture;
    }

    /// Apply the AutoMoqCustomization
    public static IFixture UseAutoMoq(this IFixture fixture) =>
        fixture.Apply<AutoMoqCustomization>();

    /// Apply the NoAutoPropertiesCustomization to Type
    public static IFixture UseNoAutoProperties(this IFixture fixture, Type type) =>
        fixture.Customize(new NoAutoPropertiesCustomization(type));

    /// Apply the NoAutoPropertiesCustomization to T
    public static IFixture UseNoAutoProperties<T>(this IFixture fixture) =>
        fixture.Customize(new NoAutoPropertiesCustomization(typeof(T)));

    /// Apply the OmitOnRecursionBehavior
    public static IFixture UseOmitOnRecursion(this IFixture fixture) =>
        fixture.RemoveBehavior<ThrowingRecursionBehavior>().AddBehavior<OmitOnRecursionBehavior>();

    /// Apply the TracingBehavior
    public static IFixture UseTracing(this IFixture fixture) =>
        fixture.AddBehavior<TracingBehavior>();
}
