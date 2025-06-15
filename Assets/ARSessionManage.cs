using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class ARSessionManage : MonoBehaviour
{
    [SerializeField] private ARSession arSession;

    private void OnEnable()
    {
        StartCoroutine(ResetARSession());
    }

    private IEnumerator ResetARSession()
    {
        if (arSession != null)
        {
            arSession.Reset();
            yield return null;
        }
    }
}
