
using System;
using Plugin.Connectivity;
using Plugin.Media.Abstractions;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Stuffinder
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SpeedWriter : ContentPage
    {
        Users data;
        MediaFile photo;

        public SpeedWriter(Users _data)
        {
            ((NavigationPage)Application.Current.MainPage).BarBackgroundColor = Color.FromHex("#262626");
            InitializeComponent();

            data = new Users();
            data.user1 = _data.user1;
            data.user2 = _data.user2;

            PlayerA.Text = data.user1;
            PlayerB.Text = data.user2;

        }

        private async void CameraClicked(object sender, EventArgs e)
        { 
            var button = sender as Button;
            button.IsEnabled = false;
            photo = await Plugin.Media.CrossMedia.Current.TakePhotoAsync(new Plugin.Media.Abstractions.StoreCameraMediaOptions() { });

            if (!Plugin.Media.CrossMedia.Current.IsCameraAvailable &&
                !Plugin.Media.CrossMedia.Current.IsTakePhotoSupported)
            {
                await DisplayAlert("Error", "No camera available.", "OK");
                photo = null;
            }
            else if (photo != null)
            {
                if (!CrossConnectivity.Current.IsConnected)
                    await DisplayAlert("Error", "Please check your network connection and retry.", "OK");
                else
                {
                    Analyze cognitive = new Analyze();
                    var visionTask = cognitive.AnalyzeTextAsync(photo.GetStream());
                    await Task.WhenAll(visionTask);
                    var vision = await visionTask;

                    if (vision.Contains(Text.Text))
                    {
                        await DisplayAlert("Congrats", button.Text + " wrote well", "OK");
                        Navigation.PopToRootAsync();
                    }
                    else if (!PlayerA.IsEnabled && !PlayerB.IsEnabled)
                    {
                        await DisplayAlert("Game Over", "No one wins.", "OK");
                        Navigation.PopToRootAsync();
                    }
                    else
                        await DisplayAlert("Game Over", "Sorry, we could not found the right text.", "OK");
                    //Text.Text = vision;
                }
            }
        }
    }
}