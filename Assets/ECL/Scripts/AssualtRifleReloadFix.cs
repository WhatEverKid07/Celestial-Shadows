using UnityEngine;

public class AssualtRifleReloadFix : MonoBehaviour
{
    public void CallARReload()
    {
        AudioManager.instance.AssaultRifleReload();
    }
}
