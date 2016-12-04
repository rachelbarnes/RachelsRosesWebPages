﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using NUnit.Framework;
namespace RachelsRosesWebPages {
    public class Convert {
        public decimal teaspoonsToTablespoons(decimal t) {
            var ret = Math.Round((t / 3), 2);
            if (ret.ToString().Contains(".00"))
                ret = Math.Round((t / 3), 0);
            return ret;
        }
        public decimal TablespoonsToTeaspoons(decimal T) {
            var ret = Math.Round((T * 3), 2);
            if (ret.ToString().Contains(".00"))
                ret = Math.Round((T * 3), 0);
            return ret;
        }
        public decimal teaspoonsToCups(decimal t) {
            var ret = Math.Round((t / 48), 2);
            if (ret.ToString().Contains(".00"))
                ret = Math.Round((t / 48), 0);
            return ret;
        }
        public decimal CupsToTeaspoons(decimal c) {
            var ret = Math.Round((c * 48), 2);
            if (ret.ToString().Contains(".00"))
                ret = Math.Round((c * 48), 0);
            return ret;
        }
        public decimal CupsToTablespoons(decimal c) {
            var ret = Math.Round((c * 16), 2);
            if (ret.ToString().Contains(".00"))
                ret = Math.Round((c * 16), 0);
            return ret;
        }
        public decimal TablespoonsToCups(decimal T) {
            var ret = Math.Round((T / 16), 2);
            if (ret.ToString().Contains(".00"))
                ret = Math.Round((T / 16), 0);
            return ret;
        }
        public Func<int, decimal, int> ChangeYield = (originalServingSize, multiplicationFactor) => (int)(Math.Round((originalServingSize * multiplicationFactor), 0));
        public Func<int, int, decimal> ChangeYieldMultiplier = (originalServingSize, updatedServingSize) => Math.Round(((decimal)updatedServingSize / originalServingSize), 4);
        public Func<decimal, decimal, decimal> AdjustTeaspoonsBasedOnMultiplier = (originalTeaspoonMeasurement, multiplier) => Math.Round((originalTeaspoonMeasurement * multiplier), 2);
        public string[] SplitMultiLevelMeasurement(string multiLevelMeasurement) {
            string[] splitMeasurement = new string[] { };
            int previous;
            int next;
            int n;
            var count = 0;
            var countEggs = 0;
            var commonMeasurements = new string[] { "cup", "tablespoon", "teaspoon", "egg" };
            var firstMeasurement = "";
            var secondMeasurement = "";
            var thirdMeasurement = "";
            var latterMeasurement = "";
            foreach (var meas in commonMeasurements) {
                if (multiLevelMeasurement.Contains(meas))
                    if (meas == "egg") {
                        countEggs++;
                        //using this, we should be able to do functionality based on the eggs... i've never really seen more than one instance of egg as an ingredient in a recipe, except for maybe egg wash on the top of bread, etc. 
                    } else { count++; }
            }
            if (count == 1)
                splitMeasurement = new string[] { multiLevelMeasurement };
            if (count > 1) {
                for (int i = 0; i < multiLevelMeasurement.Count(); i++) {
                    if ((i > 1) && (i < multiLevelMeasurement.Count() - 1)) {
                        previous = i - 1;
                        next = i + 1;
                        if ((multiLevelMeasurement[i] == ' ') && (!int.TryParse(multiLevelMeasurement[previous].ToString(), out n)) && (int.TryParse(multiLevelMeasurement[next].ToString(), out n))) {
                            firstMeasurement = multiLevelMeasurement.Substring(0, i);
                            latterMeasurement = multiLevelMeasurement.Substring(next, (multiLevelMeasurement.Count()) - (i + 1));
                            splitMeasurement = new string[] { firstMeasurement, latterMeasurement };
                            break;
                        }
                    }
                }
                if (count == 3) {
                    for (int j = 0; j < latterMeasurement.Count(); j++) {
                        if ((j > 1) && (j < multiLevelMeasurement.Count() - 1)) {
                            previous = j - 1;
                            next = j + 1;
                            var previousChar = latterMeasurement[previous];
                            var currentChar = latterMeasurement[j];
                            var nextChar = latterMeasurement[next]; 
                            if ((latterMeasurement[j] == ' ') && (!int.TryParse(latterMeasurement[previous].ToString(), out n)) && (int.TryParse(latterMeasurement[next].ToString(), out n))) {
                                secondMeasurement = latterMeasurement.Substring(0, j);
                                thirdMeasurement = latterMeasurement.Substring(next, (latterMeasurement.Count()) - (j + 1));
                                splitMeasurement = new string[] { firstMeasurement, secondMeasurement, thirdMeasurement };
                                break;
                            }
                        }
                    }
                }
            }
            //should i add the split egg measurement here? I shouldn't need to, it should still split the string
            return splitMeasurement;
        }
        //ok... a ?? i have is why does the condition on line 90 work, while I have a different condition for line 115 works, but it's different... but doing the same thing... woah
        public string[] SplitEggMeasurement(string eggMeasurement) {
            var eggSplitMeasurement = new string[] { }; 
            int n;
            var eggQuantity = "";
            var egg = ""; 
            for (int i = 0; i < eggMeasurement.Count(); i++) {
                if ((i > 0) && (i < eggMeasurement.Count() - 1)) {
                    var previous = i - 1;
                    var next = i + 1;
                    var previousChar = eggMeasurement[previous];
                    var currentchar = eggMeasurement[i];
                    var nextChar = eggMeasurement[next]; 
                    if ((eggMeasurement[i] == ' ') && (int.TryParse(eggMeasurement[previous].ToString(), out n)) && (!int.TryParse(eggMeasurement[next].ToString(), out n))) {
                        eggQuantity = eggMeasurement.Substring(0, i);
                        egg = eggMeasurement.Substring(next, (eggMeasurement.Count()) - (i + 1));
                        eggSplitMeasurement = new string[] { eggQuantity, egg };
                        break;
                    }
                }
            }
            return eggSplitMeasurement; 
        }
        public decimal AdjustToTeaspoons(string measurement) {
            var parseFraction = new ParseFraction();
            var splitMeasurement = new string[] { };
            var decimalMeasurement = 0m;
            var trimmedMeasurement = "";
            var convertToTeaspoonMeasurement = 0m;
            if (measurement.ToLower().Contains("cup")) {
                splitMeasurement = measurement.ToLower().Split('c');
                trimmedMeasurement = splitMeasurement[0].TrimEnd();
                decimalMeasurement = parseFraction.Parse(trimmedMeasurement);
                convertToTeaspoonMeasurement = CupsToTeaspoons(decimalMeasurement);
            }
            if ((measurement.ToLower().Contains("table"))) {
                splitMeasurement = measurement.ToLower().Split('t');
                trimmedMeasurement = splitMeasurement[0].TrimEnd();
                decimalMeasurement = parseFraction.Parse(trimmedMeasurement);
                convertToTeaspoonMeasurement = TablespoonsToTeaspoons(decimalMeasurement);
            }
            if ((measurement.ToLower().Contains("tea"))) {
                splitMeasurement = measurement.ToLower().Split('t');
                trimmedMeasurement = splitMeasurement[0].TrimEnd();
                decimalMeasurement = parseFraction.Parse(trimmedMeasurement);
                convertToTeaspoonMeasurement = decimalMeasurement;
            }
            return Math.Round(convertToTeaspoonMeasurement, 2);
        }
        public decimal AccumulatedTeaspoonMeasurement(string measurement) {
            var splitMeasurements = SplitMultiLevelMeasurement(measurement);
            var accumulatedTeaspoons = 0m;
            foreach (var meas in splitMeasurements)
                accumulatedTeaspoons += AdjustToTeaspoons(meas);
            return accumulatedTeaspoons;
        }
        public Func<decimal, decimal, decimal> ApplyMultiplierToTeaspoons => (teaspoons, multiplier) => Math.Round((teaspoons * multiplier), 2);
        public Func<decimal, decimal, decimal> ApplyMultiplierToEggs => (eggs, multiplier) => Math.Round((eggs * multiplier), 2);
        //this is literally the same method as above, except i have it with eggs. I'm doing this very temporarily to see the different places for the teaspoon manipulation and eggs manipulation, then i'll combine them. 
        public string CondenseTeaspoonMeasurement(decimal teaspoons) {
            var measDict = new Dictionary<string, decimal>();
            var condensedMeasurement = "";
            var adjustedTeaspoonMesaurement = teaspoons;
            do {
                if (adjustedTeaspoonMesaurement >= 576) {
                    if (measDict.Keys.Contains("cups"))
                        measDict["cups"] = measDict["cups"] + 12m;
                    if (!measDict.Keys.Contains("cups"))
                        measDict.Add("cups", 12m);
                    adjustedTeaspoonMesaurement -= 576m;
                }
                if (adjustedTeaspoonMesaurement >= 384) {
                    if (measDict.Keys.Contains("cups"))
                        measDict["cups"] = measDict["cups"] + 8m;
                    if (!measDict.Keys.Contains("cups"))
                        measDict.Add("cups", 8m);
                    adjustedTeaspoonMesaurement -= 384m;
                }
                if (adjustedTeaspoonMesaurement >= 192) {
                    if (measDict.Keys.Contains("cups"))
                        measDict["cups"] = measDict["cups"] + 4m;
                    if (!measDict.Keys.Contains("cups"))
                        measDict.Add("cups", 4m);
                    adjustedTeaspoonMesaurement -= 192m;
                }
                if (adjustedTeaspoonMesaurement >= 96) {
                    if (measDict.Keys.Contains("cups"))
                        measDict["cups"] = measDict["cups"] + 2m;
                    if (!measDict.Keys.Contains("cups"))
                        measDict.Add("cups", 2m);
                    adjustedTeaspoonMesaurement -= 96m;
                }
                if (adjustedTeaspoonMesaurement >= 48m) {
                    if (measDict.Keys.Contains("cups"))
                        measDict["cups"] = measDict["cups"] + 1m;
                    if (!measDict.Keys.Contains("cups"))
                        measDict.Add("cups", 1m);
                    adjustedTeaspoonMesaurement -= 48m;
                }
                if (adjustedTeaspoonMesaurement < 48 && adjustedTeaspoonMesaurement >= 24) {
                    if (measDict.Keys.Contains("cups"))
                        measDict["cups"] = measDict["cups"] + .5m;
                    if (!measDict.Keys.Contains("cups"))
                        measDict.Add("cups", .5m);
                    adjustedTeaspoonMesaurement -= 24m;
                }
                if (adjustedTeaspoonMesaurement < 24 && adjustedTeaspoonMesaurement >= 12) {
                    if (measDict.Keys.Contains("cups"))
                        measDict["cups"] = measDict["cups"] + .25m;
                    if (!measDict.Keys.Contains("cups"))
                        measDict.Add("cups", .25m);
                    adjustedTeaspoonMesaurement -= 12m;
                }
                if (adjustedTeaspoonMesaurement < 12m && adjustedTeaspoonMesaurement >= 3m) {
                    if (measDict.Keys.Contains("tablespoons"))
                        measDict["tablespoons"] = measDict["tablespoons"] + 1m;
                    if (!measDict.Keys.Contains("tablespoons"))
                        measDict.Add("tablespoons", 1m);
                    adjustedTeaspoonMesaurement -= 3m;
                }
                if (adjustedTeaspoonMesaurement < 3m && adjustedTeaspoonMesaurement >= 1) {
                    if (measDict.Keys.Contains("teaspoons"))
                        measDict["teaspoons"] = measDict["teaspoons"] + 1m;
                    if (!measDict.Keys.Contains("teaspoons"))
                        measDict.Add("teaspoons", 1m);
                    adjustedTeaspoonMesaurement -= 1m;
                }
                if (adjustedTeaspoonMesaurement < 1m && adjustedTeaspoonMesaurement > 0m) {
                    if (adjustedTeaspoonMesaurement < .28m && adjustedTeaspoonMesaurement > .22m)
                        adjustedTeaspoonMesaurement = .25m;
                    if (adjustedTeaspoonMesaurement == .50m)
                        adjustedTeaspoonMesaurement = .5m;
                    if (adjustedTeaspoonMesaurement > .95m)
                        adjustedTeaspoonMesaurement = 1m;
                    if (measDict.Keys.Contains("teaspoons"))
                        measDict["teaspoons"] = measDict["teaspoons"] + adjustedTeaspoonMesaurement;
                    if (!measDict.Keys.Contains("teaspoons"))
                        measDict.Add("teaspoons", adjustedTeaspoonMesaurement);
                    adjustedTeaspoonMesaurement -= adjustedTeaspoonMesaurement;
                }
            } while (adjustedTeaspoonMesaurement > 0m);
            foreach (KeyValuePair<string, decimal> measurement in measDict)
                condensedMeasurement += measurement.Value.ToString() + " " + measurement.Key + " ";
            return condensedMeasurement.TrimEnd();
        }
        public string AdjustIngredientMeasurement(string measurement, int originalYield, int desiredYield) {
            var multiplier = ChangeYieldMultiplier(originalYield, desiredYield);
            var eggs = 0m;
            string[] eggsSplitMeasurement; 
            var splitMeasurement = SplitMultiLevelMeasurement(measurement);
            //foreach (var meas in splitMeasurement) {
            //    if (meas.Contains("egg")) {
            //        eggsSplitMeasurement = SplitEggMeasurement(meas);
            //        eggs = (eggsSplitMeasurement[0]); 
            //    }
            //}
            var measurementConvertedToTeaspoons = AccumulatedTeaspoonMeasurement(measurement);
            var totalEggs = 0;
            var multipliedTeaspoonsAdjustment = multiplier * measurementConvertedToTeaspoons;
            var updatedMeasurement = CondenseTeaspoonMeasurement(multipliedTeaspoonsAdjustment);
            return updatedMeasurement;
        }
    }
    /*
     other desired functionalities for the Convert class for measurement ingredients: 
        *convert the measurement/amount of eggs. this will require some unique methods for this alone, as I won't need to accumulate teaspoons and parse them out to a string of measurements
            I can intertwine it in the SplitMultiLevelMeasurement, but that seems unnecessary, i want to just have it on its own. the egg quantity will not be a part of a multilevel measurement
        *converting the weight of ingredients based on their density, from and to the ingredient measurement
            if the weight of the ingredient is given, give the ingredient measurement in the comments, based on the density from the ingredient density database
            if the measurement is given, give the ingredient weight for checking the ingredient prices from the rest calls
        *convert the decimals from the decimals returned from the AdjustIngredientMeasurement to fractions, but keep the decimals in a database so i can multiply those decimals 
            with the multiplier as opposed to multiplying the approximate fractions given (keeping the best precision I can
               for example, .1667 (1/6) is only .0417 from .125 (1/8), but multiply 1/6 * 5 = 0.8335 whereas 1/8 * 5 = .625
               I realize this may not be as dramatic of a difference, but it's still important to get correct. 
    */

    public class ParseFraction {
        public decimal Parse(string fraction) {
            var splitComplexFraction = new string[] { };
            var finaldecimal = 0m;
            if (!fraction.Contains('/') && !fraction.Contains(' ')) {
                finaldecimal = decimal.Parse(fraction);
                return finaldecimal;
            }
            if (fraction.Contains(' ')) {
                splitComplexFraction = fraction.Split(' ');
                var split = splitComplexFraction[1].Split('/');
                var final = new decimal[] { decimal.Parse(splitComplexFraction[0]), decimal.Parse(split[0]), decimal.Parse(split[1]) };
                finaldecimal = (((final[0] * final[2]) + final[1]) / final[2]);
            }
            if (!fraction.Contains(' ')) {
                splitComplexFraction = fraction.Split('/');
                finaldecimal = decimal.Parse(splitComplexFraction[0]) / decimal.Parse(splitComplexFraction[1]);
            }
            return Math.Round(finaldecimal, 4);
        }
    }
}