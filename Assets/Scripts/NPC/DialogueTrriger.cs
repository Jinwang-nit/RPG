using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class DialogueTrriger : MonoBehaviour
{
    [SerializeField] private List<DialogueString> dialogueStrings = new List<DialogueString>();
    [SerializeField] private Transform NPCTransform;
    [SerializeField] private TMP_Text notice;
    [SerializeField] private DialogueManager dialogueManager;

    public bool hasDialogue = false;
    private bool inTrigger = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !hasDialogue)
        {
            notice.enabled = true;
            inTrigger = true;
        }
    }

    private void Update()
    {
        if ( inTrigger && !hasDialogue && Input.GetKeyDown(KeyCode.F))
        {
            notice.enabled = false;
            dialogueManager.DialogueStart(dialogueStrings, NPCTransform);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") && !hasDialogue)
        {
            notice.enabled = false;
            inTrigger = false;
        }
    }
}

[System.Serializable]
public class DialogueString
{
    public string text; // NPC˵�Ļ�
    public bool isEnd; // ��ǰ�Ի��Ƿ����

    [Header("�Ի���֧")]
    public bool isQuestion;
    public string answerOption1;
    public string answerOption2;
    public int option1IndexJump;
    public int option2IndexJump;

    [Header("�����¼�")]
    public UnityEvent startDialogueEvent;
    public UnityEvent endDialogueEvent;
}
