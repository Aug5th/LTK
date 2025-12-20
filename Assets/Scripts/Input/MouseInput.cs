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
            Collider2D hit = Physics2D.OverlapPoint(mousePos);

            if (hit != null)
            {
                var dam = hit.GetComponent<IDamagable>();
                if (dam != null)
                {
                    OnRightClickTarget?.Invoke(dam);
                    return;
                }

                OnRightClick?.Invoke(mousePos);
            }
        }
    }
}
