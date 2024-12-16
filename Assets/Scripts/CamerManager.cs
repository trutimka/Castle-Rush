using Photon.Pun;
using Unity.VisualScripting;
using UnityEngine;

public class CamerManager : MonoBehaviour
{
    [SerializeField] private Camera camera1;
    [SerializeField] private Camera camera2;
    void Start()
    {
        UnityEngine.Debug.Log(PhotonNetwork.IsMasterClient);
        if (PhotonNetwork.IsMasterClient)
        {
            camera1.gameObject.SetActive(true);
        }
        else
        {
            camera2.gameObject.SetActive(true);
        }
    }

}
