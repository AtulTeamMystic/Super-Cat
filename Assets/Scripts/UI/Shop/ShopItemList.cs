using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.HighDefinition;

#if UNITY_ANALYTICS
using UnityEngine.Analytics;
#endif

public class ShopItemList : ShopList
{
    static public Consumable.ConsumableType[] s_ConsumablesTypes = System.Enum.GetValues(typeof(Consumable.ConsumableType)) as Consumable.ConsumableType[];
    public static Action onCoinShop;
    private CharacterInputController _cc;
    public static Action<ShopItemListItem,Consumable> afterPurchase;

    private void Start()
    {
        _cc = FindObjectOfType<CharacterInputController>();
    }
    
    public override void Populate()
    {
        m_RefreshCallback = null;
        foreach (Transform t in listRoot)
        {
            Destroy(t.gameObject);
        }
      
        for (int i = 0; i < s_ConsumablesTypes.Length; ++i)
        {
            Consumable c = ConsumableDatabase.GetConsumbale(s_ConsumablesTypes[i]);
            if (c != null)
            {
                prefabItem.InstantiateAsync().Completed += (op) =>
                {
                    if (op.Result == null || !(op.Result is GameObject))
                    {
                        Debug.LogWarning(string.Format("Unable to load item shop list {0}.", prefabItem.RuntimeKey));
                        return;
                    }
                    GameObject newEntry = op.Result;
                    newEntry.transform.SetParent(listRoot, false);

                    ShopItemListItem itm = newEntry.GetComponent<ShopItemListItem>();

                    itm.buyButton.image.sprite = itm.buyButtonSprite;

                    itm.nameText.text = c.GetConsumableName();
                    itm.pricetext.text = c.GetPrice().ToString();
                    itm.icon.sprite = c.icon;
                    itm.countText.gameObject.SetActive(true);
                    itm.buyButton.onClick.AddListener(delegate
                        {
                            if (c.GetPrice() > PlayerData.instance.coins)
                            {
                                StartCoroutine(ShopUIManager.instance.ShowBalanceWarnings());
                                return;
                            }
                            ShopUIManager.instance.assetName.text = c.GetConsumableName();
                            DebugLog.Log($"Buying '{c.GetConsumableName()}' with coins");
                            ShopUIManager.instance.OpenCoinBuyConfirmationPanel(delegate {
                                Buy(c);
                                ShopUIManager.instance.CloseCoinBuyConfirmationPanel();
                                onCoinShop?.Invoke();
                                StartCoroutine(UpdateItems(itm,c));
                            });
                        });
                    
                    m_RefreshCallback += delegate () { StartCoroutine(UpdateItems(itm, c));};
                   StartCoroutine(UpdateItems(itm, c));
                };
            }
        }
    }

    #region Old Code
    protected void RefreshButton(ShopItemListItem itemList, Consumable c)
    {
        int count = 0;
        PlayerData.instance.consumables.TryGetValue(c.GetConsumableType(), out count);
        if (c.GetConsumableType() == Consumable.ConsumableType.COIN_MAG)
        {
            itemList.countText.text = _cc.c_MagnetList.Count.ToString();
        } 
        if (c.GetConsumableType() == Consumable.ConsumableType.EXTRALIFE)
        {
            itemList.countText.text = _cc.c_ExtraLifeList.Count.ToString();
        } 
        if (c.GetConsumableType() == Consumable.ConsumableType.INVINCIBILITY)
        {
            itemList.countText.text = _cc.c_InvinciblityList.Count.ToString();
        } 
        if (c.GetConsumableType() == Consumable.ConsumableType.SCORE_MULTIPLAYER)
        {
            itemList.countText.text = _cc.c_ScoreMultiplierList.Count.ToString();
        }
        if (c.GetPrice() > PlayerData.instance.coins)
        {
            itemList.buyButton.interactable = false;
        }
    }

    #endregion

    void AddingItems(ShopItemListItem item, Consumable c)
    {
        if (c.GetConsumableType() == Consumable.ConsumableType.COIN_MAG)
        {
            item.countText.text = _cc.c_MagnetList.Count.ToString();
        } 
        if (c.GetConsumableType() == Consumable.ConsumableType.EXTRALIFE)
        {
            item.countText.text = _cc.c_ExtraLifeList.Count.ToString();
        } 
        if (c.GetConsumableType() == Consumable.ConsumableType.INVINCIBILITY)
        {
            item.countText.text = _cc.c_InvinciblityList.Count.ToString();
           
        } 
        if (c.GetConsumableType() == Consumable.ConsumableType.SCORE_MULTIPLAYER)
        {
            item.countText.text = _cc.c_ScoreMultiplierList.Count.ToString();
        }
    }

    IEnumerator UpdateItems(ShopItemListItem item, Consumable c)
    {
        yield return new WaitForSeconds(0.5f);
        AddingItems(item, c);
    }
    public void BuyFromGsp(Consumable c)
    {
        PlayerData.instance.Add(c.GetConsumableType());
        PlayerData.instance.Save();
    }
    public void Buy(Consumable c)
    {
        PlayerData.instance.coins -= c.GetPrice();
        PlayerData.instance.premium -= c.GetPremiumCost();
        PlayerData.instance.Add(c.GetConsumableType());
        PlayerData.instance.Save();

#if UNITY_ANALYTICS // Using Analytics Standard Events v0.3.0
        var transactionId = System.Guid.NewGuid().ToString();
        var transactionContext = "store";
        var level = PlayerData.instance.rank.ToString();
        var itemId = c.GetConsumableName();
        var itemType = "consumable";
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

        if (c.GetPrice() > 0)
        {
            AnalyticsEvent.ItemSpent(
                AcquisitionType.Soft, // Currency type
                transactionContext,
                c.GetPrice(),
                itemId,
                PlayerData.instance.coins, // Balance
                itemType,
                level,
                transactionId
            );
        }

        if (c.GetPremiumCost() > 0)
        {
            AnalyticsEvent.ItemSpent(
                AcquisitionType.Premium, // Currency type
                transactionContext,
                c.GetPremiumCost(),
                itemId,
                PlayerData.instance.premium, // Balance
                itemType,
                level,
                transactionId
            );
        }
#endif


    }
}
