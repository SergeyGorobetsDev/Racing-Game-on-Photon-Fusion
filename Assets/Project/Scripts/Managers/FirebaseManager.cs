using Firebase;
using Firebase.Extensions;
using FusionExamples.Utility;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Project.Scripts.Managers
{
    public class FirebaseManager : MonoBehaviour
    {
        private const string ImageDownloadPathFrom = "gs://photon-race-game.firebasestorage.app/lykan-hypersport-4978269_640.png";

        [SerializeField]
        private ImageDownloader imageDownloader;

        [SerializeField]
        private Texture2D texture;

        public static FirebaseManager Instance => Singleton<FirebaseManager>.Instance;

        private void Awake()
        {
            DontDestroyOnLoad(this);
            Initialize();
        }

        public void Initialize()
        {
            FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
            {
                var status = task.Result;
                if (status == DependencyStatus.Available)
                    Debug.Log("✅ Firebase успешно инициализирован!");
                else Debug.LogError("❌ Ошибка инициализации Firebase: " + status);
            });
        }

        public async Task<Texture2D> GetImageAsync()
        {
            imageDownloader = new ImageDownloader();
            return await imageDownloader.LoadImageWithCacheAsync(ImageDownloadPathFrom);
        }
    }
}
