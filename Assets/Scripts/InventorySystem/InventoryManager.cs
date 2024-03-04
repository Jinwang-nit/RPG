using Cinemachine;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

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

    [Header("相机")]
    public CinemachineFreeLook freeLookCamera;

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
    }

    private void itemRaycast(bool hasClickend = false)
    {
        Ray ray = new Ray(rayOrigin.position, player.Model.transform.forward);
        RaycastHit hit;
        Debug.DrawRay(ray.origin, ray.direction, Color.red);
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
                        itemHoverText.enabled = true;
                        itemHoverText.text = newItem.name;
                    }
                }
            }
        }
        else
        {
            itemHoverText.enabled = false;
        }
    }

    private void AddItemToInventory(Item item)
    {
        int leftoverQuantity = item.currentQuantity;
        Slot openSlot = null;
        for (int i = 0; i < InventorySlots.Count; i++)
        {
            Item  heldItem = InventorySlots[i].getItem();
            if (heldItem != null && heldItem.name == item.name ) // 背包中已经有了这样物品
            {
                int freeSpaceSlot = heldItem.maxQuantity - heldItem.currentQuantity;
                if (freeSpaceSlot >= leftoverQuantity) // 余量足够就直接全放进去
                {
                    heldItem.currentQuantity += leftoverQuantity;
                    Destroy(item.gameObject);
                    return;
                }
                else // 不够就放满为止
                {
                    heldItem.currentQuantity = heldItem.maxQuantity;
                    leftoverQuantity -= freeSpaceSlot;
                }
            }
            else if (heldItem == null) // 背包中没有这样物品或者是一个格子塞不下，就新开一个格子
            {
                if (!openSlot) openSlot = InventorySlots[i];
            }
        }

        if (leftoverQuantity > 0 && openSlot)
        {
            openSlot.setSlot(item);
            item.currentQuantity = Mathf.Min(item.maxQuantity, leftoverQuantity);
            item.gameObject.SetActive(false);
        }
        else
        {
            item.currentQuantity = leftoverQuantity;
        }
    }

    private void toggleInventory(bool enable)
    {
        inventory.SetActive(enable);
        Cursor.lockState = enable ? CursorLockMode.None : CursorLockMode.Locked;
        Cursor.visible = enable;
        freeLookCamera.enabled = !enable;
    }
}
