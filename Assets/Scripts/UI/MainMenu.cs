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
        AudioManager.Instance?.Play(AudioManager.Instance.buttonClick);
        SceneManager.LoadScene(1);
    }

    private void Start()
    {
        int bestXP = PlayerPrefs.GetInt("HighScoreXP", 0);
        int bestDungeons = PlayerPrefs.GetInt("HighScoreDungeons", 0);

        AudioManager.Instance?.PlayMusic(AudioManager.Instance.mainMenuMusic);

        highScoreXPText.text = "Best XP: " + bestXP;
        highScoreDungeonsText.text = "Best Dungeons Cleared: " + bestDungeons;
    }

    public void Quit()
    {
        AudioManager.Instance?.Play(AudioManager.Instance.buttonClick);
        Application.Quit();
    }
}
