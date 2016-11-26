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
        public Func<string, string[]> SplitMeasurement = MultiLevelMeasurement => MultiLevelMeasurement.ToLower().Split('p');
        //this split('p') doesn't work with tablesPoons, because of the p in spoons... 
        //i might try to see if i can find the next number... do a foreach loop o nteh chars and see if a tryparse will work, and then create a new string from there... `
        public decimal SplitMultiLevelMeasurements(string measurement) {
            //split measurement into 2 distint measurements, then add both the decimal teaspoons measurements and return the conglomerate teaspoons
            var parseFraction = new ParseFraction();
            var count = 0;
            var totalRetTeaspoons = 0m;
            var splitMeasurement = new string[] { };
            var secondMeasurementArray = new string[] { };
            var secondSplitMeasurement = new string[] { };
            var secondMeasurement = "";
            var trimmedMeasurement = "";
            var decimalMeasurement = 0m;
            var commonMeasurements = new string[] { "cups", "c", "teaspoons", "t", "tablespoons", "T", "oz", "ounces", "pinch", "eggs", "egg" };
            foreach (var meas in commonMeasurements) {
                if (measurement.Contains(meas))
                    count++;
            }
            if (count > 1) {
                if (((measurement.ToLower().Contains("cups")) || measurement.ToLower().Contains("cup")) && ((measurement.ToLower().Contains("tablespoon")) || (measurement.ToLower().Contains("tablespoons")))) {
                    int n;
                    splitMeasurement = measurement.ToLower().Split('p');
                    totalRetTeaspoons += AdjustToTeaspoons(splitMeasurement[0]);
                    secondMeasurement = splitMeasurement[1] + splitMeasurement[2];
                    if ((secondMeasurement.Contains("ablespoon")) || (secondMeasurement.Contains("eas"))) {
                        totalRetTeaspoons += AdjustToTeaspoons(secondMeasurement);
                    }
                }
            }
            if (count == 1)
                totalRetTeaspoons += AdjustToTeaspoons(measurement);
            return totalRetTeaspoons;
        }
        //eventually, i'll have a method that uses this AdjustToTeaspoons and uses the AdjustTeaspoonsBasedOnMultiplier and the ChangYeidlMultiplie
        public decimal AdjustToTeaspoons(string measurement) {
            var parseFraction = new ParseFraction();
            var splitMeasurement = new string[] { };
            var decimalMeasurement = 0m;
            var trimmedMeasurement = "";
            var convertToTeaspoonMeasurement = 0m;
            if ((measurement.ToLower().Contains("cu")) || (measurement.ToLower().Contains("ps"))) {
                //i have this weird stuff here because of the impairments that I have from splitting the measuremnts when there are multilevel measurements (1 cup 2 tablespoons)
                //if ((measurement.ToLower().Contains("cups")) || (measurement.ToLower().Contains("cup")) || (measurement.ToLower().Contains(" c"))) {
                //maybe put in functionality that handles already converted decimals (so .5 instead of 1/2)
                splitMeasurement = measurement.ToLower().Split('c'); //this should split it at the beginning of "cups" or 'c'... so splitMeasurement[1] is "ups" or "up"
                trimmedMeasurement = splitMeasurement[0].TrimEnd();
                decimalMeasurement = parseFraction.Parse(trimmedMeasurement);
                convertToTeaspoonMeasurement = CupsToTeaspoons(decimalMeasurement);
            }
            if ((measurement.ToLower().Contains("able"))) {
                //if ((measurement.ToLower().Contains("tablespoon")) || (measurement.ToLower().Contains("tablespoon")) || (measurement.ToLower().Contains("ablespoon"))) {
                splitMeasurement = measurement.ToLower().Split('t');
                trimmedMeasurement = splitMeasurement[0].TrimEnd();
                decimalMeasurement = parseFraction.Parse(trimmedMeasurement);
                convertToTeaspoonMeasurement = TablespoonsToTeaspoons(decimalMeasurement);
            }
            if ((measurement.ToLower().Contains("ea"))) {
                //if ((measurement.ToLower().Contains("teaspoons")) || measurement.ToLower().Contains("teaspoon")) {
                splitMeasurement = measurement.ToLower().Split('t');
                trimmedMeasurement = splitMeasurement[0].TrimEnd();
                decimalMeasurement = parseFraction.Parse(trimmedMeasurement);
                convertToTeaspoonMeasurement = decimalMeasurement;
            }
            return Math.Round(convertToTeaspoonMeasurement, 2);
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