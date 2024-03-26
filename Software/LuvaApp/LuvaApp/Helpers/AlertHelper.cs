using Microsoft.Maui.Controls;
using System.Diagnostics;
using System.Threading.Tasks;

namespace LuvaApp.Helpers.AlertHelper
{
    public static class AlertHelper
    {
        private static ContentPage MontaContentAlerta(string title, string message)
        {
            var popup = new ContentPage
            {
                BackgroundColor = Colors.Transparent,
                Content = new StackLayout
                {
                    BackgroundColor = Colors.White,
                    VerticalOptions = LayoutOptions.Center,
                    HorizontalOptions = LayoutOptions.Center,
                    Children =
                    {
                        new Label
                        {
                            Text = title,
                            TextColor = Colors.Black,
                            FontAttributes = FontAttributes.Bold,
                            FontSize = 18,
                            HorizontalOptions = LayoutOptions.Center,
                            VerticalOptions = LayoutOptions.Center,
                            HorizontalTextAlignment = TextAlignment.Center,
                            VerticalTextAlignment = TextAlignment.Center
                        },
                        new Label
                        {
                            Text = message,
                            FontSize = 18,
                            TextColor = Colors.Black,
                            HorizontalOptions = LayoutOptions.Center,
                            VerticalOptions = LayoutOptions.Center,
                            HorizontalTextAlignment = TextAlignment.Center,
                            VerticalTextAlignment = TextAlignment.Center
                        }
                    }
                },
            };

            return popup;
        }

        public async static Task ShowDialog(Page paginaAtual, int timeout, bool cancelar)
        {
            //await paginaAtual.Navigation.PushModalAsync(popup);

            AguardaTempo(timeout, ref cancelar);

            if (paginaAtual.Navigation.ModalStack.Count > 0)
                await paginaAtual.Navigation.PopModalAsync();
        }

        private static void AguardaTempo(int tempo, ref bool cancelar)
        {
            var ciclos = 0;

            while (ciclos * 100 <= tempo && !cancelar)
            {
                Thread.Sleep(100);
                ciclos++;
            }
        }
    }
}