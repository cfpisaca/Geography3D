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
    public string countryName { get; set; }

    void Start()
    {
        SetOriginalColor();
        countryCollider = GetComponent<Collider>();
    }

    void Update() // Update is called once per frame
    {
        CheckGaze();
        CheckInput();
    }

    private void CheckGaze()
    {
        RaycastHit hit;
        Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);

        if (Physics.Raycast(ray, out hit) && hit.collider == countryCollider)
        {
            if (!guessedCorrectly && interactable)
                ChangeColor(hoverColor);
        }
        else
        {
            if (!guessedCorrectly && interactable)
                ChangeColor(originalColor);
        }
    }

    private void CheckInput()
    {
        if (interactable && OVRInput.GetDown(OVRInput.Button.One))
        {
            RaycastHit hit;
            Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);
            if (Physics.Raycast(ray, out hit) && hit.collider == countryCollider)
            {
                GameManager gameManager = FindObjectOfType<GameManager>();
                if (gameManager != null)
                {
                    string displayedCountryName = gameManager.GetDisplayedCountryName();
                    if (displayedCountryName != null && displayedCountryName == countryName)
                    {
                        SetGuessedCorrectly();
                    }
                    else
                    {
                        ChangeColor(wrongGuessColor);
                    }
                }
            }
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

        GameManager gameManager = FindObjectOfType<GameManager>();
        if (gameManager != null)
        {
            gameManager.OnCountryGuessedCorrectly();
        }
    }

    public bool IsGuessedCorrectly()
    {
        return guessedCorrectly;
    }

    public void SetInteractable(bool value)
    {
        interactable = value;
    }

    public void ResetCountry()
    {
        guessedCorrectly = false;
        interactable = true;
        ChangeColor(originalColor);
    }
}

