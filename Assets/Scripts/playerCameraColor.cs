using UnityEngine;
using Fusion;

public class playerCameraColor : NetworkBehaviour
{
    public Camera p1Camera;
    public Camera p2Camera;

    public override void Spawned()
    {
        if (Object.HasStateAuthority)
        {
            setupCamera();
        }
    }

    private void setupCamera()
    {
        bool isPlayerOne = Runner.LocalPlayer == PlayerRef.None;

        p1Camera.gameObject.SetActive(isPlayerOne);
        p2Camera.gameObject.SetActive(!isPlayerOne);

        if (isPlayerOne)
        {
            p2Camera.enabled = false;
        }
        else
        {
            p1Camera.enabled = false;
        }
    }
}
