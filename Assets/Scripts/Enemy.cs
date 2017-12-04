using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{

    [System.Serializable]
    public class EnemyStats
    {
        public int maxHealth = 100;

        private int _curHealth;
        public int curHealth
        {
            get { return _curHealth; }
            set { _curHealth = Mathf.Clamp(value, 0, maxHealth); }
        }

        public int damage = 40;

        public void Init()
        {
            curHealth = maxHealth;
        }
    }

    public EnemyStats enemyStats = new EnemyStats();

    public Transform deathParticles;
    public float shakeAmount = 0.1f;
    public float shakeLength = 0.1f;

    [Header("Optional:")]
    [SerializeField]
    private StatusIndicator statusIndicator;

    public string deathSoundName;

    private AudioManager audioManager;

    private void Start()
    {
        audioManager = AudioManager.instance;
        if (audioManager == null)
        {
            Debug.Log("No audio manager found");
        }

        enemyStats.Init();

        if(statusIndicator != null)
        {
            statusIndicator.SetHealth(enemyStats.curHealth, enemyStats.maxHealth);
        }

        if(deathParticles == null)
        {
            Debug.Log("no particles");
        }
    }

    public void DamageEnemy(int damage)
    {
        enemyStats.curHealth -= damage;
        if (enemyStats.curHealth <= 0)
        {
            GameManager.KillEnemy(this);
            audioManager.PlaySound(deathSoundName);
        }

        if (statusIndicator != null)
        {
            statusIndicator.SetHealth(enemyStats.curHealth, enemyStats.maxHealth);
        }
    }

    private void OnCollisionEnter2D(Collision2D _colInfo)
    {
        Player _player = _colInfo.collider.GetComponent<Player>();
        if (_player != null)
        {
            _player.DamagePlayer(enemyStats.damage);
            DamageEnemy(99999);
        }
    }
}
