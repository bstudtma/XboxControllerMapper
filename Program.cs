using SharpDX.XInput;
using WindowsInput;
using WindowsInput.Native;

namespace XboxControllerMapper
{
    public class Program : Form
    {
        //private NotifyIcon trayIcon;
        private Controller controller;
        private InputSimulator inputSimulator;
        private System.Threading.Thread? pollingThread;
        private volatile bool isRunning = true;

        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Program());
        }

        public Program()
        {
            InitializeComponents();
            controller = new Controller(UserIndex.One);
            inputSimulator = new InputSimulator();
            StartMapping();
        }

        private void InitializeComponents()
        {
            //ContextMenuStrip contextMenuStrip = new ContextMenuStrip();
            //contextMenuStrip.Items.Add("Exit", null, Exit);

            //trayIcon = new NotifyIcon()
            //{
            //    Icon = System.Drawing.SystemIcons.Application,
            //    ContextMenuStrip = contextMenuStrip,
            //    Visible = true,
            //    Text = "Xbox Controller Mapper"
            //};
            this.WindowState = FormWindowState.Minimized;
            //this.ShowInTaskbar = false;
        }

        private void StartMapping()
        {
            pollingThread = new System.Threading.Thread(PollController);
            if ( pollingThread != null ) pollingThread.Start();
        }

        private void PollController()
        {
            bool wasAPressed = false;
            while (isRunning)
            {
                if (controller.IsConnected)
                {
                    State state = controller.GetState();
                    bool isAPressed = state.Gamepad.Buttons.HasFlag(GamepadButtonFlags.LeftShoulder);

                    if (isAPressed != wasAPressed)
                    {
                        if (isAPressed)
                        {
                            inputSimulator.Keyboard.KeyDown(VirtualKeyCode.F1);
                        }
                        else
                        {
                            inputSimulator.Keyboard.KeyUp(VirtualKeyCode.F1);
                        }
                        wasAPressed = isAPressed;
                    }
                }
                System.Threading.Thread.Sleep(16); // Poll at about 60Hz
            }
        }

        private void Exit(object sender, EventArgs e)
        {
            isRunning = false;
            if (pollingThread != null) pollingThread.Join(); // Wait for the polling thread to finish
            //trayIcon.Visible = false;
            Application.Exit();
        }

        private void InitializeComponent()
        {
            SuspendLayout();
            // 
            // Program
            // 
            ClientSize = new Size(284, 261);
            MaximizeBox = false;
            Name = "Program";
            ShowIcon = false;
            ShowInTaskbar = false;
            WindowState = FormWindowState.Minimized;
            ResumeLayout(false);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                isRunning = false;
                if (pollingThread != null && pollingThread.IsAlive)
                {
                    pollingThread.Join();
                }
                //trayIcon.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
