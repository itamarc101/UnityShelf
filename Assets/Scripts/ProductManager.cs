using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class ProductManager : MonoBehaviour
{
    [SerializeField] private Transform[] spawnerPoints; // Array of Transforms for the placement points
    [SerializeField] private Canvas[] spawnerCanvases; // Array of Canvases, one for each spawner
    [SerializeField] private List<ProductLoader.ItemData> products; // List of products
    private List<int> usedProductIndices;  // List of indices for products already assigned to prevent duplicates

    private string url = "https://homework.mocart.io/api/products"; // Server Address to fetch products

    void Start()
    {
        StartCoroutine(GetProducts());   // Fetch details from server
    }

    IEnumerator GetProducts()
    {
        UnityWebRequest request = UnityWebRequest.Get(url); // Request from the url

        yield return request.SendWebRequest(); // Sends web request and wait for response

        // Check if error occur
        if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.LogError("Error: " + request.error);
        }
        else
        {
            string jsonResponse = request.downloadHandler.text; // Get the json response
            Debug.Log("JSON Response: " + jsonResponse); // Log to show the response for debugging

            // Get the product list
            ProductLoader.ProductList productList = JsonUtility.FromJson<ProductLoader.ProductList>(jsonResponse);
            products = productList.products; // Assign fetched product list to the local products list

            // Proceed with assigning products only if data was fetched successfully
            if (products != null && products.Count > 0)
            {
                // Assign each product to a unique spawner
                AssignUniqueProductsToLoaders();
            }
            else
            {
                Debug.LogError("No products fetched from server.");
            }
        }
    }

    private void AssignUniqueProductsToLoaders()
    {
        // Validate there are products and spawner points 
        if (products == null || spawnerPoints == null || spawnerPoints.Length == 0)
        {
            Debug.LogError("Missing products or spawner points.");
            return;
        }

        // List of used indices to avoid duplicate products
        usedProductIndices = new List<int>();

        // Number of spawners based on the fetched data
        int numberOfSpawnersToUse = Mathf.Min(products.Count, 3); // Number of spawners based on the fetched data

        for (int i = 0; i < numberOfSpawnersToUse; i++) // Loop through spawners
        {
            // Get random product
            int randomIndex = GetUniqueRandomIndex();
            if (randomIndex != -1)
            {
                // Get the ProductLoader component on the spawner and set the product
                ProductLoader productLoader = spawnerPoints[i].GetComponent<ProductLoader>();
                if (productLoader != null)
                {
                    //Set the product at the spawner location with the assigned canvas
                    productLoader.SetProduct(products[randomIndex], spawnerPoints[i], spawnerCanvases[i]);
                }
                else
                {
                    Debug.LogError("No ProductLoader component found on spawner " + i);
                }
            }
        }
    }

    // Generates a unique random index for a product that hasn't been used before.
    private int GetUniqueRandomIndex()
    {
        // Initialize a list to store unused product indices
        List<int> availableIndices = new List<int>();

        // Add to the availableIndices list, unused product indices
        for (int i = 0; i < products.Count; i++)
        {
            if (!usedProductIndices.Contains(i))
            {
                availableIndices.Add(i);
            }
        }

        if (availableIndices.Count == 0) return -1;

        int randomIndex = availableIndices[Random.Range(0, availableIndices.Count)];
        usedProductIndices.Add(randomIndex);
        return randomIndex;
    }
}