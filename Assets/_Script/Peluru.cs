using UnityEngine;

public class Peluru : MonoBehaviour
{
    [Header("🚀 Pengaturan Peluru")]
    [Range(0.01f, 1f)] public float kecepatan = 0.05f; 
    
    [Header("🎵 Suara & Visual (VFX)")]
    public AudioClip suaraKenaMusuh;
    public GameObject efekKenaMusuh;

    // ==================================================
    // FITUR BARU: SAKLAR PELURU NANCAP!
    // ==================================================
    [Header("🎯 Tipe Peluru")]
    [Tooltip("CENTANG ini khusus untuk Prefab Panah, biar nancap di musuh!")]
    public bool peluruNancap = false;
    // ==================================================

    private Transform target;
    private float damageBawaan; 

    public void SetTarget(Transform targetBaru)
    {
        target = targetBaru;
    }

    public void SetDamage(float damageDariTower)
    {
        damageBawaan = damageDariTower;
    }

    void Update()
    {
        if (target == null)
        {
            Destroy(gameObject); 
            return;
        }

        Vector3 arah = target.position - transform.position;
        float jarakFrameIni = kecepatan * Time.deltaTime;

        if (arah.magnitude <= jarakFrameIni)
        {
            KenaTarget();
            return;
        }

        if (arah != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(arah);
        }

        transform.Translate(arah.normalized * jarakFrameIni, Space.World);
    }

    void KenaTarget()
    {
        GerakMusuh scriptMusuh = target.GetComponent<GerakMusuh>();

        if (scriptMusuh != null)
        {
            scriptMusuh.KenaDamage(damageBawaan);
        }

        if (AudioManager.instance != null && suaraKenaMusuh != null)
        {
            AudioManager.instance.PutarSFX(suaraKenaMusuh);
        }

        if (efekKenaMusuh != null)
        {
            GameObject vfx = Instantiate(efekKenaMusuh, transform.position, transform.rotation);
            Destroy(vfx, 2f); 
        }

        // ==================================================
        // LOGIKA EFEK NANCAP ATAU HANCUR
        // ==================================================
        if (peluruNancap)
        {
            // 1. Jadikan panah ini 'anak' dari objek musuh, jadi bakal ikut kebawa jalan
            transform.SetParent(target);
            
            // 2. Matikan pergerakan script ini biar panahnya nge-freeze (nancap)
            this.enabled = false;
            
            // 3. Hancurkan objek panahnya setelah 2 detik (biar map bersih)
            Destroy(gameObject, 2f);
        }
        else
        {
            // Kalau kotak tidak dicentang (Canon dll), peluru langsung hancur lebur
            Destroy(gameObject); 
        }
    }
}