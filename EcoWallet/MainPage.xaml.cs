using EcoCoinSharedTypes;
#if WINDOWS
using System.Reflection;
using Microsoft.UI.Input;
#elif MACCATALYST
using AppKit;
using UIKit;
#endif

namespace EcoWallet
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();

            if (EcoCoinSharedTypes.GlobalVars.ECOWalletConfiguration.Accounts.Count > 0)
            {
                AccountDetails AD = new AccountDetails(EcoCoinSharedTypes.GlobalVars.ECOWalletConfiguration.Accounts[0]);

                lblAccountNumber.Text = "Account: " + AD.AccountName + "(" + AD.AccountID.ToString() + ")";
            }
            else
            {
                Shell.Current.GoToAsync("CreateAccountPage");
                //GlobalFunctions.NavigateToPageAndDropNavigation("CreateAccountPage");
            }
        }

        private void OnPageRegainFocus(object sender, EventArgs e)
        {
            if (EcoCoinSharedTypes.GlobalVars.ECOWalletConfiguration.Accounts.Count > 0)
            {
                FillOutAccountBalance();
            }
            else
            {
                Shell.Current.GoToAsync("CreateAccountPage");
                //GlobalFunctions.NavigateToPageAndDropNavigation("CreateAccountPage");
            }
        }

        private void FillOutAccountBalance()
        {
            AccountDetails AD = new AccountDetails(EcoCoinSharedTypes.GlobalVars.ECOWalletConfiguration.Accounts[0]);

            lblAccountNumber.Text = "Account: " + AD.AccountName + " (" + AD.AccountID.ToString() + ")";

            lblTotalBalance.Text = AD.PrimaryBalance.ToString() + " ECO";

            decimal HoldAmount = 0;

            foreach (BalanceHold BH in AD.BalanceHolds)
            {
                HoldAmount += BH.HoldAmount;
            }

            lblHeld.Text = HoldAmount.ToString() + " ECO";
        }

        private void PointerGestureRecognizer_PointerExited(object sender, Microsoft.Maui.Controls.PointerEventArgs e)
        {
            ChangeCursor(sender as VisualElement, true);
        }

        private void PointerGestureRecognizer_PointerEntered(object sender, Microsoft.Maui.Controls.PointerEventArgs e)
        {
            ChangeCursor(sender as VisualElement, false);
        }

        private void ChangeCursor(VisualElement element, bool isHovered)
        {
            if (element?.Handler?.PlatformView == null) return;

#if WINDOWS
    var nativeView = element.Handler.PlatformView as Microsoft.UI.Xaml.UIElement;
    if (nativeView != null)
    {
        // Define the shape (Hand or default Arrow)
        var cursor = isHovered 
            ? InputSystemCursor.Create(InputSystemCursorShape.Hand) 
            : null;

        // Bypass the protection level restriction using reflection
        typeof(Microsoft.UI.Xaml.UIElement).InvokeMember(
            "ProtectedCursor",
            BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.SetProperty,
            null,
            nativeView,
            new object[] { cursor });
    }
#elif MACCATALYST
    var nativeView = element.Handler.PlatformView as UIView;
    if (nativeView != null)
    {
        if (isHovered)
        {
            NSCursor.PointingHandCursor.Set();
        }
        else
        {
            NSCursor.ArrowCursor.Set();
        }
    }
#endif
        }
    }

}
