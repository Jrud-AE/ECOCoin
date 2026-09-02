using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EcoWallet
{
    internal class GlobalFunctions
    {
        public static void NavigateToPageAndDropNavigation(string pageName)
        {
            // Implement navigation logic here
            Shell.Current.GoToAsync("//CreateAccountPage");

            //var navigationStack = Shell.Current.Navigation.NavigationStack.ToArray();

            //3.Remove all pages preceding the active one(leaving only index 0)
            //for (int i = navigationStack.Length - 1; i > 0; i--)
            //{
            //    Shell.Current.Navigation.RemovePage(navigationStack[i]);
            //}
        }
    }
}
