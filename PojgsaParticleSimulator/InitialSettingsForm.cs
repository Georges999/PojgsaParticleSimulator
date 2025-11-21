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
        public float Gravity { get; private set; } = 9.81f;
        public float WindIntensity { get; private set; }
        public float WindDirection { get; private set; }
        public int ParticleCount { get; private set; } = 50; 
        public float TimeScale { get; private set; } = 1;

        private TextBox gravityTextBox;
        private TextBox windIntensityTextBox;
        private TextBox windDirectionTextBox;
        private TextBox particleCountTextBox;
        private TextBox timeScaleTextBox;
        private Button startButton;

        public InitialSettingsForm()
        {
            InitializeComponent();

            // Title Label
            Label titleLabel = new Label
            {
                Text = "Pojgsa Particle Simulator",
                Font = new Font("Arial", 18, FontStyle.Bold),
                ForeColor = Color.DarkBlue,
                Location = new Point(10, 10),
                AutoSize = true
            };
            Controls.Add(titleLabel);

            // Labels and TextBoxes for inputs with improved spacing and font styling
            Label gravityLabel = new Label
            { Text = "Gravity:",
                Location = new Point(10, 50),
                Font = new Font("Arial", 10),
                AutoSize = true};
            gravityTextBox = new TextBox
            { Text = "9.81",
                Location = new Point(120, 50),
                Width = 80};

            Label windIntensityLabel = new Label
            { Text = "Wind Intensity:",
                Location = new Point(10, 80),
                Font = new Font("Arial", 10),
                AutoSize = true};
            windIntensityTextBox = new TextBox
            {Text = "0",
             Location = new Point(120, 80),
             Width = 80};

            Label windDirectionLabel = new Label
            { Text = "Wind Direction:",
                Location = new Point(10, 110),
                Font = new Font("Arial", 10),
                 AutoSize = true};
            windDirectionTextBox = new TextBox
            {Text = "0",
                Location = new Point(120, 110),
                Width = 80};

            Label particleCountLabel = new Label
            {Text = "Particle Count:",
                Location = new Point(10, 140),
                Font = new Font("Arial", 10),
                AutoSize = true};
            particleCountTextBox = new TextBox
            {Text = "50",
                Location = new Point(120, 140),
                Width = 80};

            Label timeScaleLabel = new Label
            {Text = "Time Scale:",
                Location = new Point(10, 170),
                Font = new Font("Arial", 10),
                AutoSize = true};
            timeScaleTextBox = new TextBox
            { Text = "1.0",
                Location = new Point(120, 170),
                Width = 80};

            // Start button setup
            startButton = new Button
            {
                Text = "Start Simulation",
                Location = new Point(10, 210),
                Font = new Font("Arial", 10, FontStyle.Bold),
                BackColor = Color.LightSkyBlue,
                ForeColor = Color.Black,
                Width = 190,
                Height = 30
            };

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
