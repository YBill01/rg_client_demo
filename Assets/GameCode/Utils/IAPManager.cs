using Legacy.Database;
using System;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Purchasing;

namespace Legacy.Client {
    public class IAPManager : MonoBehaviour, IStoreListener
    {
        public static IAPManager Instance;

        private bool isPlayerPayer = false;

        private ProfileInstance Profile => ClientWorld.Instance.Profile;

        public static bool InPayment { get; internal set; }

        private static IStoreController m_StoreController;          // The Unity Purchasing system.
        private static IExtensionProvider m_StoreExtensionProvider; // The store-specific Purchasing subsystems.

        // Product identifiers for all products capable of being purchased: 
        // "convenience" general identifiers for use with Purchasing, and their store-specific identifier 
        // counterparts for use with and outside of Unity Purchasing. Define store-specific identifiers 
        // also on each platform's publisher dashboard (iTunes Connect, Google Play Developer Console, etc.)

        // General product identifiers for the consumable, non-consumable, and subscription products.
        // Use these handles in the code to reference which product to purchase. Also use these values 
        // when defining the Product Identifiers on the store. Except, for illustration purposes, the 
        // kProductIDSubscription - it has custom Apple and Google identifiers. We declare their store-
        // specific mapping to Unity Purchasing's AddProduct, below.
        public static string battle_pass = "err.bp2";

        private ProductMetadata boughtMetaData;

        public static List<string> storeKeys = new List<string>();

        public Action<FixedString4096> BuyCallback = null;

        public ProductMetadata GetItemMetadata(string storeKey)
        {
            return m_StoreController.products.WithStoreSpecificID(storeKey).metadata;
        }

        internal ProductMetadata GetBattlePassMetadata()
        {
            return GetItemMetadata(battle_pass);
        }

        void Start()
        {
            Instance = this;
        }

        public void Init()
        {            
            // If we haven't set up the Unity Purchasing reference
            if (m_StoreController == null)
            {
                // Begin to configure our connection to Purchasing
                InitializePurchasing();
            }
        }

        public void SetPayer(bool payer)
        {
            isPlayerPayer = payer;
        }

        public void InitializePurchasing()
        {
            // If we have already connected to Purchasing ...
            if (IsInitialized())
            {
                // ... we are done here.
                return;
            }

            // Create a builder, first passing in a suite of Unity provided stores.
            var builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());

            // Add a product to sell / restore by way of its identifier, associating the general identifier
            // with its store-specific identifiers.
            builder.AddProduct(battle_pass, ProductType.Consumable);
            storeKeys.Add(battle_pass);
            foreach(var pair in Shop.Instance.Bank)
            {
                if(pair.Value.storeKeys.android.Length > 0)
                {
                    builder.AddProduct(pair.Value.storeKeys.android, ProductType.Consumable);
                    storeKeys.Add(pair.Value.storeKeys.android);
                }
            }

            foreach(var pair in Heroes.Instance.List)
            {
                if (pair.Value.price.isReal)
                {
                    builder.AddProduct(pair.Value.price.store_key, ProductType.Consumable);
                    storeKeys.Add(pair.Value.price.store_key);
                }
            }

            foreach(var pair in Profile.actions.GetActualActionsList())
            {
                if (pair.Value.store_keys.android.Length > 0)
                {
                    builder.AddProduct(pair.Value.store_keys.android, ProductType.Consumable);
                    storeKeys.Add(pair.Value.store_keys.android);
                }
            }

            // Kick off the remainder of the set-up with an asynchrounous call, passing the configuration 
            // and this class' instance. Expect a response either in OnInitialized or OnInitializeFailed.
            UnityPurchasing.Initialize(this, builder);
        }


        private bool IsInitialized()
        {
            // Only say we are initialized if both the Purchasing references are set.
            return m_StoreController != null && m_StoreExtensionProvider != null;
        }

        public void BuyBattlePass(Action<FixedString4096> callback)
        {
            BuyCallback = callback;
            BuyProductID(battle_pass);
        }

        public void BuyCustomKey(string key, Action<FixedString4096> callback)
        {
            BuyCallback = callback;
            BuyProductID(key);
        }


