using UnityEngine;
using UnityEngine.Localization.Components;

public class EPPEvaluatorUI : MonoBehaviour
{
    [SerializeField] private GameObject itemPrefab;
    
    [Space]

    [Header("List containers")]
    [SerializeField] private Transform correctListContainer;
    [SerializeField] private Transform wrongListContainer;
    [SerializeField] private Transform missingListContainer;

    [Space]

    [Header("Materials")]
    public Material alternativeMaterial;
    public Material missingMaterial;
    public Material wrongMaterial;

    private void Start()
    {
        var evaluator = FindObjectOfType<EPPEvaluator>();

        var alternativeMaterial = InstanciateMaterial(this.alternativeMaterial);
        var missingMaterial = InstanciateMaterial(this.missingMaterial);
        var wrongMaterial = InstanciateMaterial(this.wrongMaterial);

        evaluator.onShowResult += () =>
        {
            foreach (var epp in evaluator.correct)
            {
                InstanciateEppEvaluationItem(epp, correctListContainer);
            }

            foreach (var epp in evaluator.alternatives)
            {
                InstanciateEppEvaluationItem(epp, correctListContainer);
            }

            foreach (var epp in evaluator.missing)
            {
                InstanciateEppEvaluationItem(epp, missingListContainer);
                SetMaterials(epp.Outline.Renderers, missingMaterial);
            }

            foreach (var epp in evaluator.wrong)
            {
                InstanciateEppEvaluationItem(epp, wrongListContainer);
                SetMaterials(epp.Outline.Renderers, wrongMaterial);
            }
        };
    }

    private Material InstanciateMaterial(Material original)
    {
        var ins = new Material(original);
        ins.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
        ins.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
        ins.SetInt("_ZWrite", 1);
        ins.SetFloat("_Surface", 1);
        ins.SetInt("_Blend", 0);
        ins.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Transparent;
        ins.SetShaderPassEnabled("DepthOnly", false);
        ins.SetShaderPassEnabled("SHADOWCASTER", true);
        ins.SetOverrideTag("RenderType", "Transparent");
        ins.EnableKeyword("_SURFACE_TYPE_TRANSPARENT");
        return ins;
    }

    private static void SetMaterials(Renderer[] renderers, Material material)
    {
        for (int ri = 0; ri < renderers.Length; ri++)
        {
            var renderer = renderers[ri];
            renderer.sharedMaterial = material;
        }
    }

    private GameObject InstanciateEppEvaluationItem(EPPItem epp, Transform list)
    {
        var item = Instantiate(itemPrefab, list);
        item.SetActive(true);
        item.name = $"Item {epp.name}";

        var localizeStringEvent = item.GetComponentInChildren<LocalizeStringEvent>();
        localizeStringEvent.StringReference = epp.socket.displayName;

        return item;
    }
}
