using System.Threading.Tasks;

public interface IFileProvider
{
    void CreateSavesFolder(string directoryPath);
    void DeleteFile(string filePath);
    Task<string> ReadFileAsync(string filePath);
    Task WriteFileAsync(string filePath, string text);
    void Cancel();
}