using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.ComponentModel;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Diagnostics;
using System.Threading;

//external libraries required
using vJoyInterfaceWrap;
using KernelHotkey;

namespace MultiKeys
{
    public partial class MainWindow : Window
    {
        //joystick
        public string prt;
        public vJoy joystick1 = new vJoy();
        public uint joyid = 1;

        //wihch keyboard button code will correspond to which joystick key
        uint jx_up = 0;
        uint jx_down = 0;
        uint jy_up = 0;
        uint jy_down = 0;
        uint jbutton_1 = 0;
        uint jbutton_2 = 0;
        uint jbutton_3 = 0;
        uint jbutton_4 = 0;
        uint jbutton_5 = 0;
        uint jbutton_6 = 0;
        uint jbutton_7 = 0;
        uint jbutton_8 = 0;



        //keyboards
        int totalkbd = 10;  // 10 is just a default number, it can only handle upto 2 keyboards  at once
        Keyboard[] keyboards;
        Keyboard kbd;
        Keyboard.Stroke keystroke;
        int kbdid = 3;  //keyboard with this keyboard id will act as joystick  default keyboard id set to 3

        public MainWindow()
        {
            //code execution or entry point..............->>>
            InitializeComponent();  //Initialize Main window 
            Process.GetCurrentProcess().PriorityClass = ProcessPriorityClass.High;  //set High priority, so that all keys first pass through this software 
            updateconfig();  //update or refresh default settings
        }

        // functionality of  3 UI buttons

        private void free_button_Click(object sender, RoutedEventArgs e)
        {
            freejoy(); //reset or free all virtual joysticks
        }


        private void start_button_Click(object sender, RoutedEventArgs e)
        {
            check(); check2(); acquire();  //check if joystick is available for acquiring , if it is available then acquire it

            keyboards = new Keyboard[totalkbd];
            for (int i = 0; i < totalkbd; i++)
            {
                keyboards[i] = new Keyboard(i);
                keyboards[i].Filter = Keyboard.Filters.MAKE | Keyboard.Filters.BREAK | Keyboard.Filters.ALL; // filtering all keys                
            }
            startthreads();

        }

        private void update_button_Click(object sender, RoutedEventArgs e)
        {
            updateconfig();  //update configuration from UI values
        }


