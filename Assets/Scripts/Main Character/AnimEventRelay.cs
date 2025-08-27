using UnityEngine;

public class AnimEventRelay : MonoBehaviour
{
    [SerializeField] private PlayerChop receiver; // el script que está en el padre

    public void AE_TalarEnd()
    {
        Debug.Log("[AnimEventRelay] AE_TalarEnd recibido");
        receiver?.AE_TalarEnd();
    }

}