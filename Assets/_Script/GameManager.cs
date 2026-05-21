using UnityEngine;
using UnityEngine.SceneManagement; 
using TMPro; 

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [Header("🛡️ Sistem Toko Tower")]
    public GameObject towerYangDipilih;

    [Header("🏰 Sistem Pertahanan Base")]
    [Range(1, 100)] public int hpBaseMaksimal = 10;
    private int hpBaseSaatIni;
    private bool isGameOver = false;

    // =======================================================
    // FITUR EKONOMI BARU: SISTEM DOMPET & TEKS KOIN
    // =======================================================
    [Header("💰 Sistem Ekonomi (Koin)")]
    [Tooltip("Modal koin awal pemain pas baru main.")]
    public int koinAwal = 100;
    public int koinSaatIni;

    [Tooltip("Tarik objek Text_Koin dari Hierarchy ke sini, King!")]
    public TextMeshProUGUI uiTeksKoin; // Penampung teks koin di layar
    // =======================================================

    [Header("📺 Hubungan UI")]
    [Tooltip("Tarik objek Text_HPBase dari Hierarchy ke sini, King!")]
    public TextMeshProUGUI uiTeksHPBase; 

    [Header("📺 Panel UI Akhir Game")]
    public GameObject panelGameOver;
    public GameObject panelVictory;
    public GameObject tombolRestart; 

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    void Start()
    {
        Time.timeScale = 1f; 
        hpBaseSaatIni = hpBaseMaksimal;
        isGameOver = false;

        // Isi dompet dengan modal awal pas game start
        koinSaatIni = koinAwal;

        UpdateVisualHPBase();
        UpdateVisualKoin(); // Perbarui teks koin di awal game
    }

    public void BaseKenaBobol(int damageMusuh)
    {
        if (isGameOver) return; 

        hpBaseSaatIni -= damageMusuh;
        UpdateVisualHPBase();

        Debug.Log("⚠️ BASE KEBOBOLAN! Sisa HP Base: " + hpBaseSaatIni);

        if (hpBaseSaatIni <= 0)
        {
            hpBaseSaatIni = 0;
            UpdateVisualHPBase(); 
            TriggerGameOver();
        }
    }

    void UpdateVisualHPBase()
    {
        if (uiTeksHPBase != null)
        {
            uiTeksHPBase.text = "BASE HP: " + hpBaseSaatIni + " / " + hpBaseMaksimal;
        }
    }

    // =======================================================
    // FUNGSI EKONOMI: SISTEM MANAJEMEN KOIN
    // =======================================================
    public void TambahKoin(int jumlah)
    {
        koinSaatIni += jumlah;
        UpdateVisualKoin();
        Debug.Log("💰 Dapat Duit! Koin saat ini: " + koinSaatIni);
    }

    public void KurangKoin(int jumlah)
    {
        koinSaatIni -= jumlah;
        UpdateVisualKoin();
        Debug.Log("💸 Koin Berkurang! Sisa koin: " + koinSaatIni);
    }

    void UpdateVisualKoin()
    {
        if (uiTeksKoin != null)
        {
            uiTeksKoin.text = "KOIN: " + koinSaatIni;
        }
    }
    // =======================================================

    void TriggerGameOver()
    {
        isGameOver = true;
        Time.timeScale = 0f; 
        
        if (AudioManager.instance != null)
        {
            AudioManager.instance.PutarBGMGameOver();
        }
        
        if (panelGameOver != null) panelGameOver.SetActive(true);
        StartCoroutine(MunculkanTombolRestart());
    }

    public void TriggerVictory()
    {
        isGameOver = true;
        Time.timeScale = 0f; 

        if (AudioManager.instance != null)
        {
            AudioManager.instance.PutarBGMVictory();
        }

        if (panelVictory != null) panelVictory.SetActive(true);
        StartCoroutine(MunculkanTombolRestart());
    }

    private System.Collections.IEnumerator MunculkanTombolRestart()
    {
        if (tombolRestart != null) tombolRestart.SetActive(false);
        yield return new WaitForSecondsRealtime(2f);
        if (tombolRestart != null) tombolRestart.SetActive(true);
    }

    public void RestartGame()
    {
        Debug.Log("Melakukan Soft Reset Map AR... Bersiap, King!");
        Time.timeScale = 1f;
        isGameOver = false;
        hpBaseSaatIni = hpBaseMaksimal;
        
        // RESET KOIN KEMBALI KE MODAL AWAL PAS RESTART
        koinSaatIni = koinAwal;

        UpdateVisualHPBase();
        UpdateVisualKoin(); // Perbarui teks koin pas reset

        if (panelGameOver != null) panelGameOver.SetActive(false);
        if (panelVictory != null) panelVictory.SetActive(false);
        if (AudioManager.instance != null) AudioManager.instance.PutarLaguUtama();

        GameObject[] rombonganMusuh = GameObject.FindGameObjectsWithTag("Musuh");
        foreach (GameObject musuh in rombonganMusuh)
        {
            Destroy(musuh);
        }

        GameObject[] sisaPeluru = GameObject.FindGameObjectsWithTag("Peluru"); 
        foreach (GameObject peluru in sisaPeluru)
        {
            Destroy(peluru);
        }

        PlatformTower[] semuaPondasi = GameObject.FindObjectsByType<PlatformTower>();
        foreach (PlatformTower pondasi in semuaPondasi)
        {
            pondasi.HancurkanTowerUntukReset();
        }

        WaveSpawner scriptWave = GetComponent<WaveSpawner>();
        if (scriptWave != null)
        {
            scriptWave.ResetWaveManual(); 
        }

        Debug.Log("Soft Reset Berhasil! Map bersih, Vuforia AR tetap aman terjaga! 😎");
    }

    public void PilihTowerDariUI(GameObject prefabTowerBaru)
    {
        modeHapusAktif = false; 
        towerYangDipilih = prefabTowerBaru;
        Debug.Log("Pemain memilih tower: " + prefabTowerBaru.name + ". Siap dipasang di pondasi!");
    }

    [Header("🗑️ Sistem Hapus Tower")]
    public bool modeHapusAktif = false;

    public void AktifkanModeHapus()
    {
        modeHapusAktif = true;
        towerYangDipilih = null; 
        Debug.Log("🧹 Mode Hapus Active! Klik pada tower/pondasi untuk menghancurkannya, King!");
    }
}