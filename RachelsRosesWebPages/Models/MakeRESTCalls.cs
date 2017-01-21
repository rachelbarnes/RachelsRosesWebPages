using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using RachelsRosesWebPages.Models;

namespace RachelsRosesWebPages {
    [DataContract]
    public class ItemResponse {
        [DataMember(Name = "salePrice")]
        public decimal salePrice { get; set; }
        [DataMember(Name = "name")]
        public string name { get; set; }
        [DataMember(Name = "itemId")]
        public int itemId { get; set; }
        public ItemResponse() { }
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
        public ItemResponse GetItemResponse(Ingredient i) {
            var db = new DatabaseAccess();
            var convert = new ConvertWeight();
            var newItemResponse = new ItemResponse();
            var tempItemResponse = new ItemResponse();
            if (string.IsNullOrEmpty(i.classification) || (i.classification == " ") || !(i.classification.ToLower().Contains("dairy")) || !(i.classification.ToLower().Contains("egg"))) { 
                if ((MakeRequest<SearchResponse>(buildSearchRequest(i)).Items.Count() == 0))
                    return newItemResponse;//ok, selling weight is not being transfered
                var items = MakeRequest<SearchResponse>(buildSearchRequest(i)).Items;
                var sellingWeightOunces = convert.ConvertWeightToOunces(i.sellingWeight);
                foreach (var item in items) {
                    if (!item.name.Contains('(')) {
                        if ((!item.name.ToLower().Contains("pack of")) || (!item.name.ToLower().Contains(("pk")))) {
                            if ((parseItemResponseName(item).Count() != 0) && (CompareWeightInOuncesFromItemResponseToIngredientSellingWeight(item, i) && (CompareItemResponseNameAndIngredientName(item, i)))) {
                                tempItemResponse = item;
                                break;
                            }
                        }
                    }
                }
            } else {
                if ((i.classification.ToLower().Contains("dairy")) || i.classification.ToLower().Contains("eggs"))
                    return newItemResponse;
            }
            return tempItemResponse;
            //i would like to be able to return all brands that fit a certain selling weight, and give all of them as an option, and give the best price? 
        }
        public List<ItemResponse> GetListItemResponses(Ingredient i) {
            var db = new DatabaseAccess();
            var convert = new ConvertWeight();
            var items = MakeRequest<SearchResponse>(buildSearchRequest(i)).Items;
            var sellingWeightOunces = convert.ConvertWeightToOunces(i.sellingWeight);
            var myListOfItemResponses = new List<ItemResponse>(); 
            foreach (var item in items) {
                if (!item.name.Contains('(')) {
                    if ((!item.name.ToLower().Contains("pack of")) || (!item.name.ToLower().Contains(("pk")))) {
                        if ((parseItemResponseName(item).Count() != 0) && (CompareWeightInOuncesFromItemResponseToIngredientSellingWeight(item, i) && (CompareItemResponseNameAndIngredientName(item, i))))
                            myListOfItemResponses.Add(item); 
                    }
                }
            }
            return myListOfItemResponses;
            //i would like to be able to return all brands that fit a certain selling weight, and give all of them as an option, and give the best price? 
            //i think a good idea would be to have the item id associated with the ingredient in the ingredient database or the cost database, that way you can get the exact same item
        }
        public decimal AverageDecimals(List<ItemResponse> ItemPrices) {
            var ItemResponsePrices = new List<decimal>();
            foreach (var response in ItemPrices)
                ItemResponsePrices.Add(response.salePrice);
            var addedValue = 0m;
            var countOfPrices = ItemResponsePrices.Count();
            foreach (var dec in ItemResponsePrices)
                addedValue += dec;
            return Math.Round((addedValue / countOfPrices), 2);
        }
        public List<ItemResponse> AverageItemResponseSalePrices(List<ItemResponse> ItemPrices) {
            var average = AverageDecimals(ItemPrices);
            var condensedItems = new List<ItemResponse>();
            var greatestPrice = 0m;
            var greatestPriceItem = new ItemResponse();
            var secondGreatestPrice = 0m;
            var secondGreatestPricedItem = new ItemResponse();
            foreach (var item in ItemPrices) {
                if (item.salePrice > greatestPrice) {
                    secondGreatestPrice = greatestPrice;
                    secondGreatestPricedItem = greatestPriceItem;
                    greatestPrice = item.salePrice;
                    greatestPriceItem = item;
                }
            }
            condensedItems = ItemPrices;
            condensedItems.Remove(greatestPriceItem);
            if (condensedItems.Count() > 4 && (secondGreatestPrice > (average + (average * 3))))
                condensedItems.Remove(secondGreatestPricedItem);
            return condensedItems;
        }