        private void feedback_button_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Wants to give any feedback, report a bug or need a new feature, mail me at lalitsom27@gmail.com  :)");
        }


        // functions ...................



        public void check()         //check if virual joystick is enabled
        {
            if (!joystick1.vJoyEnabled())
            {
                Debug.WriteLine("not enabled");
            }
            else
            {
                Debug.WriteLine("Vendor: {0}\nProduct :{1}\nVersion Number:{2}\n", joystick1.GetvJoyManufacturerString(),
                    joystick1.GetvJoyProductString(),
                    joystick1.GetvJoySerialNumberString()); ;
            }
        }




        public void check2() //check if virual joystick have write driver installed and compatible with versions of vjoy
        {
            UInt32 DllVer = 0, DrvVer = 0;
            bool match = joystick1.DriverMatch(ref DllVer, ref DrvVer);
            if (match)
                Debug.WriteLine("Version of Driver Matches DLL Version ({0:X})\n", DllVer);
            else
                Debug.WriteLine("Version of Driver ({0:X}) does NOT match DLL Version ({1:X})\n",
                DrvVer, DllVer);

            VjdStat status = joystick1.GetVJDStatus(joyid);
            switch (status)
            {
                case VjdStat.VJD_STAT_OWN:
                    Debug.WriteLine("vJoy Device {0} is already owned by this feeder\n", joyid);
                    break;
                case VjdStat.VJD_STAT_FREE:
                    Debug.WriteLine("vJoy Device {0} is free\n", joyid);
                    break;
                case VjdStat.VJD_STAT_BUSY:
                    Debug.WriteLine(
                    "vJoy Device {0} is already owned by another feeder\nCannot continue\n", joyid);
                    return;
                case VjdStat.VJD_STAT_MISS:
                    Debug.WriteLine(
                    "vJoy Device {0} is not installed or disabled\nCannot continue\n", joyid);
                    return;
                default:
                    Debug.WriteLine("vJoy Device {0} general error\nCannot continue\n", joyid);
                    return;
            };

            int nBtn = joystick1.GetVJDButtonNumber(joyid);
            int nDPov = joystick1.GetVJDDiscPovNumber(joyid);
            int nCPov = joystick1.GetVJDContPovNumber(joyid);
            bool X_Exist = joystick1.GetVJDAxisExist(joyid, HID_USAGES.HID_USAGE_X);
            bool Y_Exist = joystick1.GetVJDAxisExist(joyid, HID_USAGES.HID_USAGE_Y);
            bool Z_Exist = joystick1.GetVJDAxisExist(joyid, HID_USAGES.HID_USAGE_Z);
            bool RX_Exist = joystick1.GetVJDAxisExist(joyid, HID_USAGES.HID_USAGE_RX);
            prt = String.Format("Device[{0}]: Buttons={1}; DiscPOVs:{2}; ContPOVs:{3}",
                     joyid, nBtn, nDPov, nCPov);
            Debug.WriteLine(prt);
        }


        public void acquire()
        {
            ///acquire virtual joystick of id 'joyid' for manipulating

            VjdStat status;
            status = joystick1.GetVJDStatus(joyid); ///checking the status
            // Acquire the target
            if ((status == VjdStat.VJD_STAT_OWN) ||
            ((status == VjdStat.VJD_STAT_FREE) && (!joystick1.AcquireVJD(joyid))))
                prt = String.Format("Failed to acquire vJoy device number {0}.", joyid);
            else
                prt = String.Format("Acquired: vJoy device number {0}.", joyid);
            Debug.WriteLine(prt);
        }


        public void freejoy()
        {  // free and reset all joystick controls
            joystick1.ResetVJD(joyid);
            joystick1.RelinquishVJD(joyid);

        }

        public void updateconfig()
        {
            // Update the settings, like which button on which keyboard corrospond to which joystick button
            //Reading values from window interface
            jx_down = Convert.ToUInt32(x_inleft.Text);
            jx_up = Convert.ToUInt32(x_inright.Text);
            jy_down = Convert.ToUInt32(y_indown.Text);
            jy_up = Convert.ToUInt32(y_inup.Text);
            jbutton_1 = Convert.ToUInt32(b1_in.Text);
            jbutton_2 = Convert.ToUInt32(b2_in.Text);
            jbutton_3 = Convert.ToUInt32(b3_in.Text);
            jbutton_4 = Convert.ToUInt32(b4_in.Text);
            jbutton_5 = Convert.ToUInt32(b5_in.Text);
            jbutton_6 = Convert.ToUInt32(b6_in.Text);
            jbutton_7 = Convert.ToUInt32(b7_in.Text);
            jbutton_8 = Convert.ToUInt32(b8_in.Text);
            kbdid = Convert.ToInt32(setid_in.Text);      //setting keyboard id to distinguish between multiple keyboards
            Debug.WriteLine("" + jx_down + jx_up + jy_down + jy_up + jbutton_1 + jbutton_2);
        }



        protected override void OnClosing(CancelEventArgs e)
        {
            freejoy();
            Debug.WriteLine("free vjoy");
            base.OnClosing(e);
            closeall();                       //close all background threads and processes if any running
        }

        private void closeall()
        {
            String pname = Process.GetCurrentProcess().ProcessName;
            Debug.WriteLine(pname);
            foreach (var process in Process.GetProcessesByName(pname))
            {
                process.Kill();
            }

        }


        public void startthreads()
        {
            Thread thread1 = new Thread(new ThreadStart(func));
            thread1.Start();  //create a new thread so that UI remains  responsive 
        }

        public void func()
        {
            int i = 0;
            while (true) //infinite loop to listen to keyboard keys and perform actions
            {
                i++;
                Debug.WriteLine("waiting");  //wait for a key to press 
                kbd = Keyboard.Wait(keyboards);
                Debug.WriteLine("typed" + i);
                if (kbd == null) break;

                keystroke = kbd.Read(); //read keycode

                Dispatcher.Invoke((Action)delegate () { id_info.Text = "Id: " + kbd.ID + " pressed: " + keystroke.code; }); //reflect key info on UI                


                if (kbd.ID == kbdid)
                {
                    //block all strokes of keyboard with matching kbdid and press corresponding joystick keys                   
                    pressjoy(keystroke);
                }
                else
                {
                    //if it was different keyboard then send this stroke to other programs
                    kbd.Write(keystroke);
                }
            }
        }

        public void pressjoy(Keyboard.Stroke keystroke)
        {
            // perform actions according to keycode and keystate and joystick button assign to it   
            Boolean down = false;
            if (keystroke.state == Keyboard.States.MAKE)
                down = true;
            else if (keystroke.state == Keyboard.States.BREAK)
                down = false;

            if (keystroke.state == Keyboard.States.E0)
                down = true;
            else if ((int)keystroke.state == 3)
                down = false;


            if (down)
            {

                if (keystroke.code == jy_down)
                    joystick1.SetAxis(36000, joyid, HID_USAGES.HID_USAGE_Y);
                else if (keystroke.code == jy_up)
                    joystick1.SetAxis(0, joyid, HID_USAGES.HID_USAGE_Y);
                else if (keystroke.code == jx_down)
                    joystick1.SetAxis(0, joyid, HID_USAGES.HID_USAGE_X);
                else if (keystroke.code == jx_up)
                    joystick1.SetAxis(36000, joyid, HID_USAGES.HID_USAGE_X);
            }
            else
            {
                if (keystroke.code == jy_down)
                    joystick1.SetAxis(18000, joyid, HID_USAGES.HID_USAGE_Y);
                else if (keystroke.code == jy_up)
                    joystick1.SetAxis(18000, joyid, HID_USAGES.HID_USAGE_Y);

                else if (keystroke.code == jx_down)
                    joystick1.SetAxis(18000, joyid, HID_USAGES.HID_USAGE_X);
                else if (keystroke.code == jx_up)
                    joystick1.SetAxis(18000, joyid, HID_USAGES.HID_USAGE_X);
            }

            if (keystroke.code == jbutton_1)
                joystick1.SetBtn(down, joyid, 1);
            else if (keystroke.code == jbutton_2)
                joystick1.SetBtn(down, joyid, 2);
            else if (keystroke.code == jbutton_3)
                joystick1.SetBtn(down, joyid, 3);
            else if (keystroke.code == jbutton_4)
                joystick1.SetBtn(down, joyid, 4);
            else if (keystroke.code == jbutton_5)
                joystick1.SetBtn(down, joyid, 5);
            else if (keystroke.code == jbutton_6)
                joystick1.SetBtn(down, joyid, 6);
            else if (keystroke.code == jbutton_7)
                joystick1.SetBtn(down, joyid, 7);
            else if (keystroke.code == jbutton_8)
                joystick1.SetBtn(down, joyid, 8);

        }

    }
}
