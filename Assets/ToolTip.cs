using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToolTip : MonoBehaviour
{
    [SerializeField] private GameObject toolTip1;
    [SerializeField] private GameObject toolTip2;

    private void Start()
    {
        ToolTip1();
        toolTip2.SetActive(false);

        Invoke(nameof(ToolTip2), 10f);
    }

    private void ToolTip1()
    {
        toolTip1.SetActive(true);
        Destroy(toolTip1, 10f);
    }

    private void ToolTip2()
    {
        toolTip2.SetActive(true);
        Destroy(toolTip2, 10f);
    }
}
