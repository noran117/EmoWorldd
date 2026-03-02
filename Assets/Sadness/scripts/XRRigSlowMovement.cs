using UnityEngine;
using System.Reflection;

public class XRRigSlowMovement : MonoBehaviour
{
    public static XRRigSlowMovement Instance;

    [Header("Target (XR Rig Advanced root)")]
    public Transform xrRigRoot;   

    [Header("Speed")]
    public float normalSpeed = 1.5f;
    public float slowSpeed = 0.4f;

    Component targetComponent;
    MemberInfo speedMember;   
    float cachedNormal;
    bool cached;

    readonly string[] candidates =
    {
        "moveSpeed", "MoveSpeed",
        "speed", "Speed",
        "locomotionSpeed", "LocomotionSpeed",
        "movementSpeed", "MovementSpeed",
        "stickSpeed", "StickSpeed",
        "moveMultiplier", "MoveMultiplier",
        "movementMultiplier", "MovementMultiplier"
    };

    void Awake()
    {
        if (Instance == null) Instance = this;
        else { Destroy(gameObject); return; }

        if (xrRigRoot == null)
            xrRigRoot = transform; 

        FindSpeedMember();
        CacheNormalSpeed();
    }

    void FindSpeedMember()
    {
        var comps = xrRigRoot.GetComponentsInChildren<Component>(true);

        foreach (var c in comps)
        {
            if (c == null) continue;

            var type = c.GetType();

            for (int i = 0; i < candidates.Length; i++)
            {
                string name = candidates[i];

                var f = type.GetField(name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                if (f != null && f.FieldType == typeof(float))
                {
                    targetComponent = c;
                    speedMember = f;
                    return;
                }

                var p = type.GetProperty(name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                if (p != null && p.PropertyType == typeof(float) && p.CanWrite)
                {
                    targetComponent = c;
                    speedMember = p;
                    return;
                }
            }
        }

        Debug.LogWarning("XRRigSlowMovement is not found");
    }

    void CacheNormalSpeed()
    {
        if (targetComponent == null || speedMember == null) return;

        float v = GetSpeed();
        if (!cached)
        {
            cachedNormal = v;
            cached = true;
        }

    }

    float GetSpeed()
    {
        if (speedMember is FieldInfo f) return (float)f.GetValue(targetComponent);
        if (speedMember is PropertyInfo p) return (float)p.GetValue(targetComponent);
        return 0f;
    }

    void SetSpeed(float v)
    {
        if (speedMember is FieldInfo f) f.SetValue(targetComponent, v);
        else if (speedMember is PropertyInfo p) p.SetValue(targetComponent, v);
    }

    public void StartDepressionSlowdown()
    {
        if (targetComponent == null || speedMember == null) return;
        SetSpeed(slowSpeed);
    }

    public void ResetSpeed()
    {
        if (targetComponent == null || speedMember == null) return;

        if (cached) SetSpeed(cachedNormal);
        else SetSpeed(normalSpeed);
    }
}
