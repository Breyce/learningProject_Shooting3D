using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameUI : MonoBehaviour
{
    public Image fadePlane;
    public GameObject gameOverUI;
    public GameObject pauseUI;

    public RectTransform newWaveBanner;
    public RectTransform healthBar;
    public Text newWaveTitle;
    public Text newWaveEnemyCount;
    public Text newWaveWeapon;
    public Text newWaveWeaponFireMode;
    public Text ScoreText;
    public Text GameOverScoreText;


    bool isPause;
    Spawner spawner;
    Player player;
    GunController gun;
    ScoreKeeper scoreKeeper;

    private void Awake()
    {
        spawner = FindObjectOfType<Spawner>();
        gun = FindObjectOfType<Player>().GetComponent<GunController>();
        scoreKeeper = FindObjectOfType<ScoreKeeper>().GetComponent<ScoreKeeper>();
        spawner.OnNewWave += OnNewWave;
    }

    private void Start()
    {
        isPause = false;
        player = FindObjectOfType<Player>();
        player.OnDeath += GameOver;
    }

    private void Update()
    {
        ScoreText.text ="Scores: " + scoreKeeper.score.ToString("D6");
        float healthPercent = 0;
        if (player != null)
        {
            healthPercent = player.health / player.startingHealth;
        } 
        healthBar.localScale = new Vector3(healthPercent, 1, 1);

        if (Input.GetKeyDown(KeyCode.Escape)) { 
            
            isPause = !isPause;
            pauseUI.gameObject.SetActive(isPause);

        }
    }

    void OnNewWave(int waveNumber)
    {
        string[] number = { "One", "Two", "Three", "Four", "Five" };
        newWaveTitle.text = "- Wave " + number[waveNumber - 1] + " -";
        newWaveWeapon.text = "- Weapon: " + gun.allGun[waveNumber - 1].name + " -";
        newWaveWeaponFireMode.text = "- Fire Mode: " + gun.allGun[waveNumber - 1].fireMode + " -";

        if (spawner.waves[waveNumber - 1].enemyCount != -1)
        {
            newWaveEnemyCount.text = " - Enemy Count: " + spawner.waves[waveNumber - 1].enemyCount + " -";
        }
        else
        {
            newWaveEnemyCount.text = " - Enemy Count: Infinite -";
        }

        StopCoroutine(AnimateNewWavBanner());
        StartCoroutine(AnimateNewWavBanner());
    }

    IEnumerator AnimateNewWavBanner()
    {
        float animatePercent = 0;
        float delayTime = 2.5f;
        float speed = 2.5f;
        int dir = 1;

        float endDelayTime = Time.time + 1/speed + delayTime;

        while(animatePercent >= 0)
        {
            animatePercent += Time.deltaTime * speed * dir;

            if(animatePercent >= 1) {
                animatePercent = 1;
                if(Time.time > endDelayTime)
                {
                    dir = -1;
                }
            }
            newWaveBanner.anchoredPosition = Vector2.up * Mathf.Lerp(-225, 50, animatePercent);
            yield return null;
        }
    }

    void GameOver()
    {
        Cursor.visible = true;
        StartCoroutine(Fade(Color.clear, new Color(0,0,0,.95f), 1));
        GameOverScoreText.text = ScoreText.text;
        ScoreText.gameObject.SetActive(false);
        healthBar.transform.parent.gameObject.SetActive(false);
        gameOverUI.SetActive(true);
    }

    IEnumerator Fade(Color from,Color to, float time)
    {
        float speed = 1 / time;
        float percent = 0;

        while(percent < 1)
        {
            percent += Time.deltaTime * speed;
            fadePlane.color = Color.Lerp(from,to,percent);
            yield return null;
        }
    }

    public void StartNewGame()
    {
        SceneManager.LoadScene("Game");
    }

    public void BackToMenu()
    {
        SceneManager.LoadScene("Menu");
        scoreKeeper.score = 0;
    }

    public void RestartGame()
    {
        SceneManager.LoadScene("Game");
        scoreKeeper.score = 0;
    }
}
