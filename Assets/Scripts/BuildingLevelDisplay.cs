using System;
using UnityEngine;
using TMPro;

public class BuildingLevelDisplay : MonoBehaviour
{
    [SerializeField] private Building building;
    [SerializeField] private TMP_Text levelText;

    [SerializeField] private Camera mainCamera;

    private void Awake()
    {

    }

    private void Start()
    {
        building.OnLevelChanged += UpdateText;
        building.OnOwnerChanged += OwnerChanged;
        mainCamera = Camera.main;
        transform.LookAt(transform.position + mainCamera.transform.rotation * Vector3.forward,
            mainCamera.transform.rotation * Vector3.up);
        UpdateText(building.Level);
    }

    private void OwnerChanged()
    {
        if (building.Owner == null)
        {
            levelText.color = Color.white;
            return;
        }
        levelText.color = building.Owner.PlayerColor;
    }

    private void LookAtCamera(Camera newCamera)
    {
        transform.LookAt(transform.position + newCamera.transform.rotation * Vector3.forward,
            newCamera.transform.rotation * Vector3.up);
    }

    private void UpdateText(int newLevel)
    {
        levelText.text = "Lvl: " + newLevel;
    }
}