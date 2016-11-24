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

    }
    public class ParseFraction {
        public decimal Parse(string fraction) {
            var splitComplexFraction = new string[] { };
            var finaldecimal = 0m;
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
            return Math.Round(finaldecimal, 2);
        }
    }
}
//var parse = new ParseFraction();
//var commonMeasurements = new string[] { "cups", "c", "teaspoons", "t", "tablespoons", "T", "oz", "ounces", "pinch", "eggs", "egg" };
//bool boolMeasurement = false;
//var ingredientMeasurement = "";
//var ingredientMeasurementArray = new string[] { }; 
//            //this is determining if it is an ingredient measurement string or not
//            foreach (var meas in commonMeasurements) {
//                if (measurement.Contains(meas))
//                    boolMeasurement = true;
//                else boolMeasurement = false;
//            }
//            if (boolMeasurement == true) {
//                var parsedMeasurement = measurement.Split(' ');
//var fraction = "";
//                foreach (var splitString in parsedMeasurement) {
//                    if (splitString != parsedMeasurement.Last())
//                        fraction += splitString.ToString();
//                        //an immediate problem i see here is I lose my spaces, which means i dramatically alter my fractional accuracy...
//                    if (splitString == parsedMeasurement.Last())
//                        ingredientMeasurement = splitString;
//                }
//                ingredientMeasurementArray.
//                ingredientMeasurementArray.ElementAt(0) = fraction; 
//            }