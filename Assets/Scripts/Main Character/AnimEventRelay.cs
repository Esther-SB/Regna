using UnityEngine;

public class AnimEventRelay : MonoBehaviour
{
    [SerializeField] private PlayerToolUser receiver; // el script que está en el padre

    public void AE_ToolActionEnd()
    {
        Debug.Log("[AnimEventRelay] AE_TalarEnd recibido");
        receiver?.AE_ToolActionEnd();
    }

}