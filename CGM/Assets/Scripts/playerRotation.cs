using Fusion;
using UnityEngine;

public class playerRotation : NetworkBehaviour
{
    [Networked] private Quaternion networkedRotation { get; set; }

    [SerializeField] private float rotationSpeed = 10f;
    private Camera mainCamera;

    public override void Spawned()
    {
        mainCamera = Camera.main;
    }

    public override void FixedUpdateNetwork()
    {
            if (HasStateAuthority)
            {
                rotateTowardsMouse();
            }
        transform.rotation = Quaternion.Slerp(transform.rotation, networkedRotation, Runner.DeltaTime * rotationSpeed);
    }

    void rotateTowardsMouse ()
    {
        Vector3 mouseScreenPosition = Input.mousePosition;
        Ray ray = mainCamera.ScreenPointToRay(mouseScreenPosition);
        Plane groundPlane = new Plane(Vector3.up, Vector3.zero);
        if (groundPlane.Raycast(ray, out float enter))
        {
            Vector3 mouseWorldPosition = ray.GetPoint(enter);
            Vector3 direction = (mouseWorldPosition - transform.position).normalized;
            direction.y = 0;
            networkedRotation = Quaternion.LookRotation(direction);
        }
    }
   
}


