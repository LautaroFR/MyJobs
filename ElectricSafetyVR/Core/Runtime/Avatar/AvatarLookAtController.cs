namespace ISVR.Avatar
{
    using CrazyMinnow.SALSA;
    using UnityEngine;

    public class AvatarLookAtController : MonoBehaviour
    {
        [SerializeField] private bool LookAtPlayerMainCamera = true;
        [SerializeField] private Transform LookAtTarget;
        [SerializeField] private float LookAtFOV = 60f;

        private Vector3 _avatarForward;
        private Eyes _eyes;

        private bool _isInsideFOV = false;
        
        // Start is called before the first frame update
        void Start()
        {
            _avatarForward = transform.forward;
            _eyes = GetComponent<Eyes>();

            if (LookAtPlayerMainCamera)
            {
                LookAtTarget = Camera.main.transform;
            }
        }

        // Update is called once per frame
        void Update()
        {            
            // TODO: Calcular cual es el vector forward efectivo del avatar cuando tiene una animaci�n de fondo
            var currentAngle = Vector3.Angle(_avatarForward, (LookAtTarget.position - transform.position).normalized);
            _isInsideFOV = currentAngle < LookAtFOV * 0.5f;
            _eyes.lookTarget = _isInsideFOV ? LookAtTarget : null;
        }

        private void OnDrawGizmos()
        {
            if (Application.isPlaying)
            {
                if (LookAtTarget != null)
                {
                    
                    Gizmos.color = _isInsideFOV ? Color.green : Color.white;
                    Gizmos.DrawLine(transform.position, LookAtTarget.position);                    
                }

                Vector3 forward = transform.forward;
                Vector3 origin = transform.position;

                float fovAngle = LookAtFOV;
                float maxDistance = 10f;

                // Calcular la mitad del �ngulo de apertura del cono
                float halfFovRadians = fovAngle * Mathf.Deg2Rad / 2f;

                // Calcular la direcci�n del vector superior del cono
                Vector3 topDirection = Quaternion.AngleAxis(halfFovRadians, transform.right) * forward;

                // Dibujar l�neas desde el origen hasta los extremos del cono
                Gizmos.color = Color.yellow;
                Gizmos.DrawRay(origin, forward * maxDistance); // Rayo hacia adelante
                Gizmos.DrawRay(origin, Quaternion.AngleAxis(fovAngle / 2f, transform.up) * forward * maxDistance); // Rayo derecho del cono
                Gizmos.DrawRay(origin, Quaternion.AngleAxis(-fovAngle / 2f, transform.up) * forward * maxDistance); // Rayo izquierdo del cono

                // Dibujar l�neas desde el origen hasta la punta del cono
                Gizmos.color = Color.red;
                Gizmos.DrawRay(origin, topDirection * maxDistance); // Rayo superior del cono

                // Dibujar l�neas para conectar los extremos del cono
                Gizmos.color = Color.blue;
                Gizmos.DrawLine(origin + forward * maxDistance, origin + topDirection * maxDistance); // L�nea superior del cono
                Gizmos.DrawLine(origin + forward * maxDistance, origin + Quaternion.AngleAxis(fovAngle / 2f, transform.up) * forward * maxDistance); // L�nea derecha del cono
                Gizmos.DrawLine(origin + forward * maxDistance, origin + Quaternion.AngleAxis(-fovAngle / 2f, transform.up) * forward * maxDistance); // L�nea izquierda del cono
            }
        }
    }
}