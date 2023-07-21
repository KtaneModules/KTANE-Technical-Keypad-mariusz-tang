using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(TextMesh))]
public class FlickerDisplay : MonoBehaviour {

    private TextMesh _mesh;

    public string Text {
        get { return _mesh.text; }
        set { _mesh.text = value; }
    }

    private void Awake() {
        _mesh = GetComponent<TextMesh>();
    }
}
