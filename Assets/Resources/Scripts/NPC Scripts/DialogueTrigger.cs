using UnityEngine;

public class DialogueTrigger : MonoBehaviour
{
    [Header("UI Reference")]
    public DialogueUI dialogueUI;

    [Header("Fallback Dialogue (if no quest giver or DB found)")]
    [TextArea(3, 5)]
    public string[] defaultDialogueLines = new string[]
    {
        "Hey there, welcome to the ranch!",
        "Something strange has been happening around here...",
        "The animals don’t seem... normal.",
        "Could you help me fix the fence?"
    };

    private bool playerInRange = false;
    private QuestGiver questGiver;

    void Start()
    {
        questGiver = GetComponent<QuestGiver>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            InteractUI.Instance.ShowPrompt("Press E to talk to the boss.");
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            InteractUI.Instance.HidePrompt();
        }
    }

    void Update()
    {
        if (playerInRange && Input.GetKeyDown(KeyCode.E))
        {
            StartDialogue();
        }
    }

    void StartDialogue()
    {
        InteractUI.Instance.HidePrompt();

        string debugNext = questGiver != null ? questGiver.GetNextQuestID() : "NO QUESTGIVER";
        Debug.Log($"[DialogueTrigger] Next quest: {debugNext}");

        // NO QUESTGIVER (fallback)
        if (questGiver == null || questGiver.dialogueDatabase == null)
        {
            dialogueUI.gameObject.SetActive(true);
            dialogueUI.ShowDialogue(defaultDialogueLines);
            return;
        }

        // QUEST COMPLETED — NOT YET REWARDED
        foreach (string id in questGiver.parentQuestIDs)
        {
            Quest q = QuestManager.Instance.quests.Find(x => x.questID == id);
            if (q == null) continue;

            if (q.state == QuestState.Completed && !questGiver.HasBeenRewarded(id))
            {
                questGiver.TryGiveQuestReward();

                var done = questGiver.dialogueDatabase.GetDialogue(id);

                dialogueUI.gameObject.SetActive(true);

                if (done != null && done.completeLines.Length > 0)
                {
                    dialogueUI.ShowDialogue(done.completeLines);
                }
                else
                {
                    dialogueUI.ShowDialogue(new string[]
                    {
                        "Thanks for taking care of that! Here's your reward.",
                        "Come see me again when you're ready for another job."
                    });
                }
                return;
            }
        }

        // QUEST ACTIVE
        string active = questGiver.GetActiveQuestID();
        if (!string.IsNullOrEmpty(active))
        {
            var d = questGiver.dialogueDatabase.GetDialogue(active);

            dialogueUI.gameObject.SetActive(true);

            if (d != null && d.activeLines.Length > 0)
            {
                dialogueUI.ShowDialogue(d.activeLines);
            }
            else
            {
                dialogueUI.ShowDialogue(new string[]
                {
                    "You're still working on that task.",
                    "Finish that up and come back to me after!"
                });
            }
            return;
        }

        // OFFER NEXT QUEST
        string next = questGiver.GetNextQuestID();
        if (!string.IsNullOrEmpty(next))
        {
            var nd = questGiver.dialogueDatabase.GetDialogue(next);
            string[] offer = (nd != null && nd.introLines.Length > 0)
                ? nd.introLines
                : new string[]
                {
                    "I’ve got another job for you.",
                    "Interested?"
                };

            dialogueUI.gameObject.SetActive(true);

            dialogueUI.ShowDialogue(
                offer,
                () => OnAccept(next),
                () => OnDecline()
            );
            return;
        }

        // ALL QUESTS COMPLETED
        if (questGiver.AllQuestsComplete())
        {
            var final = questGiver.dialogueDatabase.allQuestsCompleteLines;

            dialogueUI.gameObject.SetActive(true);

            if (final != null && final.Length > 0)
            {
                dialogueUI.ShowDialogue(final);
            }
            else
            {
                dialogueUI.ShowDialogue(new string[]
                {
                    "You’ve done a fine job out there!",
                    "The ranch is back in shape, thanks to you."
                });
            }
        }
    }
    // ACCEPT QUEST
    void OnAccept(string questID)
    {
        Debug.Log($"[DialogueTrigger] Accepted: {questID}");

        if (questGiver != null)
            questGiver.GiveNextParentQuest();

        var d = questGiver.dialogueDatabase.GetDialogue(questID);

        dialogueUI.gameObject.SetActive(true);

        if (d != null && d.introLines.Length > 0)
        {
            dialogueUI.ShowDialogue(new string[]
            {
                "Glad to have your help. Watch your step out there."
            });
        }
        else
        {
            dialogueUI.ShowDialogue(new string[]
            {
                "Thanks for the help! The ranch could use a hand."
            });
        }
    }
    // DECLINE QUEST
    void OnDecline()
    {
        dialogueUI.gameObject.SetActive(true);
        dialogueUI.ShowDialogue(new string[]
        {
            "Ah, maybe next time. Take care out there!"
        });
    }
}
