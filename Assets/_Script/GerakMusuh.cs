using UnityEngine;
using UnityEngine.UI; // 🚨 JANGAN LUPA INI, KING! Wajib buat mengontrol UI Image Darah

public class GerakMusuh : MonoBehaviour
{
    [HideInInspector] public Transform[] ruteWaypoint; 
    
    [Header("⚙️ Pengaturan Jalan Musuh (AR)")]
    [Tooltip("Kecepatan lari musuh. Geser slider untuk mengatur!")]
    [Range(0.01f, 1f)] public float kecepatanJalan = 0.1f; 

    [Header("💰 Hadiah Koin")]
    [Tooltip("Jumlah koin yang didapat pemain kalau musuh ini mati.")]
    public int hadiahKoin = 10;
    
    [Tooltip("Seberapa mulus musuh berbelok di tikungan.")]
    [Range(1f, 20f)] public float kecepatanBelok = 10f;
    
    [Tooltip("Jarak deteksi ke titik belokan (biarkan default).")]
    public float jarakToleransi = 0.01f; 
    
    [Header("❤️ Sistem Darah (HP) Musuh")]
    [Tooltip("Berapa kali hantaman damage yang kuat diterima Rehman sebelum mati.")]
    [Range(1f, 100f)] public float hpMaksimal = 3f; 
    private float hpSaatIni;

    // =======================================================
    // SLOT BARU: UNTUK MENAMPUNG VISUAL BAR HIJAU DI KEPALA REHMAN
    // =======================================================
    [Header("🩸 Pengaturan Darah (UI)")]
    [Tooltip("Tarik objek 'IsiDarah' hijau yang ada di bawah Canvas kepala Rehman ke sini, King!")]
    public Image barIsiDarah; 
    // =======================================================

    private int indexTarget = 1; 

    [Header("🎨 Pengaturan Visual")]
    [Tooltip("Masukkan objek Cube.051 Rehman di sini untuk ubah warna.")]
    public Renderer meshRendererMusuh; 

    void Start()
    {
        hpSaatIni = hpMaksimal; // Isi penuh saat lahir

        // Memastikan bar visual hijaunya penuh (100%) pas musuh baru spawn
        if (barIsiDarah != null)
        {
            barIsiDarah.fillAmount = 1f;
        }
    }

    public void KenaDamage(float jumlahDamage)
    {
        hpSaatIni -= jumlahDamage;

        // =======================================================
        // FITUR BARU: UPDATE VISUAL BAR DARAH HIJAU SETIAP KENA HIT!
        // =======================================================
        if (barIsiDarah != null)
        {
            // Nilai fillAmount wajib berupa pecahan antara 0.0 sampai 1.0
            barIsiDarah.fillAmount = hpSaatIni / hpMaksimal;
        }
        // =======================================================

        if (hpSaatIni <= 0)
        {
            // FITUR EKONOMI: SETOR KOIN KE DOMPET SEBELUM MUSUH HANCUR!
            if (GameManager.instance != null)
            {
                GameManager.instance.TambahKoin(hadiahKoin);
            }
            Destroy(gameObject);
        }
    }

    public void SetWarna(Material warnaBaru)
    {
        if (meshRendererMusuh != null && warnaBaru != null)
        {
            meshRendererMusuh.material = warnaBaru;
        }
    }

    void Update()
    {
        if (ruteWaypoint == null || ruteWaypoint.Length == 0) return;

        Transform targetWP = ruteWaypoint[indexTarget];

        // 1. Jalan menuju target
        transform.position = Vector3.MoveTowards(transform.position, targetWP.position, kecepatanJalan * Time.deltaTime);

        // 2. Belok menghadap target
        Vector3 arah = targetWP.position - transform.position;
        arah.y = 0; 
        
        if (arah != Vector3.zero)
        {
            Quaternion rotasiTujuan = Quaternion.LookRotation(arah);
            transform.rotation = Quaternion.Slerp(transform.rotation, rotasiTujuan, kecepatanBelok * Time.deltaTime);
        }

        // 3. Cek jarak
        Vector2 posMusuh2D = new Vector2(transform.position.x, transform.position.z);
        Vector2 posWP2D = new Vector2(targetWP.position.x, targetWP.position.z);

        if (Vector2.Distance(posMusuh2D, posWP2D) < jarakToleransi)
        {
            indexTarget++;
            
            if (indexTarget >= ruteWaypoint.Length)
            {
                Debug.Log("Duh, musuh masuk Base!");

                if (GameManager.instance != null)
                {
                    GameManager.instance.BaseKenaBobol(1); 
                }

                Destroy(gameObject); 
            }
        }
    }
}