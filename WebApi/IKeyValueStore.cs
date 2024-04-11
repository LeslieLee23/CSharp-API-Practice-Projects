public interface IKeyValueStore
{
    bool Create(string key, string value);
    string Read(string key);
    bool Update(string key, string value);
    bool Delete(string key);
    IDictionary<string, string> GetAll();

}