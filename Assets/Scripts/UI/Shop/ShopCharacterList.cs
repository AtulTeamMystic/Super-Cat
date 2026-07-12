using UnityEngine;
using System.Collections.Generic;
using UnityEngine.AddressableAssets;

#if UNITY_ANALYTICS
using UnityEngine.Analytics;
#endif

public class ShopCharacterList : ShopList
{
    public override void Populate()
    {
        m_RefreshCallback = null;
        foreach (Transform t in listRoot)
        {
            Destroy(t.gameObject);
        }

        foreach (KeyValuePair<string, Skin> pair in SkinDatabase.dictionary)
        {
            Skin skin = pair.Value;
            if (skin != null)
            {
                prefabItem.InstantiateAsync().Completed += (op) =>
                {
                    if (op.Result == null || !(op.Result is GameObject))
                    {
                        Debug.LogWarning(string.Format("Unable to load character shop list {0}.", prefabItem.Asset.name));
                        return;
                    }
                    GameObject newEntry = op.Result;
                    newEntry.transform.SetParent(listRoot, false);

                    ShopItemListItem itm = newEntry.GetComponent<ShopItemListItem>();
                    itm.icon.sprite = skin.icon;
                    itm.nameText.text = skin.skinName;
                    itm.pricetext.text = skin.cost.ToString();

                    itm.buyButton.image.sprite = itm.buyButtonSprite;

                    itm.buyButton.onClick.AddListener(delegate () { Buy(skin); });

                    m_RefreshCallback += delegate () { RefreshButton(itm, skin); };
                    RefreshButton(itm, skin);
                };
            }
        }
    }

    protected void RefreshButton(ShopItemListItem itm, Skin skin)
    {
        if (skin.cost > PlayerData.instance.coins)
        {
            itm.buyButton.interactable = false;
            itm.pricetext.color = new Color(1f, 0.82f, 0f);
        }
        else
        {
            itm.pricetext.color = new Color(1f, 0.82f, 0f);
        }

        if (PlayerData.instance.characters.Contains(skin.skinName))
        {
            itm.pricetext.color = new Color(1f, 0.82f, 0f);
            string currentEquippedSkin = PlayerData.instance.characters[PlayerData.instance.usedCharacter];
            if (currentEquippedSkin == skin.skinName)
            {
                itm.buyButton.interactable = false;
                itm.buyButton.image.sprite = itm.disabledButtonSprite;
                itm.buyButton.transform.GetChild(0).GetComponent<UnityEngine.UI.Text>().text = "Equipped";
            }
            else
            {
                itm.buyButton.interactable = true;
                itm.buyButton.image.sprite = itm.buyButtonSprite;
                itm.buyButton.transform.GetChild(0).GetComponent<UnityEngine.UI.Text>().text = "Equip";

                itm.buyButton.onClick.RemoveAllListeners();
                itm.buyButton.onClick.AddListener(delegate () { Equip(skin); });
            }
        }
    }



    public void Buy(Skin skin)
    {
        PlayerData.instance.coins -= skin.cost;
        PlayerData.instance.AddCharacter(skin.skinName);
        PlayerData.instance.Save();

#if UNITY_ANALYTICS // Using Analytics Standard Events v0.3.0
        var transactionId = System.Guid.NewGuid().ToString();
        var transactionContext = "store";
        var level = PlayerData.instance.rank.ToString();
        var itemId = skin.skinName;
        var itemType = "non_consumable";
        var itemQty = 1;

        AnalyticsEvent.ItemAcquired(
            AcquisitionType.Soft,
            transactionContext,
            itemQty,
            itemId,
            itemType,
            level,
            transactionId
        );
        
        if (skin.cost > 0)
        {
            AnalyticsEvent.ItemSpent(
                AcquisitionType.Soft, // Currency type
                transactionContext,
                skin.cost,
                itemId,
                PlayerData.instance.coins, // Balance
                itemType,
                level,
                transactionId
            );
        }
#endif

        // Repopulate to change button accordingly.
        Populate();
    }

    public void Equip(Skin skin)
    {
        int index = PlayerData.instance.characters.IndexOf(skin.skinName);
        if (index >= 0)
        {
            PlayerData.instance.usedCharacter = index;
            PlayerData.instance.Save();
        }

        // Repopulate to update all buttons
        Populate();

        // Refresh main menu model
        if (GameManager.instance != null)
        {
            LoadoutState loadoutState = GameManager.instance.topState as LoadoutState;
            if (loadoutState != null)
            {
                loadoutState.Refresh();
            }
        }
    }
}
