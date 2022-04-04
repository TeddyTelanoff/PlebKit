using UnityEngine;

[RequireComponent(typeof(Collider))]
public class ActivityArea: MonoBehaviour
{
	[SerializeField]
	Activity activity;

	void OnTriggerEnter(Collider other) {
		if (other.TryGetComponent(out PlayerActivity playerActivity))
			playerActivity.SwitchActivity(activity);
	}
	
	void OnTriggerExit(Collider other) {
		if (other.TryGetComponent(out PlayerActivity playerActivity))
			playerActivity.SwitchActivity(Activity.None);
	}
}
