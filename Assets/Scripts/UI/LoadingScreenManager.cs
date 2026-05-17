using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

namespace UI
{
    /// <summary>
    /// Loading screen controller that shows during dungeon generation.
    /// </summary>
    public class LoadingScreenManager : MonoBehaviour
    {
        [Header("UI References")]
        public GameObject loadingPanel;
        public TextMeshProUGUI loadingText;
        public TextMeshProUGUI tipText;
        public Image loadingSpinner;
        
        [Header("Loading Animation")]
        public float spinnerSpeed = 180f;
        public float dotAnimationSpeed = 0.5f;
        
        [Header("Tips")]
        [TextArea(2, 4)]
        public string[] loadingTips = new string[]
        {
            "Explore every room to find hidden treasures!",
            "Enemies drop valuable loot when defeated.",
            "Chests contain random items - weapons, armor, or gold!",
            "Watch your health bar and retreat when necessary.",
            "Each dungeon is procedurally generated - no two runs are the same!"
        };
        
        private int dotCount = 0;
        private Coroutine dotAnimationCoroutine;
        
        private void Start()
        {
            // Hide loading screen initially
            if (loadingPanel != null)
                loadingPanel.SetActive(false);
            
            // Subscribe to GameManager events
            if (Dungeon.GameManager.Instance != null)
            {
                Dungeon.GameManager.Instance.OnGameStateChanged += OnGameStateChanged;
                Dungeon.GameManager.Instance.OnDungeonGenerationStarted += OnGenerationStarted;
                Dungeon.GameManager.Instance.OnDungeonGenerationCompleted += OnGenerationCompleted;
            }
        }
        
        private void OnDestroy()
        {
            if (Dungeon.GameManager.Instance != null)
            {
                Dungeon.GameManager.Instance.OnGameStateChanged -= OnGameStateChanged;
                Dungeon.GameManager.Instance.OnDungeonGenerationStarted -= OnGenerationStarted;
                Dungeon.GameManager.Instance.OnDungeonGenerationCompleted -= OnGenerationCompleted;
            }
        }
        
        private void Update()
        {
            // Rotate spinner
            if (loadingSpinner != null && loadingPanel != null && loadingPanel.activeSelf)
            {
                loadingSpinner.transform.Rotate(0, 0, -spinnerSpeed * Time.unscaledDeltaTime);
            }
        }
        
        private void OnGameStateChanged(Dungeon.GameState newState)
        {
            if (newState == Dungeon.GameState.Loading)
            {
                ShowLoading();
            }
            else
            {
                HideLoading();
            }
        }
        
        private void OnGenerationStarted()
        {
            ShowLoading();
        }
        
        private void OnGenerationCompleted()
        {
            // Delay hiding slightly to ensure smooth transition
            StartCoroutine(HideLoadingDelayed(0.5f));
        }
        
        public void ShowLoading()
        {
            if (loadingPanel != null)
            {
                loadingPanel.SetActive(true);
            }
            
            // Show random tip
            if (tipText != null && loadingTips.Length > 0)
            {
                int tipIndex = Random.Range(0, loadingTips.Length);
                tipText.text = loadingTips[tipIndex];
            }
            
            // Start dot animation
            if (dotAnimationCoroutine != null)
                StopCoroutine(dotAnimationCoroutine);
            dotAnimationCoroutine = StartCoroutine(AnimateLoadingDots());
        }
        
        public void HideLoading()
        {
            if (loadingPanel != null)
            {
                loadingPanel.SetActive(false);
            }
            
            if (dotAnimationCoroutine != null)
            {
                StopCoroutine(dotAnimationCoroutine);
                dotAnimationCoroutine = null;
            }
        }
        
        private IEnumerator HideLoadingDelayed(float delay)
        {
            yield return new WaitForSecondsRealtime(delay);
            HideLoading();
        }
        
        private IEnumerator AnimateLoadingDots()
        {
            while (true)
            {
                dotCount = (dotCount + 1) % 4;
                
                if (loadingText != null)
                {
                    loadingText.text = "Generating Dungeon" + new string('.', dotCount);
                }
                
                yield return new WaitForSecondsRealtime(dotAnimationSpeed);
            }
        }
    }
}

