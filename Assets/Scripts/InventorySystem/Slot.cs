using System.Xml.Serialization;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Slot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("配置")]
    private TMP_Text itemQuantityText;
    public bool hovered;
    private Item heldItem; // 格子存的物品
    private Color opaque = new Color(1, 1, 1, 1);
    private Color transparent = new Color(1, 1, 1, 0);
    private Image thisSlotImage;

    public void InitSlot()
    {
        itemQuantityText = transform.GetChild(0).GetComponent<TMP_Text>();
        thisSlotImage = GetComponent<Image>();
        thisSlotImage.sprite = null;
        thisSlotImage.color = transparent;
        SetSlot(null);
    }

    public void SetSlot(Item item)
    {
        heldItem = item;
        if (item != null)
        {
            thisSlotImage.sprite = item.icon;
            thisSlotImage.color = opaque;
        }
        else
        {
            thisSlotImage.sprite = null;
            thisSlotImage.color = transparent;
        }
        Updata();
    }

    public Item GetItem()
    {
        return heldItem;
    }

    public bool hasItem()
    {
        return heldItem != null;
    }

    public void Updata()
    {
        if (heldItem != null)
        {
            itemQuantityText.text = heldItem.currentQuantity.ToString();
        }
        else
        {
            itemQuantityText.text = ""; 
        }
    }

    public void OnPointerEnter(PointerEventData eventData) // 鼠标进入函数API
    {
        hovered = true;
    }

    public void OnPointerExit(PointerEventData eventData) // 鼠标移出函数API
    {
        hovered= false;
    }
}
