using Photon.Pun;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    [SerializeField] private Camera camera1;
    [SerializeField] private Camera camera2;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            camera1.enabled = true;
            camera1.GetComponentInChildren<PlayerGoldDisplay>().enabled = true;
        }
        else
        {
            camera2.enabled = true;
            camera2.GetComponentInChildren<PlayerGoldDisplay>().enabled = true;
        }
    }

    public int GetPlayerNumber(Player player)
    {
        if (camera1.GetComponent<Player>() == player)
        {
            return 1;
        }

        if (camera2.GetComponent<Player>() == player)
        {
            return 2;
        }

        return 0;
    }

    public Player GetPlayer(int number)
    {
        if (number == 1) return camera1.GetComponent<Player>();
        if (number == 2) return camera2.GetComponent<Player>();
        return null;
    }
}
