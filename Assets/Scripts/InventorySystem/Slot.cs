using System.Xml.Serialization;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Slot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public bool hovered;
    private Item heldItem;
    private Color opaque = new Color(1, 1, 1, 1);
    private Color transparent = new Color(1, 1, 1, 0);
    private Image thisSlotImage;

    public void InitSlot()
    {
        thisSlotImage = GetComponent<Image>();
        thisSlotImage.sprite = null;
        thisSlotImage.color = transparent;
        setSlot(null);
    }

    public void setSlot(Item item)
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
    }

    public Item getItem()
    {
        return heldItem;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        hovered = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        hovered= false;
    }
}
