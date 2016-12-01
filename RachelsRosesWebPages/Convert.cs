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
        public string[] SplitMultiLevelMeasurement(string multiLevelMeasurement) { //this doens't account for eggs... i'll need something special for the eggs, but it shouldn't be difficult... 
            string[] splitMeasurement = new string[] { };
            //for (int i = 0; i < multiLevelMeasurement.Count(); i++) {
            int previous;
            int next;
            int n;
            var count = 0;
            var commonMeasurements = new string[] { "cup", "tablespoon", "teaspoon" };
            var firstMeasurement = "";
            var secondMeasurement = "";
            var thirdMeasurement = "";
            var latterMeasurement = "";
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
                        var previousChar = multiLevelMeasurement[previous];
                        var nextChar = multiLevelMeasurement[next];
                        var currentChar = multiLevelMeasurement[i]; 
                        if ((multiLevelMeasurement[i] == ' ') && (!int.TryParse(multiLevelMeasurement[previous].ToString(), out n)) && (int.TryParse(multiLevelMeasurement[next].ToString(), out n))) {
                            firstMeasurement = multiLevelMeasurement.Substring(0, i);
                            latterMeasurement = multiLevelMeasurement.Substring(i + 1, (multiLevelMeasurement.Count()) - (i + 1));
                            break;
                            //the firstMeasurement and finalMeasurements here are fine... but when i return the splitMeasurement, I keep getting errors
                        }
                    }
                    splitMeasurement = new string[] { firstMeasurement, latterMeasurement}; 
                }
                if (count == 3) {
                    for (int j = 0; j < latterMeasurement.Count(); j++) {
                        if ((j > 1) && (j < multiLevelMeasurement.Count() - 1)) {
                            previous = j - 1;
                            next = j + 1;
                            var previousChar = latterMeasurement[previous];
                            var nextChar = latterMeasurement[next];
                            if ((latterMeasurement[j] == ' ') && (!int.TryParse(latterMeasurement[previous].ToString(), out n)) && (int.TryParse(latterMeasurement[next].ToString(), out n))) {
                                secondMeasurement = latterMeasurement.Substring(0, j);
                                thirdMeasurement = latterMeasurement.Substring(j + 1, (latterMeasurement.Count()) - (j + 1));
                                break;
                            }
                        }
                    }
                    splitMeasurement = new string[] { firstMeasurement, secondMeasurement, thirdMeasurement };
                }
            }
            //}
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
        public string CondenseTeaspoonMeasurement(decimal teaspoons) {
            var measDict = new Dictionary<string, decimal>();
            var condensedMeasurement = "";
            var adjustedTeaspoonMesaurement = teaspoons;
            do {
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
                if (adjustedTeaspoonMesaurement < 48 && adjustedTeaspoonMesaurement >= 12) {
                    if (measDict.Keys.Contains("cups"))
                        measDict["cups"] = measDict["cups"] + .25m;
                    if (!measDict.Keys.Contains("cups"))
                        measDict.Add("cups", .25m);
                    adjustedTeaspoonMesaurement -= 12m;
                }
                if (adjustedTeaspoonMesaurement < 48m && adjustedTeaspoonMesaurement >= 3m) {
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
            //i'm not a big fan of the order of this desired and original yield... 
            //it's just an order thing, but this is done, so it's all good
            var measurementConvertedToTeaspoons = AccumulatedTeaspoonMeasurement(measurement);
            var multipliedTeaspoonsAdjustment = multiplier * measurementConvertedToTeaspoons;
            var updatedMeasurement = CondenseTeaspoonMeasurement(multipliedTeaspoonsAdjustment);
            return updatedMeasurement;
        }
    }

    public class ParseFraction {
        public decimal Parse(string fraction) {
            var splitComplexFraction = new string[] { };
            var finaldecimal = 0m;
            if (!fraction.Contains('/') && !fraction.Contains(' ')) {
                finaldecimal = Int32.Parse(fraction);
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