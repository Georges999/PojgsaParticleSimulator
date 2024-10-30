using System;
using System.Windows.Forms;

namespace PojgsaParticleSimulator
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // Show InitialSettingsForm to collect initial settings
            InitialSettingsForm settingsForm = new InitialSettingsForm();
            if (settingsForm.ShowDialog() == DialogResult.OK)
            {
                // Use the values from InitialSettingsForm to initialize Form1
                Application.Run(new Form1(
                    settingsForm.Gravity,
                    settingsForm.WindIntensity,
                    settingsForm.WindDirection,
                    settingsForm.ParticleCount,
                    settingsForm.TimeScale
                ));
            }
        }
    }
}

