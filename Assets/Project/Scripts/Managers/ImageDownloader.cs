using Firebase.Storage;
using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace Assets.Project.Scripts.Managers
{
    public class ImageDownloader
    {
        public async Task<Texture2D> LoadImageWithCacheAsync(string firebasePath)
        {
            try
            {
                Uri url = await FirebaseStorage.DefaultInstance.GetReferenceFromUrl(firebasePath).GetDownloadUrlAsync();
                string cachePath = GetHashedCachePath(url.ToString());

                Texture2D texture;

                if (File.Exists(cachePath))
                {
                    byte[] data = await File.ReadAllBytesAsync(cachePath);
                    texture = new Texture2D(2, 2);
                    texture.LoadImage(data);
                    return texture;
                }
                else
                {
                    texture = await DownloadTextureAsync(url.ToString());

                    if (texture != null)
                    {
                        byte[] pngData = texture.EncodeToPNG();
                        await File.WriteAllBytesAsync(cachePath, pngData);
                    }

                    return texture;
                }
            }
            catch (Exception ex)
            {
                Debug.LogError("🔥 Ошибка при загрузке изображения: " + ex.Message);
            }

            return null;
        }

        private async Task<Texture2D> DownloadTextureAsync(string url)
        {
            using UnityWebRequest uwr = UnityWebRequestTexture.GetTexture(url);
            var operation = uwr.SendWebRequest();

            while (!operation.isDone)
                await Task.Yield();

            if (uwr.result == UnityWebRequest.Result.Success)
            {
                return DownloadHandlerTexture.GetContent(uwr);
            }
            else
            {
                Debug.LogError("⚠️ Ошибка при скачивании изображения: " + uwr.error);
                return null;
            }
        }

        private string GetHashedCachePath(string url)
        {
            using var sha = SHA256.Create();
            byte[] hash = sha.ComputeHash(Encoding.UTF8.GetBytes(url));
            string hashName = BitConverter.ToString(hash).Replace("-", "").ToLower();
            return Path.Combine(Application.persistentDataPath, hashName + ".png");
        }
    }
}
