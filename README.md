## 3D Shelf display in Unity that dynamically shows product information fetched from a server. The shelf will display between 1 and 3 products which will be randomly selected from a predefined list.

# Unity project - WebGL
- When loading the shelf, the server provides number of products to put on the shelf and provide details about each product, such as name, description and price.

# User Interactivity
- The users can modify the name and price of each product and it will be displayed instead of the old information.

# Interface
- Buttons for change name and price and a button to save the changes.

# Compatibility
- The game is compatible with WebGL for both desktop and mobile browsers

# Assets used
- Floor: https://assetstore.unity.com/packages/3d/props/interior/dungeon-floor-traps-77765 
- Products: https://assetstore.unity.com/packages/3d/props/food/food-grocery-items-low-poly-75494
- Shelf: https://assetstore.unity.com/packages/3d/props/shelves01-pack-289927
- Font: https://www.dafont.com/jmh-typewriter.font

# Instructions for running the project locally
- To run it on Desktop browser.
1.  Navigate to BuildShelf folder
2.  Open PowerShell window
3.  Type: py -m http.server
4.  Navigate to any browser desired
5.  Type: localhost:8000 (Press enter)
6.  The game will load

- To run it on Mobile browser.
1. Open the build folder in Visual Studio Code
2. Make sure you have live server extension - If so click on go Live
3. Go to mobile browser
4. Check your ipv4 address
5. Enter your ipv4 address with port - 192.168.x.x:5500/index.html
6. The game will load

Or enter itch.io - [games2024.itch.io/shelf](https://games2024.itch.io/shelf) - (Password: Mocart)

# Overview of Code Structure and Design Decisions
In my implementation, I aimed to create a visually appealing environment for the shelf and products. I added actual products to enhance the user experience of viewing products with their details.
I focused on designing an attractive environment, from floor details to the shelf itself. The products are placed on the shelf, with their details displayed above each product.

To ensure clarity, the details are displayed on a background, and I added a buttons that allows the option to change name/price if the client wish to change.
When the user clicks on the change name/price buttons, a "Save" button appears to allow them to submit their modifications.
On mobile when the user clicks on change name/price a keyboard pops and sends the modified data to update the desired product's information.

My code structure is based of two scripts.
The first is a manager script for loading the products.
The **ProductManager.cs** contains the reference to the spawn points (an array of Transform objects), the canvas of each spawner to display the details of the corresponding product, and a list of products to be displayed.
The script initially requests a list of products from the server via a GET request to fetch JSON response. After successful response, it parses the JSON data into a product list, then assigns the products to the spawners. Products are assigned randomly, ensuring each product is displayed uniquely, with usedProductIndices that tracks which products have already been assigned. 

The second script is **ProductLoader.cs**, designed to handle individual product data and enable users to modify each product's name and price.
It contains logic for displaying, updating and saving changes to product information while providing feedback on whether the action was successful.
It stores the current product data, displays the product itself, and presents detailed information about the product on the canvas. 

![image](https://github.com/user-attachments/assets/3db53556-72e8-4233-a037-ac3eea6f6ec4)
