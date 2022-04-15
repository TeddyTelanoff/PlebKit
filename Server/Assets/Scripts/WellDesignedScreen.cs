using UnityEngine;

public class WellDesignedScreen: MonoBehaviour
{
	void Update() {
		HideIfClickedOutSide();
	}

	void HideIfClickedOutSide() {
		if (Input.GetMouseButton(0) &&
			!RectTransformUtility.RectangleContainsScreenPoint(
				GetComponent<RectTransform>(), 
				Input.mousePosition
			))
			gameObject.SetActive(false);
	}
}