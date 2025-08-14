using FusionExamples.Utility;
using Newtonsoft.Json;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;

public interface ISaveManager
{
    Task SaveAsync();
    Task LoadAsync();
}

public sealed class SaveManager : MonoBehaviour, ISaveManager
{
    public const string SavesFileName = "UserData";
    public const string SavesFileFormat = ".json";
    private readonly IFileProvider fileProvider = new FileProvider();

    [SerializeField]
    private UserData activeUserData;

    public static SaveManager Instance => Singleton<SaveManager>.Instance;

    private async void Awake()
    {
        DontDestroyOnLoad(gameObject);
        await LoadAsync();
    }

    private async void OnDestroy() =>
        await SaveAsync();

    public async Task LoadAsync()
    {
        string toLoad = await fileProvider.ReadFileAsync(GetPlatformPath());
        if (string.IsNullOrEmpty(toLoad))
        {
            activeUserData = new();
            activeUserData.CarsModificationData = new();
            for (int i = 0; i < ResourceManager.Instance.CarConfigs.Length; i++)
                activeUserData.CarsModificationData.Add(new());
            fileProvider.Cancel();
            return;
        }

        activeUserData = JsonConvert.DeserializeObject<UserData>(toLoad);

        Debug.Log($"User Data Loaded {activeUserData.Username}");
    }

    public async Task SaveAsync()
    {
        string toSave = JsonConvert.SerializeObject(activeUserData);
        Debug.Log($"User Data Saved {activeUserData.Username}");
        await fileProvider.WriteFileAsync(GetPlatformPath(), toSave);
    }

    public UserData GetUserData() => activeUserData;

    private string GetPlatformPath()
    {
#if UNITY_EDITOR
        return Path.Combine(Application.dataPath, SavesFileName + SavesFileFormat);
#elif UNITY_ANDROID
            return Path.Combine(Application.persistentDataPath, SavesFileName + SavesFileFormat);
#elif UNITY_STANDALONE_WIN || UNITY_STANDALONE
            return Path.Combine(Application.persistentDataPath, SavesFileName + SavesFileFormat);
#endif
    }
}