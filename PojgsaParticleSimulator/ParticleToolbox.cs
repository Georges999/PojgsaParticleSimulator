using System;
using System.Windows.Forms;
using System.Drawing;

namespace PojgsaParticleSimulator
{
    public class ParticleToolbox : UserControl
    {
        private Button addButton;
        private TrackBar gravityTrackBar;
        private Label gravityLabel;
        private NumericUpDown gravityNumericUpDown;
        private TrackBar timeScaleTrackBar;
        private Label timeScaleLabel;
        private Button minimizeButton;
        private bool isMinimized = false;
        private Form1 mainForm;
        private Label windIntensityLabel;
        private NumericUpDown windIntensityControl;
        private Label windDirectionLabel;
        private NumericUpDown windDirectionControl;
        private CheckBox windVectorCheckBox;
        private CheckBox velocityVectorCheckBox;
        private CheckBox gravityVectorCheckBox;


        public ParticleToolbox(Form1 form)
        {
            mainForm = form;
            InitializeComponents();
        }

        private void InitializeComponents()
        {
            this.Size = new Size(200, 500);
            this.BackColor = Color.LightGray;

            // Minimize Button
            minimizeButton = new Button
            {
                Text = "-",
                Location = new Point(this.Width - 30, 5),
                Size = new Size(20, 20)
            };
            minimizeButton.Click += MinimizeButton_Click;
            this.Controls.Add(minimizeButton);

            // Add Particle Button
            addButton = new Button
            {
                Text = "Add Particle",
                Location = new Point(10, 25),
                Width = 180
            };
            addButton.Click += AddButton_Click;
            this.Controls.Add(addButton);

            // Gravity Label
            gravityLabel = new Label
            {
                Text = "Gravity: 9.81",
                Location = new Point(10, 50),
                Width = 180
            };
            this.Controls.Add(gravityLabel);

            // Gravity TrackBar
            gravityTrackBar = new TrackBar
            {
                Minimum = -30,
                Maximum = 30,
                Value = 10,
                Location = new Point(10, 70),
                Width = 180
            };
            gravityTrackBar.Scroll += GravityTrackBar_Scroll;
            this.Controls.Add(gravityTrackBar);

            // Gravity NumericUpDown
            gravityNumericUpDown = new NumericUpDown
            {
                Minimum = -30,
                Maximum = 30,
                Value = 10,
                DecimalPlaces = 2,
                Increment = 0.1M,
                Location = new Point(10, 120),
                Width = 180
            };
            gravityNumericUpDown.ValueChanged += GravityNumericUpDown_ValueChanged;
            this.Controls.Add(gravityNumericUpDown);

            // Time Scale Label
            timeScaleLabel = new Label
            {
                Text = "Time Scale: 1.0x",
                Location = new Point(10, 160),
                Width = 180
            };
            this.Controls.Add(timeScaleLabel);

            // Time Scale TrackBar
            timeScaleTrackBar = new TrackBar
            {
                Minimum = 1,
                Maximum = 20,
                Value = 10,
                Location = new Point(10, 180),
                Width = 180
            };
            timeScaleTrackBar.Scroll += TimeScaleTrackBar_Scroll;
            this.Controls.Add(timeScaleTrackBar);

            // Wind Intensity Label
            windIntensityLabel = new Label
            {
                Text = "Wind Intensity ",
                Location = new Point(10, 239),
                Width = 180
            };
            this.Controls.Add(windIntensityLabel);

            // Wind Intensity Control
            windIntensityControl = new NumericUpDown
            {
                Minimum = 0,
                Maximum = 100,
                Value = 0,
                DecimalPlaces = 1,
                Increment = 0.5M,
                Location = new Point(10, 260),
                Width = 180
            };
            windIntensityControl.ValueChanged += WindIntensityControl_ValueChanged;
            this.Controls.Add(windIntensityControl);

            // Wind Direction Label
            windDirectionLabel = new Label
            {
                Text = "Wind Direction: 0°",
                Location = new Point(10, 290),
                Width = 180
            };
            this.Controls.Add(windDirectionLabel);

            // Wind Direction Control
            windDirectionControl = new NumericUpDown
            {
                Minimum = 0,
                Maximum = 360,
                Value = 0,
                Location = new Point(10, 310),
                Width = 180
            };
            windDirectionControl.ValueChanged += WindDirectionControl_ValueChanged;
            this.Controls.Add(windDirectionControl);

            // Wind Vector CheckBox
            windVectorCheckBox = new CheckBox
            {
                Text = "Show Wind Vector",
                Location = new Point(10, 340),
                Checked = true // Default to checked
            };
            windVectorCheckBox.CheckedChanged += WindVectorCheckBox_CheckedChanged;
            this.Controls.Add(windVectorCheckBox);

            // Velocity Vector CheckBox
            velocityVectorCheckBox = new CheckBox
            {
                Text = "Show Velocity Vectors",
                Location = new Point(10, 365),
                Checked = true // Default to checked
            };
            velocityVectorCheckBox.CheckedChanged += VelocityVectorCheckBox_CheckedChanged;
            this.Controls.Add(velocityVectorCheckBox);

            // Gravity Vector CheckBox
            gravityVectorCheckBox = new CheckBox
            {
                Text = "Show Gravity Vector",
                Location = new Point(10, 390),
                Checked = true // Default to checked
            };
            gravityVectorCheckBox.CheckedChanged += GravityVectorCheckBox_CheckedChanged;
            this.Controls.Add(gravityVectorCheckBox);

        }



