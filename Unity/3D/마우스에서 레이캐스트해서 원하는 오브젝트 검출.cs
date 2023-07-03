using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NavPlayerController : MonoBehaviour
{
    private NavManager navManager;

    private void Awake()
    {
        navManager = GetComponent<NavManager>();
    }

    private void Update()
    {
        if (Input.GetMouseButton(0))
        {
            RaycastHit hit;

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit, Mathf.Infinity))
            {

                navManager.OnMove(hit.point);
            }
        }
    }
}
