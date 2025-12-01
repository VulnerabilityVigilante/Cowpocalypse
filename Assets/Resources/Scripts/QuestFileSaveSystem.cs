using UnityEngine;
using System.IO;
using System.Text;

public static class QuestFileSaveSystem
{
    private static string Folder =>
        Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments), "CowpocalypseSave");

    private static string SavePath =>
        Path.Combine(Folder, "save.txt");

    public static void SaveAll(QuestGiver questGiver)
    {
        // Ensure directory exists
        if (!Directory.Exists(Folder))
            Directory.CreateDirectory(Folder);

        StringBuilder sb = new StringBuilder();

        // --- Save Quest States ---
        foreach (Quest q in QuestManager.Instance.quests)
        {
            sb.AppendLine($"{q.questID}:{(int)q.state}");
        }

        sb.AppendLine();

        // --- Save Rewarded Quests ---
        foreach (string id in questGiver.rewardedQuests)
        {
            sb.AppendLine($"Rewarded:{id}");
        }

        sb.AppendLine();

        // --- Save Coins ---
        sb.AppendLine($"Coins:{CurrencyManager.Instance.GetCoins()}");

        File.WriteAllText(SavePath, sb.ToString());
        Debug.Log($"💾 Saved to: {SavePath}");
    }


    public static void LoadAll()
    {
        if (!File.Exists(SavePath))
        {
            Debug.Log("📄 No save file found in Documents — starting fresh.");
            return;
        }

        QuestGiver questGiver = Object.FindObjectOfType<QuestGiver>();
        if (questGiver == null)
        {
            Debug.LogError("QuestGiver not found when loading save file.");
            return;
        }

        string[] lines = File.ReadAllLines(SavePath);

        foreach (string line in lines)
        {
            if (string.IsNullOrWhiteSpace(line))
                continue;

            // Quest States
            if (line.Contains(":") && !line.StartsWith("Coins") && !line.StartsWith("Rewarded"))
            {
                string[] parts = line.Split(':');
                string questID = parts[0];
                int state = int.Parse(parts[1]);

                Quest quest = QuestManager.Instance.quests.Find(q => q.questID == questID);
                if (quest != null)
                    quest.state = (QuestState)state;
            }

            // Rewarded Flags
            if (line.StartsWith("Rewarded"))
            {
                string id = line.Split(':')[1];
                if (!questGiver.rewardedQuests.Contains(id))
                    questGiver.rewardedQuests.Add(id);
            }

            // Coins
            if (line.StartsWith("Coins"))
            {
                int amount = int.Parse(line.Split(':')[1]);
                CurrencyManager.Instance.SetCoinsFromLoad(amount);
            }
        }

        Debug.Log($"📥 Loaded save from: {SavePath}");
    }
}
