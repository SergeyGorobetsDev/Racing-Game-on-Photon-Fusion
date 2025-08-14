using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public sealed class FileProvider : IFileProvider
{
    private CancellationTokenSource cancellationToken;

    public FileProvider() =>
        this.cancellationToken = new();

    public async Task<string> ReadFileAsync(string filePath)
    {
        this.cancellationToken = new();
        if (!File.Exists(filePath))
            File.Create(filePath).Dispose();

        using FileStream sourceStream = new(filePath, FileMode.Open, FileAccess.Read, FileShare.Read, bufferSize: 4096, useAsync: true);
        using StreamReader reader = new(sourceStream);
        StringBuilder sb = new();

        while (!reader.EndOfStream && !cancellationToken.IsCancellationRequested)
        {
            string line = await reader.ReadLineAsync();
            sb.AppendLine(line);
        }
        this.cancellationToken.Cancel();
        this.cancellationToken = default;
        return sb.ToString();

    }

    public async Task WriteFileAsync(string filePath, string text)
    {
        this.cancellationToken = new();

        if (!File.Exists(filePath))
            File.Create(filePath).Dispose();

        using FileStream destinationStream = new(filePath, FileMode.Open, FileAccess.Write, FileShare.Write, bufferSize: 4096, useAsync: true);
        using StreamWriter writer = new(destinationStream);
        await writer.WriteLineAsync(text);
        this.cancellationToken.Cancel();
        this.cancellationToken = default;
    }

    public void DeleteFile(string filePath)
    {
        if (File.Exists(filePath))
            File.Delete(filePath);
        else
        {
#if UNITY_EDITOR
            Debug.Log($"Can't delete file by path : {filePath}");
#endif
        }
    }

    public void CreateSavesFolder(string directoryPath)
    {
        if (!Directory.Exists(directoryPath))
            Directory.CreateDirectory(directoryPath);
    }

    public void Cancel()
    {
        if (this.cancellationToken is null) return;
        this.cancellationToken.Cancel();
        this.cancellationToken.Dispose();
    }
}