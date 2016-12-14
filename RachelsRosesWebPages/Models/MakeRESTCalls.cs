using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;

namespace RachelsRosesWebPages {
    [DataContract]
    public class ItemResponse {
        [DataMember(Name = "salePrice")]
        public decimal salePrice { get; set; }
        [DataMember(Name = "name")]
        public string name { get; set; }
        [DataMember(Name = "itemId")]
        public int itemId { get; set; }
    }
    [DataContract]
    public class SearchResponse {
        [DataMember(Name = "items")]
        public List<ItemResponse> Items { get; set; }
    }
    public class MakeRESTCalls {
        public static Func<Ingredient, string> buildSearchRequest = i => String.Format("http://api.walmartlabs.com/v1/search?query={0}&format=json&apiKey={1}", i.name, apiKey.WalmartAPILogKey);
        public static Func<string, string> buildItemIDRequest = productId => String.Format("http://api.walmartlabs.com/v1/items/{0}?apiKey={1}&format=json", productId, apiKey.WalmartAPILogKey);
        public static T MakeRequest<T>(string requestUrl) {
            try {
                HttpWebRequest request = WebRequest.Create(requestUrl) as HttpWebRequest;
                using (HttpWebResponse response = request.GetResponse() as HttpWebResponse) {
                    if (response.StatusCode != HttpStatusCode.OK)
                        throw new Exception(string.Format("Server error (HTTP {0}: {1}).", response.StatusCode, response.StatusDescription));
                    DataContractJsonSerializer jsonSerializer = new DataContractJsonSerializer(typeof(T));
                    return (T)jsonSerializer.ReadObject(response.GetResponseStream());
                }
            } catch (Exception e) {
                Console.WriteLine(e.Message);
                return default(T);
            }
        }
        public decimal GetItemResponse(Ingredient i) {
            var convert = new ConvertWeight(); 
            var items = MakeRequest<SearchResponse>(buildSearchRequest(i)).Items;
            var sellingWeightOunces = convert.ConvertWeightToOunces(i.sellingWeight);
            var itemPrice = 0m;
            foreach (var item in items) {
                if (CompareWeightInOuncesFromItemResponseToIngredientSellingWeight(item.name, i)) {
                    itemPrice = item.salePrice;
                    break; 
                }
            }
            //var myItems = items.Where(item => item.name.ToLower().Contains(i.sellingWeight));
            //var firstItem = myItems.First();
            //var firstItemName = myItems.First().name;
            //var firstItemPrice = 0m;
            //if (CompareWeightInOuncesFromItemResponseToIngredientSellingWeight(firstItemName, i))
            //    firstItemPrice = myItems.First().salePrice;
            return itemPrice;
            //i would like to be able to return all brands that fit a certain selling weight, and give all of them as an option, and give the best price? 
        }
        //public List<ItemResponse> GetListOfItemsFromItemResponse(Ingredient i) {
        //    var items = MakeRequest<SearchResponse>(buildSearchRequest(i)).Items;
        //    var myItems = items.Where(item => item.name.ToLower().Contains(i.sellingWeight));
        //    var myReturnedItems = new List<ItemResponse>();
        //    foreach (var item in myItems) {
        //        if (CompareWeightInOuncesFromItemResponseToIngredientSellingWeight(item.name, i))
        //            myReturnedItems.Add(item);
        //    }
        //    return myReturnedItems;
        //}
        /*
        Samples: 
        Red Star: Active Dry Yeast, .25 Oz
        Fleischmann's ActiveDry Yeast Original, 4.0 OZ
        King Arthur Flour Unbleached Bread Flour, 5.0 LB
        Gold Medal® Better for Bread® Flour 5 lb. Bag
        Pillsbury Bread Flour 5 lb 
        King Arthur Flour 100% Whole Grain Whole Wheat Flour, 5.0 LB
        */
        public string[] parseItemResponseName(string itemResponseProductName) {
            var itemResponseArray = new string[] { };
            var product = "";
            var productWeight = "";
            var count = itemResponseProductName.Count();
            for (int i = count - 1; i > 0; i--) {
                if (i > 0 && i < itemResponseProductName.Count() - 2) {
                    int n;
                    var previous = i + 1;
                    var next = i - 1;
                    var previousChar = itemResponseProductName[previous];
                    var currentChar = itemResponseProductName[i];
                    var nextChar = itemResponseProductName[next];
                    if ((itemResponseProductName[i] == ' ') && (!int.TryParse((itemResponseProductName[next].ToString()), out n)) && (int.TryParse((itemResponseProductName[previous].ToString()), out n))) {
                        var weightSubstringLength = (itemResponseProductName.Count() - i);
                        productWeight = itemResponseProductName.Substring(i, weightSubstringLength).Trim();
                        product = itemResponseProductName.Substring(0, i).Trim();
                        itemResponseArray = new string[] { product, productWeight };
                        break;
                    }
                }
            }
            return itemResponseArray;
        }
        //this method is impt because it will allow me to compare the weight of the ingredient with the weight of the product
        //right now, i can have 1 lb be presented in 1 lb or 1 pound, which I have to be able to choose either, not just what i have in my ingredient selling weight and my product response name
        public bool CompareWeightInOuncesFromItemResponseToIngredientSellingWeight(string productName, Ingredient i) {
            var convert = new ConvertWeight();
            var productNameArray = parseItemResponseName(productName);
            var productWeight = productNameArray[1];
            //this is where i'm getting the out of bounds exception
            var productWeightOunces = convert.ConvertWeightToOunces(productWeight);
            if (convert.ConvertWeightToOunces(i.sellingWeight) == productWeightOunces)
                return true;
            else return false;
        }

    }
}
