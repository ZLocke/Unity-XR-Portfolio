using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GestureDebugUI : MonoBehaviour
{
    [SerializeField] TMP_Text palmOrientText;
    [SerializeField] HandOrientationDetector detector;
    [SerializeField] Transform relativeTo;
    [SerializeField] bool isLeft;
    Transform _mainCamera;

    private void Awake()
    {
        if(!relativeTo) relativeTo = Camera.main.transform;
        _mainCamera = Camera.main.transform;
    }
    private void Update()
    {
        transform.LookAt(_mainCamera, Vector3.up);
        transform.Rotate(0, 180f, 0);

        var orientations = detector.Determine(relativeTo);
        palmOrientText.text = string.Join(", ", orientations);
    }  
 
}
