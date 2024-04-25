using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public TMP_Text countryNameText;
    public LoadGeoData loadGeoData;
    public CountryInteract[] countries;
    private int currentIndex = 0;

    void Start()
    {
        if (loadGeoData != null && loadGeoData.LoadGeodata != null)
        {
            DisplayCountryName(currentIndex);
        }
        else
        {
            Debug.LogError("JSON data not loaded. Make sure the loadGeoData component is attached and has a JSON file assigned.");
        }
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (currentIndex < countries.Length && !countries[currentIndex].IsGuessedCorrectly())
            {
                countries[currentIndex].SetInteractable(false);
                countries[currentIndex].SetGuessedCorrectly();
                currentIndex++;

                if (currentIndex < countries.Length)
                    DisplayCountryName(currentIndex);
                else
                    Debug.Log("Congratulations! You guessed all countries.");
            }
        }
    }

    void DisplayCountryName(int index)
    {
        if (index >= 0 && index < loadGeoData.LoadGeodata.features.Length)
        {
            string countryName = loadGeoData.LoadGeodata.features[index].properties.ADMIN;
            countryNameText.text = countryName;
        }
    }
}
