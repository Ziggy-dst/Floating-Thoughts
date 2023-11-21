using System;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DynamicCollider : MonoBehaviour
{
    private TMP_Text textMesh;
    [HideInInspector]
    public List<GameObject> colliders;

    void Awake()
    {
        textMesh = GetComponent<TMP_Text>();
        TextInput.OnTextInput += UpdateColliders;
        colliders = new List<GameObject>();
    }

    private void OnDestroy()
    {
        TextInput.OnTextInput -= UpdateColliders;
    }

    void UpdateColliders()
    {
        string[] lines = textMesh.text.Split('\n');

        while (colliders.Count > lines.Length)
        {
            GameObject colliderToRemove = colliders[colliders.Count - 1];
            colliders.RemoveAt(colliders.Count - 1);
            Destroy(colliderToRemove);
            lines = textMesh.text.Split('\n');
        }

        while (lines.Length > colliders.Count)
        {
            GameObject newCollider = CreateCollider("ColliderLine_" + colliders.Count);
            newCollider.tag = "Ground";
            newCollider.transform.SetParent(textMesh.transform);
            colliders.Add(newCollider);
            lines = textMesh.text.Split('\n');
        }

        UpdateColliderTransform(lines);
    }

    void UpdateColliderTransform(string[] lines)
    {
        for (int i = 0; i < lines.Length; i++)
        {
            float width = textMesh.GetPreferredValues(lines[i]).x;
            float height = textMesh.fontSize;

            Vector3 position = textMesh.transform.position + new Vector3(0, -height * i, 0);

            colliders[i].transform.position = position;
            BoxCollider2D collider = colliders[i].GetComponent<BoxCollider2D>();
            collider.size = new Vector2(width, height);
        }
    }

    GameObject CreateCollider(string name)
    {
        GameObject colliderObject = new GameObject(name);
        colliderObject.AddComponent<BoxCollider2D>();
        if (!tag.Equals("Player")) colliderObject.layer = LayerMask.NameToLayer("Wall");
        return colliderObject;
    }
}
