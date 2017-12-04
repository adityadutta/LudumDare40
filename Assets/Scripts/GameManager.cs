using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

    public static GameManager Instance;

    [SerializeField]
    private int maxLives = 3;

    private static int _remainingLives = 3;
    public static int RemainingLives
    {
        get { return _remainingLives; }
    }

    private void Awake()
    {
        if (Instance == null)
            Instance = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
    }

    public Transform playerPrefab;
    public Transform spawnPoint;
    public int spawnDelay = 2;
    public Transform spawnPrefab;
    public string spawnSoundName;

    public CameraShake cameraShake;

    [SerializeField]
    private GameObject gameOverUI;

    private AudioManager audioManager;

    private void Start()
    {
        if(cameraShake == null)
        {
            Debug.Log("No camera shake in GM");
        }

        _remainingLives = maxLives;

        audioManager = AudioManager.instance;
        if(audioManager == null)
        {
            Debug.Log("No audio manager found");
        }
    }

    public void EndGame()
    {
        Debug.Log("GAME OVER");
        gameOverUI.SetActive(true);
    }

    public IEnumerator _RespawnPlayer()
    {
        yield return new WaitForSeconds(spawnDelay);
        audioManager.PlaySound(spawnSoundName);

        Instantiate(playerPrefab, spawnPoint.position, spawnPoint.rotation);
        Transform clone = Instantiate(spawnPrefab, spawnPoint.position, spawnPoint.rotation) as Transform;
        Destroy(clone.gameObject, 3f);
    }

    public static void KillPlayer(Player player)
    {
        Destroy(player.gameObject);
        _remainingLives -= 1;
        if (_remainingLives <= 0)
        {
            Instance.EndGame();
        }
        else
        {
            Instance.StartCoroutine(Instance._RespawnPlayer());
        }
    }

    public static void KillEnemy(Enemy enemy)
    {
        Instance._KillEnemy(enemy);
    }

    public void _KillEnemy(Enemy _enemy)
    {
        Transform _clone = Instantiate(_enemy.deathParticles, _enemy.transform.position, Quaternion.identity) as Transform;
        cameraShake.Shake(_enemy.shakeAmount, _enemy.shakeLength);
        Destroy(_enemy.gameObject);
        Destroy(_clone.gameObject, 3f);
    }
}
