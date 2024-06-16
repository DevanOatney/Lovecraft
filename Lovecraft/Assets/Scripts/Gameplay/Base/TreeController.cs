using UnityEngine;
using UnityEngine.UI;

public class TreeController : MonoBehaviour
{
    public int maxHealth = 100;
    private int currentHealth;

    [SerializeField] Image TreeHealthImage;

    void Start()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(int amount)
    {
        currentHealth -= amount;
        if (currentHealth <= 0)
        {
            currentHealth = 0;
            Die();
        }
        TreeHealthImage.fillAmount = (float)currentHealth / (float)maxHealth;
    }

    void Die()
    {
        // Implement what happens when the tree is destroyed
        Debug.Log("Tree destroyed!");
    }
}