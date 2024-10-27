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

        public ParticleToolbox(Form1 form)
        {
            mainForm = form;
            InitializeComponents();
        }

        private void InitializeComponents()
        {
            this.Size = new Size(200, 400);
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
                Minimum = -20,
                Maximum = 20,
                Value = 10,
                Location = new Point(10, 70),
                Width = 180
            };
            gravityTrackBar.Scroll += GravityTrackBar_Scroll;
            this.Controls.Add(gravityTrackBar);

            // Gravity NumericUpDown
            gravityNumericUpDown = new NumericUpDown
            {
                Minimum = -20,
                Maximum = 20,
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
                this.Size = new Size(200, 400); // Restore original size
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
    }
}
