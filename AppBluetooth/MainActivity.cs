using System;
using Android.App;
using Android.OS;
using Android.Runtime;
using Android.Support.Design.Widget;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using Android.Content;
using Android.Bluetooth;
using System.Collections.ObjectModel;

namespace AppBluetooth
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme.NoActionBar", MainLauncher = true)]
    public class MainActivity : AppCompatActivity
    {
        public static TextView textview;
        public Button button;
        public readonly static Receiver1 receiver = new Receiver1();
        public static IntentFilter Filter { get; set; }
        private BluetoothAdapter ba = BluetoothAdapter.DefaultAdapter;
        private readonly static int RequestEnableBt = 1;
        bool toStart;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_main);

            Android.Support.V7.Widget.Toolbar toolbar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar);
            SetSupportActionBar(toolbar);

            //
            button = FindViewById<Button>(Resource.Id.buttonSearchDevice);//кнопка поиск устройства
            textview = FindViewById<TextView>(Resource.Id.textView1);//текстовое поле (вывод сообщений)
            textview.Text = "Начать поиск!";

            button.Click += Button_Click;
            toStart = true;

            Filter = new IntentFilter();
            Filter.AddAction(BluetoothAdapter.ActionStateChanged);
            Filter.AddAction(BluetoothAdapter.ActionDiscoveryStarted);
            Filter.AddAction(BluetoothAdapter.ActionDiscoveryFinished);
            Filter.AddAction(BluetoothDevice.ActionFound);

            RegisterReceiver(receiver, Filter);

            if(!ba.IsEnabled)
            {
                Intent enableBtIntent = new Intent(BluetoothAdapter.ActionRequestEnable);
                this.StartActivityForResult(enableBtIntent, RequestEnableBt);
            }

            //FloatingActionButton fab = FindViewById<FloatingActionButton>(Resource.Id.fab);
            //fab.Click += FabOnClick;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click(object sender, EventArgs e)
        {
            if(toStart)
            {
                textview.Text = String.Empty;
                ba.StartDiscovery();
                button.Text = "Идет поиск...";
                toStart = false;
            }
            else
            {
                ba.CancelDiscovery();
                toStart = true;
                textview.Text = "Начать поиск!";
                button.Text = "Поиск...";
            }
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.menu_main, menu);
            return true;
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            int id = item.ItemId;
            if (id == Resource.Id.action_settings)
            {
                return true;
            }

            return base.OnOptionsItemSelected(item);
        }

        private void FabOnClick(object sender, EventArgs eventArgs)
        {
            View view = (View) sender;
            Snackbar.Make(view, "Replace with your own action", Snackbar.LengthLong)
                .SetAction("Action", (Android.Views.View.IOnClickListener)null).Show();
        }

        [BroadcastReceiver]
        public class Receiver1 : BroadcastReceiver
        {
            public override void OnReceive(Context context, Intent intent)
            {
                string action = intent.Action;
                ObservableCollection<MainActivity> devices = new ObservableCollection<MainActivity>();
                if (BluetoothDevice.ActionFound.Equals(action))
                {
                    BluetoothDevice device = (BluetoothDevice)intent.GetParcelableExtra(BluetoothDevice.ExtraDevice);
                    string deviceName = device.Name;
                    string deviceHardwareAddres = device.Address;//Mac
                    textview.Text += "Name: " + deviceName + " Mac-адрес: " + deviceHardwareAddres + System.Environment.NewLine;
                }
            }
        }
    }
}

