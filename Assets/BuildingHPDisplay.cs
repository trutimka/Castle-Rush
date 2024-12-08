using System;
using UnityEngine;
using TMPro;


public class BuildingHPDisplay : MonoBehaviour
{
    [SerializeField] private Building _building;
    [SerializeField] private TMP_Text _hpText;
    
    [SerializeField] private Camera mainCamera;

    private void Awake()
    {
        _building.OnHealthChanged += BuildingOnOnHealthChanged;
        _building.OnOwnerChanged += BuildingOnOnOwnerChanged;
        
        mainCamera = Camera.main;
    }
    
    private void Start()
    {
        transform.LookAt(transform.position + mainCamera.transform.rotation * Vector3.forward,
            mainCamera.transform.rotation * Vector3.up);
    }

    private void BuildingOnOnOwnerChanged()
    {
        if (_building.Owner == null)
        {
            _hpText.color = Color.white;
            return;
        }
        _hpText.color = _building.Owner.PlayerColor;
    }

    private void BuildingOnOnHealthChanged(int hp)
    {
        _hpText.text = "HP: " + hp;
    }
}