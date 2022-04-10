using UnityEngine;

[RequireComponent(typeof(Collider))]
public class ActivityArea: MonoBehaviour
{
    public Activity activity;
    public new Collider collider;

    void OnValidate() {
        if (collider == null)
            collider = GetComponent<Collider>();

        collider.isTrigger = true;
    }

    void OnTriggerEnter(Collider other) {
        if (other.TryGetComponent(out PlayerActivity playerActivity))
            playerActivity.SwitchActivity(activity);
    }
	
    void OnTriggerExit(Collider other) {
        if (other.TryGetComponent(out PlayerActivity playerActivity))
            playerActivity.SwitchActivity(Activity.None);
    }
}
