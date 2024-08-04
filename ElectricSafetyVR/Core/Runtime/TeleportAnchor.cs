using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class TeleportAnchor : MonoBehaviour
{
    [SerializeField]
    private Mesh torusMesh;

    [SerializeField]
    private Mesh pointMesh;

    [SerializeField]
    private MeshFilter mesh;

    private TeleportationAnchor _setup;

    private void Awake()
    {
        _setup = GetComponent<TeleportationAnchor>();
        _setup.hoverEntered.AddListener((x) => MeshSetup(true));
        _setup.hoverExited.AddListener((x)  => MeshSetup(false));
    }
    private void Start() => MeshSetup(false);

    public void MeshSetup(bool isHovering)
    {
        var meshObject = mesh.gameObject.transform;

        if (isHovering)
        {
            mesh.sharedMesh = torusMesh;
            meshObject.rotation = Quaternion.Euler(90f, 0f, 0f);
            meshObject.localScale = new Vector3(0.5f, 0.5f, 0.1f);
        }

        else
        {
            mesh.sharedMesh = pointMesh;
            meshObject.rotation = Quaternion.Euler(0f, 0f, 0f);
            meshObject.localScale = new Vector3(0.1f, 0.1f, 0.1f);
        }
    }
}
