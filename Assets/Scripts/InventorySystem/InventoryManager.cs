using Cinemachine;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public PlayerController player;

    [Header("��Ʒ����")]
    public GameObject inventory;
    public List<Slot> InventorySlots = new List<Slot>();
    public TMP_Text itemHoverText;

    [Header("����")]
    public Transform rayOrigin;
    public float raycastDistance = 1f;
    public LayerMask itemMask;

    [Header("���")]
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
        itemRaycast(Input.GetKeyDown(KeyCode.F)); // ��fʰȡ
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

                if (hasClickend) // ʰȡ������
                {
                    Item newItem = hit.collider.GetComponent<Item>();
                    if (newItem)
                    {
                        AddItemToInventory(newItem);
                    }
                }
                else // ��ʾʰȡUI
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
            if (heldItem != null && heldItem.name == item.name ) // �������Ѿ�����������Ʒ
            {
                int freeSpaceSlot = heldItem.maxQuantity - heldItem.currentQuantity;
                if (freeSpaceSlot >= leftoverQuantity) // �����㹻��ֱ��ȫ�Ž�ȥ
                {
                    heldItem.currentQuantity += leftoverQuantity;
                    Destroy(item.gameObject);
                    return;
                }
                else // �����ͷ���Ϊֹ
                {
                    heldItem.currentQuantity = heldItem.maxQuantity;
                    leftoverQuantity -= freeSpaceSlot;
                }
            }
            else if (heldItem == null) // ������û��������Ʒ������һ�����������£����¿�һ������
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
