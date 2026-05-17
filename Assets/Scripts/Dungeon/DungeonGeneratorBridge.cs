﻿using UnityEngine;
using Edgar.Unity;

namespace Dungeon
{
    /// <summary>
    /// Bridge component that connects the Edgar dungeon generator with our GameManager.
    /// Attach this to the same GameObject as the DungeonGeneratorGrid2D.
    /// </summary>
    [RequireComponent(typeof(DungeonGeneratorGrid2D))]
    public class DungeonGeneratorBridge : MonoBehaviour
    {
        [Header("References")]
        [Tooltip("Reference to the dungeon generator (auto-found if not set)")]
        public DungeonGeneratorGrid2D dungeonGenerator;
        
        [Header("Settings")]
        [Tooltip("If true, will use the seed from GameManager")]
        public bool useGameManagerSeed = true;
        
        [Tooltip("If true, automatically generates dungeon on Start")]
        public bool generateOnStart = true;
        
        private void Awake()
        {
            if (dungeonGenerator == null)
            {
                dungeonGenerator = GetComponent<DungeonGeneratorGrid2D>();
            }
            
            // IMPORTANT: Disable Edgar's built-in GenerateOnStart since we handle generation ourselves
            // This prevents the dungeon from being generated twice
            if (dungeonGenerator != null)
            {
                dungeonGenerator.GenerateOn = GenerateOn.Manually;
            }
        }
        
        private void Start()
        {
            // Apply seed from GameManager if available
            if (useGameManagerSeed && GameManager.Instance != null)
            {
                SetSeed(GameManager.Instance.CurrentSeed);
            }
            
            if (generateOnStart)
            {
                Generate();
            }
        }
        
        /// <summary>
        /// Set the random seed for dungeon generation
        /// </summary>
        public void SetSeed(int seed)
        {
            if (dungeonGenerator != null)
            {
                dungeonGenerator.UseRandomSeed = false;
                dungeonGenerator.RandomGeneratorSeed = seed;
                Debug.Log($"DungeonGeneratorBridge: Set seed to {seed}");
            }
        }
        
        /// <summary>
        /// Generate the dungeon
        /// </summary>
        public void Generate()
        {
            if (dungeonGenerator == null)
            {
                Debug.LogError("DungeonGeneratorBridge: No dungeon generator found!");
                return;
            }
            
            // Notify GameManager that generation is starting
            if (GameManager.Instance != null)
            {
                GameManager.Instance.NotifyDungeonGenerationStarted();
            }
            
            Debug.Log("DungeonGeneratorBridge: Starting dungeon generation...");
            
            // Generate the dungeon
            dungeonGenerator.Generate();
            
            // Notify GameManager that generation is complete
            if (GameManager.Instance != null)
            {
                GameManager.Instance.NotifyDungeonGenerationCompleted();
            }
            
            Debug.Log("DungeonGeneratorBridge: Dungeon generation completed!");
        }
        
        /// <summary>
        /// Regenerate the dungeon with a new random seed
        /// </summary>
        public void RegenerateWithNewSeed()
        {
            int newSeed = System.Environment.TickCount;
            
            if (GameManager.Instance != null)
            {
                GameManager.Instance.StartNewGameWithSeed(newSeed);
            }
            else
            {
                SetSeed(newSeed);
                Generate();
            }
        }
    }
}

