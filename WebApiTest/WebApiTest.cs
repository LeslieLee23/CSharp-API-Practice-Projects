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

        Assert.AreEqual(DbResultStatus.Success, result, "Creating a new key for a new user should return Success.");

    }
    [TestMethod]
    public void Create_ExistingUserAndKey_ShouldReturnKeyAlreadyExists()
    {
        var userId = "user1";
        var key = "key1";
        var value = "value1";
        _store.Create(userId, key, value);
        var result = _store.Create(userId, key, "newValue");

        Assert.AreEqual(DbResultStatus.KeyAlreadyExists, result, "Creating an existing key should return KeyAlreadyExists.");
    }

    [TestMethod]
    public void Create_MultipleKeysForSameUser_ShouldReturnSuccess()
    {
        var userId = "user1";
        var value1 = "value1";
        var value2 = "value2";

        Assert.AreEqual(DbResultStatus.Success, _store.Create(userId, "key1", value1));
        Assert.AreEqual(DbResultStatus.Success, _store.Create(userId, "key2", value2));
    }

    [TestMethod]
    public void Create_KeysForMultipleUsers_ShouldReturnSuccess()
    {
        var user1Id = "user1";
        var user2Id = "user2";
        var key = "key";
        var value = "value";

        Assert.AreEqual(DbResultStatus.Success, _store.Create(user1Id, key, value));
        Assert.AreEqual(DbResultStatus.Success, _store.Create(user2Id, key, value));
    }

    [TestMethod]
    public void Read_ExistingKey_ReturnsSuccessAndCorrectValue()
    {
        string userId = "user1";
        string key = "key1";
        string expectedValue = "value1";
        _store.Create(userId, key, expectedValue);

        string actualValue;
        var result = _store.Read(userId, key, out actualValue);

        Assert.AreEqual(DbResultStatus.Success, result, "Reading an existing key should return Success.");
        Assert.AreEqual(expectedValue, actualValue, "The value returned should match the expected value.");
    }

    [TestMethod]
    public void Read_NonExistentUser_ReturnsUserNotFound()
    {
        string userId = "user1";
        string key = "key1";
        _store.Create(userId, key, "someValue");

        var result = _store.Read("nonExistentUser", key, out var value);
        Assert.AreEqual(DbResultStatus.UserNotFound, result, "Readibg a non-existent user ID should return UserNotFound.");
        Assert.IsNull(value, "The result value should be null when user is not found");
    }

    [TestMethod]
    public void Read_NonExistentKey_ReturnsKeyNotFound()
    {
        string userId = "user1";
        string key = "nonExistentKey";
        _store.Create(userId, "existingKey", "someValue");
        var result = _store.Read(userId, key, out var value);

        Assert.AreEqual(DbResultStatus.KeyNotFound, result, "Reading with a non-existent key should return KeyNotFound.");
        Assert.IsNull(value, "The value should be null when the key is not found.");
    }

    [TestMethod]
    public void Update_ExistingKey_SuccessfullyUpdatesValue()
    {
        // Arrange
        string userId = "user1";
        string key = "key1";
        string initialValue = "initialValue";
        string updatedValue = "updatedValue";
        _store.Create(userId, key, initialValue);
        var result = _store.Update(userId, key, updatedValue);

        string valueAfterUpdate;
        _store.Read(userId, key, out valueAfterUpdate);
        Assert.AreEqual(DbResultStatus.Success, result, "Updating an existing key should return Success.");
        Assert.AreEqual(updatedValue, valueAfterUpdate, "The value should be updated to the new value.");
    }

    [TestMethod]
    public void Update_NonExistentKey_ReturnsKeyNotFound()
    {
        string userId = "user1";
        string key = "nonExistentKey";
        string value = "newValue";
        _store.Create(userId, "someOtherKey", "someValue");
        var result = _store.Update(userId, key, value);

        Assert.AreEqual(DbResultStatus.KeyNotFound, result, "Updating a non-existent key should return KeyNotFound.");
    }

    [TestMethod]
    public void Update_NonExistentUser_ReturnsUserNotFound()
    {
        string userId = "nonExistentUser";
        string key = "key1";
        string value = "newValue";
        var result = _store.Update(userId, key, value);

        Assert.AreEqual(DbResultStatus.UserNotFound, result, "Updating a key for a non-existent user should return UserNotFound.");
    }

    [TestMethod]
    public void Delete_ExistingKey_ReturnsSuccessAndRemovesKey()
    {
        string userId = "user1";
        string key = "key1";
        string value = "value1";
        _store.Create(userId, key, value);
        var result = _store.Delete(userId, key);

        var readResult = _store.Read(userId, key, out var retrievedValue);
        Assert.AreEqual(DbResultStatus.Success, result, "Deleting an existing key should return Success.");
        Assert.AreEqual(DbResultStatus.KeyNotFound, readResult, "The key should no longer exist.");
        Assert.IsNull(retrievedValue, "No value should be retrieved for the deleted key.");
    }

    [TestMethod]
    public void Delete_NonExistentKey_ReturnsKeyNotFound()
    {
        string userId = "user1";
        string key = "nonExistentKey";
        _store.Create(userId, "someOtherKey", "someValue");
        var result = _store.Delete(userId, key);

        Assert.AreEqual(DbResultStatus.KeyNotFound, result, "Deleting a non-existent key should return KeyNotFound.");
    }

    [TestMethod]
    public void Delete_NonExistentUser_ReturnsUserNotFound()
    {
        string userId = "nonExistentUser";
        string key = "key1";
        var result = _store.Delete(userId, key);

        Assert.AreEqual(DbResultStatus.UserNotFound, result, "Deleting a key for a non-existent user should return UserNotFound.");
    }

    [TestMethod]
    public void GetAll_ExistingUser_ReturnsAllData()
    {
        string userId = "user1";
        _store.Create(userId, "key1", "value1");
        _store.Create(userId, "key2", "value2");
        _store.Create(userId, "key3", "value3");
        IDictionary<string, string> allData;
        var result = _store.GetAll(userId, out allData);

        Assert.AreEqual(DbResultStatus.Success, result, "Retrieving all data for an existing user should return Success.");
        Assert.IsNotNull(allData, "The returned data dictionary should not be null.");
        Assert.AreEqual(3, allData.Count, "The returned dictionary should contain exactly three entries.");
        Assert.AreEqual("value1", allData["key1"], "The value for 'key1' should be 'value1'.");
        Assert.AreEqual("value2", allData["key2"], "The value for 'key2' should be 'value2'.");
        Assert.AreEqual("value3", allData["key3"], "The value for 'key3' should be 'value3'.");
    }

    [TestMethod]
    public void GetAll_NonExistentUser_ReturnsUserNotFound()
    {
        IDictionary<string, string> allData;
        var result = _store.GetAll("nonExistentUser", out allData);

        Assert.AreEqual(DbResultStatus.UserNotFound, result, "Trying to retrieve all data for a non-existent user should return UserNotFound.");
        Assert.IsNotNull(allData, "The returned data dictionary should not be null even if user is not found.");
        Assert.AreEqual(0, allData.Count, "The returned dictionary should be empty for a non-existent user.");
    }



}