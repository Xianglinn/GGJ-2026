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
            string[] lines = File.ReadAllLines(filePath);
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

            // Link Portrait
            if (!string.IsNullOrEmpty(data.PortraitName))
            {
                string portraitResourcePath = Path.Combine("Sprites/Portraits/", data.PortraitName);
                Sprite portrait = Resources.Load<Sprite>(portraitResourcePath);
                if (portrait == null)
                {
                    Debug.LogWarning($"[DialogueImportTool] Portrait not found: {portraitResourcePath} for ID {id}");
                }
                newLine.characterPortrait = portrait;
            }

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
