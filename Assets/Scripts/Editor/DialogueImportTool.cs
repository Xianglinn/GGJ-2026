using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class DialogueImportTool : EditorWindow
{
    private const string SAVE_PATH = "Assets/Resources/Data/Dialogues/";
    private const string PORTRAIT_PATH = "Resources/Sprites/Portraits/";

    [MenuItem("Tools/对话系统/Import From CSV")]
    public static void ImportFromCSV()
    {
        string filePath = EditorUtility.OpenFilePanel("Select Dialogue CSV", "", "csv");
        if (string.IsNullOrEmpty(filePath)) return;

        try
        {
            List<string> linesList = new List<string>();
            using (var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            using (var sr = new StreamReader(fs))
            {
                while (!sr.EndOfStream)
                {
                    linesList.Add(sr.ReadLine());
                }
            }
            string[] lines = linesList.ToArray();
            if (lines.Length <= 1)
            {
                Debug.LogError("[DialogueImportTool] CSV file is empty or only contains header.");
                return;
            }

            // Headers: ID, CharacterName, DialogueText, PortraitName, BGMName, BackgroundName
            Dictionary<int, List<DialogueLineData>> groupedDialogues = new Dictionary<int, List<DialogueLineData>>();

            for (int i = 1; i < lines.Length; i++)
            {
                string line = lines[i];
                if (string.IsNullOrWhiteSpace(line)) continue;

                string[] fields = ParseCSVLine(line);
                if (fields.Length < 6)
                {
                    Debug.LogWarning($"[DialogueImportTool] Skipping malformed line at index {i}: {line}");
                    continue;
                }

                if (!int.TryParse(fields[0], out int id))
                {
                    Debug.LogWarning($"[DialogueImportTool] Invalid ID at index {i}: {fields[0]}");
                    continue;
                }

                DialogueLineData data = new DialogueLineData
                {
                    CharacterName = fields[1],
                    Text = fields[2],
                    PortraitName = fields[3],
                    BGMName = fields[4],
                    BackgroundName = fields[5]
                };

                if (!groupedDialogues.ContainsKey(id))
                    groupedDialogues[id] = new List<DialogueLineData>();
                
                groupedDialogues[id].Add(data);
            }

            int count = 0;
            foreach (var kvp in groupedDialogues)
            {
                CreateOrUpdateDialogueAsset(kvp.Key, kvp.Value);
                count++;
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            EditorUtility.DisplayDialog("Import Successful", $"Imported {count} dialogue assets.", "OK");
        }
        catch (Exception e)
        {
            Debug.LogError($"[DialogueImportTool] Error importing CSV: {e.Message}");
        }
    }

    private static void CreateOrUpdateDialogueAsset(int id, List<DialogueLineData> lineDatas)
    {
        string assetName = $"Dialogue_{id}.asset";
        string fullPath = Path.Combine(SAVE_PATH, assetName);

        DialogueData asset = AssetDatabase.LoadAssetAtPath<DialogueData>(fullPath);
        bool isNew = false;

        if (asset == null)
        {
            asset = ScriptableObject.CreateInstance<DialogueData>();
            isNew = true;
        }

        asset.dialogueID = id.ToString();
        asset.lines.Clear();

        foreach (var data in lineDatas)
        {
            DialogueLine newLine = new DialogueLine
            {
                characterName = data.CharacterName,
                dialogueText = data.Text,
                bgmName = data.BGMName,
                backgroundName = data.BackgroundName,
                typewriterSpeed = 30f // Default value
            };

            // Link Portrait (Search globally)
            if (!string.IsNullOrEmpty(data.PortraitName))
            {
                Sprite portrait = FindAssetByName<Sprite>(data.PortraitName);
                if (portrait != null)
                {
                    newLine.characterPortrait = portrait;
                }
                else
                {
                    Debug.LogWarning($"[DialogueImportTool] Portrait not found: {data.PortraitName} for ID {id}");
                }
            }

            // Attempt to link Voice Clip if an exact match is found (Optional feature)
            // Assuming BGMName might be used for voice if valid? No, keep BGM as string.
            // But if user meant "Audio Files", maybe they want us to find a voice clip matching the ID or something?
            // Let's stick to just fixing the asset search for Portrait for now as it's the only object reference.
            
            asset.lines.Add(newLine);
        }

        if (isNew)
        {
            AssetDatabase.CreateAsset(asset, fullPath);
        }
        else
        {
            EditorUtility.SetDirty(asset);
        }
    }

    private static T FindAssetByName<T>(string name) where T : UnityEngine.Object
    {
        if (string.IsNullOrEmpty(name)) return null;

        string[] guids = AssetDatabase.FindAssets($"{name} t:{typeof(T).Name}");
        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            string fileName = Path.GetFileNameWithoutExtension(path);
            if (fileName.Equals(name, StringComparison.OrdinalIgnoreCase))
            {
                return AssetDatabase.LoadAssetAtPath<T>(path);
            }
        }
        return null;
    }

    private static string[] ParseCSVLine(string line)
    {
        // Basic CSV parser handling quotes and commas
        List<string> fields = new List<string>();
        bool inQuotes = false;
        string currentField = "";

        for (int i = 0; i < line.Length; i++)
        {
            char c = line[i];
            if (c == '\"')
            {
                inQuotes = !inQuotes;
            }
            else if (c == ',' && !inQuotes)
            {
                fields.Add(currentField.Trim('\"'));
                currentField = "";
            }
            else
            {
                currentField += c;
            }
        }
        fields.Add(currentField.Trim('\"'));

        return fields.ToArray();
    }

    private struct DialogueLineData
    {
        public string CharacterName;
        public string Text;
        public string PortraitName;
        public string BGMName;
        public string BackgroundName;
    }
}
