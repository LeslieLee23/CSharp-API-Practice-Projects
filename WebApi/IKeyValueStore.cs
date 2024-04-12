
public interface IKeyValueStore
{
    DbResultStatus Create(string userId, string key, string value);
    DbResultStatus Read(string userId, string key, out string value);
    DbResultStatus Update(string userId, string key, string value);
    DbResultStatus Delete(string userId, string key);
    DbResultStatus GetAll(string userId, out IDictionary<string, string> allData);
}
