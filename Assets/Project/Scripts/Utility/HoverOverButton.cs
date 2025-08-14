using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class HoverOverButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [SerializeField]
    private float scaleModifier = 0.1f;
    private Vector3 defaultScale;

    [HideInInspector]
    public bool isHovered = false;

    private Button button;

    private void Awake()
    {
        button = GetComponent<Button>();
        SetDefaultScale();
    }

    public void SetDefaultScale() =>
        defaultScale = transform.localScale;

    public Vector3 GetHoveredScale() =>
        defaultScale + new Vector3(scaleModifier, scaleModifier, scaleModifier);

    public void OnPointerClick(PointerEventData eventData) =>
        transform.localScale = defaultScale;

    public void Select()
    {
        if (button.IsInteractable())
        {
            isHovered = true;
            transform.localScale = GetHoveredScale();
        }
    }

    public void UnSelect()
    {
        if (button.IsInteractable())
        {
            isHovered = false;
            transform.localScale = defaultScale;
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (button.IsInteractable())
        {
            isHovered = true;
            transform.localScale = GetHoveredScale();
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isHovered = false;
        transform.localScale = defaultScale;
    }

    private void OnDisable() =>
        isHovered = false;
}
