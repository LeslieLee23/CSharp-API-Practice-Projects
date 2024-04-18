using System.Threading;
public class InMemoryKeyValueStore : IKeyValueStore
{
    private readonly ReaderWriterLockSlim _lock = new();

    private readonly Dictionary<string, Dictionary<string, string>> _store = new(); // Create nested dictionary to contain both the key value and the namespace 

    private DbResultStatus ValidateParameters(string userId, string key, string value)
    {
        if (userId == null)
            return DbResultStatus.UserIdIsNull;
        if (key == null)
            return DbResultStatus.KeyIsNull;
        if (value == null)
            return DbResultStatus.ValueIsNull;

        return DbResultStatus.Success;
    }

    private DbResultStatus ValidateParameters(string userId, string key)
    {
        if (userId == null)
            return DbResultStatus.UserIdIsNull;
        if (key == null)
            return DbResultStatus.KeyIsNull;

        return DbResultStatus.Success;
    }

    private DbResultStatus ValidateParameters(string userId)
    {
        if (userId == null)
            return DbResultStatus.UserIdIsNull;

        return DbResultStatus.Success;
    }

    public DbResultStatus Create(string userId, string key, string value)
    {
        var validationResult = ValidateParameters(userId, key, value);
        if (validationResult != DbResultStatus.Success)
            return validationResult;

        _lock.EnterWriteLock();
        try
        {
            if (!_store.ContainsKey(userId))
            {
                _store[userId] = new Dictionary<string, string>();
                _store[userId].Add(key, value);
                return DbResultStatus.Success;
            }

            if (_store[userId].ContainsKey(key))
            {
                return DbResultStatus.KeyAlreadyExists;
            }
            _store[userId].Add(key, value);
            return DbResultStatus.Success;
        }
        finally
        {
            _lock.ExitWriteLock();
        }
    }

    public DbResultStatus Read(string userId, string key, out string value)
    {
        value = null;
        var validationResult = ValidateParameters(userId, key);
        if (validationResult != DbResultStatus.Success)
            return validationResult;

        _lock.EnterReadLock();
        try
        {

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
        finally
        {
            _lock.ExitReadLock();
        }
    }

    public DbResultStatus Update(string userId, string key, string value)
    {
        var validationResult = ValidateParameters(userId, key, value);
        if (validationResult != DbResultStatus.Success)
            return validationResult;

        _lock.EnterWriteLock();
        try
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
        finally
        {
            _lock.ExitWriteLock();
        }
    }

    public DbResultStatus Delete(string userId, string key)
    {
        var validationResult = ValidateParameters(userId, key);
        if (validationResult != DbResultStatus.Success)
            return validationResult;

        _lock.EnterWriteLock();
        try
        {
            if (_store.TryGetValue(userId, out var userStore))
            {
                bool removed = userStore.Remove(key);
                return removed ? DbResultStatus.Success : DbResultStatus.KeyNotFound;
            }
            return DbResultStatus.UserNotFound;
        }
        finally
        {
            _lock.ExitWriteLock();
        }
    }

    public DbResultStatus GetAll(string userId, out IDictionary<string, string> allData)
    {
        allData = new Dictionary<string, string>();
        var validationResult = ValidateParameters(userId);
        if (validationResult != DbResultStatus.Success)
            return validationResult;

        _lock.EnterReadLock();
        try
        {
            if (_store.TryGetValue(userId, out var userStore))
            {
                allData = new Dictionary<string, string>(userStore);
                return DbResultStatus.Success;
            }
            return DbResultStatus.UserNotFound;
        }
        finally
        {
            _lock.ExitReadLock();
        }
    }
}