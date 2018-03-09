using System;
using Plugin.Connectivity;
using Plugin.Media.Abstractions;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Stuffinder
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class GamePage : ContentPage
    {
        Random rand;
        int _second;
        int item;
        string name;
        MediaFile photo;
        Users data;
        int turn;
        bool phototaken;

        enum ItemName
        {
            Pen = 0,
            Scissors
        }

        public GamePage(Users _data, int _turn)
        {
            InitializeComponent();
            ShowingTimer();

            rand = new Random();
            item = rand.Next(2);
            Item.Text = ItemRand(item);
            data = new Users();
            data.user1 = _data.user2;
            data.user2 = _data.user1;
            turn = _turn;
            photo = null;
            phototaken = false;

            Color _color = Color.FromHex("#ff07baff");
            if (turn == 2)
                _color = Color.FromRgb(254, 46, 100);
            ((NavigationPage)Application.Current.MainPage).BarBackgroundColor = _color;
            Labela.TextColor = _color;
            Countdown.TextColor = _color;
            Labelb.TextColor = _color;
            Item.TextColor = _color;
        }

        private async void ShowingTimer()
        {
            for (_second = 30; _second >= 0; _second--)
            {
                Countdown.Text = Convert.ToString(_second) + " seconds";
                await Task.Delay(1000);
            }
            TakePhoto.IsEnabled = false;
            if (!phototaken)
                ValidatePhoto.IsEnabled = true;
        }

        private string ItemRand(int item)
        {
            var img = new Image { Aspect = Aspect.AspectFit };
            switch (item)
            {
                case (int)ItemName.Pen:
                    ItemImage.Source = "scissors.png";
                    return (name = "scissors");
                case (int)ItemName.Scissors:
                    ItemImage.Source = "pen.png";
                    return (name = "pen");
            }
            return (null);
        }

        private async void CameraClicked(object sender, EventArgs e)
        {
            photo = await Plugin.Media.CrossMedia.Current.TakePhotoAsync(new Plugin.Media.Abstractions.StoreCameraMediaOptions() { });

            if (!Plugin.Media.CrossMedia.Current.IsCameraAvailable &&
                !Plugin.Media.CrossMedia.Current.IsTakePhotoSupported)
            {
                await DisplayAlert("Error", "No camera available.", "OK");
                photo = null;
            }
            else if (_second <= 0)
                photo = null;
            else if (photo != null && _second > 0)
            {
                ItemImage.Source = ImageSource.FromStream(() => { return photo.GetStream(); });
                ValidatePhoto.IsEnabled = true;
            }
        }

        async private void NextClicked(object sender, EventArgs e)
        {
            phototaken = true;
            ValidatePhoto.IsEnabled = false;
            TakePhoto.IsEnabled = false;
            if (!CrossConnectivity.Current.IsConnected)
                await DisplayAlert("Error", "Please check your network connection and retry.", "OK");
            else if (photo != null)
            {
                string str = "";
                Analyze cognitive = new Analyze();

                var visionTask = cognitive.AnalyzePictureAsync(photo.GetStream());
                await Task.WhenAll(visionTask);
                var vision = await visionTask;

                if (vision != null && vision.Tags != null && vision.Captions[0] != null)
                {
                    foreach (var tag in vision.Tags)
                    {
                        str += tag + ", ";
                    }
                    Item.Text = vision.Captions[0].Text;
                }
                if (CheckValues(str, vision.Captions[0].Text))
                    await DisplayAlert("Congrats", "You found the item !", "OK");
                else
                    await DisplayAlert("Game Over", "Sorry, we could not found the item.", "OK");
                ValidatePhoto.Text = data.user2 + " turn";
                ValidatePhoto.Clicked -= NextClicked;
                ValidatePhoto.Clicked += ChangePage;
                ValidatePhoto.IsEnabled = true;
                ChangePage();
            }
            else
            {
                await DisplayAlert("Game Over", "Sorry, we could not found the item.", "OK");
                ValidatePhoto.Text = data.user2 + " turn";
                ValidatePhoto.Clicked -= NextClicked;
                ValidatePhoto.Clicked += ChangePage;
                ValidatePhoto.IsEnabled = true;
                ChangePage();
            }
        }

        void ChangePage(object sender = null, EventArgs e = null)
        {
            if (turn >= 2)
            {
                Navigation.PopToRootAsync();
            }

            var stack = Navigation.NavigationStack;
            var GameP = new GamePage(data, 2);
            GameP.BindingContext = data;

            //((NavigationPage)Application.Current.MainPage).BarTextColor = Color.White;
            //NavigationPage.SetHasBackButton(GameP, false);
            //#if __IOS__
            //                    NavigationPage.SetHasBackButton(GameP, true);
            //#endif

            if (stack.Count <= 2)
                Navigation.PushAsync(GameP);
        }

        bool CheckValues(string str, string txt)
        {
            if (str.Contains(name) || txt.Contains(name))
                return (true);
            return (false);
        }
    }
}