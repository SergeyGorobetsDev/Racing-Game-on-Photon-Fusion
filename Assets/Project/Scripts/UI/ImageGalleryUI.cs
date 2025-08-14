using Assets.Project.Scripts.Managers;
using UnityEngine;
using UnityEngine.UI;

public class ImageGalleryUI : MonoBehaviour
{
    [SerializeField]
    private Image previewImage;

    [SerializeField]
    private GameObject loaderContainer;

    [SerializeField]
    private Color hidenColor;

    [SerializeField]
    private Color visibleColor;

    private void Awake()
    {
        loaderContainer.SetActive(false);
    }

    private async void Start()
    {
        loaderContainer.SetActive(true);
        previewImage.color = hidenColor;

        var texture = await FirebaseManager.Instance.GetImageAsync();

        if (texture != null)
        {
            Rect rect = new Rect(0, 0, texture.width, texture.height);
            Vector2 pivot = new Vector2(0.5f, 0.5f);
            Sprite sprite = Sprite.Create(texture, rect, pivot);
            previewImage.sprite = sprite;

            RectTransform rt = previewImage.rectTransform;
            rt.sizeDelta = new Vector2(texture.width, texture.height);
            previewImage.color = visibleColor;
        }

        loaderContainer.SetActive(false);
    }
}
