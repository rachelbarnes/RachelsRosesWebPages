using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RachelsRosesWebPages {
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
            var convert = new Convert();
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
                if (dec == 5m || dec == 50m)
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
                returnedMeasurement = string.Format("{0} {1}", fractionSplitAtDecimalPoint[0], retFraction); 
                //returnedMeasurement = fractionSplitAtDecimalPoint[0] + retFraction; 
                //returnedMeasurement = fractionSplitAtDecimalPoint[0] + " " + retFraction + " " + splitMeasurement[1];
            return returnedMeasurement;
        }
    }
}