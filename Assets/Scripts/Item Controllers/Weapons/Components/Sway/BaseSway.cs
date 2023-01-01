using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(SwayInput))]
public class BaseSway : MonoBehaviour
{
    [SerializeField] private Transform swayTarget;
    [SerializeField] private float swayAmount;
    [SerializeField] private float maxSway;
    [SerializeField] private float swaySmoothing;

    private Vector2 _mouseLook;
    
    private void Update()
    {
        float x = _mouseLook.x * swayAmount * Time.deltaTime;
        float y = _mouseLook.y * swayAmount * Time.deltaTime;

        x = Mathf.Clamp(x, -maxSway, maxSway);
        y = Mathf.Clamp(y, -maxSway, maxSway);

        
        Quaternion swayRot = Quaternion.Euler(y, -x, x);
        
        swayTarget.localRotation = Quaternion.Lerp(swayTarget.localRotation, swayRot, Time.deltaTime * 5f);
    }
    

    public void UpdateInput(Vector2 mouseLook) => _mouseLook = mouseLook;
    public virtual float CalculateSwaySpeed() => swaySmoothing;
}
