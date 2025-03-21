using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.Networking;
using System.Collections;
using UnityEngine;

public class ShopScreen : MonoBehaviour
{
    private VisualElement rootElement;
    private ScrollView itemsContainer;
    private string apiUrl = "http://localhost:3002/api/items"; // URL del servidor Node.js

    void OnEnable()
    {
        rootElement = GetComponent<UIDocument>().rootVisualElement;
        itemsContainer = rootElement.Q<ScrollView>("itemsContainer");

        // Cargar los items desde el servidor
        StartCoroutine(GetItemsFromServer());
    }

    private IEnumerator GetItemsFromServer()
    {
        UnityWebRequest request = UnityWebRequest.Get(apiUrl);
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            // Deserializar los datos JSON (ahora es un objeto con una propiedad "items")
            var itemsData = JsonUtility.FromJson<ItemList>(request.downloadHandler.text);
            foreach (var item in itemsData.items)
            {
                CreateItemUI(item);
            }
        }
        else
        {
            Debug.LogError("Error al obtener los ítems: " + request.error);
        }
    }

    private void CreateItemUI(Item item)
    {
        // Crear el contenedor del item
        VisualElement itemElement = new VisualElement();
        itemElement.AddToClassList("item");

        // Crear y añadir la imagen del item
        Image itemImage = new Image();
        itemImage.style.backgroundImage = new StyleBackground(new Texture2D(1, 1)); // Aquí agregamos la imagen
        StartCoroutine(LoadImage(item.imagePath, itemImage));

        itemImage.AddToClassList("image");
        itemElement.Add(itemImage);

        // Nombre del item
        var nameLabel = new Label(item.name);
        nameLabel.AddToClassList("name");
        itemElement.Add(nameLabel);

        // Precio del item
        var priceLabel = new Label("$" + item.price);
        priceLabel.AddToClassList("price");
        itemElement.Add(priceLabel);

        // Botón de compra
        var buyButton = new Button(() => BuyItem(item)) { text = "Comprar" };
        buyButton.AddToClassList("buy-button");
        itemElement.Add(buyButton);

        // Añadir el item a la lista de la tienda
        itemsContainer.Add(itemElement);
    }

    private void BuyItem(Item item)
    {
        // Aquí puedes poner la lógica para manejar la compra del item
        Debug.Log("Comprando: " + item.name);
        
        // Puedes añadir más lógica aquí para permitir que el jugador reciba el objeto en el juego
    }

    private IEnumerator LoadImage(string imageUrl, Image imageElement)
    {
        UnityWebRequest request = UnityWebRequestTexture.GetTexture("http://localhost:3002" + imageUrl);
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            Texture2D texture = ((DownloadHandlerTexture)request.downloadHandler).texture;
            imageElement.style.backgroundImage = new StyleBackground(texture);
        }
        else
        {
            Debug.LogError("Error al cargar la imagen: " + request.error);
        }
    }

    // Clases para deserializar los datos del servidor
    [System.Serializable]
    public class Item
    {
        public int id;
        public string name;
        public int price;
        public string imagePath;
        public bool active;
    }

    [System.Serializable]
    public class ItemList
    {
        public Item[] items;
    }
}