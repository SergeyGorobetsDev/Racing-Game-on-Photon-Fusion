using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SlotUI : Selectable, IPointerClickHandler
{
    [SerializeField]
    private int slotIndex = 0;
    [SerializeField]
    private Image icon;

    public event Action<int> SlotSelected;

    protected override void Awake()
    {
        base.Awake();
        icon ??= GetComponentInChildren<Image>();
    }

    protected override void OnEnable()
    {
        base.OnEnable();
    }

    public void SetData(Sprite icon) =>
        this.icon.sprite = icon;

    public void ResetData() =>
        this.icon.sprite = null;

    public void ChangeVisibleState(bool isVisible) =>
        this.gameObject.SetActive(isVisible);

    public override void OnSelect(BaseEventData eventData)
    {
        base.OnSelect(eventData);
        SlotSelected?.Invoke(slotIndex);
    }

    public void OnPointerClick(PointerEventData eventData) =>
        SlotSelected?.Invoke(slotIndex);
}