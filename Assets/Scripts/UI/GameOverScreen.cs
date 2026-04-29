using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverScreen : MonoBehaviour
{
    private static GameOverScreen instance;
    public GameObject gameOverPanel;

    [Header("Current Run")]
    public TextMeshProUGUI xpText;
    public TextMeshProUGUI dungeonsText;

    [Header("High Score")]
    public TextMeshProUGUI highScoreXPText;
    public TextMeshProUGUI highScoreDungeonsText;

    private void Awake()
    {
        instance = this;
        gameOverPanel.SetActive(false);
    }

    public static void Show(int xp, int dungeons, int highXP, int highDungeons)
    {
        instance.xpText.text = "XP Earned: " + xp;
        instance.dungeonsText.text = "Dungeons Cleared: " + (dungeons - 1);
        instance.highScoreXPText.text = "Best XP: " + highXP;
        instance.highScoreDungeonsText.text = "Best Dungeons: " + highDungeons;
        instance.gameOverPanel.SetActive(true);
        Time.timeScale = 0f;
    }

    public void Restart()
    {
        AudioManager.Instance?.Play(AudioManager.Instance.buttonClick);
        Time.timeScale = 1f;
        GameManager.Instance?.ResetProgress();
        GameManager.Instance?.GenerateNewDungeon();
        gameOverPanel.SetActive(false);
    }

    public void QuitToMenu()
    {
        AudioManager.Instance?.Play(AudioManager.Instance.buttonClick);
        Time.timeScale = 1f;
        SceneManager.LoadScene(0);
    }
}