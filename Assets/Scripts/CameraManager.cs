using Photon.Pun;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    [SerializeField] private Camera camera1;
    [SerializeField] private Camera camera2;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            camera1.enabled = true;
        }
        else
        {
            camera2.enabled = true;
        }
    }
}
