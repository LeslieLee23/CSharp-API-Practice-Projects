
public class InMemoryKeyValueStore : IKeyValueStore
{
    private readonly Dictionary<string, Dictionary<string, string>> _store = new(); // Create nested dictionary to contain both the key value and the namespace 

    public DbResultStatus Create(string userId, string key, string value)
    {
        if (!_store.ContainsKey(userId))
        {
            _store[userId] = new Dictionary<string, string>(); // If user is new, create a new dictionary with the new userId as the key and a empty dictionary as the value
        }

        if (_store[userId].ContainsKey(key))
        {
            return DbResultStatus.KeyAlreadyExists;
        }
        bool added = _store[userId].TryAdd(key, value);
        return added ? DbResultStatus.Success : DbResultStatus.Error;
    }
    public DbResultStatus Read(string userId, string key, out string value)
    {
        value = null;
        if (_store.TryGetValue(userId, out var userStore))
        {
            if (userStore.TryGetValue(key, out value))
            {
                return DbResultStatus.Success;
            }
            return DbResultStatus.KeyNotFound;
        }
        return DbResultStatus.UserNotFound;
    }

    public DbResultStatus Update(string userId, string key, string value)
    {
        if (_store.TryGetValue(userId, out var userStore))
        {
            if (userStore.ContainsKey(key))
            {
                userStore[key] = value;
                return DbResultStatus.Success;
            }
            return DbResultStatus.KeyNotFound;
        }
        return DbResultStatus.UserNotFound;
    }

    public DbResultStatus Delete(string userId, string key)
    {
        if (_store.TryGetValue(userId, out var userStore))
        {
            bool removed = userStore.Remove(key);
            return removed ? DbResultStatus.Success : DbResultStatus.KeyNotFound;
        }
        return DbResultStatus.UserNotFound;
    }

    public DbResultStatus GetAll(string userId, out IDictionary<string, string> allData)
    {
        allData = new Dictionary<string, string>();
        if (_store.TryGetValue(userId, out var userStore))
        {
            allData = new Dictionary<string, string>(userStore);
            return DbResultStatus.Success;
        }
        return DbResultStatus.UserNotFound;
    }
}