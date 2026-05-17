using UnityEngine;
using System.Collections;
using UnityEngine.Serialization;

public class Health : MonoBehaviour
{
    private SpriteRenderer _spriteRenderer;
    private Color _originalColor;
    
    public int CurrentHealth = 100;
    public int MaxHealth = 100;
    
    [Header("Death Settings")]
    [Tooltip("If true and this is the player, notifies GameManager on death")]
    public bool notifyGameManagerOnDeath = true;
    
    void Start()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        if (_spriteRenderer != null)
        {
            _originalColor = _spriteRenderer.color;
        }
    }

    void FixedUpdate()
    {
        if (CurrentHealth <= 0)
        {
            OnDeath();
        }
    }
    
    private void OnDeath()
    {
        // Check if this is the player
        if (notifyGameManagerOnDeath && CompareTag("Player"))
        {
            // Notify GameManager of player death
            if (Dungeon.GameManager.Instance != null)
            {
                Dungeon.GameManager.Instance.OnPlayerDeath();
            }
            
            // Delay destruction slightly to allow UI to capture player data
            // and to prevent camera from being destroyed immediately
            StartCoroutine(DelayedDestroy());
            return;
        }
        
        Destroy(gameObject);
    }
    
    private IEnumerator DelayedDestroy()
    {
        // Disable the player visually and functionally
        var spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            spriteRenderer.enabled = false;
        }
        
        // Disable colliders so player doesn't interact
        var colliders = GetComponents<Collider2D>();
        foreach (var col in colliders)
        {
            col.enabled = false;
        }
        
        // Disable player controller
        var controller = GetComponent<Controllers.BaseController>();
        if (controller != null)
        {
            controller.enabled = false;
        }
        
        // Wait a frame for UI to initialize
        yield return null;
        
        // Don't destroy the player - keep the camera alive!
        // The scene will be reloaded on restart anyway
        // Just disable the gameObject instead
        gameObject.SetActive(false);
    }
    
    public void DecreaseHealth(int damage)
    {
        CurrentHealth -= damage;
        if (_spriteRenderer != null)
        {
            StartCoroutine(FlashRed());
        }
    }
    
    public void SetMaxHealth(int newMax)
    {
        int diff = newMax - MaxHealth;
        MaxHealth = newMax;
        if (diff > 0)
        {
            CurrentHealth += diff;
        }
    }

    public int GetHealthPoints()
    {
        return CurrentHealth;
    }

    private IEnumerator FlashRed()
    {
        _spriteRenderer.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        _spriteRenderer.color = _originalColor;
    }
}