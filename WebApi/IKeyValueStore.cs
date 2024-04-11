using YamlDotNet.Core.Tokens;

public interface IKeyValueStore
{
    bool Create(string key, string value);
    string Read(string key);
    bool Update(string key, string value);
    bool Delete(string key);
    IDictionary<string, string> GetAll();
}

public class KeyValueStore : IKeyValueStore
{
    private readonly Dictionary<string, string> _store = new();
    public bool Create(string key, string value)
    {
        return _store.TryAdd(key, value);
    }

    public string Read(string key)
    {
        _store.TryGetValue(key, out var value);
        return value;
    }

    public bool Update(string key, string value)
    {
        if (_store.ContainsKey(key))
        {
             _store[key] = value;
             return true;
        }
        return false;
    }

    public bool Delete(string key)
    {
        return _store.Remove(key);
    }

    public IDictionary<string, string> GetAll()
    {
        return new Dictionary<string, string>(_store);
    }


}