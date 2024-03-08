using Cinemachine;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class InventoryManager : MonoBehaviour
{
    public PlayerController player;

    [Header("物品属性")]
    public GameObject inventory;
    private List<Slot> allInventorySlots = new List<Slot>();
    public List<Slot> inventorySlots = new List<Slot>();
    public List<Slot> hotbarsSlots = new List<Slot>();
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

    [Header("可装备物品")]
    public List<GameObject> equippableItems = new List<GameObject>();

    [Header("数据存储")]
    public List<GameObject> allItemPrefabs = new List<GameObject>();
    public string saveFileName = "inventoryData.json";

    private bool hasInventory = false;

    private void Start()
    {
        toggleInventory(hasInventory);

        allInventorySlots.AddRange(hotbarsSlots);
        allInventorySlots.AddRange(inventorySlots);

        foreach (Slot slot in allInventorySlots)
        {
            slot.InitSlot();
        }

        LoadInventory();
    }

    private void OnApplicationQuit() // 每次退出的时候都会保存整个背包，而不是谁改变就保存谁
    {
        SaveInventory();
    }

    private void Update()
    {
        // 按F拾取物品
        itemRaycast(Input.GetKeyDown(KeyCode.F));

        // 按B打开关闭背包
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

        // 移动背包中的物体
        if (hasInventory && Input.GetMouseButtonDown(0))
        {
            dragInventoryIcon();
        }
        else if (currentDragIndex != -1 && Input.GetMouseButtonUp(0) || currentDragIndex != -1 && !hasInventory)
        {
            dropInventoryIcon();
        }

        dragIconImage.transform.position = Input.mousePosition;

        // 丢弃物品
        if (Input.GetKeyDown(KeyCode.Q))
        {
            DropItem();
        }

        // 切换手中的物品
        for (int i = 0; i < hotbarsSlots.Count; i++)
        {
            if (Input.GetKeyDown((i + 1).ToString()))
            {
                EnableHotbarItem(i);
            }
        }
    }

    private void DropItem()
    {
        for (int i = 0; i < allInventorySlots.Count; i++)
        {
            Slot curSlot = allInventorySlots[i];
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

    private void AddItemToInventory(Item item, int overrideIndex = -1)
    {
        if (overrideIndex != -1)
        {
            allInventorySlots[overrideIndex].SetSlot(item);
            item.gameObject.SetActive(false);
            allInventorySlots[overrideIndex].Updata();
            return;
        }


        int leftoverQuantity = item.currentQuantity;
        for (int i = 0; i < allInventorySlots.Count; i++)
        {
            Item heldItem = allInventorySlots[i].GetItem();
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

                allInventorySlots[i].Updata(); // 更新一下数据
                break;
            }
            else if (heldItem == null) // 背包中没有这样物品就新开一个格子
            {
                item.currentQuantity = Mathf.Min(item.maxQuantity, leftoverQuantity);
                allInventorySlots[i].SetSlot(item);
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
        for (int i = 0; i < allInventorySlots.Count; i++)
        {
            Slot curSlot = allInventorySlots[i];

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
        for (int i = 0; i < allInventorySlots.Count; i++)
        {
            Slot curSlot = allInventorySlots[i];
            if (curSlot.hovered)
            {
                if (curSlot.hasItem()) // 不是空格子，就交换
                {
                    Item temp = curSlot.GetItem();
                    curSlot.SetSlot(currentDragItem);
                    allInventorySlots[currentDragIndex].SetSlot(temp);
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
        allInventorySlots[currentDragIndex].SetSlot(currentDragItem);
        resetDragVariables();
    }

    private void resetDragVariables()
    {
        currentDragIndex = -1;
        currentDragItem = null;
    }

    private void EnableHotbarItem(int hotbarIndex)
    {
       foreach (GameObject a in equippableItems)
        {
            a.SetActive(false);
        }

        Slot hotbarSlot = hotbarsSlots[hotbarIndex];
        if (hotbarSlot.hasItem())
        {
            if (hotbarSlot.GetItem().equippableItemIndex != -1) // 说明它是可装备的
            {
                equippableItems[hotbarSlot.GetItem().equippableItemIndex].SetActive(true);
            }
        }
    }

    private void LoadInventory()
    {
        if (File.Exists(saveFileName))
        {
            string jsonData = File.ReadAllText(saveFileName);
            InventoryData data = JsonUtility.FromJson<InventoryData>(jsonData); // 解析JSON
            ClearInventory();
            
            foreach (ItemData itemData in data.slotData)
            {
                GameObject itemprefab = allItemPrefabs.Find(prefab => prefab.GetComponent<Item>().name == itemData.itemName);
                if (itemprefab != null)
                {
                    GameObject createItem = Instantiate(itemprefab, dropLocation.position, Quaternion.identity);
                    Item item = createItem.GetComponent<Item>();

                    item.currentQuantity = itemData.quantity;
                    AddItemToInventory(item, itemData.slotIndex);
                }
            }
        }

        foreach (Slot slot in allInventorySlots)
        {
            slot.Updata();
        }
    }

    private void SaveInventory() // 将背包所有物品数据化，并将数据转成JSON存入本地
    {
        InventoryData data = new InventoryData();
        foreach (Slot slot in allInventorySlots)
        {
            Item item = slot.GetItem();
            if (item != null)
            {
                ItemData itemData = new ItemData(item.name, item.currentQuantity, allInventorySlots.IndexOf(slot));
                data.slotData.Add(itemData);
            }
        }

        string jsonData = JsonUtility.ToJson(data);
        File.WriteAllText(saveFileName, jsonData); // 覆盖之前数据
    }

    private void ClearInventory()
    {
        foreach (Slot slot in allInventorySlots)
        {
            slot.SetSlot(null);
        }
    }
}

[System.Serializable]
public class ItemData // JSON数据格式
{
    public string itemName;
    public int quantity;
    public int slotIndex;

    public ItemData(string itemName, int quantity, int slotIndex)
    {
        this.itemName = itemName;
        this.quantity = quantity;
        this.slotIndex = slotIndex;
    }
}

[System.Serializable]
public class InventoryData
{
    public List<ItemData> slotData = new List<ItemData>();
}
