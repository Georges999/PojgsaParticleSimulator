using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PojgsaParticleSimulator
{
    public partial class InitialSettingsForm : Form
    {
        // Properties for initial settings
        public float Gravity { get; private set; } = 9.81f;  // Default value
        public float WindIntensity { get; private set; }
        public float WindDirection { get; private set; }
        public int ParticleCount { get; private set; } = 50; // Default value
        public float TimeScale { get; private set; }

        private TextBox gravityTextBox;
        private TextBox windIntensityTextBox;
        private TextBox windDirectionTextBox;
        private TextBox particleCountTextBox;
        private TextBox timeScaleTextBox;
        private Button startButton;

        public InitialSettingsForm()
        {
            InitializeComponent();

            // Labels and TextBoxes for inputs
            Label gravityLabel = new Label { Text = "Gravity:", Location = new Point(10, 10), AutoSize = true };
            gravityTextBox = new TextBox { Text = Gravity.ToString(), Location = new Point(120, 10) };

            Label windIntensityLabel = new Label { Text = "Wind Intensity:", Location = new Point(10, 40), AutoSize = true };
            windIntensityTextBox = new TextBox { Text = WindIntensity.ToString(), Location = new Point(120, 40) };

            Label windDirectionLabel = new Label { Text = "Wind Direction:", Location = new Point(10, 70), AutoSize = true };
            windDirectionTextBox = new TextBox { Text = WindDirection.ToString(), Location = new Point(120, 70) };

            Label particleCountLabel = new Label { Text = "Particle Count:", Location = new Point(10, 100), AutoSize = true };
            particleCountTextBox = new TextBox { Text = ParticleCount.ToString(), Location = new Point(120, 100) };

            Label timeScaleLabel = new Label { Text = "Time Scale:", Location = new Point(10, 130), AutoSize = true };
            timeScaleTextBox = new TextBox { Text = TimeScale.ToString(), Location = new Point(120, 130) };

            // Start button setup
            startButton = new Button { Text = "Start", Location = new Point(10, 170) };
            startButton.Click += startButton_Click;

            // Add controls to form
            Controls.Add(gravityLabel);
            Controls.Add(gravityTextBox);
            Controls.Add(windIntensityLabel);
            Controls.Add(windIntensityTextBox);
            Controls.Add(windDirectionLabel);
            Controls.Add(windDirectionTextBox);
            Controls.Add(particleCountLabel);
            Controls.Add(particleCountTextBox);
            Controls.Add(timeScaleLabel);
            Controls.Add(timeScaleTextBox);
            Controls.Add(startButton);
        }

        private void startButton_Click(object sender, EventArgs e)
        {
            // Validate and collect values from UI
            if (float.TryParse(gravityTextBox.Text, out float gravity) &&
                float.TryParse(windIntensityTextBox.Text, out float windIntensity) &&
                float.TryParse(windDirectionTextBox.Text, out float windDirection) &&
                int.TryParse(particleCountTextBox.Text, out int particleCount) &&
                float.TryParse(timeScaleTextBox.Text, out float timeScale))
            {
                Gravity = gravity;
                WindIntensity = windIntensity;
                WindDirection = windDirection;
                ParticleCount = particleCount;
                TimeScale = timeScale;

                // Close the form and return DialogResult.OK to indicate success
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            else
            {
                MessageBox.Show("Please enter valid values for all fields.", "Invalid Input", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
    }
}
