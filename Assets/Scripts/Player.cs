using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {

    [System.Serializable]
    public class PlayerStats
    {
        public int maxHealth = 100;

        private int _curHealth;
        public int curHealth
        {
            get { return _curHealth; }
            set { _curHealth = Mathf.Clamp(value, 0, maxHealth); }
        }

        public void Init()
        {
            curHealth = maxHealth;
        }
    }

    public PlayerStats stats = new PlayerStats();

    public int fallBoundary = -10;

    [SerializeField]
    private StatusIndicator statusIndicator;

    public string deathSoundName;

    private AudioManager audioManager;


    private void Start()
    {
        stats.Init();

        audioManager = AudioManager.instance;
        if (audioManager == null)
        {
            Debug.Log("No audio manager found");
        }


        if (statusIndicator == null)
        {
            Debug.Log("No status indicator");
        }
        else
        {
            statusIndicator.SetHealth(stats.curHealth, stats.maxHealth);
        }
    }

    private void Update()
    {
        if (transform.position.y <= fallBoundary)
            DamagePlayer(9999);
    }

    public void DamagePlayer(int damage)
    {
        stats.curHealth -= damage;
        if (stats.curHealth <= 0)
        {
            GameManager.KillPlayer(this);
            audioManager.PlaySound(deathSoundName);
        }

        statusIndicator.SetHealth(stats.curHealth, stats.maxHealth);
    }


}
