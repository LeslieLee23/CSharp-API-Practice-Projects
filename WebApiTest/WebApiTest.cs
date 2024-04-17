namespace WebApiTest;

[TestClass]
public class InMemoryKeyValueStoreTests
{
    private InMemoryKeyValueStore _store;

    [TestInitialize]
    public void Initialize()
    {
        _store = new InMemoryKeyValueStore();
    }

    [TestMethod]
    public void Create_NewUserAndKey_ShouldReturnSuccess()
    {
        var userId = "user1";
        var key = "key1";
        var value = "value1";
        var result = _store.Create(userId, key, value);
        Assert.AreEqual(DbResultStatus.Success, result);

    }
}