        private void MinimizeButton_Click(object sender, EventArgs e)
        {
            // Toggle minimized state
            isMinimized = !isMinimized;

            if (isMinimized)
            {
                // Hide controls and resize to smaller dimensions
                addButton.Visible = false;
                gravityLabel.Visible = false;
                gravityTrackBar.Visible = false;
                gravityNumericUpDown.Visible = false;
                timeScaleLabel.Visible = false;
                timeScaleTrackBar.Visible = false;
                windIntensityLabel.Visible = false;
                windIntensityControl.Visible = false;
                windDirectionLabel.Visible = false;
                windDirectionControl.Visible = false;
                this.Size = new Size(200, 30); // Adjust to smaller size
                minimizeButton.Text = "+";
            }
            else
            {
                // Show controls and restore to original size
                addButton.Visible = true;
                gravityLabel.Visible = true;
                gravityTrackBar.Visible = true;
                gravityNumericUpDown.Visible = true;
                timeScaleLabel.Visible = true;
                timeScaleTrackBar.Visible = true;
                windIntensityLabel.Visible = true;
                windIntensityControl.Visible = true;
                windDirectionLabel.Visible = true;
                windDirectionControl.Visible = true;
                this.Size = new Size(200, 500); // Restore original size
                minimizeButton.Text = "-";
            }
        }

        private void AddButton_Click(object sender, EventArgs e)
        {
            mainForm.CreateParticle();
        }

        private void GravityTrackBar_Scroll(object sender, EventArgs e)
        {
            float gravity = gravityTrackBar.Value;
            gravityLabel.Text = $"Gravity: {gravity}";
            gravityNumericUpDown.Value = (decimal)gravity;
            mainForm.UpdateGravity(gravity);
        }

        private void GravityNumericUpDown_ValueChanged(object sender, EventArgs e)
        {
            float gravity = (float)gravityNumericUpDown.Value;
            gravityLabel.Text = $"Gravity: {gravity}";
            gravityTrackBar.Value = (int)gravity;
            mainForm.UpdateGravity(gravity);
        }

        private void TimeScaleTrackBar_Scroll(object sender, EventArgs e)
        {
            float timeScale = timeScaleTrackBar.Value / 10f;
            timeScaleLabel.Text = $"Time Scale: {timeScale}x";
            mainForm.UpdateTimeScale(timeScale);
        }

        private void WindIntensityControl_ValueChanged(object sender, EventArgs e)
        {
            float windIntensity = (float)windIntensityControl.Value;
            windIntensityLabel.Text = $"Wind Intensity: {windIntensity}";
            mainForm.UpdateWindIntensity(windIntensity);
        }

        private void WindDirectionControl_ValueChanged(object sender, EventArgs e)
        {
            int windDirection = (int)windDirectionControl.Value;
            windDirectionLabel.Text = $"Wind Direction: {windDirection}°";
            mainForm.UpdateWindDirection(windDirection);
        }

        private void WindVectorCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            Particle.ShowWindVector = windVectorCheckBox.Checked;
        }

        private void VelocityVectorCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            Particle.ShowVelocityVector = velocityVectorCheckBox.Checked;
        }

        private void GravityVectorCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            Particle.ShowGravityVector = gravityVectorCheckBox.Checked;
        }
    }
}
