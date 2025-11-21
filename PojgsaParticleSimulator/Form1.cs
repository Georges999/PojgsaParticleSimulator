using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Numerics;
using System.Windows.Forms;

namespace PojgsaParticleSimulator
{
    public partial class Form1 : Form
    {
        private List<Particle> particles = new List<Particle>();
        private System.Windows.Forms.Timer timer;
        private Random rand = new Random();
        private ParticleToolbox toolbox;
        private float timeScale = 1.0f;
        private Bitmap offscreenBitmap;
        private SimulationGrid grid;

        // Interaction
        private Vector2 mousePos;
        private bool isMouseDown;
        private bool isRightClick;

        public Form1()
        {
            InitializeComponent();
            SetupForm();
            InitializeSimulation();
            for (int i = 0; i < 200; i++) CreateParticle();
        }

        public Form1(float gravity, float windIntensity, float windDirection, int particleCount, float timeScale)
        {
            InitializeComponent();
            SetupForm();
            InitializeSimulation();

            Particle.SetGravity(gravity);
            UpdateWindDirection((int)windDirection);
            UpdateWindIntensity(windIntensity);
            this.timeScale = timeScale;

            for (int i = 0; i < particleCount; i++) CreateParticle();
        }

        private void SetupForm()
        {
            this.DoubleBuffered = true;
            this.Resize += Form1_Resize;
            this.BackColor = Color.FromArgb(20, 20, 30); // Dark blue-black background
            this.WindowState = FormWindowState.Maximized;
            
            // Mouse Events
            this.MouseMove += (s, e) => mousePos = new Vector2(e.X, e.Y);
            this.MouseDown += (s, e) => 
            { 
                isMouseDown = true; 
                isRightClick = e.Button == MouseButtons.Right; 
            };
            this.MouseUp += (s, e) => isMouseDown = false;

            // Toolbox
            toolbox = new ParticleToolbox(this);
            this.Controls.Add(toolbox);
        }

        private void InitializeSimulation()
        {
            int width = Math.Max(ClientSize.Width, 100);
            int height = Math.Max(ClientSize.Height, 100);

            // Initialize Grid (Cell size ~ max particle diameter * 2)
            grid = new SimulationGrid(width, height, 50);

            // Timer
            timer = new System.Windows.Forms.Timer();
            timer.Interval = 16; // ~60 FPS
            timer.Tick += Timer_Tick;
            timer.Start();
        }

        private void Form1_Resize(object? sender, EventArgs e)
        {
            if (ClientSize.Width > 0 && ClientSize.Height > 0)
            {
                offscreenBitmap = new Bitmap(ClientSize.Width, ClientSize.Height);
                // Re-init grid with new dimensions
                grid = new SimulationGrid(ClientSize.Width, ClientSize.Height, 50);
            }
        }

        private void Form1_Load(object? sender, EventArgs e)
        {
        }

        public void CreateParticle(Vector2? pos = null)
        {
            Vector2 p = pos ?? new Vector2(rand.Next(ClientSize.Width), rand.Next(ClientSize.Height));
            Vector2 v = new Vector2((float)rand.NextDouble() * 400 - 200, (float)rand.NextDouble() * 400 - 200);
            float radius = (float)(rand.NextDouble() * 5 + 3); // 3 to 8 radius
            float lifetime = 60.0f; // seconds
            
            // Random color
            Color c = Color.FromArgb(
                rand.Next(100, 255),
                rand.Next(100, 255),
                rand.Next(200, 255)
            );

            particles.Add(new Particle(p, v, radius, lifetime, c));
        }

        // Public methods for Toolbox interactions
        public void UpdateGravity(float gravity) => Particle.SetGravity(gravity);
        public void UpdateWindIntensity(float intensity) => Particle.Wind = Vector2.Normalize(Particle.Wind == Vector2.Zero ? Vector2.UnitX : Particle.Wind) * intensity;
        public void UpdateWindDirection(int degrees) 
        {
             float radians = degrees * (float)Math.PI / 180f;
             float intensity = Particle.Wind.Length();
             Particle.SetWind(intensity, radians);
        }
        public void UpdateTimeScale(float scale) => timeScale = scale;
        public void ClearParticles() => particles.Clear();


        private void Timer_Tick(object? sender, EventArgs e)
        {
            float dt = 0.016f * timeScale;
            int w = ClientSize.Width;
            int h = ClientSize.Height;
            int subSteps = 4;
            float subDt = dt / subSteps;

            if (w == 0 || h == 0) return;

            for (int step = 0; step < subSteps; step++)
            {
                grid.Clear();

                // Update and Add to Grid
                for (int i = particles.Count - 1; i >= 0; i--)
                {
                    var p = particles[i];
                    if (!p.IsAlive && step == 0) // Only remove once per frame
                    {
                        particles.RemoveAt(i);
                        continue;
                    }

                    // Interaction (Apply once per frame or scaled per sub-step)
                    if (step == 0 && isMouseDown)
                    {
                        Vector2 dir = mousePos - p.Position;
                        float dist = dir.Length();
                        if (dist > 10 && dist < 400)
                        {
                            dir = Vector2.Normalize(dir);
                            float strength = isRightClick ? -1000f : 1000f;
                            p.ApplyForce(dir * strength / dist); 
                        }
                    }

                    p.Update(subDt, w, h);
                    grid.AddParticle(p);
                }

                // Collision Resolution
                grid.HandleCollisions();
            }

            Invalidate();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            if (offscreenBitmap == null)
            {
                if (ClientSize.Width > 0 && ClientSize.Height > 0)
                     offscreenBitmap = new Bitmap(ClientSize.Width, ClientSize.Height);
                else return;
            }

            using (Graphics g = Graphics.FromImage(offscreenBitmap))
            {
                g.SmoothingMode = SmoothingMode.AntiAlias;
                g.Clear(this.BackColor);

                foreach (var p in particles)
                {
                    // Speed-based color intensity
                    float speed = p.Velocity.Length();
                    int alpha = Math.Min(255, (int)(p.Lifetime / p.MaxLifetime * 255));
                    
                    // Draw Particle
                    using (Brush brush = new SolidBrush(Color.FromArgb(alpha, p.Color)))
                    {
                        g.FillEllipse(brush, p.Position.X - p.Radius, p.Position.Y - p.Radius, p.Radius * 2, p.Radius * 2);
                    }

                    // Draw Vectors (Optional)
                    if (Particle.ShowVelocityVector)
                        DrawVector(g, p.Position, p.Velocity * 0.1f, Color.Yellow);
                    if (Particle.ShowGravityVector)
                        DrawVector(g, p.Position, new Vector2(0, Particle.Gravity * 5), Color.Red); // Simple visual
                    if (Particle.ShowWindVector)
                        DrawVector(g, p.Position, Particle.Wind * 10, Color.Green);
                }
                
                // Draw Interaction Indicator
                if (isMouseDown)
                {
                    Color c = isRightClick ? Color.Red : Color.Green;
                    using (Pen pen = new Pen(Color.FromArgb(50, c), 2))
                    {
                        g.DrawEllipse(pen, mousePos.X - 20, mousePos.Y - 20, 40, 40);
                    }
                }
            }

            e.Graphics.DrawImage(offscreenBitmap, 0, 0);
        }

        private void DrawVector(Graphics g, Vector2 start, Vector2 vector, Color color)
        {
            using (Pen pen = new Pen(color, 1))
            {
                g.DrawLine(pen, start.X, start.Y, start.X + vector.X, start.Y + vector.Y);
            }
        }
    }
}
