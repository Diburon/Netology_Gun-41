namespace FinalTask.Services;

public class FileSystemSaveLoadService : ISaveLoadService<string>
{
    private readonly string _path;

    public FileSystemSaveLoadService(string path)
    {
        _path = path ?? throw new ArgumentNullException(nameof(path));
    }

    public void SaveData(string data, string id)
    {
        if (string.IsNullOrWhiteSpace(id))
            throw new ArgumentException("Identifier cannot be null or empty.", nameof(id));

        if (!Directory.Exists(_path))
            Directory.CreateDirectory(_path);

        string filePath = Path.Combine(_path, $"{id}.txt");
        File.WriteAllText(filePath, data ?? string.Empty);
    }

    public string LoadData(string id)
    {
        if (string.IsNullOrWhiteSpace(id))
            throw new ArgumentException("Identifier cannot be null or empty.", nameof(id));

        if (!Directory.Exists(_path))
            Directory.CreateDirectory(_path);

        string filePath = Path.Combine(_path, $"{id}.txt");

        if (!File.Exists(filePath))
            throw new FileNotFoundException($"File not found: {id}", filePath);

        return File.ReadAllText(filePath);
    }
}