using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.Networking;
using System.Collections;
using System.IO;
using UnityEngine.SceneManagement;
using System.Text;

public class ShopScreen : MonoBehaviour
{
    private VisualElement rootElement;
    private ScrollView skinsContainer;
    private Button buttonBack;
    private Label purchaseMessage;
    private string apiUrlPost = "http://localhost:3002/api/purchases/new-purchase";
    private string apiUrl = "http://localhost:3002/api/skins";
    private string apiUrlIdSkin = "http://localhost:3002/api/purchases/history";

    private void OnEnable()
    {
        rootElement = GetComponent<UIDocument>().rootVisualElement;
        skinsContainer = rootElement.Q<ScrollView>("itemsContainer");
        buttonBack = rootElement.Q<Button>("buttonBack");
        purchaseMessage = rootElement.Q<Label>("purchaseMessage");

        buttonBack.clicked += () => SceneManager.LoadScene("MainMenu");

        StartCoroutine(GetSkinsFromServer());
    }

    private IEnumerator GetSkinsFromServer()
    {
        UnityWebRequest request = UnityWebRequest.Get(apiUrl);
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            var skinsData = JsonUtility.FromJson<SkinList>(request.downloadHandler.text);

            if (skinsData.skins != null && skinsData.skins.Length > 0)
            {
                foreach (var skin in skinsData.skins)
                {
                    if (skin != null) CreateSkinUI(skin);
                }
            }
        }
    }

    private void CreateSkinUI(Skin skin)
    {
        VisualElement skinElement = new();
        skinElement.AddToClassList("item");

        Image skinImage = new();
        skinImage.style.backgroundImage = new StyleBackground(new Texture2D(1, 1));
        StartCoroutine(LoadImage(skin.imagePath, skinImage));

        skinImage.AddToClassList("image");
        skinElement.Add(skinImage);

        var nameLabel = new Label(skin.name);
        nameLabel.AddToClassList("name");
        skinElement.Add(nameLabel);

        var priceLabel = new Label("$" + skin.price);
        priceLabel.AddToClassList("price");
        skinElement.Add(priceLabel);

        Button buyButton = new() { text = "Verificant..." };
        buyButton.AddToClassList("buy-button");
        skinElement.Add(buyButton);

        skinsContainer.Add(skinElement);

        StartCoroutine(CheckPurchaseHistory(skin.id, isOwned =>
        {
            buyButton.text = isOwned ? "Descarregar" : "Comprar";
            buyButton.clicked += () => HandleSkinAction(skin, isOwned);
        }));
    }

    private void HandleSkinAction(Skin skin, bool isOwned)
    {
        purchaseMessage.style.display = DisplayStyle.None;

        if (isOwned)
        {
            StartCoroutine(DownloadAndSaveAssetBundle(skin, () => {
                purchaseMessage.text = "Descarregat amb èxit.";
                purchaseMessage.style.display = DisplayStyle.Flex;
            }));
        }
        else if (UserSession.GetUserEmail() != null)
        {
            BuySkin(skin, () => {
                purchaseMessage.text = "Comprat i descarregat.";
                purchaseMessage.style.display = DisplayStyle.Flex;
            });
        }
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
    }

    private void BuySkin(Skin skin, System.Action onComplete = null)
    {
        StartCoroutine(SaveBillToServer(skin.id, () => {
            StartCoroutine(DownloadAndSaveAssetBundle(skin, onComplete));
        }));
    }

    private IEnumerator SaveBillToServer(int skinId, System.Action onSuccess = null)
    {
        string email = UserSession.GetUserEmail();
        BuyData buyData = new() { skinId = skinId, email = email };

        string jsonData = JsonUtility.ToJson(buyData);
        byte[] jsonBytes = Encoding.UTF8.GetBytes(jsonData);

        UnityWebRequest request = new(apiUrlPost, "POST")
        {
            uploadHandler = new UploadHandlerRaw(jsonBytes),
            downloadHandler = new DownloadHandlerBuffer()
        };

        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            ResponsePurchaseData response = JsonUtility.FromJson<ResponsePurchaseData>(request.downloadHandler.text);
            if (response.message == "success")
            {
                onSuccess?.Invoke();
            }
        }
    }

    private IEnumerator DownloadAndSaveAssetBundle(Skin skin, System.Action onComplete = null)
    {
        string localPath = Application.persistentDataPath + "/" + skin.name;

        UnityWebRequest bundleRequest = UnityWebRequest.Get("http://localhost:3002" + skin.assetBundlePath);
        yield return bundleRequest.SendWebRequest();

        if (bundleRequest.result == UnityWebRequest.Result.Success)
        {
            File.WriteAllBytes(localPath, bundleRequest.downloadHandler.data);
            PlayerSkins.AddSkin(skin.id, skin.name, localPath);
            onComplete?.Invoke();
        }
    }

    private IEnumerator CheckPurchaseHistory(int id, System.Action<bool> callback)
    {
        if (PlayerSkins.HasSkin(id))
        {
            callback(true);
            yield break;
        }

        string email = UserSession.GetUserEmail();
        SkinData skinData = new() { skinId = id, email = email };

        string jsonData = JsonUtility.ToJson(skinData);
        byte[] jsonBytes = Encoding.UTF8.GetBytes(jsonData);

        UnityWebRequest request = new(apiUrlIdSkin, "POST")
        {
            uploadHandler = new UploadHandlerRaw(jsonBytes),
            downloadHandler = new DownloadHandlerBuffer()
        };

        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            ResponseHistory response = JsonUtility.FromJson<ResponseHistory>(request.downloadHandler.text);
            callback(response.message == "success");
        }
        else
        {
            callback(false);
        }
    }

    [System.Serializable] 
    public class BuyData 
    { 
        public int skinId; 
        public string email; 
    }

    [System.Serializable] 
    public class ResponsePurchaseData 
    { 
        public string message; 
    }

    [System.Serializable] 
    public class SkinData 
    { 
        public int skinId; 
        public string email; 
    }

    [System.Serializable] 
    public class ResponseHistory 
    { 
        public string message; 
    }

    [System.Serializable] 
    public class Skin 
    { 
        public int id; 
        public string name; 
        public int price; 
        public string imagePath;
        public string assetBundlePath; 
        public bool active; 
    }

    [System.Serializable] 
    public class SkinList 
    { 
        public Skin[] skins; 
    }
}
