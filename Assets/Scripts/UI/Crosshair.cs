using Fusion;
using UnityEngine;
using UnityEngine.UI;

public class Crosshair : SimulationBehaviour
{
    [SerializeField] private GameObject crossHair;
    [SerializeField] private bool disableOnSize = true;
    [SerializeField] private RectTransform[] rectTransform;


    private float _currentSize;
    private float _walkSize;

    private void Awake()
    {
        _walkSize = rectTransform[0].localPosition.y;
    }

    private void Update()
    {
        if (NetworkPlayer.Local == null)
        {
            return;
        }
        
        CalculateCrosshairPosition();
    }
    

    private void CalculateCrosshairPosition()
    {
        ItemController currentWeapon = NetworkPlayer.Local.Inventory.EquippedItem;
        
        _currentSize = _walkSize * currentWeapon.crossHairSize;

        if (disableOnSize)
            crossHair.SetActive(currentWeapon.crossHairSize > 0 || !NetworkPlayer.Local.Inventory.IsOpen);

        if (!NetworkPlayer.Local.Movement.IsMoving)
        {
            _currentSize = _walkSize * currentWeapon.crossHairSize / 1.5f;
        }

        if (NetworkPlayer.Local.Movement.IsSprinting)
        {
            _currentSize = _walkSize * currentWeapon.crossHairSize * 1.5f;
        }
        
        if (currentWeapon.GetService<ProceduralAim>() != null)
        {
            bool isAiming = currentWeapon.GetService<ProceduralAim>().IsAiming;
            
            if (isAiming)
            {
                _currentSize = 0;
            }

            UpdatePositions(_currentSize, isAiming);
        }
    }

    public void ShowCrosshair()
    {
        crossHair.SetActive(true);
        Invoke(nameof(ResetCrosshair), 0.5f);
    }

    private void ResetCrosshair()
    {
        crossHair.SetActive(false);
    }

    private void UpdatePositions(float target, bool isAiming)
    {
        rectTransform [0].localPosition = Vector3.Slerp (rectTransform [0].localPosition, new Vector3 (0f, target, 0f), Time.deltaTime * 3.5f);
        rectTransform [1].localPosition = Vector3.Slerp (rectTransform [1].localPosition, new Vector3 (0f, -target, 0f), Time.deltaTime * 3.5f);
        rectTransform [2].localPosition = Vector3.Slerp (rectTransform [2].localPosition, new Vector3 (target, 0f, 0f), Time.deltaTime * 3.5f);
        rectTransform [3].localPosition = Vector3.Slerp (rectTransform [3].localPosition, new Vector3 (-target, 0f, 0f), Time.deltaTime * 3.5f);

        ItemController currentWeapon = NetworkPlayer.Local.Inventory.EquippedItem;

        if(isAiming && rectTransform[0].GetComponent <Image>().color.a > 0)
        {
            foreach(RectTransform rect in rectTransform)
            {
                rect.GetComponent <Image>().color -= new Color (0f, 0f, 0f, 256f * Time.deltaTime * 5.0f);
            }
        }
        else if(!isAiming && rectTransform[0].GetComponent <Image>().color.a < 256)
        {
            foreach(RectTransform rect in rectTransform)
            {
                rect.GetComponent <Image>().color += new Color (0f, 0f, 0f, 256f * Time.deltaTime * 5.0f);
            }
        }
    }
    
}
