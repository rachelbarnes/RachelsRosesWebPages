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
        //public static string buildSearchRequest(Ingredient i) {
        //    var rest = new MakeRESTCalls(); 
        //    return String.Format("http://api.walmartlabs.com/v1/search?query={0}&format=json&apiKey={1}", rest.CapitalizeString(i.name), apiKey.WalmartAPILogKey);
        //}
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
            var db = new DatabaseAccessRecipe();
            var convert = new ConvertWeight();
            var newItemResponse = new ItemResponse();
            var tempItemResponse = new ItemResponse();
            try {
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
            } catch {
                return newItemResponse;
            }
            return tempItemResponse;
            //i would like to be able to return all brands that fit a certain selling weight, and give all of them as an option, and give the best price? 
        }
        public List<ItemResponse> GetListItemResponses(Ingredient i) {
            var db = new DatabaseAccessRecipe();
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
        public List<ItemResponse> GetListItemResponseNoSellingWeights(Ingredient i) {
            var db = new DatabaseAccessRecipe();
            //var convert = new ConvertWeight();
            var items = MakeRequest<SearchResponse>(buildSearchRequest(i)).Items;
            var myListOfResponses = new List<ItemResponse>();
            foreach (var item in items) {
                if (item.name.Contains(i.name)) {
                    myListOfResponses.Add(item);
                }
            }
            return myListOfResponses;
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

        //public List<ItemResponse> GetListOfItemResponses(Ingredient i) {
        //    var convert = new ConvertWeight();
        //    var items = MakeRequest<SearchResponse>(buildSearchRequest(i)).Items;
        //    var sellingWeightOunces = convert.ConvertWeightToOunces(i.sellingWeight);
        //    var myItems = new List<ItemResponse>();
        //    foreach (var item in items) {
        //        if ((parseItemResponseName(item).Count() != 0) && (!item.name.ToLower().Contains("pack of")) && (CompareWeightInOuncesFromItemResponseToIngredientSellingWeight(item, i)) && CompareItemResponseNameAndIngredientName(item, i))
        //            myItems.Add(item);
        //    }
        //    return myItems;
        //}
        public List<ItemResponse> CombineItemResponses(Ingredient i) {
            var myFinalList = new List<ItemResponse>();
            var listWithWeights = GetListItemResponses(i);
            var listWithoutWeights = GetListItemResponseNoSellingWeights(i);
            foreach (var itemresponse in listWithWeights) {
                if (!myFinalList.Contains(itemresponse))
                    myFinalList.Add(itemresponse);
            }
            foreach (var itemresponse in listWithoutWeights) {
                if (!myFinalList.Contains(itemresponse))
                    myFinalList.Add(itemresponse);
            }
            return myFinalList;
        }
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
        public string CapitalizeString(string toBeCapitalized) {
            var finalString = "";
            var toBeCapitalizedArray = toBeCapitalized.Split(' ');
            foreach (var word in toBeCapitalizedArray) {
                var finalWord = "";
                if (word.Contains('-')) {
                    var splitWord = word.Split('-');
                    if (splitWord.Count() == 2) {
                        var firstLetterOfFirstWord = char.ToString(splitWord[0][0]);
                        var capitalizedFirstWord = firstLetterOfFirstWord.ToString() + splitWord[0].Substring(1);
                        var firstLetterOfSecondWord = char.ToUpper(splitWord[1][0]);
                        var capitalizedSecondWord = firstLetterOfSecondWord.ToString() + splitWord[1].Substring(1);
                        finalWord = string.Format("{0}-{1}", splitWord[0], capitalizedSecondWord);
                        finalString += finalWord.ToString() + " ";
                    }
                } else {
                    var firstLetter = char.ToUpper(word[0]);
                    var capitalizedWord = firstLetter.ToString() + word.Substring(1);
                    finalString += capitalizedWord.ToString() + " ";
                }
            }
            return finalString.Trim();
        }
        public Ingredient SplitItemResponseName(ItemResponse itemresponse) {
            //look for the name, look for the weight, get the price, get whether or not it's a pack
            //separate the measurement from the name
            //then check for packs and do what needs to be done there
            int n;
            var autoPopulatedIngredient = new Ingredient();
            for (int i = 0; i < itemresponse.name.Length; i++) {
                if (i > 0 && i < itemresponse.name.Length - 1) {
                    var prevChar = itemresponse.name[i - 1];
                    var nextChar = itemresponse.name[i + 1];
                    var currChar = itemresponse.name[i];
                    if (currChar == ' ' && !(int.TryParse(prevChar.ToString(), out n)) && (int.TryParse(nextChar.ToString(), out n))) {
                        var name = itemresponse.name.Substring(0, i);
                        var weight = itemresponse.name.Substring(i + 1, (itemresponse.name.Length - (i + 1)));
                        autoPopulatedIngredient.name = name;
                        autoPopulatedIngredient.sellingWeight = weight;
                        autoPopulatedIngredient.sellingPrice = itemresponse.salePrice;
                        break;
                    }
                }
            }
            return autoPopulatedIngredient;
        }
        //public decimal CalculateSellingWeightInOuncesFromItemResponseName(ItemResponse itemresponse) {
        //    var parsedItemResponseName = SplitItemResponseName(itemresponse);
        //    var name = parsedItemResponseName[0];
        //    var weight = parsedItemResponseName[1]; 
        //    if (name.ToLower().Contains("pack") || name.ToLower().Contains("pk") || name.ToLower().Contains(")")
        //        //would it be easier to have them fill out this information? 
        //            //knowing that i cannot predict the responses makes it much more difficult to try to parse them...
        //                //and then they have to go through and proof their ingredients as opposed to 
        //}
    }
}
