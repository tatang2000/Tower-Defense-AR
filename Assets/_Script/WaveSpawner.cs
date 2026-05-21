using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro; // 🚨 WAJIB: Tambahkan ini di paling atas untuk mengontrol TextMeshPro!

public class WaveSpawner : MonoBehaviour
{
    [System.Serializable]
    public struct DataWave
    {
        public int jumlahMusuh;
        public Material warnaMusuh;
        public float jedaSpawn;
    }

    [Header("Pengaturan Rute")]
    public Transform titikRuteUtama;
    private Transform[] titikWaypoint;

    [Header("Pengaturan Spawn")]
    public GameObject prefabMusuh;
    
    [Header("Pengaturan Gelombang (Wave)")]
    public List<DataWave> daftarWave;
    public int waveSaatIni = 0;
    public bool sedangSpawn = false;

    [Header("Sistem Transisi Saklar (UI/UX)")]
    [Tooltip("CENTANG = Otomatis nunggu 10 detik. UNCHECK = Harus pencet tombol Start manual.")]
    public bool otomatisLanjutWave = false; 
    public float jedaAntarWave = 10f; 
    private float hitungMundurWave = 0f;
    private bool statusWaveSelesaiLahir = false;

    // =======================================================
    // VARIABEL BARU: Hubungan ke Teks Visual UI Wave
    // =======================================================
    [Header("📺 Hubungan UI Wave")]
    [Tooltip("Tarik objek Text_Wave dari Hierarchy ke sini, King!")]
    public TextMeshProUGUI uiTeksWave; 
    // =======================================================

    void Start()
    {
        if (titikRuteUtama == null) return;

        int jumlahTitik = titikRuteUtama.childCount;
        titikWaypoint = new Transform[jumlahTitik];
        for (int i = 0; i < jumlahTitik; i++)
        {
            titikWaypoint[i] = titikRuteUtama.GetChild(i);
        }

        hitungMundurWave = jedaAntarWave; 
        
        // Perbarui teks visual wave saat game pertama kali dinyalakan (0 / Total Wave)
        UpdateVisualWave();
    }

void Update()
    {
        // Jika musuh masih dalam proses keluar dari gerbang, jangan cek apa-apa dulu
        if (sedangSpawn) return;

        // DETEKSI FISIK: Jika musuh di map sudah habis beneran DAN wave sebelumnya sudah selesai lahir
        if (GameObject.FindWithTag("Musuh") == null && statusWaveSelesaiLahir == true)
        {
            // =======================================================================
            // FIX AMAN: Cek kondisi menang di paling atas, BEBAS dari gembok mode otomatis/manual!
            // =======================================================================
            if (waveSaatIni >= daftarWave.Count)
            {
                Debug.Log("CONGRATS! Semua wave tamat, King! 🎉");
                
                // Tembak fungsi Victory di GameManager biar panelnya langsung nongol di HP
                if (GameManager.instance != null)
                {
                    GameManager.instance.TriggerVictory();
                }
                
                this.enabled = false; // Matikan script spawner agar tidak memanggil fungsi berulang-ulang
                return; // Langsung keluar fungsi
            }
            // =======================================================================

            // Jika dikonfigurasi otomatis pakai Cooldown 10 detik
            if (otomatisLanjutWave)
            {
                // Kita tidak perlu cek (waveSaatIni < daftarWave.Count) lagi karena sudah disaring di atas
                hitungMundurWave -= Time.deltaTime;
                Debug.Log("Wave berikutnya otomatis lahir dalam: " + Mathf.Round(hitungMundurWave) + " detik.");

                if (hitungMundurWave <= 0f)
                {
                    MulaiWave();
                    hitungMundurWave = jedaAntarWave; // Reset timer untuk wave masa depan
                }
            }
            // Jika mode manual (otomatisLanjutWave tidak dicentang), script akan standby aman di sini
        }
    }

    public void MulaiWave()
    {
        if (Time.timeScale == 0f)
        {
            Debug.LogWarning("Gak bisa panggil wave pas lagi Game Over, King! Pencet RESTART dulu!");
            return; 
        }

        if (sedangSpawn) return;
        if (GameObject.FindWithTag("Musuh") != null) 
        {
            Debug.LogWarning("Eitss ga bisa dicheat! Habisin dulu Rehman yang ada di map, king!");
            return; 
        }
        if (waveSaatIni >= daftarWave.Count) 
        {
            Debug.Log("Semua paket gelombang wave sudah habis!");
            return;
        }

        StartCoroutine(ProsesSpawn());
    }

    IEnumerator ProsesSpawn()
    {
        sedangSpawn = true;
        statusWaveSelesaiLahir = false; 
        
        // 🚨 UPDATE DI SINI: Naikkan angka visual teks saat kloter musuh mulai keluar
        UpdateVisualWaveAktif();

        Debug.Log("MEMULAI WAVE KE- " + (waveSaatIni + 1));

        DataWave waveSekarang = daftarWave[waveSaatIni];

        for (int i = 0; i < waveSekarang.jumlahMusuh; i++)
        {
            GameObject musuhBaru = Instantiate(prefabMusuh, titikWaypoint[0].position, Quaternion.identity);
            musuhBaru.tag = "Musuh"; 

            GerakMusuh scriptMusuh = musuhBaru.GetComponent<GerakMusuh>();
            if (scriptMusuh != null)
            {
                scriptMusuh.ruteWaypoint = titikWaypoint;
                scriptMusuh.SetWarna(waveSekarang.warnaMusuh);
            }

            yield return new WaitForSeconds(waveSekarang.jedaSpawn);
        }

        waveSaatIni++; 
        sedangSpawn = false; 
        statusWaveSelesaiLahir = true; 
    }

    public void ResetWaveManual()
    {
        StopAllCoroutines(); 
        waveSaatIni = 0; 
        sedangSpawn = false; 
        statusWaveSelesaiLahir = false; 
        hitungMundurWave = jedaAntarWave; 

        this.enabled = true;
        
        // 🚨 UPDATE DI SINI: Kembalikan teks visual ke kondisi awal (0 / Total Wave) pas restart
        UpdateVisualWave();

        Debug.Log("Sistem internal WaveSpawner RESMI dikembalikan ke Wave 1, King! 🔥");
    }

    // =======================================================
    // FUNGSI PEMBANTU BARU: Mengatur Isi Teks di Layar HP
    // =======================================================
    void UpdateVisualWave()
    {
        if (uiTeksWave != null)
        {
            // Menampilkan kondisi standby (contoh: 🌊 WAVE: 0 / 3)
            uiTeksWave.text = "WAVE: " + waveSaatIni + " / " + daftarWave.Count;
        }
    }

    void UpdateVisualWaveAktif()
    {
        if (uiTeksWave != null)
        {
            // Menampilkan wave yang sedang berjalan (ditambah 1 karena index C# dimulai dari 0)
            uiTeksWave.text = "WAVE: " + (waveSaatIni + 1) + " / " + daftarWave.Count;
        }
    }
    // =======================================================
}