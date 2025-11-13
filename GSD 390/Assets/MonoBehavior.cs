using System;
using System.Collections.Generic;
using UnityEngine;

//This script rotates a cube every frame in Update and lets me "collect" it when I press Space
/*
Features in this scrip:
1. Statics: TotalCollected and OnAnyCubeCollected are static so every
   Week1Behaviour object shares the same global collected count and event.
2. Property: RotationSpeed is a property that wraps the rotationSpeed field and
   clamps it to non-negative values before it is used in Update.
3. Attributes: [SerializeField] and [Range] are applied to rotationSpeed so I
   can adjust the value from the Inspector with a slider while keeping the
   field private and limiting it to a specific range.
*/

public class Week1Behaviour : MonoBehaviour
{
    public static int TotalCollected = 0;
    public static event Action<int> OnAnyCubeCollected;

    [SerializeField, Range(0f, 360f)]
    private float rotationSpeed = 90f;

    public float RotationSpeed
    {
        get => rotationSpeed;
        set => rotationSpeed = Mathf.Max(0f, value);
    }

    private readonly List<string> debugMessages = new List<string>();
    private bool isCollected = false;

    private void OnEnable()
    {
        OnAnyCubeCollected += HandleAnyCubeCollected;
    }

    private void OnDisable()
    {
        OnAnyCubeCollected -= HandleAnyCubeCollected;
    }

    private void Update()
    {
        transform.Rotate(Vector3.up, RotationSpeed * Time.deltaTime);

        if (!isCollected && Input.GetKeyDown(KeyCode.Space))
        {
            Collect();
        }
    }

    private void Collect()
    {
        isCollected = true;
        TotalCollected++;

        var renderer = GetComponent<Renderer>();
        if (renderer != null)
        {
            renderer.enabled = false;
        }

        string message = $"{name} collected! Total collected: {TotalCollected}";
        debugMessages.Add(message);
        Debug.Log(message);

        OnAnyCubeCollected?.Invoke(TotalCollected);
    }

    private void HandleAnyCubeCollected(int newTotal)
    {
        Debug.Log($"[Listener on {name}] OnAnyCubeCollected fired. New total: {newTotal}");
    }
}
