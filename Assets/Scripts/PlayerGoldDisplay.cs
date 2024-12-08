using System;
using UnityEngine;
using TMPro;

public class PlayerGoldDisplay : MonoBehaviour
{
    [SerializeField] private TMP_Text goldText;

    [SerializeField] private Camera mainCamera;
    private Player player;
    private void Awake()
    {
        mainCamera = Camera.main;
        player = mainCamera.GetComponent<Player>();
        player.OnGoldChanged += UpdateText;
    }

    private void Start()
    {
        transform.LookAt(transform.position + mainCamera.transform.rotation * Vector3.forward,
            mainCamera.transform.rotation * Vector3.up);
    }

    private void LookAtCamera(Camera newCamera)
    {
        transform.LookAt(transform.position + newCamera.transform.rotation * Vector3.forward,
            newCamera.transform.rotation * Vector3.up);
    }

    private void UpdateText(float gold)
    {
        goldText.text = $"Gold: {gold:F1}";;
    }
}