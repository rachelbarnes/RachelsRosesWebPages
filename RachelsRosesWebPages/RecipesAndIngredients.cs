using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RachelsRosesWebPages {
    public class Ingredient {
        public string name;
        public string measurement;
        public int recipeId;
        public int ingredientId;
        public decimal density;
        public decimal pricePerOunce;
        public string sellingWeight;
        public decimal sellingWeightInOunces; 
        public decimal sellingPrice;
        public int itemId;
        public decimal ouncesConsumed;
        public decimal ouncesRemaining;
        public decimal priceOfMeasuredConsumption; 
        public List<string> comments;
        public string typeOfIngredient;
        public string classification;
        public string itemResponseName;
        public bool restock; //1 = false, 0 = true
        public DateTime expirationDate; 
        public Ingredient(string _name, string _measurement) {
            name = _name;
            measurement = _measurement;
            recipeId = 0;
            ingredientId = 0;
            density = 0m;
            sellingWeight = "";
            sellingWeightInOunces = 0m;
            sellingPrice = 0m; 
            pricePerOunce = 0m;
            itemId = 0;
            ouncesConsumed = 0m;
            ouncesRemaining = 0m;
            priceOfMeasuredConsumption = 0m;
            typeOfIngredient = "";
            classification = ""; 
            itemResponseName = "";
            restock = false; 
            expirationDate = new DateTime(); 
        }
        public Ingredient(string _name) {
            name = _name;
            measurement = ""; 
        }
        public Ingredient() { }
    }
    public class Recipe {
        public string name;
        public int id;
        public List<Ingredient> ingredients;
        public List<string> instructions; 
        public int yield;
        public decimal aggregatedPrice;
        public List<string> comments;
        public string typeOfRecipe;
        public decimal pricePerServing; 
        public Recipe(string _name) {
            name = _name;
            id = 0;
            ingredients = new List<Ingredient>();
            yield = 0;
            aggregatedPrice = 0m;
            typeOfRecipe = "";
            pricePerServing = 0m; 

                //this type of recipe is meant to be "bread" or "cake" or "frosting"
        }
        public Recipe(int _id) {
            name = "";
            id = _id;
            ingredients = new List<Ingredient>();
            yield = 0;
        }
        public Recipe() { }
    }
    public class Error {
        public string repeatedRecipeName = "This recipe is already in your recipe box.";
        public Error() { }
    }
}
/*
 Among the things to do next: 

 read files (you could update the density database quicker if you could read through a file and see new insertions, but this will do fine for now 
    (I would prefer word and excel, but if I have to settle for notepad it wouldn't be the end of the world for this project
*/
