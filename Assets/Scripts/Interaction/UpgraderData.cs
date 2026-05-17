using System.Collections.Generic;
using UnityEngine;
using System.IO;

namespace Interaction
{
    [System.Serializable]
    public class UpgradeStat
    {
        public int value;
        public int price;
    }

    [System.Serializable]
    public class UpgradeLevel
    {
        public int level;
        public UpgradeStat armor;
        public UpgradeStat damage;
        public UpgradeStat health;
    }

    public static class UpgraderDataLoader
    {
        public static List<UpgradeLevel> LoadData()
        {
            var textAsset = Resources.Load<TextAsset>("UpgraderLevels"); 
            // the user said the json is file is UpgraderLevels.json. If it is not in Resources, we'll try streaming assets or just read from path for now since it's Editor testing usually.
            // Oh wait, path is C:\Users\gabi\UnityExplorationGame\Assets\Scripts\UpgraderLevels.json.
            // So we can read it using File.ReadAllText.
            
            string path = Path.Combine(Application.dataPath, "Scripts", "UpgraderLevels.json");
            if (File.Exists(path))
            {
                string json = File.ReadAllText(path);
                // Unity's JsonUtility cannot parse top-level arrays easily, we have to wrap it
                string wrappedJson = "{\"levels\":" + json + "}";
                UpgradeLevelWrapper wrapper = JsonUtility.FromJson<UpgradeLevelWrapper>(wrappedJson);
                return wrapper.levels;
            }
            return new List<UpgradeLevel>();
        }

        [System.Serializable]
        private class UpgradeLevelWrapper
        {
            public List<UpgradeLevel> levels;
        }
    }
}

