using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseInput : MonoBehaviour
{
    public static event Action<Vector3> OnRightClick;
    public static event Action<IDamagable> OnRightClickTarget;

    void Update()
    {
        OnMouseRightClick();
    }

    void OnMouseRightClick()
    {
       if (Input.GetMouseButtonDown(1)) // right mouse
        {
            if (Camera.main == null) return;

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                var dam = hit.collider.GetComponent<IDamagable>();
                if (dam != null)
                {
                    OnRightClickTarget?.Invoke(dam);
                    return;
                }

                OnRightClick?.Invoke(hit.point);
            }
        }
    }
}
