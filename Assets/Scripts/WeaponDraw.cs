using UnityEngine;

public class WeaponDraw : MonoBehaviour
{
    public GameObject weapon;         // Assigned in Inspector
    public GameObject placeholder;    // Assigned in Inspector

    private Animator animator;
    private bool isSwitching = false;

    void Start()
    {
        animator = GetComponent<Animator>();
        if (animator == null)
        {
            Debug.LogWarning("Animator not found on GameObject.");
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.H) && !isSwitching)
        {
            StartCoroutine(DrawOrHolster());
        }
    }

    private System.Collections.IEnumerator DrawOrHolster()
    {
        isSwitching = true;

        // Play animation with tag "DrawWeapon"
        if (animator != null)
        {
            animator.Play("HighBack"); // Assuming this trigger exists
        }

        yield return new WaitForSeconds(0.7f);

        // Toggle active states
        bool weaponActive = weapon.activeSelf;

        weapon.SetActive(!weaponActive);
        placeholder.SetActive(weaponActive);

        isSwitching = false;
    }
}

