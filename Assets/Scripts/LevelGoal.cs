using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelGoal : MonoBehaviour
{
    [SerializeField] private string nextSceneName; // Boş bırakırsan aynı scene restart
    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            CompleteLevel();
        }
    }
    
    private void CompleteLevel()
    {
        Debug.Log("Level Complete!");
        
        if (!string.IsNullOrEmpty(nextSceneName))
        {
            SceneManager.LoadScene(nextSceneName);
        }
        else
        {
            // Aynı scene'i restart et
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }
}
