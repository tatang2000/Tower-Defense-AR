using UnityEngine;

public class TowerNembak : MonoBehaviour
{
    [Header("🎯 Deteksi Target")]
    public Transform target;
    
    [Tooltip("Radius lingkaran jangkauan tembak (Warna Merah).")]
    [Range(0.05f, 1f)] public float jarakTembak = 0.2f; 
    public string tagMusuh = "Musuh";

    [Header("⚔️ Status Tempur Tower")]
    [Tooltip("Harga untuk membangun menara ini, King!")]
    public int hargaTower = 30; // Nilai default (bisa diganti sesukamu di Inspector)
    
    [Tooltip("Daya hancur (Damage) yang akan dikirim tower ini ke musuh.")]
    [Range(1f, 50f)] public float damageTower = 1f; 
    
    [Tooltip("Jeda antar tembakan (Semakin kecil semakin kayak Machine Gun).")]
    [Range(0.1f, 3f)] public float cooldownNembak = 0.5f; 
    private float hitungMundur = 0f;

    [Header("📦 Referensi Objek")]
    public GameObject prefabPeluru;

    [Header("🎵 Suara Tembakan")]
    public AudioClip suaraTembakanTowerIni;
    
    [Header("🔄 Pengaturan Rotasi")]
    [Tooltip("Pastikan TitikTembak ini ada DI DALAM KepalaParent biar ikut muter!")]
    public Transform titikTembak;

    [Tooltip("Tarik objek 'KepalaParent' dari Hierarchy ke sini, king!")]
    public Transform kepalaTowerToRotate;

    [Header("✨ Efek Visual (VFX)")]
    [Tooltip("Masukin prefab partikel ledakan ke sini, king!")]
    public GameObject efekTembakan;

    void Update()
    {
        CariMusuh();
        
        if (target == null) return;
        
        // --- (Sisa kode di bawahnya biarkan saja persis seperti yang kamu punya) ---

        // Validasi keamanan biar tidak error kalau kepala belum dicolok
        if (kepalaTowerToRotate == null)
        {
            Debug.LogWarning("Tarik objek 'KepalaParent' tower ke slot 'Kepala Tower To Rotate' di Inspector, king!");
            return;
        }

        // ==========================================
        // FITUR: HANYA KEPALA NENGOK TANPA NUNDUK
        // ==========================================
        
        // Cari tahu arah dari tower ke musuh (pakai trik tuker posisi kemarin biar tidak ngadep belakang)
        Vector3 arahTarget = transform.position - target.position;
        
        // KUNCI SUMBU Y!
        arahTarget.y = 0f; 
        
        // Ubah arah menjadi rotasi Quaternion
        Quaternion rotasiTujuan = Quaternion.LookRotation(arahTarget);
        
        // PERBAIKAN 2: Yang diputar adalah kepalaTowerToRotate, BUKAN induk transform!
        kepalaTowerToRotate.rotation = Quaternion.Slerp(kepalaTowerToRotate.rotation, rotasiTujuan, Time.deltaTime * 10f);
        
        // ==========================================

        // Hitung mundur jeda menembak
        if (hitungMundur <= 0f)
        {
            Nembak(); 
            hitungMundur = cooldownNembak;
        }

        hitungMundur -= Time.deltaTime;
    }

    // --- Fungsi lain (CariMusuh, Nembak, Gizmos) tetap sama seperti versi kemarin ---
    void CariMusuh()
    {
        // ====================================================
        // 1. SISTEM LOCK-ON (CEK TARGET LAMA DULU)
        // ====================================================
        if (target != null)
        {
            // Hitung jarak target yang sedang dikunci saat ini
            float jarakKeTargetAktif = Vector3.Distance(transform.position, target.position);
            
            // Jika target MASIH HIDUP dan MASIH DALAM RADIUS, Tower harus setia!
            if (jarakKeTargetAktif <= jarakTembak)
            {
                // Berhenti menjalankan kode di bawahnya, tetap kunci target ini
                return; 
            }
            else
            {
                // Jika target sudah lari keluar batas radius, lepas kunciannya
                target = null;
            }
        }

        // ====================================================
        // 2. JIKA TARGET KOSONG/MATI/KABUR, BARU CARI YANG BARU
        // ====================================================
        GameObject[] musuhDiMap = GameObject.FindGameObjectsWithTag(tagMusuh);
        float jarakTerdekat = Mathf.Infinity;
        GameObject musuhTerdekat = null;

        foreach (GameObject musuh in musuhDiMap)
        {
            float jarakKeMusuh = Vector3.Distance(transform.position, musuh.transform.position);
            
            if (jarakKeMusuh < jarakTerdekat && jarakKeMusuh <= jarakTembak)
            {
                jarakTerdekat = jarakKeMusuh;
                musuhTerdekat = musuh;
            }
        }

        // Kunci target ke musuh terdekat yang baru ditemukan
        if (musuhTerdekat != null)
        {
            target = musuhTerdekat.transform;
        }
    }

    void Nembak()
    {
        // Fitur safety, biar moncong tembak valid
        if (prefabPeluru == null || titikTembak == null) return;

        GameObject peluruGO = Instantiate(prefabPeluru, titikTembak.position, titikTembak.rotation);
        Peluru scriptPeluru = peluruGO.GetComponent<Peluru>();

        if (scriptPeluru != null)
        {
            scriptPeluru.SetTarget(target);
            scriptPeluru.SetDamage(damageTower); 
        }
        if (AudioManager.instance != null && suaraTembakanTowerIni != null)
        {
            AudioManager.instance.PutarSFX(suaraTembakanTowerIni);
        }
        // ==========================================
        // MUNCULIN EFEK LEDAKAN PAS NEMBAK
        // ==========================================
        if (efekTembakan != null)
        {
            // Lahirkan partikel di ujung moncong (titikTembak)
            GameObject vfxLedakan = Instantiate(efekTembakan, titikTembak.position, titikTembak.rotation);
            
            // Hancurkan partikel setelah 2 detik biar map nggak penuh sampah!
            Destroy(vfxLedakan, 2f);
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, jarakTembak);
    }
}