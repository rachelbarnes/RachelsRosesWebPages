using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using NUnit.Framework;
namespace RachelsRosesWebPages {
    public class Convert {
        public decimal teaspoonsToTablespoons(decimal t) {
            var ret = Math.Round((t / 3), 2);
            if (ret.ToString().Contains(".00"))
                Math.Round(ret, 0);
            return ret;
        }
        public decimal TablespoonsToTeaspoons(decimal T) {
            var ret = Math.Round((T * 3), 2);
            if (ret.ToString().Contains(".00"))
                Math.Round(ret, 0);
            return ret;
        }
        public decimal teaspoonsToCups(decimal t) {
            var ret = Math.Round((t / 48), 2);
            if (ret.ToString().Contains(".00"))
                Math.Round(ret, 0);
            return ret;
        }
        public decimal CupsToTeaspoons(decimal c) {
            var ret = Math.Round((c * 48), 2);
            if (ret.ToString().Contains(".00"))
                Math.Round(ret, 0);
            return ret;
        }
        public decimal CupsToTablespoons(decimal c) {
            var ret = Math.Round((c * 16), 2);
            if (ret.ToString().Contains(".00"))
                Math.Round(ret, 0);
            return ret;
        }
        public decimal TablespoonsToCups(decimal T) {
            var ret = Math.Round((T / 16), 2);
            if (ret.ToString().Contains(".00"))
                Math.Round(ret, 2);
            return ret;
        }
        public decimal TeaspoonsToPinches(decimal t) {
            var ret = Math.Round((t * 16), 2);
            if (ret.ToString().Contains(".00"))
                Math.Round(ret, 0);
            return ret;
        }
        public decimal PinchesToTeaspoons(decimal p) {
            var ret = Math.Round((p / 16), 2);
            if (ret.ToString().Contains(".00"))
                Math.Round(ret, 0);
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
            var commonMeasurements = new string[] { "cup", "tablespoon", "teaspoon", "pinch", "egg" };
            var firstMeasurement = "";
            var secondMeasurement = "";
            var latterMeasurement = "";
            var thirdMeasurement = "";
            var fourthMeasurement = "";
            foreach (var meas in commonMeasurements) {
                if (multiLevelMeasurement.Contains(meas))
                    count++;
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
                if (count >= 3) {
                    for (int j = 0; j < latterMeasurement.Count(); j++) {
                        if ((j > 0) && (j < latterMeasurement.Count() - 1)) {
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
                    if (count == 4) {
                        var newThirdMeasurement = "";
                        for (int j = 0; j < thirdMeasurement.Count(); j++) {
                            if ((j > 0) && (j < thirdMeasurement.Count() - 1)) {
                                previous = j - 1;
                                next = j + 1;
                                var previousChar = thirdMeasurement[previous];
                                var currentChar = thirdMeasurement[j];
                                var nextChar = thirdMeasurement[next];
                                if ((thirdMeasurement[j] == ' ') && (!int.TryParse(thirdMeasurement[previous].ToString(), out n)) && (int.TryParse(thirdMeasurement[next].ToString(), out n))) {
                                    newThirdMeasurement = thirdMeasurement.Substring(0, j);
                                    fourthMeasurement = thirdMeasurement.Substring(next, (thirdMeasurement.Count()) - (j + 1));
                                    splitMeasurement = new string[] { firstMeasurement, secondMeasurement, newThirdMeasurement, fourthMeasurement };
                                    break;
                                }
                            }
                        }
                    }
                }
            }
            return splitMeasurement;
        }
        public string SplitAndAdjustEggMeasurement(string eggMeasurement, decimal multiplier) {
            var parse = new ParseFraction();
            var adjustedEggMeasurement = "";
            int n;
            var eggsAdjusted = "";
            var eggQuantity = "";
            var typeOfEggs = ""; 
            for (int i = 0; i < eggMeasurement.Count(); i++) {
                if ((i > 0) && (i < eggMeasurement.Count() - 1)) {
                    var previous = i - 1;
                    var next = i + 1;
                    if ((eggMeasurement[i] == ' ') && (int.TryParse(eggMeasurement[previous].ToString(), out n)) && (!int.TryParse(eggMeasurement[next].ToString(), out n))) {
                        eggQuantity = eggMeasurement.Substring(0, i);
                        typeOfEggs = eggMeasurement.Substring(next, (eggMeasurement.Count() - (i + 1))); 
                        if ((parse.Parse(eggQuantity) * multiplier) < 10) {
                            eggsAdjusted = (parse.Parse(eggQuantity) * multiplier).ToString().TrimEnd('0');
                        } else { eggsAdjusted = (parse.Parse(eggQuantity) * multiplier).ToString(); }
                        if ((eggsAdjusted.Contains(".00") && eggsAdjusted.Contains(".0")) || eggsAdjusted.Contains('.'))
                            eggsAdjusted = eggsAdjusted.TrimEnd('.');
                        adjustedEggMeasurement = eggsAdjusted.TrimStart('0') + " " + typeOfEggs;
                        break;
                    }
                }
            }
            return adjustedEggMeasurement;
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
            if ((measurement.ToLower().Contains("pinch"))) {
                splitMeasurement = measurement.ToLower().Split('p');
                trimmedMeasurement = splitMeasurement[0].TrimEnd();
                decimalMeasurement = parseFraction.Parse(trimmedMeasurement);
                convertToTeaspoonMeasurement = PinchesToTeaspoons(decimalMeasurement);
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
                if (adjustedTeaspoonMesaurement < 24 && adjustedTeaspoonMesaurement >= 16) {
                    if (measDict.Keys.Contains("cups"))
                        measDict["cups"] = measDict["cups"] + .33m;
                    if (!measDict.Keys.Contains("cups"))
                        measDict.Add("cups", .33m);
                    adjustedTeaspoonMesaurement -= 16m;
                }
                if (adjustedTeaspoonMesaurement < 16 && adjustedTeaspoonMesaurement >= 12) {
                    if (measDict.Keys.Contains("cups"))
                        measDict["cups"] = measDict["cups"] + .25m;
                    if (!measDict.Keys.Contains("cups"))
                        measDict.Add("cups", .25m);
                    adjustedTeaspoonMesaurement -= 12m;
                }
                if (adjustedTeaspoonMesaurement < 12m && adjustedTeaspoonMesaurement >= 6m) {
                    if (measDict.Keys.Contains("cups"))
                        measDict["cups"] = measDict["cups"] + .125m;
                    if (!measDict.Keys.Contains("cups"))
                        measDict.Add("cups", .125m);
                    adjustedTeaspoonMesaurement -= 6m;
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
                if (adjustedTeaspoonMesaurement < 1m && adjustedTeaspoonMesaurement >= .125m) {
                    if (adjustedTeaspoonMesaurement < .28m && adjustedTeaspoonMesaurement > .22m)
                        adjustedTeaspoonMesaurement = .25m;
                    if (adjustedTeaspoonMesaurement > .95m)
                        adjustedTeaspoonMesaurement = 1m;
                    if (measDict.Keys.Contains("teaspoons"))
                        measDict["teaspoons"] = measDict["teaspoons"] + .125m;
                    if (!measDict.Keys.Contains("teaspoons"))
                        measDict.Add("teaspoons", .125m);
                    adjustedTeaspoonMesaurement -= .125m;
                }
                if (adjustedTeaspoonMesaurement < .125m && adjustedTeaspoonMesaurement > 0m) {
                    if (measDict.Keys.Contains("pinches"))
                        measDict["pinches"] = measDict["pinches"] + 1;
                    if (!measDict.Keys.Contains("pinches"))
                        measDict.Add("pinches", 1);
                    adjustedTeaspoonMesaurement -= .06m;
                    if (adjustedTeaspoonMesaurement < .05m && adjustedTeaspoonMesaurement > 0m)
                        adjustedTeaspoonMesaurement = 0;
                }
            } while (adjustedTeaspoonMesaurement > 0m);
            foreach (KeyValuePair<string, decimal> measurement in measDict) {
                var valArr = new string[] { };
                var value = Math.Round(measurement.Value, 3).ToString().TrimStart('0');
                if (value.Contains(".00") || value.Contains(".0")) {
                    //i am not a fan of this being a split... this has to go into my To Refactor code!!!!
                    valArr = value.Split('.');
                    condensedMeasurement += valArr[0].TrimEnd('0') + " " + measurement.Key + " ";
                } else { condensedMeasurement += value.TrimEnd('0') + " " + measurement.Key + " "; }
            }
            return condensedMeasurement.TrimEnd();
        }
        public string AdjustIngredientMeasurement(string measurement, int originalYield, int desiredYield) {
            var updatedMeasurement = "";
            var multiplier = ChangeYieldMultiplier(originalYield, desiredYield);
            var splitMeasurement = SplitMultiLevelMeasurement(measurement);
            for (int i = 0; i < splitMeasurement.Count(); i++) {
                if (splitMeasurement[i].Contains("egg")) {
                    var eggMeasurement = "";
                    eggMeasurement = SplitAndAdjustEggMeasurement(splitMeasurement[i], multiplier);
                    splitMeasurement[i] = eggMeasurement;
                    updatedMeasurement = splitMeasurement[i];
                    return updatedMeasurement;
                    //here, i'm assuming that no eggs will be entered with other ingredients... 
                    //i've never an ingredient measurement with other measurements than cups... this will work apart from user error
                }
            }
            var measurementConvertedToTeaspoons = AccumulatedTeaspoonMeasurement(measurement);
            var multipliedTeaspoonsAdjustment = multiplier * measurementConvertedToTeaspoons;
            updatedMeasurement = CondenseTeaspoonMeasurement(multipliedTeaspoonsAdjustment);
            return updatedMeasurement;
        }
    }

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