        void BuyProductID(string productId)
        {
            InPayment = true;
            // If Purchasing has been initialized ...
            if (IsInitialized())
            {
                // ... look up the Product reference with the general product identifier and the Purchasing 
                // system's products collection.
                Product product = m_StoreController.products.WithID(productId);

                // If the look up found a product for this device's store and that product is ready to be sold ... 
                if (product != null && product.availableToPurchase)
                {
                    Debug.Log(string.Format("Purchasing product asychronously: '{0}'", product.definition.id));
                    // ... buy the product. Expect a response either through ProcessPurchase or OnPurchaseFailed 
                    // asynchronously.
                    m_StoreController.InitiatePurchase(product);
                }
                // Otherwise ...
                else
                {
                    // ... report the product look-up failure situation  
                    Debug.Log($"BuyProductID - {productId}: FAIL. Not purchasing product, either is not found or is not available for purchase.");
                }
            }
            // Otherwise ...
            else
            {
                // ... report the fact Purchasing has not succeeded initializing yet. Consider waiting longer or 
                // retrying initiailization.
                Debug.Log("BuyProductID FAIL. Not initialized.");
            }
        }


        // Restore purchases previously made by this customer. Some platforms automatically restore purchases, like Google. 
        // Apple currently requires explicit purchase restoration for IAP, conditionally displaying a password prompt.
        public void RestorePurchases()
        {
            // If Purchasing has not yet been set up ...
            if (!IsInitialized())
            {
                // ... report the situation and stop restoring. Consider either waiting longer, or retrying initialization.
                Debug.Log("RestorePurchases FAIL. Not initialized.");
                return;
            }

            // If we are running on an Apple device ... 
            if (Application.platform == RuntimePlatform.IPhonePlayer ||
                Application.platform == RuntimePlatform.OSXPlayer)
            {
                // ... begin restoring purchases
                Debug.Log("RestorePurchases started ...");

                // Fetch the Apple store-specific subsystem.
                var apple = m_StoreExtensionProvider.GetExtension<IAppleExtensions>();
                // Begin the asynchronous process of restoring purchases. Expect a confirmation response in 
                // the Action<bool> below, and ProcessPurchase if there are previously purchased products to restore.
                apple.RestoreTransactions((result) =>
                {
                    // The first phase of restoration. If no more responses are received on ProcessPurchase then 
                    // no purchases are available to be restored.
                    Debug.Log("RestorePurchases continuing: " + result + ". If no further messages, no purchases available to restore.");
                });
            }
            // Otherwise ...
            else
            {
                // We are not running on an Apple device. No work is necessary to restore purchases.
                Debug.Log("RestorePurchases FAIL. Not supported on this platform. Current = " + Application.platform);
            }
        }


        //  
        // --- IStoreListener
        //

        public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
        {
            // Purchasing has succeeded initializing. Collect our Purchasing references.
            Debug.Log("OnInitialized: PASS");

            // Overall Purchasing system, configured with products for this application.
            m_StoreController = controller;
            // Store specific subsystem, for accessing device-specific store features.
            m_StoreExtensionProvider = extensions;
        }


        public void OnInitializeFailed(InitializationFailureReason error)
        {
            // Purchasing set-up has not succeeded. Check error for reason. Consider sharing this reason with the user.
            Debug.Log("OnInitializeFailed InitializationFailureReason:" + error);
        }

        public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs args)
        {
            if (storeKeys.Contains(args.purchasedProduct.definition.id))
            {
                BuyCallback?.Invoke(new FixedString4096(args.purchasedProduct.receipt));
                boughtMetaData = args.purchasedProduct.metadata;
            }
            else
            {
                throw new Exception($"No such productID in storeKeys. Key: {args.purchasedProduct.definition.id}");
            }
            InPayment = false;
            // to be reminded of this purchase at next app launch. Use PurchaseProcessingResult.Pending when still 
            // saving purchased products to the cloud, and when that save is delayed. 
            return PurchaseProcessingResult.Complete;
        }

        public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason)
        {
            InPayment = false;

            //var key = product.receipt.GetHashCode();
            //if (key != null)
            //{
            //    if (pendingPurchases.ContainsKey(key))
            //    {
            //        pendingPurchases.Remove(key);
            //    }
            //    if (pendingPurchasesGameCallbacks.ContainsKey(key))
            //    {
            //        pendingPurchasesGameCallbacks.Remove(key);
            //    }
            //}
            Debug.Log(string.Format("OnPurchaseFailed: FAIL. Product: '{0}', PurchaseFailureReason: {1}", product.definition.id, failureReason));
        }

        internal void CompletePurchase(PlayerPayment payment)
        {
            GameDebug.Log($"CompletePurchase. Payment: {payment}.");

            if (!isPlayerPayer)
            {
                isPlayerPayer = true;
                AnalyticsManager.Instance.FirstPayment(payment);
            }
            AnalyticsManager.Instance.RealPayment(payment);
        }
    }
}
