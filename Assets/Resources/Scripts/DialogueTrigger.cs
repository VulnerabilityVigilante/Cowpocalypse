using UnityEngine;

public class DialogueTrigger : MonoBehaviour
{
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
    private QuestGiver questGiver; // Reference to attached quest giver

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

        // ✅ If there’s no quest giver or no DB, show default text
        if (questGiver == null || questGiver.dialogueDatabase == null)
        {
            DialogueUI.Instance.ShowDialogue(defaultDialogueLines);
            return;
        }

        // ✅ Step: Check if any parent quest was completed but not yet rewarded
        if (questGiver != null)
        {
            foreach (string id in questGiver.parentQuestIDs)
            {
                Quest quest = QuestManager.Instance.quests.Find(q => q.questID == id);
                if (quest != null && quest.state == QuestState.Completed)
                {
                    // Try to give the reward
                    questGiver.TryGiveQuestReward();

                    // After rewarding, show the completion dialogue
                    var completedDialogue = questGiver.dialogueDatabase?.GetDialogue(id);
                    if (completedDialogue != null && completedDialogue.completeLines.Length > 0)
                    {
                        DialogueUI.Instance.ShowDialogue(completedDialogue.completeLines);
                    }
                    else
                    {
                        DialogueUI.Instance.ShowDialogue(new string[]
                        {
                            "Thanks for taking care of that! Here's your reward.",
                            "Come see me again when you're ready for another job."
                        });
                    }

                    return; // stop here since we handled this interaction
                }
            }
        }


        // --- Handle quest dialogue logic ---
        if (questGiver.AllQuestsComplete())
        {
            // ✅ Check if there’s a special "all quests done" message in the dialogue database
            var finalDialogue = questGiver.dialogueDatabase.allQuestsCompleteLines;
            if (finalDialogue != null && finalDialogue.Length > 0)
            {
                DialogueUI.Instance.ShowDialogue(finalDialogue);
            }
            else
            {
                DialogueUI.Instance.ShowDialogue(new string[]
                {
                    "You’ve done a fine job out there!",
                    "The ranch is back in shape, thanks to you."
                });
            }

            return;
        }


        string activeQuest = questGiver.GetActiveQuestID();
        if (!string.IsNullOrEmpty(activeQuest))
        {
            var activeDialogue = questGiver.dialogueDatabase.GetDialogue(activeQuest);
            if (activeDialogue != null && activeDialogue.activeLines.Length > 0)
            {
                DialogueUI.Instance.ShowDialogue(activeDialogue.activeLines);
            }
            else
            {
                DialogueUI.Instance.ShowDialogue(new string[]
                {
                    "You're still working on that task.",
                    "Finish that up and come back to me after!"
                });
            }
            return;
        }

        string nextQuest = questGiver.GetNextQuestID();
        if (!string.IsNullOrEmpty(nextQuest))
        {
            var nextDialogue = questGiver.dialogueDatabase.GetDialogue(nextQuest);
            string[] introLines = (nextDialogue != null && nextDialogue.introLines.Length > 0)
                ? nextDialogue.introLines
                : defaultDialogueLines;

            DialogueUI.Instance.ShowDialogue(
                introLines,
                () => OnAccept(nextQuest),
                () => OnDecline()
            );
        }
    }

    void OnAccept(string questID)
    {
        if (questGiver != null)
        {
            questGiver.GiveNextParentQuest();
        }

        // ✅ Show an "acceptance" follow-up line (not completion text)
        var dialogueData = questGiver.dialogueDatabase?.GetDialogue(questID);
        if (dialogueData != null && dialogueData.introLines.Length > 0)
        {
            // You can either repeat the final intro line or show a short follow-up
            DialogueUI.Instance.ShowDialogue(new string[]
            {
                "Glad to have your help. You can start with the fence by the pasture — watch your step out there."
            });
        }
        else
        {
            DialogueUI.Instance.ShowDialogue(new string[]
            {
                "Thanks for the help! The ranch could use a hand."
            });
        }
    }


    void OnDecline()
    {
        DialogueUI.Instance.ShowDialogue(new string[]
        {
            "Ah, maybe next time. Take care out there!"
        });
    }
}
