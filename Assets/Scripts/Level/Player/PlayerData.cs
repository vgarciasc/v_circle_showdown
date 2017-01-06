using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "PlayerData", order = 1)]
public class PlayerData : ScriptableObject {
    public float maxVelocity = 10f;
    public float maxSecondsOutOfScreen = 3f;
    public float maxChargeBuildup = 100f;
    public float chargeWeight = 0.5f;
    public float maxSize = 15f;
    public float minSize = 0.6f;
    public float mass = 0.5f;
    public Vector3 scale = new Vector3(1f, 1f, 1f);
    
    [Range(0, 300f)]
    public float chargeForce = 50f;
    [Range(0, 900f)]
    public float jumpForce = 300f;
	[Range(0, 1f)]
    public float speed = 0.3f;
    [Range(0, 1f)]
    public float hitSizeIncrement = 0.2f;
    [Range(0, 0.2f)]
    public float timeSizeIncrement = 0.02f;
    [Range(0.8f, 10.0f)]
    public float timeBetweenSpurts = 1.0f;
    [Range(1, 80f)]
    public float hitTransferRatio = 40f;
    [Range(0, 60)]
    public int invincibleFrames = 20;
    [SerializeField]
    public float portalCooldown = 1.0f;
}
