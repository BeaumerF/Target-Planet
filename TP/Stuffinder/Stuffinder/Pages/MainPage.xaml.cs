using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Stuffinder
{
    public partial class MainPage : ContentPage
    {
        Users data;

        public MainPage()
        {
            InitializeComponent();
            data = new Users();
            NavigationPage.SetHasNavigationBar(this, false);
        }

        void OnClicked(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(Player1Entry.Text) || string.IsNullOrWhiteSpace(Player2Entry.Text))
                Device.BeginInvokeOnMainThread(async () => await DisplayAlert("Error", "You have to complete the fields", "OK"));
            else
            {
                data.user1 = Player1Entry.Text;
                data.user2 = Player2Entry.Text;
                var GameP = new GamePage(data, 1);
                GameP.BindingContext = data;
                NavigationPage.SetHasBackButton(GameP, true);
                ((NavigationPage)Application.Current.MainPage).BarTextColor = Color.White;

#if __ANDROID__
                NavigationPage.SetHasBackButton(GameP, false);
#endif
                var stack = Navigation.NavigationStack;
                if (stack[stack.Count - 1].GetType() != typeof(GamePage))
                    Navigation.PushAsync(GameP);
            }
        }
    }
}