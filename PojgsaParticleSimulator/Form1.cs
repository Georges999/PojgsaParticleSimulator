using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Drawing;

namespace PojgsaParticleSimulator
{
    public partial class Form1 : Form
    {
        private List<Particle> particles = new List<Particle>();
        private Timer timer;
        private Random rand = new Random();
        private ParticleToolbox toolbox; // Ensure this is declared here
        private float timeScale = 1.0f;
        private Bitmap offscreenBitmap;

        public Form1()
        {
            InitializeComponent();
            this.DoubleBuffered = true;  // Enable double buffering
            this.Resize += Form1_Resize; // Add resize event

            toolbox = new ParticleToolbox(this); // Initialize toolbox
            this.Controls.Add(toolbox); // Add toolbox to form's controls

            // Initialize timer
            timer = new Timer();
            timer.Interval = 18; // Approximately 60 FPS
            timer.Tick += Timer_Tick;
            timer.Start();

            // Initialize particles
            InitializeParticles();
        }

        private void Form1_Resize(object sender, EventArgs e)
        {
            // Recreate offscreen bitmap to match new form size
            if (ClientSize.Width > 0 && ClientSize.Height > 0)
            {
                offscreenBitmap = new Bitmap(ClientSize.Width, ClientSize.Height);
            }
        }



        private void InitializeParticles()
        {
            // Initialize offscreen bitmap
            offscreenBitmap = new Bitmap(ClientSize.Width, ClientSize.Height);

            Random rand = new Random(); // Ensure you have a Random instance
            for (int i = 0; i < 1; i++)
            {
                float sizeFactor = (float)(rand.NextDouble() * 1.5 + 0.5); // Size factor between 0.5 and 2.0

                particles.Add(new Particle(
                    x: rand.Next(ClientSize.Width),
                    y: rand.Next(ClientSize.Height),
                    velocityX: (float)(rand.NextDouble() * 200 - 100), // Random velocity between -100 and 100
                    velocityY: (float)(rand.NextDouble() * 200 - 100), // Random velocity between -100 and 100
                    sizeFactor: sizeFactor, // Pass the size factor instead of a fixed radius
                    lifetime: 100000f // Lifetime in seconds
                ));
            }
        }

        // Method to update gravity
        public void UpdateGravity(float gravity)
        {
            Particle.SetGravity(gravity); // Assuming you have a static method in Particle to set gravity
        }

        public void UpdateTimeScale(float scale)
        {
            // Adjust the simulation speed by modifying the timer interval or deltaTime
            timeScale = scale;
 
        }

        public void CreateParticle()
        {
            float sizeFactor = (float)(rand.NextDouble() * 1.5 + 0.5); // Size factor between 0.5 and 2.0

            Particle newParticle = new Particle(
                x: rand.Next(ClientSize.Width),
                y: rand.Next(ClientSize.Height),
                velocityX: (float)(rand.NextDouble() * 200 - 100), // Random velocity between -100 and 100
                velocityY: (float)(rand.NextDouble() * 200 - 100), // Random velocity between -100 and 100
                sizeFactor: sizeFactor, // Use the size factor
                lifetime: 100000f // Lifetime in seconds
            );

            particles.Add(newParticle);
            Invalidate();// request redraw to show the new particle
        }
            private void Timer_Tick(object sender, EventArgs e)
        {
            float deltaTime = timer.Interval / 100f * timeScale; // Scale deltaTime by timeScale;

            foreach (var particle in particles)
            {
                particle.Update(deltaTime, ClientSize.Width, ClientSize.Height);
            }

            HandleParticleCollisions();

            Invalidate(); // Request to redraw the form (might be causing the flickering)
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            using (Graphics g = Graphics.FromImage(offscreenBitmap))
            {
                g.Clear(this.BackColor); // Clear with form's background color
                foreach (var particle in particles)
                {
                    if (particle.IsAlive)
                    {
                        Brush brush = new SolidBrush(Color.Black);
                        g.FillEllipse(brush, particle.X - particle.Radius, particle.Y - particle.Radius, particle.Radius * 2, particle.Radius * 2);
                    }
                }
            }
            e.Graphics.DrawImage(offscreenBitmap, 0, 0);  // Draw the offscreen bitmap to the form
        }

        private void HandleParticleCollisions()
        {
            for (int i = 0; i < particles.Count; i++)
            {
                for (int j = i + 1; j < particles.Count; j++)
                {
                    Particle particleA = particles[i];
                    Particle particleB = particles[j];

                    if (particleA.CheckCollision(particleB))
                    {
                        // Resolve collision
                        particleA.ResolveCollision(particleB);
                    }
                }
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }
}


