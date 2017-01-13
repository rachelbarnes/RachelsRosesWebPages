using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using RachelsRosesWebPages.Models;
namespace RachelsRosesWebPages {
    public class ConvertMeasurement {
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
            var commonMeasurements = new string[] { "cup", "tablespoon", "teaspoon", "pinch", };
            //i dont have eggs in here because i've never seen eggs in with a multi level measurement
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
        public string[] SplitAndAdjustEggMeasurement(string eggMeasurement, decimal multiplier) {
            var parse = new ParseFraction();
            var adjustedEggMeasurement = new string[] { };
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
                        eggsAdjusted = (parse.Parse(eggQuantity) * multiplier).ToString();
                        var eggsAdjustedArr = new string[] { };
                        if ((eggsAdjusted.Contains(".00") || eggsAdjusted.Contains(".0")) || eggsAdjusted[eggsAdjusted.Count() - 1] == '.') {
                            eggsAdjustedArr = eggsAdjusted.Split('.');
                            eggsAdjusted = eggsAdjustedArr[0];
                        }
                        adjustedEggMeasurement = new string[] { eggsAdjusted, typeOfEggs };
                        break;
                    }
                }
            }
            return adjustedEggMeasurement;
            //a question for the future: what to do with the liquid eggs, like the egg beaters... that will obviously have eggs in the name, but will need to be dealt with differently
            //the easiest solution I can see if the name has cups, tablespoons or teaspoons to return the valule of splitMultiLevelMeasurement(egg measurement)
        }
        public string[] SplitMeasurement(string measurement) {
            var measurementLength = measurement.Length;
            var firstPartOfTheMeasurement = "";
            var secondPartOfTheMeasurement = "";
            var splitMeasurement = new string[] { };
            for (int i = 0; i < measurement.Length; i++) {
                if (i > 0 && i < measurement.Length - 1) {
                    var prev = i - 1;
                    var next = i + 1;
                    var curr = i;
                    var prevChar = measurement[prev];
                    var nextChar = measurement[next];
                    var currChar = measurement[curr];
                    int n;
                    if ((int.TryParse(measurement[prev].ToString(), out n)) && (measurement[curr] == ' ') && !(int.TryParse(measurement[next].ToString(), out n))) {
                        splitMeasurement = measurement.Trim().Split(measurement[curr]);
                        firstPartOfTheMeasurement = splitMeasurement[0];
                        secondPartOfTheMeasurement = splitMeasurement[1];
                        if (splitMeasurement.Count() == 3) {
                            firstPartOfTheMeasurement = firstPartOfTheMeasurement + " " + secondPartOfTheMeasurement;
                            secondPartOfTheMeasurement = splitMeasurement[2];
                            splitMeasurement = new string[] { firstPartOfTheMeasurement, secondPartOfTheMeasurement };
                            break;
                        }
                        break;
                    }
                }
            }
            return splitMeasurement;
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
        //there is so much repetition here... this has to be fixed.
        //pattern: dictionary with keys: cups, tablespoons, teaspoons, pinches, 
        //i can have these numbers in an array, and have the teaspoon measurement go through the numbers in the array and add to the dictionary based on the corresponding value... 
        //the majority of the filled lines here come from adding to the dictionary... 
        //this one should be pretty easy, actually, just having a method to add to the dictionary cups, tablespoons or pinches based whether the teaspoon amount >= 48, etc. 
        public string CondenseTeaspoonMeasurement(decimal teaspoons) {
            var parse = new ParseFraction();
            var measDict = new Dictionary<string, decimal>();
            var condensedMeasurement = "";
            do {
                if (teaspoons >= 576) {
                    if (measDict.Keys.Contains("cups"))
                        measDict["cups"] = measDict["cups"] + 12m;
                    if (!measDict.Keys.Contains("cups"))
                        measDict.Add("cups", 12m);
                    teaspoons -= 576m;
                }
                if (teaspoons >= 384) {
                    if (measDict.Keys.Contains("cups"))
                        measDict["cups"] = measDict["cups"] + 8m;
                    if (!measDict.Keys.Contains("cups"))
                        measDict.Add("cups", 8m);
                    teaspoons -= 384m;
                }
                if (teaspoons >= 192) {
                    if (measDict.Keys.Contains("cups"))
                        measDict["cups"] = measDict["cups"] + 4m;
                    if (!measDict.Keys.Contains("cups"))
                        measDict.Add("cups", 4m);
                    teaspoons -= 192m;
                }
                if (teaspoons >= 96) {
                    if (measDict.Keys.Contains("cups"))
                        measDict["cups"] = measDict["cups"] + 2m;
                    if (!measDict.Keys.Contains("cups"))
                        measDict.Add("cups", 2m);
                    teaspoons -= 96m;
                }
                if (teaspoons >= 48m) {
                    if (measDict.Keys.Contains("cups"))
                        measDict["cups"] = measDict["cups"] + 1m;
                    if (!measDict.Keys.Contains("cups"))
                        measDict.Add("cups", 1m);
                    teaspoons -= 48m;
                }
                if (teaspoons < 48 && teaspoons >= 32) {
                    if (measDict.Keys.Contains("cups"))
                        measDict["cups"] = measDict["cups"] + .66m;
                    if (!measDict.Keys.Contains("cups"))
                        measDict.Add("cups", .66m);
                    teaspoons -= 32;
                }
                if (teaspoons < 32 && teaspoons >= 24) {
                    if (measDict.Keys.Contains("cups"))
                        measDict["cups"] = measDict["cups"] + .5m;
                    if (!measDict.Keys.Contains("cups"))
                        measDict.Add("cups", .5m);
                    teaspoons -= 24m;
                }
                if (teaspoons < 24 && teaspoons >= 16) {
                    if (measDict.Keys.Contains("cups"))
                        measDict["cups"] = measDict["cups"] + .33m;
                    if (!measDict.Keys.Contains("cups"))
                        measDict.Add("cups", .33m);
                    teaspoons -= 16m;
                }
                if (teaspoons < 16 && teaspoons >= 12) {
                    if (measDict.Keys.Contains("cups"))
                        measDict["cups"] = measDict["cups"] + .25m;
                    if (!measDict.Keys.Contains("cups"))
                        measDict.Add("cups", .25m);
                    teaspoons -= 12m;
                }
                if (teaspoons < 12m && teaspoons >= 3m) {
                    if (measDict.Keys.Contains("tablespoons"))
                        measDict["tablespoons"] = measDict["tablespoons"] + 1m;
                    if (!measDict.Keys.Contains("tablespoons"))
                        measDict.Add("tablespoons", 1m);
                    teaspoons -= 3m;
                }
                if (teaspoons < 3m && teaspoons >= 1) {
                    if (measDict.Keys.Contains("teaspoons"))
                        measDict["teaspoons"] = measDict["teaspoons"] + 1m;
                    if (!measDict.Keys.Contains("teaspoons"))
                        measDict.Add("teaspoons", 1m);
                    teaspoons -= 1m;
                }
                if (teaspoons < 1m && teaspoons >= .125m) {
                    if (teaspoons < .28m && teaspoons > .22m)
                        teaspoons = .25m;
                    if (teaspoons > .95m)
                        teaspoons = 1m;
                    if (measDict.Keys.Contains("teaspoons"))
                        measDict["teaspoons"] = measDict["teaspoons"] + .125m;
                    if (!measDict.Keys.Contains("teaspoons"))
                        measDict.Add("teaspoons", .125m);
                    teaspoons -= .125m;
                }
                if (teaspoons < .125m && teaspoons > 0m) {
                    if (measDict.Keys.Contains("pinches"))
                        measDict["pinches"] = measDict["pinches"] + 1;
                    if (!measDict.Keys.Contains("pinches"))
                        measDict.Add("pinches", 1);
                    teaspoons -= .06m;
                    if (teaspoons < .05m && teaspoons > 0m)
                        teaspoons = 0;
                }
            } while (teaspoons > 0m);
            foreach (KeyValuePair<string, decimal> measurement in measDict) {
                var valArr = new string[] { };
                var value = Math.Round(measurement.Value, 3).ToString().TrimStart('0');
                if (value.Contains(".00") || value.Contains(".0")) {
                    valArr = value.Split('.');
                    var singleCondensedMeasurement = parse.ParseDecimalToFraction(valArr[0].TrimEnd('0') + " " + measurement.Key) + " ";
                    condensedMeasurement += singleCondensedMeasurement;
                } else {
                    var singleCondensedMeasurement = parse.ParseDecimalToFraction(value.TrimEnd('0') + " " + measurement.Key) + " ";
                    condensedMeasurement += singleCondensedMeasurement;
                }
            }
            var sampleTest = condensedMeasurement.Trim();
            return condensedMeasurement.TrimEnd();
        }
        public string AdjustIngredientMeasurement(string measurement, int originalYield, int desiredYield) {
            var updatedMeasurement = "";
            var multiplier = ChangeYieldMultiplier(originalYield, desiredYield);
            if (measurement.Contains("egg")) {
                var eggMeasurement = SplitAndAdjustEggMeasurement(measurement, multiplier);
                updatedMeasurement = eggMeasurement[0] + " " + eggMeasurement[1];
                return updatedMeasurement;
            }
            var splitMeasurement = SplitMultiLevelMeasurement(measurement);
            var measurementConvertedToTeaspoons = AccumulatedTeaspoonMeasurement(measurement);
            var multipliedTeaspoonsAdjustment = multiplier * measurementConvertedToTeaspoons;
            updatedMeasurement = CondenseTeaspoonMeasurement(multipliedTeaspoonsAdjustment);
            return updatedMeasurement;
        }
    }

    public class ConvertWeight {
        public decimal PoundsToOunces(decimal lb) {
            var ret = Math.Round((lb * 16), 2);
            if (ret.ToString().Contains(".00"))
                Math.Round(ret, 2);
            return ret;
        }
        public decimal OuncesToPounds(decimal oz) {
            var ret = Math.Round((oz / 16), 2);
            if (ret.ToString().Contains(".00"))
                Math.Round(ret, 2);
            return ret;
        }
        public decimal QuartsToOunces(decimal q) {
            var ret = Math.Round((q * 32), 2);
            if (ret.ToString().Contains(".00"))
                Math.Round(ret, 2);
            return ret;
        }
        public decimal OuncesToQuarts(decimal oz) {
            var ret = Math.Round((oz / 32), 2);
            if (ret.ToString().Contains(".00"))
                Math.Round(ret, 2);
            return ret;
        }
        public decimal GallonsToOunces(decimal g) {
            var ret = Math.Round((g * 128), 2);
            if (ret.ToString().Contains(".00"))
                Math.Round(ret, 2);
            return ret;
        }
        public decimal OuncesToGallons(decimal oz) {
            var ret = Math.Round((oz / 128), 2);
            if (ret.ToString().Contains(".00"))
                Math.Round(ret, 2);
            return ret;
        }
        public decimal PintsToOunces(decimal pint) {
            var ret = Math.Round((pint * 16), 2);
            if (ret.ToString().Contains(".00"))
                Math.Round(ret, 2);
            return ret;
        }
        public decimal OuncesToPints(decimal oz) {
            var ret = Math.Round((oz / 16), 2);
            if (ret.ToString().Contains(".00"))
                Math.Round(ret, 2);
            return ret;
        }
        public decimal CupsToOunces(decimal c) {
            var ret = Math.Round((c * 8), 2);
            if (ret.ToString().Contains(".00"))
                Math.Round(ret, 2);
            return ret;
        }
        public decimal OuncesToCups(decimal oz) {
            var ret = Math.Round((oz / 8), 2);
            if (ret.ToString().Contains(".00"))
                Math.Round(ret, 2);
            return ret;
        }
        public decimal GramsToOunces(decimal g) {
            var ret = Math.Round((g / 28.3495m), 2);
            if (ret.ToString().Contains(".00"))
                Math.Round(ret, 2);
            return ret;
        }
        public decimal OuncesToGrams(decimal oz) {
            var ret = Math.Round((oz / .035274m), 2);
            if (ret.ToString().Contains(".00"))
                Math.Round(ret, 2);
            return ret;
        }
        public decimal PoundsToGrams(decimal lb) {
            var ret = Math.Round((lb * 453.592m), 2);
            if (ret.ToString().Contains(".00"))
                Math.Round(ret, 2);
            return ret;
        }
        public decimal GramsToPounds(decimal g) {
            var ret = Math.Round((g * .0022m), 2);
            if (ret.ToString().Contains(".00"))
                Math.Round(ret, 2);
            return ret;
        }
        public string[] SplitWeightMeasurement(string weightMeasurement) {
            var splitWeight = new string[] { };
            var weightQuantity = "";
            var weight = "";
            for (var i = 0; i < weightMeasurement.Count(); i++) {
                if (i > 0 && (i < weightMeasurement.Count() - 1)) {
                    var previous = i - 1;
                    var next = i + 1;
                    var previousChar = weightMeasurement[previous];
                    var currentChar = weightMeasurement[i];
                    var nextChar = weightMeasurement[next];
                    int n;
                    if ((weightMeasurement[i] == ' ') && (int.TryParse(weightMeasurement[previous].ToString(), out n)) && (!int.TryParse(weightMeasurement[next].ToString(), out n))) {
                        weightQuantity = weightMeasurement.Substring(0, i);
                        weight = weightMeasurement.Substring(next, (weightMeasurement.Count() - (i + 1)));
                        splitWeight = new string[] { weightQuantity, weight };
                        break;
                    }
                }
            }
            if (splitWeight.Count() == 0) {
                //if there are no spaces, split it at the end of the numbers
                for (var i = 0; i < weightMeasurement.Count(); i++) {
                    if (i > 0 && (i < weightMeasurement.Count() - 1)) {
                        var previous = i - 1;
                        var next = i + 1;
                        var previousChar = weightMeasurement[previous];
                        var currentChar = weightMeasurement[i];
                        var nextChar = weightMeasurement[next];
                        int n;
                        if ((int.TryParse(weightMeasurement[i].ToString(), out n)) && (!int.TryParse(weightMeasurement[next].ToString(), out n))) {
                            weightQuantity = weightMeasurement.Substring(0, next);
                            weight = weightMeasurement.Substring(next, (weightMeasurement.Count() - (i + 1)));
                            splitWeight = new string[] { weightQuantity, weight };
                            break;
                        }
                    }
                }
            }
            return splitWeight;
        }
        public decimal ConvertWeightToOunces(string weight) {
            var measurements = new string[] { "gall", "cup", "pint", "quart", "pound", "lb", "oz", "ounce", "gram" };
            var count = 0;
            foreach (var measurement in measurements) {
                if (weight.ToLower().Contains(measurement)) {
                    count++;
                    break;
                }
            }
            if (count == 1) {
                var parse = new ParseFraction();
                weight = weight.ToLower();
                var splitWeight = SplitWeightMeasurement(weight);
                var weightToBeConverted = parse.Parse(splitWeight[0]);
                if (weight.ToLower().Contains("gall"))
                    return GallonsToOunces(weightToBeConverted);
                if (weight.ToLower().Contains("pint"))
                    return PintsToOunces(weightToBeConverted);
                if (weight.ToLower().Contains("quart"))
                    return QuartsToOunces(weightToBeConverted);
                if (weight.ToLower().Contains("pound") || weight.Contains("lb"))
                    return PoundsToOunces(weightToBeConverted);
                if (weight.ToLower().Contains("cup"))
                    return CupsToOunces(weightToBeConverted);
                if (weight.ToLower().Contains("gram"))
                    return GramsToOunces(weightToBeConverted);
                else return Math.Round((weightToBeConverted), 2);
            } else return 0m;
        }
        public decimal NumberOfEggsFromSellingQuantity(string quantity) {
            int n;
            var quantityArray = quantity.Split(' ');
            if (quantityArray.Count() == 1 && quantity.ToLower() == "dozen")
                return 12m;
            if (quantityArray.Count() == 2 && int.TryParse(quantityArray[0], out n) && quantityArray[1].ToLower() == "dozen")
                return ((int.Parse(quantityArray[0])) * 12);
            if (quantityArray.Count() == 1 && int.TryParse(quantityArray[0], out n))
                return (int.Parse(quantityArray[0]));
            else return 0m;
        }
        public decimal EggsConsumedFromIngredientMeasurement(string measurement) {
            var parse = new ParseFraction();
            var measurementArray = SplitWeightMeasurement(measurement);
            return parse.Parse(measurementArray[0]); 
        }
    }

    public class ConvertDensity {
        public decimal PercentageUsedMeasurementToStandardMeasurement(Ingredient i) {
            var convertWeight = new ConvertWeight();
            var convert = new ConvertMeasurement();
            var measurementInOunces = 0m;
            if (i.classification.ToLower().Contains("egg")) {
                var splitEggMeasurement = convertWeight.SplitWeightMeasurement(i.sellingWeight);
                measurementInOunces = decimal.Parse(splitEggMeasurement[0]);
                return Math.Round((decimal)(measurementInOunces), 4);
            } else return Math.Round((convert.AccumulatedTeaspoonMeasurement(i.measurement) / convert.AccumulatedTeaspoonMeasurement("1 cup")), 4); 
            //return Math.Round((decimal)(measurementInOunces / convert.AccumulatedTeaspoonMeasurement("1 cup")), 4);
        }

        public decimal CalculateOuncesUsed(Ingredient i) {
            var percentage = PercentageUsedMeasurementToStandardMeasurement(i); 
            return Math.Round((PercentageUsedMeasurementToStandardMeasurement(i) * i.density), 2);
        }
    }

    public class ParseFraction {
        public decimal Parse(string fraction) {
            foreach (var ch in fraction) {
                int n;
                if ((!int.TryParse(ch.ToString(), out n)) && (ch != '/') && (ch != '.') && (ch != ' '))
                    return 0m;
            }
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
        public string ParseDecimalToFraction(string measurement) {
            var convert = new ConvertMeasurement();
            var splitMeasurement = convert.SplitMeasurement(measurement);
            var decimalPortion = decimal.Parse(splitMeasurement[0]);
            var retFraction = "";
            var fractionSplitAtDecimalPoint = new string[] { };
            if (splitMeasurement[0].Contains('.')) {
                fractionSplitAtDecimalPoint = splitMeasurement[0].Split('.');
                var dec = decimal.Parse(fractionSplitAtDecimalPoint[1]);
                if (dec == 75m)
                    retFraction = "3/4";
                if (dec == 66m)
                    retFraction = "2/3";
                if (dec == 5m)
                    retFraction = "1/2";
                if (dec == 33m)
                    retFraction = "1/3";
                if (dec == 25m)
                    retFraction = "1/4";
                if (dec == 125m)
                    retFraction = "1/8";
            }
            var returnedMeasurement = "";
            if (string.IsNullOrEmpty(retFraction))
                return decimalPortion + " " + splitMeasurement[1];
            if (string.IsNullOrEmpty(fractionSplitAtDecimalPoint[0]))
                return retFraction + " " + splitMeasurement[1];
            if (fractionSplitAtDecimalPoint.Count() != 0)
                returnedMeasurement = fractionSplitAtDecimalPoint[0] + " " + retFraction + " " + splitMeasurement[1];
            return returnedMeasurement;
        }
    }
}
/*
 other desired functionalities for the Convert class for measurement ingredients: 
    *convert the decimals from the decimals returned from the AdjustIngredientMeasurement to fractions, but keep the decimals in a database so i can multiply those decimals 
        with the multiplier as opposed to multiplying the approximate fractions given (keeping the best precision I can
           for example, .1667 (1/6) is only .0417 from .125 (1/8), but multiply 1/6 * 5 = 0.8335 whereas 1/8 * 5 = .625
           I realize this may not be as dramatic of a difference, but it's still important to get correct. 
    *make sure i can parse through the itemresponse names... some of them are giving me trouble by not being able to separate the names... 
        maybe say that foreach item that passes through the 
*/
