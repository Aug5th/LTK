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

            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero);

            if (hit.collider != null)
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
