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
    public GameObject targetObject;

    private void Awake()
    {
        PlayerSkins.DeleteAll();
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

        var buyButton = new Button(() => BuyItem(item)) { text = PlayerSkins.HasItem(item.id) ? "Comprado" : "Comprar" };
        buyButton.SetEnabled(!PlayerSkins.HasItem(item.id));
        buyButton.AddToClassList("buy-button");
        itemElement.Add(buyButton);

        itemsContainer.Add(itemElement);
    }

    private void BuyItem(Item item)
    {
        StartCoroutine(DownloadAndSaveAssetBundle(item));
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

    private IEnumerator DownloadAndSaveAssetBundle(Item item)
    {
        string imageName = Path.GetFileNameWithoutExtension(item.imagePath);
        string fullBundleUrl = "http://localhost:3002" + item.assetBundlePath;
        string localPath = Application.persistentDataPath + "/item_" + item.name;

        UnityWebRequest bundleRequest = UnityWebRequest.Get(fullBundleUrl);
        yield return bundleRequest.SendWebRequest();

        if (bundleRequest.result == UnityWebRequest.Result.Success)
        {
            // Obtener los datos binarios del AssetBundle
            byte[] assetData = bundleRequest.downloadHandler.data;

            // Comprobar si los datos son válidos
            if (assetData != null && assetData.Length > 0)
            {
                // Guardar el AssetBundle en local como un archivo binario
                File.WriteAllBytes(localPath, assetData);
                Debug.Log("AssetBundle guardado en: " + localPath);
                PlayerSkins.AddItem(item.id, imageName, localPath);
                // StartCoroutine(ApplyImageToObject(localPath, targetObject));
            }
            else
            {
                Debug.LogError("No se recibieron datos válidos del AssetBundle.");
            }
        }
        else
        {
            Debug.LogError("Error al descargar el archivo: " + bundleRequest.error);
        }
    }

    /*
    private IEnumerator ApplyImageToObject(string localPath, GameObject target)
    {
        // Cargar el AssetBundle desde el archivo local
        AssetBundle bundle = AssetBundle.LoadFromFile(localPath);

        Debug.Log(bundle);

        if (bundle == null)
        {
            Debug.LogError("No se pudo cargar el AssetBundle desde la ruta: " + localPath);
            yield break;
        }

        // Intentar obtener la textura del AssetBundle (asumiendo que la textura está con un nombre específico)
        Texture2D texture = bundle.LoadAsset<Texture2D>("fanta");  // Asegúrate de que el nombre coincida con el que está en el AssetBundle

        if (texture != null)
        {
            // Crear un sprite a partir de la textura
            Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));

            // Obtener el SpriteRenderer del objeto y asignar el sprite
            SpriteRenderer renderer = target.GetComponent<SpriteRenderer>();
            if (renderer != null)
            {
                renderer.sprite = sprite;
                Debug.Log("Imagen aplicada al objeto correctamente.");
            }
            else
            {
                Debug.LogError("El objeto no tiene un SpriteRenderer.");
            }
        }
        else
        {
            Debug.LogError("No se encontró la textura dentro del AssetBundle.");
        }

        // Liberar el AssetBundle para evitar fugas de memoria
        bundle.Unload(false);
    }
    */

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