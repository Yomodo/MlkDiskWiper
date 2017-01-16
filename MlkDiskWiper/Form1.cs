using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using MlkDiskWiper.Hardware;

namespace MlkDiskWiper
{
    public partial class Form1 : Form
    {
        Control[] _inputControls;
        Control[] _runningControls;
        CancellationTokenSource _canceler = new CancellationTokenSource();

        public Form1()
        {
            InitializeComponent();

            _inputControls = new Control[]
            {
                diskPicker,
                dataWipeCheck,
                wipe,
            };

            _runningControls = new Control[]
            {
                cancelWipe,
            };
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            diskPicker.DataSource = DiskDrives;
            diskPicker.DisplayMember = "Name";
            diskPicker.ValueMember = "DeviceNumber";

            randomWipeType.DataSource = DataWipeType.All;
            randomWipeType.DisplayMember = "Description";
        }

        private async void wipe_Click(object sender, EventArgs e)
        {
            var selected = SelectedDiskDrive;
            var result = MessageBox.Show(
                $"All data on '{selected.Name}' will be permanently deleted. Continue?",
                "Wipe Disk",
                MessageBoxButtons.OKCancel);

            if (result == DialogResult.OK)
            {
                if (SelectedDiskDrive.DeviceNumber != selected.DeviceNumber)
                    throw new Exception("Drives changed");

                await Wipe(selected);
            }
        }

        async Task Wipe(DiskDrive disk)
        {
            _canceler.Dispose();
            _canceler = new CancellationTokenSource();

            ToggleControls(true);
            try
            {
                var progress = new Progress<int>(percent =>
                {
                    randomWipeProgress.Value = percent;
                });

                var wipeType = (DataWipeType)randomWipeType.SelectedItem;

                await Task.Run(() =>
                {
                    DeviceWiper.WipePhysicalDrive(wipeType.SourceFactory, disk.DeviceNumber, progress, _canceler.Token);
                }, _canceler.Token);
            }
            catch (OperationCanceledException)
            {
                // whatevs
            }
            finally
            {
                randomWipeProgress.Value = 0;
                ToggleControls(false);
            }
        }

        void ToggleControls(bool running)
        {
            foreach (var c in _inputControls)
                c.Enabled = !running;

            foreach (var c in _runningControls)
                c.Enabled = running;
        }

        readonly DiskQueryer _diskQueryer = new DiskQueryer(new WmiWin32DiskDrive());

        IList<DiskDrive> DiskDrives => _diskQueryer.GetDiskDrives().ToList();
        DiskDrive SelectedDiskDrive => DiskDrives.Single(x => x.DeviceNumber == (int)diskPicker.SelectedValue);

        private void cancelWipe_Click(object sender, EventArgs e)
        {
            _canceler.Cancel();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (components != null)
                    components.Dispose();
                _canceler.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
