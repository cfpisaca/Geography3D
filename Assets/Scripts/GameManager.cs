using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public TMP_Text countryNameText;
    public LoadGeoData loadGeoData;
    public CountryInteract[] countries;
    private int currentIndex = 0;

    private void Start()
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

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            ResetGame();
        }
        if (Input.GetMouseButtonDown(0))
        {
            if (currentIndex < countries.Length)
            {
                if (!countries[currentIndex].IsGuessedCorrectly())
                    return;

                currentIndex++;

                if (currentIndex < countries.Length)
                {
                    DisplayCountryName(currentIndex);
                }
                else
                {
                    Debug.Log("Congratulations! You guessed all countries. Press R to reset.");
                    countryNameText.text = "Congratulations!\nYou guessed all countries.\nPress R to reset.";
                }
            }
        }
    }

    private void DisplayCountryName(int index)
    {
        if (index >= 0 && index < loadGeoData.LoadGeodata.features.Length)
        {
            string countryName = loadGeoData.LoadGeodata.features[index].properties.ADMIN;
            countryNameText.text = countryName;
        }
    }

    public string GetDisplayedCountryName()
    {
        if (currentIndex >= 0 && currentIndex < loadGeoData.LoadGeodata.features.Length)
        {
            return loadGeoData.LoadGeodata.features[currentIndex].properties.ADMIN;
        }
        return null;
    }

    public void OnCountryGuessedCorrectly()
    {
        currentIndex++;

        if (currentIndex < countries.Length)
        {
            DisplayCountryName(currentIndex);
        }
        else
        {
            if (AllCountriesGuessedCorrectly())
            {
                Debug.Log("Congratulations! You guessed all countries. Press R to reset.");
                countryNameText.text = "Congratulations!\nYou guessed all countries.\nPress R to reset.";
            }
        }

        UpdateDisplayedCountryName();
    }

    private bool AllCountriesGuessedCorrectly()
    {
        foreach (CountryInteract country in countries)
        {
            if (!country.IsGuessedCorrectly())
            {
                return false;
            }
        }
        return true;
    }

    private void UpdateDisplayedCountryName()
    {
        if (currentIndex >= 0 && currentIndex < loadGeoData.LoadGeodata.features.Length)
        {
            string countryName = loadGeoData.LoadGeodata.features[currentIndex].properties.ADMIN;
            countryNameText.text = countryName;
        }
    }

    public void ResetGame()
    {
        currentIndex = 0;

        CountryInteract[] allCountryInteracts = FindObjectsOfType<CountryInteract>();

        foreach (CountryInteract country in allCountryInteracts)
        {
            country.ResetCountry();
        }

        DisplayCountryName(currentIndex);
    }

}
