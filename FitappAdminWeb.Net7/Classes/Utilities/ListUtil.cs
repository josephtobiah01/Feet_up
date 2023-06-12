using System.Globalization;

namespace FitappAdminWeb.Net7.Classes.Utilities
{
    public static class ListUtil
    {
        public static Dictionary<string, string> CountryList()
        {
            //Creating Dictionary
            Dictionary<string, string> cultureList = new Dictionary<string, string>();

            //getting the specific CultureInfo from CultureInfo class
            CultureInfo[] getCultureInfo = CultureInfo.GetCultures(CultureTypes.SpecificCultures);

            foreach (CultureInfo getCulture in getCultureInfo)
            {
                //creating the object of RegionInfo class
                RegionInfo getRegionInfo = new RegionInfo(getCulture.Name);
                //adding each country Name into the Dictionary
                if (!(cultureList.ContainsKey(getRegionInfo.Name)))
                {
                    cultureList.Add(getRegionInfo.Name, getRegionInfo.EnglishName);
                }
            }
            //returning country list
            return cultureList;
        }
    }
}
