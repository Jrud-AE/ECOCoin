using Microsoft.Extensions.Logging;
using System.Reflection;

namespace EcoWallet
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

            EcoCoinSharedTypes.GlobalVars.EnvironmentType = EcoCoinSharedTypes.EnvironmentType.Test;

            if (EcoCoinSharedTypes.GlobalVars.EnvironmentType == EcoCoinSharedTypes.EnvironmentType.Test)
            {
                EcoCoinSharedTypes.GlobalVars.ServerURLBase = "https://ecocoinapitestnet.automateearth.com";
            }
            else
            {
                EcoCoinSharedTypes.GlobalVars.ServerURLBase = "https://ecocoinapi.automateearth.com";
            }

#if DEBUG
    		builder.Logging.AddDebug();
#endif

            Microsoft.Maui.Handlers.ButtonHandler.Mapper.AppendToMapping("CursorHand", (handler, view) =>
            {
#if WINDOWS
    handler.PlatformView.Loaded += (sender, args) =>
    {
        if (sender is Microsoft.UI.Xaml.Controls.Button btn)
        {
            var handCursor = Microsoft.UI.Input.InputSystemCursor.Create(
                Microsoft.UI.Input.InputSystemCursorShape.Hand);

            // Use reflection to bypass the 'protected' restriction on ProtectedCursor
            typeof(Microsoft.UI.Xaml.UIElement).InvokeMember(
                "ProtectedCursor",
                BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.SetProperty,
                null,
                btn,
                new[] { handCursor });
        }
    };
#endif
            });

            return builder.Build();
        }
    }
}
