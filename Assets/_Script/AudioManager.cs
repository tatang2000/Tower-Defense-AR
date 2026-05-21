using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    [Header("🎧 Sumber Suara (Masukin Komponen)")]
    public AudioSource musicSource; 
    public AudioSource sfxSource;   

    [Header("🎵 File Lagu BGM")]
    [Tooltip("Tarik file lagu MP3 utama kamu ke sini, King!")]
    public AudioClip laguBGM; 
    
    // =======================================================
    // FITUR BARU: SLOT LAGU GAME OVER & VICTORY
    // =======================================================
    [Tooltip("Lagu pas base hancur (Sedih)")]
    public AudioClip laguGameOver; 
    [Tooltip("Lagu pas semua wave tamat (Heroik)")]
    public AudioClip laguVictory; 

    void Awake()
    {
        if (instance == null) instance = this;
    }

    void Start()
    {
        PutarLaguUtama();
    }

    // Fungsi kita pisah biar gampang dipanggil pas RESTART game
    public void PutarLaguUtama()
    {
        if (musicSource != null && laguBGM != null)
        {
            musicSource.clip = laguBGM;
            musicSource.loop = true; // Muter terus
            musicSource.Play();
        }
    }

    // Fungsi dipanggil pas kalah
    public void PutarBGMGameOver()
    {
        if (musicSource != null && laguGameOver != null)
        {
            musicSource.Stop(); // Matiin lagu utama
            musicSource.clip = laguGameOver;
            musicSource.loop = false; // Biar dramatis bunyi sekali aja
            musicSource.Play();
        }
    }

    // Fungsi dipanggil pas menang
    public void PutarBGMVictory()
    {
        if (musicSource != null && laguVictory != null)
        {
            musicSource.Stop(); // Matiin lagu utama
            musicSource.clip = laguVictory;
            musicSource.loop = false; 
            musicSource.Play();
        }
    }

    public void PutarSFX(AudioClip klipSuara)
    {
        if (sfxSource != null && klipSuara != null)
        {
            sfxSource.PlayOneShot(klipSuara);
        }
    }
}