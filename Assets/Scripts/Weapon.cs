using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour {

    public float fireRate = 0.0f;
    public int damage = 10;
    public LayerMask target;

    public Transform bulletTrailPrefab;
    public Transform hitPrefab;
    public Transform muzzleFlashPrefab;
    float timeToSpawnEffect = 0;
    public float effectSpawnRate = 10;
    public string shootSoundName;

    float timeToFire = 0.0f;
    Transform firePoint;

    private AudioManager audioManager;

    private void Awake()
    {
        firePoint = transform.Find("FirePoint");
        if (firePoint == null)
            Debug.Log("No firePoint Found");
    }

    private void Start()
    {
        audioManager = AudioManager.instance;
        if (audioManager == null)
        {
            Debug.Log("No audio manager found");
        }
    }

    // Update is called once per frame
    void Update () {
		if(fireRate == 0)
        {
            if (Input.GetButtonDown("Fire1"))
            {
                Shoot();
            }
        }
        else
        {
            if(Input.GetButton("Fire1") && Time.time > timeToFire)
            {
                timeToFire = Time.time + 1 / fireRate;
                Shoot();
            }
        }
	}

    void Shoot()
    {
        audioManager.PlaySound(shootSoundName);
        Vector2 mousePosition = new Vector2(Camera.main.ScreenToWorldPoint(Input.mousePosition).x, Camera.main.ScreenToWorldPoint(Input.mousePosition).y);
        Vector2 firePointPosition = new Vector2(firePoint.position.x, firePoint.position.y);
        RaycastHit2D hit = Physics2D.Raycast(firePointPosition, mousePosition - firePointPosition, 100, target);
       
        Debug.DrawLine(firePointPosition , mousePosition);
        if(hit.collider != null)
        {
            Debug.DrawLine(firePointPosition, hit.point, Color.red);
            Enemy enemy = hit.collider.GetComponent<Enemy>();
            if(enemy != null)
            {
                enemy.DamageEnemy(damage);
                //Debug.Log(hit.collider.name + damage);
            }
        }

        if (Time.time >= timeToSpawnEffect)
        {
            Vector3 hitPos;
            Vector3 hitNormal;

            if (hit.collider == null)
            {
                hitPos = (mousePosition - firePointPosition) * 30;
                hitNormal = new Vector3(9999, 9999, 9999);
            }
            else
            {
                hitPos = hit.point;
                hitNormal = hit.normal;
            }

            Effect(hitPos, hitNormal);
            timeToSpawnEffect = Time.time + 1 / effectSpawnRate;
        }
    }

    void Effect(Vector3 hitPosition, Vector3 hitNormal)
    {
        Transform trail = Instantiate(bulletTrailPrefab, firePoint.position, firePoint.rotation) as Transform;
        LineRenderer lr = trail.GetComponent<LineRenderer>();
        if (lr != null)
        {
            lr.SetPosition(0, firePoint.position);
            lr.SetPosition(1, hitPosition);
        }

        Destroy(trail.gameObject, 0.04f);

        if(hitNormal != new Vector3(9999, 9999, 9999))
        {
           Transform hitParticles = Instantiate(hitPrefab, hitPosition, Quaternion.FromToRotation(Vector3.right, hitNormal)) as Transform;
           Destroy(hitParticles.gameObject, 1f);
        }

        Transform clone = Instantiate(muzzleFlashPrefab, firePoint.position, firePoint.rotation) as Transform;
        clone.parent = firePoint;
        float size = Random.Range(0.6f, 0.9f);
        clone.localScale = new Vector3(size, size, 0.0f);
        Destroy(clone.gameObject, 0.02f);
    }
}
