using NRedisStack;
using NRedisStack.RedisStackCommands;
using StackExchange.Redis;
public class RedisKeyValueStore : IKeyValueStore
{
    private readonly IDatabase _db;

    // Constructor
    public RedisKeyValueStore(IDatabase db)
    {
        _db = db;
    }

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

    private string BuildKey(string userId, string key)
    {
        return $"{userId}:{key}";
    }

    public DbResultStatus Create(string userId, string key, string value)
    {
        var validationResult = ValidateParameters(userId, key, value);
        if (validationResult != DbResultStatus.Success)
            return validationResult;
        string fullKey = BuildKey(userId, key);
        bool result = _db.StringSet(fullKey, value, when: When.NotExists);
        return result ? DbResultStatus.Success : DbResultStatus.KeyAlreadyExists;
    }

    public DbResultStatus Read(string userId, string key, out string value)
    {
        value = null;
        var validationResult = ValidateParameters(userId, key);
        if (validationResult != DbResultStatus.Success)
            return validationResult;
            
        string fullKey = BuildKey(userId, key);
        value = _db.StringGet(fullKey);
        return value != null ? DbResultStatus.Success : DbResultStatus.KeyNotFound;
    }

    public DbResultStatus Update(string userId, string key, string value)
    {
        string fullKey = BuildKey(userId, key);
        if (!_db.KeyExists(fullKey))
        {
            return DbResultStatus.KeyNotFound;
        }
        _db.StringSet(fullKey, value);
        return DbResultStatus.Success;
    }

    public DbResultStatus Delete(string userId, string key)
    {
        string fullKey = BuildKey(userId, key);
        bool result = _db.KeyDelete(fullKey);
        return result ? DbResultStatus.Success : DbResultStatus.KeyNotFound;
    }

    public DbResultStatus GetAll(string userId, out IDictionary<string, string> allData)
    {
        allData = new Dictionary<string, string>();
        var server = _db.Multiplexer.GetServer(_db.Multiplexer.GetEndPoints().First());
        var prefix = $"{userId}:";
        foreach (var key in server.Keys(pattern: $"{prefix}*"))
        {
            var value = _db.StringGet(key);
            // Remove prefix for user-friendliness
            allData[key.ToString().Substring(prefix.Length)] = value;
        }
        return DbResultStatus.Success;
    }
}
