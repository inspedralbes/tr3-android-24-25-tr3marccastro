using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.Networking;
using System.Collections;
using System.IO;
using System.Collections.Generic;

public class ShopScreen : MonoBehaviour
{
    private VisualElement rootElement;
    private ScrollView itemsContainer;
    private string apiUrl = "http://localhost:3002/api/items"; // URL del servidor Node.js

    private void Awake()
    {
        PlayerSkin.DeleteAll();
    }

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
        VisualElement itemElement = new VisualElement();
        itemElement.AddToClassList("item");

        Image itemImage = new Image();
        itemImage.style.backgroundImage = new StyleBackground(new Texture2D(1, 1));
        StartCoroutine(LoadImage(item.imagePath, itemImage));

        itemImage.AddToClassList("image");
        itemElement.Add(itemImage);

        var nameLabel = new Label(item.name);
        nameLabel.AddToClassList("name");
        itemElement.Add(nameLabel);

        var priceLabel = new Label("$" + item.price);
        priceLabel.AddToClassList("price");
        itemElement.Add(priceLabel);

        var buyButton = new Button(() => BuyItem(item)) { text = PlayerSkin.HasItem(item.id) ? "Comprado" : "Comprar" };
        buyButton.SetEnabled(!PlayerSkin.HasItem(item.id));
        buyButton.AddToClassList("buy-button");
        itemElement.Add(buyButton);

        itemsContainer.Add(itemElement);
    }

    private void BuyItem(Item item)
    {
        Debug.Log("Comprando: " + item.name);
        StartCoroutine(DownloadAndSaveAssetBundle(item.assetBundlePath, item.id, item.name));
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

    private IEnumerator DownloadAndSaveAssetBundle(string bundlePath, int itemId, string name)
    {
        string fullBundleUrl = "http://localhost:3002" + bundlePath;
        string localPath = Application.persistentDataPath + "/item_" + name + ".png";
        Debug.Log("Cargando imagen desde: " + localPath);

        UnityWebRequest bundleRequest = UnityWebRequest.Get(fullBundleUrl);
        yield return bundleRequest.SendWebRequest();

        if (bundleRequest.result == UnityWebRequest.Result.Success)
        {
            // Guardar el archivo descargado en el directorio local
            File.WriteAllBytes(localPath, bundleRequest.downloadHandler.data);
            Debug.Log("Archivo guardado en: " + localPath);

            // Verificar si el archivo es una imagen
            StartCoroutine(VerifyIfImage(localPath, itemId));
        }
        else
        {
            Debug.LogError("Error al descargar el archivo: " + bundleRequest.error);
        }
    }

    private IEnumerator VerifyIfImage(string localPath, int itemId)
    {
        // Intentar cargar el archivo como una textura
        UnityWebRequest textureRequest = UnityWebRequestTexture.GetTexture("file://" + localPath);
        yield return textureRequest.SendWebRequest();

        if (textureRequest.result == UnityWebRequest.Result.Success)
        {
            // Si la carga es exitosa, asumimos que es una imagen
            Texture2D texture = ((DownloadHandlerTexture)textureRequest.downloadHandler).texture;
            Debug.Log("El archivo es una imagen válida.");
            PlayerSkin.AddItem(itemId);

            // Aquí puedes usar la imagen cargada, por ejemplo, asignarla a un sprite
            // Sprite itemSprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
            // Llamar a un método para aplicar el sprite al item
            // ApplyItemSprite(itemId, itemSprite);
        }
        else
        {
            Debug.LogError("El archivo no es una imagen válida: " + textureRequest.error);
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
        public string assetBundlePath;
        public bool active;
    }

    [System.Serializable]
    public class ItemList
    {
        public Item[] items;
    }
}