using UnityEngine;
using System.Reflection;

public class XRRigSlowMovement : MonoBehaviour
{
    public static XRRigSlowMovement Instance;

    [Header("Target (XR Rig Advanced root)")]
    public Transform xrRigRoot;

    [Header("Speed")]
    public float normalSpeed = 1.5f;
    public float slowSpeed = 0.08f;

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
        else
        {
            Destroy(gameObject);
            return;
        }

        if (xrRigRoot == null)
            xrRigRoot = transform;

        FindSpeedMember();
        CacheNormalSpeed();
    }

    void FindSpeedMember()
    {
        if (xrRigRoot == null)
        {
            Debug.LogError("XRRigSlowMovement: xrRigRoot is NULL");
            return;
        }

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

                    Debug.Log("XRRigSlowMovement: Found FIELD '" + name + "' in component " + type.Name);
                    return;
                }

                var p = type.GetProperty(name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                if (p != null && p.PropertyType == typeof(float) && p.CanWrite)
                {
                    targetComponent = c;
                    speedMember = p;

                    Debug.Log("XRRigSlowMovement: Found PROPERTY '" + name + "' in component " + type.Name);
                    return;
                }
            }
        }

        Debug.LogWarning("XRRigSlowMovement: No speed member found under xrRigRoot = " + xrRigRoot.name);
    }

    void CacheNormalSpeed()
    {
        if (targetComponent == null || speedMember == null)
        {
            Debug.LogWarning("XRRigSlowMovement: Cannot cache normal speed because targetComponent or speedMember is NULL");
            return;
        }

        float v = GetSpeed();

        if (!cached)
        {
            cachedNormal = v;
            cached = true;
            Debug.Log("XRRigSlowMovement: Cached normal speed = " + cachedNormal);
        }
    }

    float GetSpeed()
    {
        if (targetComponent == null || speedMember == null)
            return 0f;

        if (speedMember is FieldInfo f)
            return (float)f.GetValue(targetComponent);

        if (speedMember is PropertyInfo p)
            return (float)p.GetValue(targetComponent);

        return 0f;
    }

    void SetSpeed(float v)
    {
        if (targetComponent == null || speedMember == null)
        {
            Debug.LogWarning("XRRigSlowMovement: Cannot SetSpeed because targetComponent or speedMember is NULL");
            return;
        }

        if (speedMember is FieldInfo f)
            f.SetValue(targetComponent, v);
        else if (speedMember is PropertyInfo p)
            p.SetValue(targetComponent, v);
    }

    public void StartDepressionSlowdown()
    {
        Debug.Log("XRRigSlowMovement: StartDepressionSlowdown CALLED");

        if (targetComponent == null || speedMember == null)
        {
            Debug.LogWarning("XRRigSlowMovement: No speed member found, trying again...");
            FindSpeedMember();
            CacheNormalSpeed();
        }

        if (targetComponent == null || speedMember == null)
        {
            Debug.LogError("XRRigSlowMovement: Still no speed member found. Slowdown failed.");
            return;
        }

        Debug.Log("XRRigSlowMovement: Speed before slow = " + GetSpeed());
        SetSpeed(slowSpeed);
        Debug.Log("XRRigSlowMovement: Speed after slow = " + GetSpeed());
    }

    public void ResetSpeed()
    {
        Debug.Log("XRRigSlowMovement: ResetSpeed CALLED");

        if (targetComponent == null || speedMember == null)
        {
            Debug.LogWarning("XRRigSlowMovement: No speed member found, reset failed.");
            return;
        }

        if (cached)
        {
            SetSpeed(cachedNormal);
            Debug.Log("XRRigSlowMovement: Reset to cached normal speed = " + cachedNormal);
        }
        else
        {
            SetSpeed(normalSpeed);
            Debug.Log("XRRigSlowMovement: Reset to fallback normalSpeed = " + normalSpeed);
        }
    }
}