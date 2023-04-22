namespace JustFunctionalEvaluatortests.IntegrationTests;

[CollectionDefinition(Name)]
public class FunctionServerFixtureTestsCollection : ICollectionFixture<FunctionServerFixture>
{
    public const string Name = nameof(FunctionServerFixtureTestsCollection);
}
