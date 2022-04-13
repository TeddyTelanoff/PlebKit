using UnityEngine;

// a bit haxy
public class UpgradeStation: ActivityArea
{
	public Upgrade upgrade;
    public GameObject header;
    public GameObject onModel;
    public GameObject offModel;

    void OnValidate() {
        activity = Activity.Upgrade;
    }

    public void Reset() {
        header.SetActive(true);
        onModel.SetActive(false);
        offModel.SetActive(true);
    }

    void OnTriggerEnter(Collider other) {
        if (other.TryGetComponent(out PlayerActivity playerActivity))
        {
            playerActivity.SwitchActivity(activity);
            playerActivity.selectedUpgradeStation = this;
        }
    }
	
    void OnTriggerExit(Collider other) {
        if (other.TryGetComponent(out PlayerActivity playerActivity))
        {
            playerActivity.SwitchActivity(Activity.None);
            playerActivity.selectedUpgradeStation = null;
        }
    }

    public void BoughtUpgrade() {
        header.SetActive(false);
        onModel.SetActive(true);
        offModel.SetActive(false);
    }
}
