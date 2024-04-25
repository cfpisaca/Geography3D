using UnityEngine;

public class CountryInteract : MonoBehaviour
{
    private Color originalColor = Color.white;
    private Color hoverColor = Color.blue;
    private Color clickColor = Color.green;
    private Color wrongGuessColor = Color.red;
    private bool guessedCorrectly = false;
    private bool interactable = true;

    private Collider countryCollider;

    void Start()
    {
        SetOriginalColor();
        countryCollider = GetComponent<Collider>();
    }

    void OnMouseEnter()
    {
        if (!guessedCorrectly && interactable)
            ChangeColor(hoverColor);
    }

    void OnMouseExit()
    {
        if (!guessedCorrectly && interactable)
            ChangeColor(originalColor);
    }

    void OnMouseDown()
    {
        if (interactable && Input.GetMouseButtonDown(0))
        {
            if (!guessedCorrectly)
                ChangeColor(wrongGuessColor);
            else
                ChangeColor(clickColor);
        }
    }

    private void SetOriginalColor()
    {
        foreach (Renderer renderer in GetComponentsInChildren<Renderer>())
        {
            if (renderer.GetComponent<LineRenderer>() != null)
            {
                originalColor = renderer.material.color;
            }
        }
    }

    private void ChangeColor(Color newColor)
    {
        foreach (Renderer renderer in GetComponentsInChildren<Renderer>())
        {
            if (renderer.GetComponent<LineRenderer>() != null)
            {
                renderer.material.color = newColor;
            }
        }
    }

    public void SetGuessedCorrectly()
    {
        guessedCorrectly = true;
        interactable = false;
        ChangeColor(clickColor);
    }
    
    public bool IsGuessedCorrectly()
    {
        return guessedCorrectly;
    }

    public void SetInteractable(bool value)
    {
        interactable = value;
    }
}
