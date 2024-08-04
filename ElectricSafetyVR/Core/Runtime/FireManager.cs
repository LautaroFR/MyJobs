using Assets._VRN.Core.Runtime.Utils;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering.Universal;

public class FireManager : MonoBehaviour
{
    private bool _fireStarted;
    private float _fireMagnitude;
    private bool _wasControlled = false;

    [Header("Fire parameters")]
    [SerializeField][Min(0.01f)] private float initialFireMagnitude = 0.1f;
    [SerializeField] private AnimationCurve fireCurve;
    [SerializeField][Min(1f)] private float timeToMaxLevel = 40f;

    [Header("Particle system")]
    [SerializeField] private ParticleSystem particles;
    [SerializeField] private float minStartSizeValue = 1f;
    [SerializeField] private float maxStartSizeValue = 4f;
    [SerializeField] private float minEmissionValue = 20f;
    [SerializeField] private float maxEmissionValue = 30f;

    [Header("Decal")]
    [SerializeField] private DecalProjector fireDecal;
    [SerializeField] private float minDecalSize = 0.5f;
    [SerializeField] private float maxDecalSize = 1.0f;
    private float _maxDecalSizeUsed = 0f;

    [Header("Sound")]
    [SerializeField] private AudioSource fireAudioSource;
    [SerializeField] private float minFireVolume = 0.1f;
    [SerializeField] private float maxFireVolume = 0.4f;

    [Header("Events")]
    public UnityEvent<FireManager> OnFireControlled = new UnityEvent<FireManager>();
    public UnityEvent<FireManager> OnBurned = new UnityEvent<FireManager>();

    public float FireMagnitudeNormalized => _fireMagnitude / timeToMaxLevel;

    private void Awake()
    {
        _fireStarted = false;
        fireDecal.enabled = false;
        fireAudioSource.Stop();
    }

    private void Start()
    {
        StartFire();
    }

    void Update()
    {
        var currentFireLevel = 0f;

        if (_fireStarted)
        {
            currentFireLevel = fireCurve.Evaluate(_fireMagnitude / timeToMaxLevel);

            if (Mathf.Approximately(_fireMagnitude, 0f))
            {
                _wasControlled = true;
                StopFire();
            }

            if (Mathf.Approximately(_fireMagnitude, timeToMaxLevel))
            {
                this.OnBurned.Invoke(this);
            }

            _fireMagnitude = Mathf.Clamp(_fireMagnitude + Time.deltaTime, 0f, timeToMaxLevel);
        }

        var particlesMain = particles.main;

        var startSizeX = particles.main.startSizeX;
        startSizeX.constantMax = Mathf.Lerp(minStartSizeValue, maxStartSizeValue, currentFireLevel);
        particlesMain.startSizeX = startSizeX;

        var startSizeY = particles.main.startSizeY;
        startSizeY.constantMax = Mathf.Lerp(minStartSizeValue, maxStartSizeValue, currentFireLevel);
        particlesMain.startSizeY = startSizeY;

        var startSizeZ = particles.main.startSizeZ;
        startSizeZ.constantMax = Mathf.Lerp(minStartSizeValue, maxStartSizeValue, currentFireLevel);
        particlesMain.startSizeZ = startSizeZ;

        var particlesEmission = particles.emission;
        particlesEmission.rateOverTime = Mathf.Lerp(minEmissionValue, maxEmissionValue, currentFireLevel);

        //Decal
        _maxDecalSizeUsed = Mathf.Max(_maxDecalSizeUsed, Mathf.Lerp(minDecalSize, maxDecalSize, currentFireLevel));
        fireDecal.size = new Vector3(_maxDecalSizeUsed, _maxDecalSizeUsed, fireDecal.size.z);

        //Sound volume
        fireAudioSource.volume = Mathf.Lerp(minFireVolume, maxFireVolume, currentFireLevel);
    }

    public void StartFire()
    {
        _fireStarted = true;
        _wasControlled = false;
        _fireMagnitude = initialFireMagnitude;
        particles.Play();
        fireAudioSource.Play();
        fireDecal.enabled = true;
    }

    private void StopFire()
    {
        _fireStarted = false;
        particles.Stop();
        fireAudioSource.Stop();
    }

    public void ReduceFire(float factor)
    {
        _fireMagnitude -= factor;
        _fireMagnitude = Mathf.Max(_fireMagnitude, 0f);
    }

    private void OnParticleSystemStopped()
    {
        if (_wasControlled)
        {
            this.OnFireControlled.Invoke(this);
        }
    }
}
