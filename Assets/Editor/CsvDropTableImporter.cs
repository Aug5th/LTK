using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;
using System.Globalization;

public class CsvDropTableImporter : EditorWindow
{
    public TextAsset dropTablesCsv;
    public TextAsset dropTableItemsCsv;

    [MenuItem("LTK Data Importer/Open CSV Importer")]
    public static void ShowWindow()
    {
        GetWindow<CsvDropTableImporter>("CSV Importer");
    }

    private void OnGUI()
    {
        GUILayout.Label("Import Drop Tables from CSV", EditorStyles.boldLabel);
        EditorGUILayout.HelpBox("Use this tool to bulk import DropTable data directly from CSV files.", MessageType.Info);

        dropTablesCsv = (TextAsset)EditorGUILayout.ObjectField("DropTables.csv", dropTablesCsv, typeof(TextAsset), false);
        dropTableItemsCsv = (TextAsset)EditorGUILayout.ObjectField("DropTableItems.csv", dropTableItemsCsv, typeof(TextAsset), false);

        GUILayout.Space(10);

        if (GUILayout.Button("Import Data", GUILayout.Height(40)))
        {
            if (dropTablesCsv != null && dropTableItemsCsv != null)
            {
                ImportData();
            }
            else
            {
                Debug.LogError("Please assign both CSV files!");
            }
        }
    }

    private void ImportData()
    {
        string saveFolder = "Assets/Data/DropTables";
        if (!Directory.Exists(saveFolder)) Directory.CreateDirectory(saveFolder);

        // Optimization: Stop importing assets until we are done to speed up the process
        AssetDatabase.StartAssetEditing();

        try
        {
            // --- STEP 1: Process DropTables.csv ---
            Dictionary<string, DropTableDefinition> createdAssets = new Dictionary<string, DropTableDefinition>();
            
            // Use StringReader to handle line endings (\r\n vs \n) safely
            using (StringReader reader = new StringReader(dropTablesCsv.text))
            {
                string headerLine = reader.ReadLine(); // Skip Header
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    if (string.IsNullOrWhiteSpace(line)) continue;

                    string[] data = SplitCsvLine(line);
                    
                    // Safety check: Ensure we have enough columns
                    if (data.Length < 6) 
                    {
                        Debug.LogWarning($"Skipped invalid line in DropTables.csv: {line}");
                        continue;
                    }

                    // Trim string data to remove accidental spaces
                    string tableId = data[0].Trim();
                    string unitId = data[1].Trim();
                    int exp = ParseInt(data[2]);
                    float gemChance = ParseFloat(data[3]);
                    int gemMin = ParseInt(data[4]);
                    int gemMax = ParseInt(data[5]);

                    // Create or Load Asset
                    string assetPath = $"{saveFolder}/{tableId}.asset";
                    DropTableDefinition asset = AssetDatabase.LoadAssetAtPath<DropTableDefinition>(assetPath);

                    if (asset == null)
                    {
                        asset = ScriptableObject.CreateInstance<DropTableDefinition>();
                        AssetDatabase.CreateAsset(asset, assetPath);
                    }

                    // Assign Data
                    asset.dropTableId = tableId;
                    asset.unitId = unitId;
                    asset.expReward = exp;
                    asset.gemDropChance = gemChance;
                    asset.gemMin = gemMin;
                    asset.gemMax = gemMax;
                    
                    // Clear old items to avoid duplication on re-import
                    asset.itemDrops = new ItemDropEntry[0]; 

                    // Mark as dirty so Unity knows to save it
                    EditorUtility.SetDirty(asset);
                    createdAssets[tableId] = asset;
                }
            }

            // --- STEP 2: Process DropTableItems.csv ---
            Dictionary<string, List<ItemDropEntry>> itemMap = new Dictionary<string, List<ItemDropEntry>>();

            using (StringReader reader = new StringReader(dropTableItemsCsv.text))
            {
                string headerLine = reader.ReadLine(); // Skip Header
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    if (string.IsNullOrWhiteSpace(line)) continue;

                    string[] data = SplitCsvLine(line);
                    if (data.Length < 5) continue;

                    string tableId = data[0].Trim();
                    string itemId = data[1].Trim();
                    float chance = ParseFloat(data[2]);
                    int min = ParseInt(data[3]);
                    int max = ParseInt(data[4]);

                    // Find the Item ScriptableObject
                    ItemDefinition realItem = FindItemDefinition(itemId);
                    if (realItem == null)
                    {
                        Debug.LogWarning($"[Item Missing] Could not find ItemDefinition file named '{itemId}' for Table '{tableId}'");
                        continue;
                    }

                    ItemDropEntry entry = new ItemDropEntry();
                    entry.item = realItem;
                    entry.dropChance = chance;
                    entry.minAmount = min;
                    entry.maxAmount = max;

                    if (!itemMap.ContainsKey(tableId))
                    {
                        itemMap[tableId] = new List<ItemDropEntry>();
                    }
                    itemMap[tableId].Add(entry);
                }
            }

            // --- STEP 3: Link Items to DropTables ---
            foreach (var kvp in createdAssets)
            {
                string tId = kvp.Key;
                DropTableDefinition asset = kvp.Value;

                if (itemMap.ContainsKey(tId))
                {
                    asset.itemDrops = itemMap[tId].ToArray();
                }
                EditorUtility.SetDirty(asset);
            }
        
            Debug.Log($"<color=green>Success!</color> Imported {createdAssets.Count} Drop Tables.");
        }
        finally
        {
            // Resume asset editing and save everything
            AssetDatabase.StopAssetEditing();
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
    }

    // --- HELPER FUNCTIONS ---

    // Handles splitting by comma but respects the line structure
    private string[] SplitCsvLine(string line)
    {
        // Simple split by comma. 
        // Note: If your data contains commas inside descriptions (e.g. "Sword, Fire"), 
        // you will need a more complex Regex parser. For IDs/Numbers, this is fine.
        return line.Split(',');
    }

    private ItemDefinition FindItemDefinition(string itemName)
    {
        // Search specifically for type ItemDefinition
        string[] guids = AssetDatabase.FindAssets($"{itemName} t:ItemDefinition");
        if (guids.Length > 0)
        {
            string path = AssetDatabase.GUIDToAssetPath(guids[0]);
            return AssetDatabase.LoadAssetAtPath<ItemDefinition>(path);
        }
        return null;
    }

    private int ParseInt(string s)
    {
        if (string.IsNullOrEmpty(s)) return 0;
        // Trim allows safe parsing even if there are spaces around like " 5 "
        if (int.TryParse(s.Trim(), out int result)) return result;
        return 0;
    }

    private float ParseFloat(string s)
    {
        if (string.IsNullOrEmpty(s)) return 0f;
        s = s.Trim().Replace(',', '.'); 
        if (float.TryParse(s, NumberStyles.Any, CultureInfo.InvariantCulture, out float result)) return result;
        return 0f;
    }
}