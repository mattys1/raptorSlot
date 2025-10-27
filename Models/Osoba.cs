using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CRUD.Models
{
    public class Osoba
    {
        public int id { get; set; }
        public string imie { get; set; }
        public string nazwisko { get; set; }
        public int waga { get; set; }
        public int wzrostCM { get; set; }

        [Display(Name = "bmi")]
        public double bmi
        {
            get
            {
                double wzrostM = wzrostCM / 100.0;
                if (wzrostM > 0)
                {
                    return Math.Round(waga / (wzrostM * wzrostM), 2);
                }
                return 0;
            }
        }

        public string InterpretacjaBMI
        {
            get
            {
                double bmi = this.bmi;

                if (bmi < 16.0)
                {
                    return "ciężka niedowaga (III stopień szczupłości)";
                }
                else if (bmi >= 16.0 && bmi <= 16.9)
                {
                    return "umiarkowana niedowaga (II stopień szczupłości)";
                }
                else if (bmi >= 17.0 && bmi <= 18.49)
                {
                    return "niedowaga (I stopień szczupłości)";
                }
                else if (bmi >= 18.5 && bmi <= 24.9)
                {
                    return "prawidłowa waga u osób w wieku 18-65";
                }
                else if (bmi >= 22.0 && bmi <= 27.0)
                {
                    return "średnia pożądana masa ciała u osób starszych (65+)";
                }
                else if (bmi >= 24.5 && bmi <= 29.9)
                {
                    return "nadwaga u osób w wieku 18-65";
                }
                else if (bmi >= 30.0 && bmi <= 34.9)
                {
                    return "otyłość I stopnia";
                }
                else if (bmi >= 35.0 && bmi <= 39.9)
                {
                    return "otyłość II stopnia";
                }
                else if (bmi > 40.0)
                {
                    return "otyłość III stopnia";
                }
                else
                {
                    return "brak danych do interpretacji";
                }
            }

        }
    }
}