        public List<ItemResponse> GetListOfItemResponses(Ingredient i) {
            var convert = new ConvertWeight();
            var items = MakeRequest<SearchResponse>(buildSearchRequest(i)).Items;
            var sellingWeightOunces = convert.ConvertWeightToOunces(i.sellingWeight);
            var myItems = new List<ItemResponse>();
            foreach (var item in items) {
                if ((parseItemResponseName(item).Count() != 0) && (!item.name.ToLower().Contains("pack of")) && (CompareWeightInOuncesFromItemResponseToIngredientSellingWeight(item, i)) && CompareItemResponseNameAndIngredientName(item, i))
                    myItems.Add(item);
            }
            return myItems;
        }
        //var myItems = items.Where(item => item.name.ToLower().Contains(i.sellingWeight));
        //var firstItem = myItems.First();
        //var firstItemName = myItems.First().name;
        //var firstItemPrice = 0m;
        //if (CompareWeightInOuncesFromItemResponseToIngredientSellingWeight(firstItemName, i))
        //    firstItemPrice = myItems.First().salePrice;
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
        public string[] parseItemResponseName(ItemResponse response) {
            var itemResponseArray = new string[] { };
            var product = "";
            var productWeight = "";
            var count = response.name.Count();
            for (int i = count - 1; i > 0; i--) {
                if (i > 0 && i < count - 2) {
                    int n;
                    var next = i + 1;
                    var previous = i - 1;
                    if ((response.name[i] == ' ') && !int.TryParse(((response.name[previous].ToString())), out n) && (((int.TryParse((response.name[next].ToString()), out n))) || (response.name[next] == '.'))) {
                        var weightSubstringLength = (count - i);
                        productWeight = response.name.Substring(i, weightSubstringLength).Trim();
                        product = response.name.Substring(0, i).Trim();
                        itemResponseArray = new string[] { product, productWeight };
                        break;
                    }
                }
            }
            return itemResponseArray;
        }
        public bool CompareWeightInOuncesFromItemResponseToIngredientSellingWeight(ItemResponse response, Ingredient i) {
            var convert = new ConvertWeight();
            var productNameArray = parseItemResponseName(response);
            var productWeight = productNameArray[1];
            var productWeightOunces = convert.ConvertWeightToOunces(productWeight);
            //this product weight in ounces is incorrect...
            if (convert.ConvertWeightToOunces(i.sellingWeight) == productWeightOunces)
                return true;
            else return false;
        }
        public bool CompareItemResponseNameAndIngredientName(ItemResponse response, Ingredient i) {
            var responseNameParsed = parseItemResponseName(response);
            var responseNameNoWeight = responseNameParsed[0];
            var ingredientName = i.name.ToLower().Split(' ');
            var countSimilarWordsFromIngredientNameWithProductName = 0;
            var countIngredientNameWords = ingredientName.Count();
            foreach (var word in ingredientName) {
                if (response.name.ToLower().Contains(word))
                    countSimilarWordsFromIngredientNameWithProductName++;
            }
            var similarities = 0;
            if (CompareWeightInOuncesFromItemResponseToIngredientSellingWeight(response, i)) {
                similarities = countSimilarWordsFromIngredientNameWithProductName / countIngredientNameWords;
            }
            if (similarities == 1 || (countSimilarWordsFromIngredientNameWithProductName == countIngredientNameWords))
                return true;
            else return false;
        }
        public bool SimilaritesInStrings(string myIngredientName, string ingredientInDensityInfoDatabase) {
            //the second parameter is the string to compare it to 
            var countSimilarites = 0;
            var myIngredientNameArray = myIngredientName.ToLower().Split(' ');
            foreach (var word in myIngredientNameArray) {
                if (ingredientInDensityInfoDatabase.ToLower().Contains(word))
                    countSimilarites++;
            }
            if (countSimilarites == myIngredientNameArray.Count())
                return true;
            return false;
        }
    }
}
