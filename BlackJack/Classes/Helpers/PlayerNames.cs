/* BlackJack - Version 1.0
 * Written by Jason James Newland
 * ©2025 - KangaSoft Software */
using System;
using System.Collections.Generic;

namespace BlackJack.Classes.Helpers
{
    public static class PlayerNames
    {
        private static readonly Random Rnd = new Random();

        private static readonly List<string> Female = new List<string>(new[]
        {
            "Rebecca",
            "Emily",
            "Melissa",
            "Amy",
            "Zoey",
            "Kate",
            "Phoebe",
            "Michelle",
            "Pauline",
            "Samantha",
            "Amanda",
            "Yolanda",
            "Claire",
            "Stacy",
            "Sarah",
            "Erin",
            "Kath",
            "Karen",
            "Annie",
            "Anna",
            "Hannah"
        });

        private static readonly List<string> Male = new List<string>(new[]
        {
            "John",
            "Paul",
            "Simon",
            "Jack",
            "Ronald",
            "Simone",
            "Lucas",
            "George",
            "Jonathan",
            "Sam",
            "Boris",
            "Peter",
            "Stuart",
            "Adrian",
            "Nathan",
            "Tim",
            "Tony",
            "Alan",
            "Aaron",
            "Xavier"
        });

        public static string GetRandomName
        {
            get
            {
                string firstName;
                var sex = Rnd.Next(0, 2); /* Yes, please! */
                switch (sex)
                {
                    case 0:
                        firstName = GetRandomFemaleName;
                        break;

                    default:
                        firstName = Male[Rnd.Next(0, Male.Count)];
                        break;
                }
                var lastInitial = (char) Rnd.Next(65, 91);
                return $"{firstName} {lastInitial}.";
            }
        }

        public static string GetRandomFemaleName => Female[Rnd.Next(0, Female.Count)];
    }
}
