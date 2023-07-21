using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(TextMesh))]
public class FlickerDisplay : MonoBehaviour
{
    private TextMesh _mesh;

    public string Text {
        get { return _mesh.text; }
        set { _mesh.text = value; }
    }

#pragma warning disable IDE0051
    private void Awake() {
        _mesh = GetComponent<TextMesh>();
    }
#pragma warning restore IDE0051
}
