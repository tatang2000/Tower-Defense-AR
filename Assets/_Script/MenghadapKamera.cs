using UnityEngine;

public class MenghadapKamera : MonoBehaviour
{
    private Camera kameraUtama;

    void Start()
    {
        kameraUtama = Camera.main;
    }

    void LateUpdate()
    {
        if (kameraUtama != null)
        {
            // Bikin Canvas selalu menatap ke arah yang sama dengan kamera HP
            transform.LookAt(transform.position + kameraUtama.transform.forward);
        }
    }
}