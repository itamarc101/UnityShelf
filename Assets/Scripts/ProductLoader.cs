using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class ProductLoader : MonoBehaviour
{
    private ItemData product;  // Product data class instance
    private GameObject instantiatedProduct;  // Instantiated GameObject reference

    [SerializeField] private Canvas spawnerCanvas; // Reference to the spawner's canvas

    // UI 
    private TMP_InputField changeNameField;
    private TMP_InputField changePriceField;
    private TextMeshProUGUI feedbackText;
    private Button saveButton;
    private Button changeNameButton;
    private Button changePriceButton;

    // Mobile keyboard and input field references
    private TouchScreenKeyboard keyboard; // Reference to keyboard
    private TMP_InputField currentInputField; // Track the currently selected input field

    [SerializeField] private float rotationSpeed = 10f;
    [SerializeField] private float feedbackCooldown = 2f;

    private void Update()
    {
        // Rotate the instantiated product around its Y-axis if it exists
        if (instantiatedProduct != null)
        {
            instantiatedProduct.transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
        }
    }


    // Set product data and the canvas for a corrsponding spawner
    public void SetProduct(ItemData assignedProduct, Transform placementPoint, Canvas canvas)
    {
        product = assignedProduct;
        spawnerCanvas = canvas;  // Assign the spawner's canvas

        if (spawnerCanvas != null)
        {
            changeNameButton = spawnerCanvas.transform.Find("ChangeNameButton")?.GetComponent<Button>();
            changePriceButton = spawnerCanvas.transform.Find("ChangePriceButton")?.GetComponent<Button>();
            saveButton = spawnerCanvas.transform.Find("SaveButton").GetComponent<Button>();
            changeNameField = spawnerCanvas.transform.Find("ChangeNameField")?.GetComponent<TMP_InputField>();
            changePriceField = spawnerCanvas.transform.Find("ChangePriceField")?.GetComponent<TMP_InputField>();
            feedbackText = spawnerCanvas.transform.Find("FeedbackText")?.GetComponent<TextMeshProUGUI>();

            // Initialize UI elements only if all necessary components are found
            if (changeNameButton != null && changePriceButton != null && saveButton != null && changeNameField != null && changePriceField != null && feedbackText != null)
            {

                changeNameButton.gameObject.SetActive(true);
                changePriceButton.gameObject.SetActive(true);
                saveButton.gameObject.SetActive(false); // Initially hidden, if changes requested show it.
                feedbackText.gameObject.SetActive(false);
                changeNameField.gameObject.SetActive(false);
                changePriceField.gameObject.SetActive(false);
                

                changeNameButton.onClick.AddListener(OnChangeNameClicked);
                changePriceButton.onClick.AddListener(OnChangePriceClicked);
                saveButton.onClick.AddListener(OnSaveChangesClicked);

                // Listeners for opening the keyboard when fields are selected
                changeNameField.onSelect.AddListener(OnNameFieldSelected);
                changePriceField.onSelect.AddListener(OnPriceFieldSelected);

                spawnerCanvas.gameObject.SetActive(true);
            }
            else {
                Debug.Log("Something isnt good on: " + spawnerCanvas.name);
            }
        }

        if (product != null)
        {
            InstantiateProduct(placementPoint); // Instantiate a product on the specific spawner.
            UpdateCanvasDetails(); // Update the spawner's canvas with product info

            spawnerCanvas.gameObject.SetActive(true);  // Enable canvas now that a product is set
        }
    }

    private void OnChangeNameClicked()
    {
        feedbackText.gameObject.SetActive(false);
        // If clicked on change name button - show the input field to change the name
        changeNameField.gameObject.SetActive(true);
        saveButton.gameObject.SetActive(true);
        currentInputField = changeNameField; // Set current input field to name
        OpenMobileKeyboard(changeNameField);
    }

    private void OnChangePriceClicked()
    {
        feedbackText.gameObject.SetActive(false);
        // If clicked on change price button - show the input field to change the price
        changePriceField.gameObject.SetActive(true);
        saveButton.gameObject.SetActive(true);
        currentInputField = changePriceField; // Set current input field to price
        OpenMobileKeyboard(changePriceField, TouchScreenKeyboardType.NumberPad);
    }

    // Opens mobile keyboard 
    private void OpenMobileKeyboard(TMP_InputField inputField, TouchScreenKeyboardType keyboardType = TouchScreenKeyboardType.Default)
    {
        if (Application.isMobilePlatform)
        {
            keyboard = TouchScreenKeyboard.Open(inputField.text, keyboardType);
            StartCoroutine(WaitForKeyboard());
        }
        else
        {
            inputField.ActivateInputField();
        }
    }

    private System.Collections.IEnumerator WaitForKeyboard()
    {
        // Loop until keyboard is closed or input is complete
        while (keyboard != null && keyboard.status != TouchScreenKeyboard.Status.Done)
        {
            // Update current input field with the text
            if (currentInputField != null) // Only update the active input field
            {
                currentInputField.text = keyboard.text;
            }
            yield return null;
        }

        if (keyboard != null && keyboard.status == TouchScreenKeyboard.Status.Done && currentInputField != null)
        {
            currentInputField.text = keyboard.text;
            currentInputField.DeactivateInputField();
            currentInputField = null; // Clear current input field after input is done
        }
    }

    private void OnNameFieldSelected(string text)
    {
        currentInputField = changeNameField; // Update the current input field to name
        keyboard = TouchScreenKeyboard.Open("", TouchScreenKeyboardType.Default);
    }

    private void OnPriceFieldSelected(string text)
    {
        currentInputField = changePriceField; // Update the current input field to price
        keyboard = TouchScreenKeyboard.Open("", TouchScreenKeyboardType.NumberPad);
    }


    private void OnSaveChangesClicked()
    {
        bool nameChanged = SaveName(changeNameField.text);
        bool priceChanged = SavePrice(changePriceField.text);

        if (nameChanged && !priceChanged)   // Case when name is good and price isn't
        {
            feedbackText.text = "Price should be number";
        }
        else if (nameChanged || priceChanged) // If only one is changed
        {
        
            feedbackText.text = "Changed Updated";
        }
        else // If input is wrong format
        {
            feedbackText.text = "Invalid format";
        }

        feedbackText.gameObject.SetActive(true); // Show success message
        Invoke(nameof(ClearSuccessMessage), feedbackCooldown); // Timeout for success message

        saveButton.gameObject.SetActive(false);

        changeNameButton.gameObject.SetActive(true);
        changePriceButton.gameObject.SetActive(true);
        changeNameField.gameObject.SetActive(false);
        changePriceField.gameObject.SetActive(false);

        changeNameField.text = string.Empty;
        changePriceField.text = string.Empty;
    }

    private bool SaveName(string newName)
    {
        if (!string.IsNullOrEmpty(newName))
        {
            product.name = newName;
            UpdateCanvasDetails(); // Only update if name changes
            return true;
        }
        return false;
    }

    private bool SavePrice(string newPrice)
    {
        // Only check price if it's not empty
        if (!string.IsNullOrEmpty(newPrice))
        {
            // Validate that the price is a valid number
            if (float.TryParse(newPrice, out float price))
            {
                product.price = price;
                UpdateCanvasDetails(); // Update canvas to reflect the price change
                return true;  // Return true if price was successfully updated
            }
            else
            {
                feedbackText.text = "Invalid price format"; // Show invalid price message
                return false;  // Return false if price format is invalid
            }
        }
        return true; // Return true if price is empty (meaning no price change needed)
    }

    private void ClearSuccessMessage()
    {
        feedbackText.gameObject.SetActive(false);
    }


    private void InstantiateProduct(Transform placementPoint)
    {
        if (product != null && placementPoint != null)
        {
            // Load the product prefab dynamically based on the product name
            GameObject productPrefab = Resources.Load<GameObject>(product.name);
            if (productPrefab != null)
            {
                instantiatedProduct = Instantiate(productPrefab, placementPoint.position, Quaternion.identity, placementPoint);
            }
            else
            {
                Debug.LogError("One or more text GameObjects (Name, Description, Price) not found under Canvas.");
            }
        }
        else
        {
            Debug.LogError("Prefab not found for: " + product.name);
        }
    }

    private void UpdateCanvasDetails()
    {
        if (spawnerCanvas != null)
        {
            // Locate and set the Text components within the spawner's canvas
            TextMeshProUGUI nameText = spawnerCanvas.transform.Find("Name").GetComponent<TextMeshProUGUI>();
            TextMeshProUGUI descriptionText = spawnerCanvas.transform.Find("Description").GetComponent<TextMeshProUGUI>();
            TextMeshProUGUI priceText = spawnerCanvas.transform.Find("Price").GetComponent<TextMeshProUGUI>();

        
            // Update the text fields with the product's information
            if (nameText != null) nameText.text = product.name;
            if (descriptionText != null) descriptionText.text = product.description;
            if (priceText != null) priceText.text = "$ " + product.price.ToString("F2");
        }
        else
        {
            Debug.LogError("Spawner canvas is not assigned.");
        }
    }

    // ItemData class for storing product information
    [System.Serializable]
    public class ItemData
    {
        public string name;
        public string description;
        public float price;
    }

    // ProductList class for managing a list of ItemData
    [System.Serializable]
    public class ProductList
    {
        public List<ItemData> products;
    }
}