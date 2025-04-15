/* BlackJack - Version 1.0
 * Written by Jason James Newland
 * ©2025 - KangaSoft Software */
namespace BlackJack.Classes.Helpers
{
    public static class Formatting
    {
        public static string FormatNumber(int num)
        {
            if (num >= 1000000000)
            {
                return (num / 1000000000D).ToString("0.##B");
            }
            if (num >= 1000000)
            {
                return (num / 1000000D).ToString("0.##M");
            }
            return num >= 1000 ? (num / 1000D).ToString("0.##k") : num.ToString("#,0");
        }
    }
}
