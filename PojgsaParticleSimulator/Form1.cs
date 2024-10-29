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
        private float gravity;
        private float windIntensity;
        private float windDirection;
        private int particleCount;

        public Form1(float gravity, float windIntensity, float windDirection, int particleCount, float timeScale)
        {
            InitializeComponent();

            this.gravity = gravity;
            this.windIntensity = windIntensity;
            this.windDirection = windDirection;
            this.particleCount = particleCount;
            this.timeScale = timeScale;

            Particle.SetGravity(gravity);
            Particle.SetWind(windIntensity, windDirection);

            InitializeParticles();
        }


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
            offscreenBitmap = new Bitmap(ClientSize.Width, ClientSize.Height);
            Random rand = new Random();

            for (int i = 0; i < particleCount; i++)
            {
                float sizeFactor = (float)(rand.NextDouble() * 1.5 + 0.5);

                particles.Add(new Particle(
                    x: rand.Next(ClientSize.Width),
                    y: rand.Next(ClientSize.Height),
                    velocityX: (float)(rand.NextDouble() * 200 - 100),
                    velocityY: (float)(rand.NextDouble() * 200 - 100),
                    sizeFactor: sizeFactor,
                    lifetime: 100000f
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
        public void UpdateWindIntensity(float intensity)
        {
            Particle.SetWind(intensity, Particle.GetWindDirection()); // Ensure to use current wind direction
        }

        public void UpdateWindDirection(int direction)
        {
            Particle.SetWind(Particle.GetWindIntensity(), direction); // Ensure to use current wind intensity
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

                    if (Particle.ShowVelocityVector)
                    {
                        DrawVector(g, particle.X, particle.Y, particle.VelocityX, particle.VelocityY, Color.Blue);
                    }

                    // Draw gravity vector
                    if (Particle.ShowGravityVector)
                    {
                        DrawVector(g, particle.X, particle.Y, 0, Particle.GetGravity(), Color.Red);
                    }

                    // Draw wind vector (implement wind direction and magnitude logic)
                    if (Particle.ShowWindVector)
                    {
                        float windX = (float)(Particle.GetWindIntensity() * Math.Cos(Particle.GetWindDirection() * Math.PI / 180));
                        float windY = (float)(Particle.GetWindIntensity() * Math.Sin(Particle.GetWindDirection() * Math.PI / 180));
                        DrawVector(g, particle.X, particle.Y, windX, windY, Color.Green);
                    }
                }
            }
            e.Graphics.DrawImage(offscreenBitmap, 0, 0);  // Draw the offscreen bitmap to the form
        }
        private void DrawVector(Graphics g, float startX, float startY, float vectorX, float vectorY, Color color)
        {
            Pen pen = new Pen(color, 2);
            g.DrawLine(pen, startX, startY, startX + vectorX, startY + vectorY);
            // Optionally draw an arrowhead
            float arrowHeadLength = 10;
            float arrowHeadAngle = (float)Math.PI / 6; // 30 degrees
            float angle = (float)Math.Atan2(vectorY, vectorX);

            // Arrowhead points
            float x1 = startX + vectorX - arrowHeadLength * (float)Math.Cos(angle - arrowHeadAngle);
            float y1 = startY + vectorY - arrowHeadLength * (float)Math.Sin(angle - arrowHeadAngle);
            float x2 = startX + vectorX - arrowHeadLength * (float)Math.Cos(angle + arrowHeadAngle);
            float y2 = startY + vectorY - arrowHeadLength * (float)Math.Sin(angle + arrowHeadAngle);

            g.DrawLine(pen, startX + vectorX, startY + vectorY, x1, y1);
            g.DrawLine(pen, startX + vectorX, startY + vectorY, x2, y2);
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


