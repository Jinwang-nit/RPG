using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Runtime.InteropServices;

public class DialogueManager : MonoBehaviour
{
    [SerializeField] private GameObject dialogueParent;
    [SerializeField] private TMP_Text dialogueText; // 当前对话框
    [SerializeField] private Button option1Button;
    [SerializeField] private Button option2Button;

    [SerializeField] private float typingSpeed = 0.05f; // 打字速度
    [SerializeField] private float turnSpeed = 2f; // 转向速度
    [SerializeField] private DialogueTrriger dialogueTrriger;

    private List<DialogueString> dialogueList;


    [Header("玩家配置")]
    [SerializeField] private PlayerController playerController;
    private ModelBase playerModel;
    private int currentDialogueIndex = 0;

    private void Start()
    {
        dialogueParent.SetActive(false);
        playerModel = playerController.Model;
    }

    public void DialogueStart(List<DialogueString> textToPrint, Transform NPC) //   开始对话
    {
        dialogueParent.SetActive(true);
        playerController.isDialog = true;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        StartCoroutine(TurnCameraToWardsNPC(NPC));

        dialogueList = textToPrint;
        currentDialogueIndex = 0;

        DisableButtons();

        StartCoroutine(PrintDialogue());
    }

    private void DisableButtons()
    {
        option1Button.interactable = false;
        option2Button.interactable = false;

        option1Button.GetComponentInChildren<TMP_Text>().text = "No Option";
        option2Button.GetComponentInChildren<TMP_Text>().text = "No Option";
    }

    private IEnumerator TurnCameraToWardsNPC(Transform NPC)
    {
        Quaternion startRotion =  playerModel.transform.rotation;
        Quaternion targetRotion = Quaternion.LookRotation(NPC.position - playerModel.transform.position);

        float elapsedTime = 0;
        while (elapsedTime < 1f)
        {
            playerModel.transform.rotation = Quaternion.Slerp(startRotion, targetRotion, elapsedTime);
            elapsedTime += Time.deltaTime * turnSpeed;
            yield return null;
        }

        playerModel.transform.rotation = targetRotion;
    }

    private bool optionSelected = false;
    private IEnumerator PrintDialogue()
    {
        while(currentDialogueIndex < dialogueList.Count)
        {
            DialogueString line = dialogueList[currentDialogueIndex];
            line.startDialogueEvent?.Invoke();
            if (line.isQuestion)
            {
                yield return StartCoroutine(TypeText(line.text));
                option1Button.interactable = true;
                option2Button.interactable = true;

                option1Button.GetComponentInChildren<TMP_Text>().text = line.answerOption1;
                option2Button.GetComponentInChildren<TMP_Text>().text = line.answerOption2;

                option1Button.onClick.AddListener(() => HandleOptionSelected(line.option1IndexJump));
                option2Button.onClick.AddListener(() => HandleOptionSelected(line.option2IndexJump));

                yield return new WaitUntil(() => optionSelected);
            }
            else
            {
                yield return StartCoroutine(TypeText(line.text));
            }
            line.endDialogueEvent?.Invoke();
        }
        DialogueStop();
    }

    private void HandleOptionSelected(int indexJump)
    {
        optionSelected = true;
        DisableButtons();
        currentDialogueIndex = indexJump;
    }

    private IEnumerator TypeText(string text)
    {
        dialogueText.text = "";
        foreach(char letter in text.ToCharArray())
        {
            dialogueText.text += letter;
            yield return new WaitForSeconds(typingSpeed);
        }

        if (!dialogueList[currentDialogueIndex].isQuestion)
        {
            yield return new WaitUntil(() => Input.GetMouseButtonDown(0));
        }

        if (dialogueList[currentDialogueIndex].isEnd)
        {
            DialogueStop();
        }

        currentDialogueIndex++;
    }

    private void DialogueStop()
    {
        StopAllCoroutines();
        dialogueText.text = "";
        dialogueParent.SetActive(false);
        playerController.isDialog = false;
        dialogueTrriger.hasDialogue = false;
        optionSelected = false;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}
