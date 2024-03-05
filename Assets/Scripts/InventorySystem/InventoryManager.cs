using Cinemachine;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventoryManager : MonoBehaviour
{
    public PlayerController player;

    [Header("物品属性")]
    public GameObject inventory;
    public List<Slot> InventorySlots = new List<Slot>();
    public TMP_Text itemHoverText;

    [Header("射线")]
    public Transform rayOrigin;
    public float raycastDistance = 1f;
    public LayerMask itemMask;
    public Transform dropLocation;

    [Header("相机")]
    public CinemachineFreeLook freeLookCamera;

    [Header("移动背包物品")]
    public Image dragIconImage;
    private Item currentDragItem;
    private int currentDragIndex = -1;

    private bool hasInventory = false;

    private void Start()
    {
        foreach (Slot slot in InventorySlots)
        {
            slot.InitSlot();
        }
    }

    private void Update()
    {
        itemRaycast(Input.GetKeyDown(KeyCode.F)); // 按f拾取
        if (!hasInventory && Input.GetKeyDown(KeyCode.B))
        {
            hasInventory = true;
            toggleInventory(hasInventory);
        }
        else if (hasInventory && Input.GetKeyDown(KeyCode.B))
        {
            hasInventory = false ; 
            toggleInventory(hasInventory);
        }


        if (hasInventory && Input.GetMouseButtonDown(0))
        {
            dragInventoryIcon();
        }
        else if (currentDragIndex != -1 && Input.GetMouseButtonUp(0) || currentDragIndex != -1 && !hasInventory)
        {
            dropInventoryIcon();
        }

        dragIconImage.transform.position = Input.mousePosition;

        if (Input.GetKeyDown(KeyCode.Q))
        {
            DropItem();
        }
    }

    private void DropItem()
    {
        for (int i = 0; i < InventorySlots.Count; i++)
        {
            Slot curSlot = InventorySlots[i];
            if (curSlot.hasItem() && curSlot.hovered)
            {
                curSlot.GetItem().gameObject.SetActive(true);
                curSlot.GetItem().gameObject.transform.position = new Vector3(dropLocation.position.x, curSlot.GetItem().gameObject.transform.position.y, dropLocation.position.z);
                curSlot.SetSlot(null);
                break;
            }
        }
    }

    private void itemRaycast(bool hasClickend = false)
    {
        Ray ray = new Ray(rayOrigin.position, player.Model.transform.forward);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, raycastDistance, itemMask))
        {
            if (hit.collider != null)
            {

                if (hasClickend) // 拾取到背包
                {
                    Item newItem = hit.collider.GetComponent<Item>();
                    if (newItem)
                    {
                        AddItemToInventory(newItem);
                    }
                }
                else // 显示拾取UI
                {
                    Item newItem = hit.collider.GetComponent<Item>();
                    if (newItem)
                    {
                        itemHoverText.text = newItem.name;
                    }
                }
            }
        }
        else
        {
            itemHoverText.text = "";
        }
    }

    private void AddItemToInventory(Item item)
    {
        int leftoverQuantity = item.currentQuantity;
        for (int i = 0; i < InventorySlots.Count; i++)
        {
            Item heldItem = InventorySlots[i].GetItem();
            if (heldItem != null && heldItem.name == item.name ) // 背包中已经有了这样物品
            {
                int freeSpaceSlot = heldItem.maxQuantity - heldItem.currentQuantity;

                if (freeSpaceSlot >= leftoverQuantity) // 余量足够就直接全放进去
                {
                    heldItem.currentQuantity += leftoverQuantity;
                    Destroy(item.gameObject);
                }
                else // 不够就放满为止
                {
                    heldItem.currentQuantity = heldItem.maxQuantity;
                    leftoverQuantity -= freeSpaceSlot;
                }

                InventorySlots[i].Updata(); // 更新一下数据
                break;
            }
            else if (heldItem == null) // 背包中没有这样物品就新开一个格子
            {
                item.currentQuantity = Mathf.Min(item.maxQuantity, leftoverQuantity);
                InventorySlots[i].SetSlot(item);
                leftoverQuantity -= Mathf.Min(item.maxQuantity, leftoverQuantity);
                break;
            }
        }
        if (leftoverQuantity <= 0) item.gameObject.SetActive(false);
        else item.currentQuantity = leftoverQuantity;
    }

    private void toggleInventory(bool enable)
    {
        inventory.SetActive(enable);
        Cursor.lockState = enable ? CursorLockMode.None : CursorLockMode.Locked;
        Cursor.visible = enable;
        freeLookCamera.enabled = !enable;
    }

    private void dragInventoryIcon()
    {
        for (int i = 0; i < InventorySlots.Count; i++)
        {
            Slot curSlot = InventorySlots[i];

            if ( curSlot.hovered && curSlot.hasItem())
            {
                currentDragIndex = i;
                currentDragItem = curSlot.GetItem();
                dragIconImage.sprite = currentDragItem.icon;
                dragIconImage.color = new Color(1, 1, 1, 1);
                curSlot.SetSlot(null);
            }
        }
    }

    private void dropInventoryIcon()
    {
        dragIconImage.sprite = null;
        dragIconImage.color = new Color(1, 1, 1, 0);
        for (int i = 0; i < InventorySlots.Count; i++)
        {
            Slot curSlot = InventorySlots[i];
            if (curSlot.hovered)
            {
                if (curSlot.hasItem()) // 不是空格子，就交换
                {
                    Item temp = curSlot.GetItem();
                    curSlot.SetSlot(currentDragItem);
                    InventorySlots[currentDragIndex].SetSlot(temp);
                    resetDragVariables();
                    return;
                }
                else // 是空格子就直接放
                {
                    curSlot.SetSlot(currentDragItem);
                    resetDragVariables();
                    return;
                }
            }
        }

        // 没有选择任何格子,就取消操作
        InventorySlots[currentDragIndex].SetSlot(currentDragItem);
        resetDragVariables();
    }

    private void resetDragVariables()
    {
        currentDragIndex = -1;
        currentDragItem = null;
    }
}
