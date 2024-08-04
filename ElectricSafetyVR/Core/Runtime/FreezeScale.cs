using UnityEngine;

public class FreezeScale : MonoBehaviour
{
    [SerializeField]
    private bool freezeScale;

    private Vector3 _initialScale;

    void Start()
    {
        _initialScale = transform.localScale;
    }

    void Update()
    {
        if (!freezeScale)
            return;

        transform.localScale = _initialScale;
    }
}
