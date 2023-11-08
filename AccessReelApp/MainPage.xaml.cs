using AccessReelApp.ViewModels;
﻿using AccessReelApp.database_structures;
using Plugin.LocalNotification;
using FirebaseAdmin.Messaging;
using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using CommunityToolkit.Mvvm.Messaging;
using CommunityToolkit.Mvvm.Messaging.Messages;
<<<<<<< HEAD
using System.Diagnostics;
using Newtonsoft.Json;
using System.Text;
=======
>>>>>>> parent of 352a0f5 (Local Notifications test working.)

//Firebase https://firebase.google.com/docs/reference/admin
//Android Setup https://firebase.google.com/docs/cloud-messaging/android/client
//Messaging https://firebase.google.com/docs/cloud-messaging/send-message

namespace AccessReelApp
{
    public class PushNotificationReceived : ValueChangedMessage<string>
    {
        public PushNotificationReceived(string message) : base(message) { }
    }

    public class PushNotificationRequest
    {
        public List<string> registration_ids { get; set; } = new List<string>();
        public NotificationMessageBody notification { get; set; }
        public object data { get; set; }
    }

    public class NotificationMessageBody
    {
        public string title { get; set; }
        public string body { get; set; }
    }

    public partial class MainPage : ContentPage
	{
		DatabaseControl databaseControl = new DatabaseControl();
		int count = 0;
		private string _deviceToken;

		public MainPage(MainViewModel vm)
		{
			InitializeComponent();
			BindingContext = vm;

			WeakReferenceMessenger.Default.Register<PushNotificationReceived>(this, (r, m) =>
			{
				string msg = m.Value;
			});

			if(Preferences.ContainsKey("DeviceToken"))
			{
				_deviceToken = Preferences.Get("DeviceToken", "");
			}

			ReadFireBaseAdminSDK();
		}

		protected override void OnAppearing()
		{
			base.OnAppearing();
			if (BindingContext is MainViewModel vm)
			{
				vm.Text = "Changed!";
			}
		}

		private void Button_Clicked(object sender, EventArgs e)
        {

			//DOES THIS PLUGIN WORK FOR FIREBASE
			var request = new NotificationRequest
			{
				NotificationId = 1337,
				Title = "Hello World",
				Subtitle = "Test",
				Description = "Working",
				BadgeNumber = 42,
				Schedule = new NotificationRequestSchedule
				{
					NotifyTime = DateTime.Now.AddSeconds(5),
					NotifyRepeatInterval = TimeSpan.FromDays(1),
				}
			};
			LocalNotificationCenter.Current.Show(request);
        }

		private async void ReadFireBaseAdminSDK()
		{
			var stream = await FileSystem.OpenAppPackageFileAsync("admin_sdk.json");
			var reader = new StreamReader(stream);

			var jsonContent = reader.ReadToEnd();

			if(FirebaseMessaging.DefaultInstance == null)
			{
				FirebaseApp.Create(new AppOptions()
				{
					Credential = GoogleCredential.FromJson(jsonContent)
				}); 
			}
		}

        private async void Button_Clicked_1(object sender, EventArgs e)
        {
            {
                //var androidNotificationObject = new Dictionary<string, string>();
                //androidNotificationObject.Add("NavigationID", "2");

                //var iosNotificationObject = new Dictionary<string, object>();
                //iosNotificationObject.Add("NavigationID", "2");

                var pushNotificationRequest = new PushNotificationRequest
                {
                    notification = new NotificationMessageBody
                    {
                        title = "Notification Title",
                        body = "Notification body"
                    },
                    //data = androidNotificationObject,
                    registration_ids = new List<string> { _deviceToken }            //ADD LIST OF TOKEN FROM API
                };

                //Updates: https://firebase.google.com/docs/cloud-messaging/migrate-v1?authuser=1#windows
                string url = "https://fcm.googleapis.com/v1/projects/myproject-734530391348/messages:send";
                using(var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("key", "=" + "BMa09HX2bwYS3y_dGm_xIDoRreyq2EDFwDPVJoBWFD6zaByfdv4uz9eh7qK8QzX7AW7mOAGk9tYgs7AwPSAVjAc\r\n8");
                    
                    string serializeRequest = JsonConvert.SerializeObject(pushNotificationRequest);
                    var response = await client.PostAsync(url, new StringContent(serializeRequest, Encoding.UTF8 , "application/json"));
                    if(response.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        await App.Current.MainPage.DisplayAlert("Notification sent", "notification sent", "OK");
                    }
                }

                //var messageList = new List<Message>();

                //var obj = new Message
                //{
                //    Token = _deviceToken,
                //    Notification = new Notification
                //    {
                //        Title = "Tilte",
                //        Body = "message body"
                //    },
                //    Data = androidNotificationObject,
                //    Apns = new ApnsConfig()
                //    {
                //        Aps = new Aps
                //        {
                //            Badge = 15,
                //            CustomData = iosNotificationObject,
                //        }
                //    }
                //};

                //messageList.Add(obj);

                //var response = await FirebaseMessaging.DefaultInstance.SendAllAsync(messageList);
            }
        }
    }
}

