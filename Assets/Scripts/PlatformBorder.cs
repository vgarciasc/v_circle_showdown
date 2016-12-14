using UnityEngine;

[ExecuteInEditMode]
public class PlatformBorder : MonoBehaviour {
	[SerializeField]
	GameObject border;
	bool editor = false;

	void Start() {
		set_border();
		editor = Application.isEditor;
	}

	void Update() {
		if (editor) set_border();
	}

	void set_border() {
		Vector3 aux = new Vector3();
		aux.x = get_correct_scale(this.transform.localScale.x);
		aux.y = get_correct_scale(this.transform.localScale.y);		
		aux.z = 1f;

		border.transform.localScale = aux; 
	}

	float get_correct_scale(float platform) {
		platform = Mathf.Abs(platform);
		float a = (1.01f - 1.2f) / 24f;
		float b = 1.2f - a;

		return (platform * a + b);
	}
}
