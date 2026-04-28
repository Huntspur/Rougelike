using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{

    [Header("High Score UI")]
    public TextMeshProUGUI highScoreXPText;
    public TextMeshProUGUI highScoreDungeonsText;

    public void Play()
    {
        SceneManager.LoadScene(1);
    }

    private void Start()
    {
        int bestXP = PlayerPrefs.GetInt("HighScoreXP", 0);
        int bestDungeons = PlayerPrefs.GetInt("HighScoreDungeons", 0);

        highScoreXPText.text = "Best XP: " + bestXP;
        highScoreDungeonsText.text = "Best Dungeons Cleared: " + bestDungeons;
    }

    public void Quit()
    {
        Application.Quit();
    }
}
