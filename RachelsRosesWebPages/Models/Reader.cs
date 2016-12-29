using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using RachelsRosesWebPages.Models;
namespace RachelsRosesWebPages {
    public class Reader {
        public Dictionary<string, decimal> ReadDensityTextFile(string filename) {
            var currentDensityLine = "";
            var densityDatabase = new Dictionary<string, decimal>();
            var currentDensityLineSplit = new string[] { };
            using (StreamReader readDensityFile = new StreamReader(filename)) {
                while ((currentDensityLine = readDensityFile.ReadLine()) != null) {
                    if (currentDensityLine.Contains(':')) {
                        currentDensityLineSplit = currentDensityLine.Split(':');
                        densityDatabase.Add(currentDensityLineSplit[0], decimal.Parse(currentDensityLineSplit[1]));
                    }
                }
            }
            return densityDatabase;
        }
    }

}