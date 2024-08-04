using System;
using UnityEngine;
using UnityEngine.Animations;

public class FireExtinguishProgressController : MonoBehaviour
{
    [Header("Debug")]
    [SerializeField] private bool overrideProgressValue = false;
    [SerializeField][Range(0f, 1f)] private float progress = 0f;

    private Animator _animator;
    FireManager _fireManager;

    void Start()
    {
        _fireManager = FindObjectOfType<FireManager>();
        if (_fireManager == null)
        {
            Debug.LogError($"{nameof(FireExtinguisherController)} Cannot find FireManager component!", this.gameObject);
            this.enabled = false;
            Destroy(this.gameObject);
        }

        _animator = GetComponent<Animator>();
        if (_animator == null )
        {
            Debug.LogError($"{nameof(FireExtinguisherController)} Cannot find Animator component!", this.gameObject);
            this.enabled = false;
            Destroy(this.gameObject);
        }

        _fireManager.OnFireControlled.AddListener(OnFireControlled);
        _fireManager.OnBurned.AddListener(OnFireBurnt);        
    }

    private void OnFireBurnt(FireManager arg0)
    {
        _animator.SetFloat("Progress", 0.99f);
        _animator.Play("Error");        
    }

    private void OnFireControlled(FireManager arg0)
    {
        _animator.SetFloat("Progress", 0f);
        _animator.Play("Ok");
    }

    void Update()
    {
        this.transform.LookAt(Camera.main.transform);

        _animator.SetFloat("Progress", _fireManager.FireMagnitudeNormalized);

#if UNITY_EDITOR
        if (overrideProgressValue)
        {
            _animator.SetFloat("Progress", progress);
        }
#endif        
    }
}
