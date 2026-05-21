using UnityEngine;

public class PlatformTower : MonoBehaviour
{
    [Header("Status Platform")]
    [Tooltip("Menyimpan data tower yang lagi berdiri di pondasi ini.")]
    private GameObject towerYangTerpasang; 

    [Header("✨ Efek Visual (VFX)")]
    public GameObject efekBangun; // Masukin partikel debu/palu
    public GameObject efekHapus;  // Masukin partikel asap lenyap

    void OnMouseDown()
    {
        // PENGAMAN: Jika game lagi freeze (Game Over), jangan izinkan bongkar pasang tower
        if (Time.timeScale == 0f) return;
        if (GameManager.instance == null) return;

        // =======================================================================
        // 1. JALUR MODE HAPUS (REMOVE TOWER)
        // =======================================================================
        if (GameManager.instance.modeHapusAktif)
        {
            if (towerYangTerpasang != null)
            {
                // MUNCULIN EFEK ASAP PAS TOWER DIHANCURKAN MANUAL
                if (efekHapus != null)
                {
                    GameObject vfxAsap = Instantiate(efekHapus, transform.position, Quaternion.identity);
                    Destroy(vfxAsap, 2f);
                }

                Destroy(towerYangTerpasang); 
                towerYangTerpasang = null; 
                Debug.Log("Tower berhasil dihapus pakai tombol Remove!");
            }
            else
            {
                Debug.Log("Pondasi ini emang udah kosong, King!");
            }
            
            return; // STOP DI SINI! Jangan lanjut ke bawah karena kita cuma mau ngehapus
        }


        // =======================================================================
        // 2. JALUR MODE BANGUN TOWER
        // =======================================================================
        if (GameManager.instance.towerYangDipilih == null)
        {
            Debug.LogWarning("Waduh king, pilih towernya dulu di tombol UI kaca kamu!");
            return; 
        }

        if (towerYangTerpasang == null) 
        {
            // Ambil info komponen dari prefab yang lagi dipilih di toko
            TowerNembak scriptInfoTower = GameManager.instance.towerYangDipilih.GetComponent<TowerNembak>();
            
            if (scriptInfoTower != null)
            {
                // Cek apakah koin kita saat ini cukup buat bayar harga tower tersebut
                if (GameManager.instance.koinSaatIni >= scriptInfoTower.hargaTower)
                {
                    // Transaksi Sukses! Potong duit di dompet GameManager
                    GameManager.instance.KurangKoin(scriptInfoTower.hargaTower);

                    // Lahirkan towernya seperti biasa
                    GameObject towerBaru = Instantiate(GameManager.instance.towerYangDipilih, transform.position, transform.rotation);
                    towerYangTerpasang = towerBaru; 
                    towerYangTerpasang.transform.SetParent(this.transform.parent, true);
                    GameManager.instance.towerYangDipilih = null; // Reset belanjaan

                    // MUNCULIN EFEK PALU / DEBU PAS BANGUN TOWER
                    if (efekBangun != null)
                    {
                        GameObject vfxBangun = Instantiate(efekBangun, transform.position, Quaternion.identity);
                        Destroy(vfxBangun, 2f);
                    }
                }
                else
                {
                    Debug.LogWarning("Duit seret, King! Koin kamu gak cukup buat beli menara ini!");
                }
            }
        }
        else
        {
            Debug.Log("Pondasi ini sudah ada towernya, king!");
        }
    }

    // Trik rahasia untuk dipanggil oleh GameManager saat reset game
    public void HancurkanTowerUntukReset()
    {
        if (towerYangTerpasang != null)
        {
            // MUNCULIN EFEK ASAP PAS TOWER DIHANCURKAN (SAAT RESTART)
            if (efekHapus != null)
            {
                GameObject vfxAsap = Instantiate(efekHapus, transform.position, Quaternion.identity);
                Destroy(vfxAsap, 2f);
            }
            
            Destroy(towerYangTerpasang); 
            towerYangTerpasang = null;  
        }
    }
}