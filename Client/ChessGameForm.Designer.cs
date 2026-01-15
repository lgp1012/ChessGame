namespace Client
{
    partial class ChessGameForm
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.Button btnPause;
        private System.Windows.Forms.Button btnExit;
        private System.Windows.Forms.Label lblTurn;
        private System.Windows.Forms.Panel pauseOverlay;
        private System.Windows.Forms.Label pauseLabel;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.btnPause = new System.Windows.Forms.Button();
            this.btnExit = new System.Windows.Forms.Button();
            this.lblTurn = new System.Windows.Forms.Label();
            this.pauseOverlay = new System.Windows.Forms.Panel();
            this.pauseLabel = new System.Windows.Forms.Label();
            this.pauseOverlay.SuspendLayout();
            this.SuspendLayout();
            
            // lblTurn - Left side
            this.lblTurn.AutoSize = true;
            this.lblTurn.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTurn.Location = new System.Drawing.Point(20, 15);
            this.lblTurn.Name = "lblTurn";
            this.lblTurn.Size = new System.Drawing.Size(120, 24);
            this.lblTurn.TabIndex = 0;
            this.lblTurn.Text = "Your Turn";
            this.lblTurn.ForeColor = System.Drawing.Color.Green;
            
            // btnPause - Right side, next to btnExit
            this.btnPause.BackColor = System.Drawing.Color.Orange;
            this.btnPause.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnPause.ForeColor = System.Drawing.Color.White;
            this.btnPause.Location = new System.Drawing.Point(360, 10);
            this.btnPause.Name = "btnPause";
            this.btnPause.Size = new System.Drawing.Size(100, 35);
            this.btnPause.TabIndex = 1;
            this.btnPause.Text = "Pause";
            this.btnPause.UseVisualStyleBackColor = false;
            this.btnPause.Click += new System.EventHandler(this.btnPause_Click);
            
            // btnExit - Right side, rightmost
            this.btnExit.BackColor = System.Drawing.Color.Red;
            this.btnExit.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnExit.ForeColor = System.Drawing.Color.White;
            this.btnExit.Location = new System.Drawing.Point(470, 10);
            this.btnExit.Name = "btnExit";
            this.btnExit.Size = new System.Drawing.Size(100, 35);
            this.btnExit.TabIndex = 2;
            this.btnExit.Text = "Exit";
            this.btnExit.UseVisualStyleBackColor = false;
            this.btnExit.Click += new System.EventHandler(this.btnExit_Click);
            
            // pauseLabel
            this.pauseLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pauseLabel.BackColor = System.Drawing.Color.Transparent;
            this.pauseLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.pauseLabel.ForeColor = System.Drawing.Color.White;
            this.pauseLabel.Name = "pauseLabel";
            this.pauseLabel.TabIndex = 0;
            this.pauseLabel.Text = "Đối thủ đã tạm dừng trận đấu";
            this.pauseLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            
            // pauseOverlay - covers chess board area only
            this.pauseOverlay.BackColor = System.Drawing.Color.FromArgb(192, 0, 0, 0);
            this.pauseOverlay.Controls.Add(this.pauseLabel);
            this.pauseOverlay.Location = new System.Drawing.Point(20, 50);
            this.pauseOverlay.Name = "pauseOverlay";
            this.pauseOverlay.Size = new System.Drawing.Size(480, 480);
            this.pauseOverlay.TabIndex = 3;
            this.pauseOverlay.Visible = false;
            
            // ChessGameForm
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(600, 700);
            this.Controls.Add(this.pauseOverlay);
            this.Controls.Add(this.btnExit);
            this.Controls.Add(this.btnPause);
            this.Controls.Add(this.lblTurn);
            this.Name = "ChessGameForm";
            this.Text = "Chess Game";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.pauseOverlay.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();
        }
    }
}