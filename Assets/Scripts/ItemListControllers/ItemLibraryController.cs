using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TerrariaAssets;
using Newtonsoft.Json;
using System;
using System.Linq;

using PolyAndCode.UI;

public class ItemLibraryController : MonoBehaviour
{
    public Transform content;
    public TextAsset itemDataFile;

    public static event Action OnDatasetLoaded;

    // Start is called before the first frame update
    private void Start()
    {
        // get the content transform if it isn't assigned from the editor
        if (content == null)
        {
            content = transform.Find("Content");
        }

        Debug.Log(itemDataFile.text);

        // get item data from assets
        Dictionary<int, TerrariaItemData> itemJsonData = JsonConvert.DeserializeObject<Dictionary<int, TerrariaItemData>>(itemDataFile.text);

        Debug.Log($"{itemJsonData.Count} items found and parsed");

        // remove unobtainable items
        itemJsonData = itemJsonData.Where(x => !(x.Value.unobtainable ?? false)).ToDictionary(p => p.Key, p => p.Value);

        Debug.Log($"{itemJsonData.Count} items found after removing unobtainable items");

        foreach (TerrariaItemData itemData in itemJsonData.Values)
        {
            // retrieve the image for the item from the assets and insert it into the image dataset
            string path = "Image Data/" + itemData.imagefile.Split(".")[0];
            Sprite sprite = Resources.Load<Sprite>(path);
            itemJsonData[itemData.itemid].sprite = sprite;

            // insert the item list element into the item dataset
            TerrariaItemDataset.Instance.Items.Add(itemData.itemid, itemData);
        }

        Debug.Log($"about to invoke OnDatasetLoaded event Action");
        OnDatasetLoaded.Invoke();
    }
}