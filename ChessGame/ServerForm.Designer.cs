namespace ChessGame
{
    partial class ServerForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.lblServerStatus = new System.Windows.Forms.Label();
            this.NameGame = new System.Windows.Forms.Label();
            this.lblConnectClient = new System.Windows.Forms.Label();
            this.lblMatchStatus = new System.Windows.Forms.Label();
            this.btnStartServer = new System.Windows.Forms.Button();
            this.btnStopMatch = new System.Windows.Forms.Button();
            this.btnEndServer = new System.Windows.Forms.Button();
            this.btnStartMatch = new System.Windows.Forms.Button();
            this.msgServer = new System.Windows.Forms.ListBox();
            this.SuspendLayout();
            // 
            // lblServerStatus
            // 
            this.lblServerStatus.AutoSize = true;
            this.lblServerStatus.BackColor = System.Drawing.Color.White;
            this.lblServerStatus.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblServerStatus.ForeColor = System.Drawing.Color.Red;
            this.lblServerStatus.Location = new System.Drawing.Point(12, 67);
            this.lblServerStatus.Name = "lblServerStatus";
            this.lblServerStatus.Size = new System.Drawing.Size(188, 17);
            this.lblServerStatus.TabIndex = 0;
            this.lblServerStatus.Text = "Server status: STOPPED";
            // 
            // NameGame
            // 
            this.NameGame.AutoSize = true;
            this.NameGame.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.NameGame.Font = new System.Drawing.Font("Microsoft Sans Serif", 20F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.NameGame.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.NameGame.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
            this.NameGame.Location = new System.Drawing.Point(94, 9);
            this.NameGame.Name = "NameGame";
            this.NameGame.Size = new System.Drawing.Size(294, 31);
            this.NameGame.TabIndex = 1;
            this.NameGame.Text = "Chess Game - Server";
            this.NameGame.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblConnectClient
            // 
            this.lblConnectClient.AutoSize = true;
            this.lblConnectClient.BackColor = System.Drawing.Color.White;
            this.lblConnectClient.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblConnectClient.ForeColor = System.Drawing.Color.Black;
            this.lblConnectClient.Location = new System.Drawing.Point(264, 67);
            this.lblConnectClient.Name = "lblConnectClient";
            this.lblConnectClient.Size = new System.Drawing.Size(143, 17);
            this.lblConnectClient.TabIndex = 3;
            this.lblConnectClient.Text = "Connected Client: 0/2";
            // 
            // lblMatchStatus
            // 
            this.lblMatchStatus.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.lblMatchStatus.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblMatchStatus.ForeColor = System.Drawing.Color.Black;
            this.lblMatchStatus.Location = new System.Drawing.Point(12, 355);
            this.lblMatchStatus.Name = "lblMatchStatus";
            this.lblMatchStatus.Size = new System.Drawing.Size(444, 48);
            this.lblMatchStatus.TabIndex = 5;
            this.lblMatchStatus.Text = "Waiting for players...";
            this.lblMatchStatus.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // btnStartServer
            // 
            this.btnStartServer.BackColor = System.Drawing.Color.White;
            this.btnStartServer.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnStartServer.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnStartServer.ForeColor = System.Drawing.Color.Black;
            this.btnStartServer.Location = new System.Drawing.Point(12, 97);
            this.btnStartServer.Name = "btnStartServer";
            this.btnStartServer.Size = new System.Drawing.Size(82, 30);
            this.btnStartServer.TabIndex = 6;
            this.btnStartServer.Text = "Start Server";
            this.btnStartServer.UseVisualStyleBackColor = false;
            this.btnStartServer.Click += new System.EventHandler(this.btnStartServer_Click);
            // 
            // btnStopMatch
            // 
            this.btnStopMatch.BackColor = System.Drawing.Color.White;
            this.btnStopMatch.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnStopMatch.Enabled = false;
            this.btnStopMatch.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnStopMatch.ForeColor = System.Drawing.Color.Black;
            this.btnStopMatch.Location = new System.Drawing.Point(267, 406);
            this.btnStopMatch.Name = "btnStopMatch";
            this.btnStopMatch.Size = new System.Drawing.Size(82, 30);
            this.btnStopMatch.TabIndex = 7;
            this.btnStopMatch.Text = "Stop Match";
            this.btnStopMatch.UseVisualStyleBackColor = false;
            this.btnStopMatch.Click += new System.EventHandler(this.btnStopMatch_Click);
            // 
            // btnEndServer
            // 
            this.btnEndServer.BackColor = System.Drawing.Color.White;
            this.btnEndServer.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnEndServer.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnEndServer.ForeColor = System.Drawing.Color.Black;
            this.btnEndServer.Location = new System.Drawing.Point(118, 97);
            this.btnEndServer.Name = "btnEndServer";
            this.btnEndServer.Size = new System.Drawing.Size(82, 30);
            this.btnEndServer.TabIndex = 9;
            this.btnEndServer.Text = "End Server";
            this.btnEndServer.UseVisualStyleBackColor = false;
            this.btnEndServer.Click += new System.EventHandler(this.btnEndServer_Click);
            // 
            // btnStartMatch
            // 
            this.btnStartMatch.BackColor = System.Drawing.Color.White;
            this.btnStartMatch.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnStartMatch.Enabled = false;
            this.btnStartMatch.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnStartMatch.ForeColor = System.Drawing.Color.Black;
            this.btnStartMatch.Location = new System.Drawing.Point(118, 406);
            this.btnStartMatch.Name = "btnStartMatch";
            this.btnStartMatch.Size = new System.Drawing.Size(82, 30);
            this.btnStartMatch.TabIndex = 10;
            this.btnStartMatch.Text = "Start Match";
            this.btnStartMatch.UseVisualStyleBackColor = false;
            this.btnStartMatch.Click += new System.EventHandler(this.btnStartMatch_Click);
            // 
            // msgServer
            // 
            this.msgServer.FormattingEnabled = true;
            this.msgServer.Location = new System.Drawing.Point(12, 133);
            this.msgServer.Name = "msgServer";
            this.msgServer.Size = new System.Drawing.Size(444, 212);
            this.msgServer.TabIndex = 11;
            this.msgServer.SelectedIndexChanged += new System.EventHandler(this.listBox1_SelectedIndexChanged);
            // 
            // ServerForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.WhiteSmoke;
            this.ClientSize = new System.Drawing.Size(468, 447);
            this.Controls.Add(this.lblMatchStatus);
            this.Controls.Add(this.btnStartMatch);
            this.Controls.Add(this.btnStopMatch);
            this.Controls.Add(this.msgServer);
            this.Controls.Add(this.btnStartServer);
            this.Controls.Add(this.btnEndServer);
            this.Controls.Add(this.lblConnectClient);
            this.Controls.Add(this.NameGame);
            this.Controls.Add(this.lblServerStatus);
            this.Cursor = System.Windows.Forms.Cursors.Default;
            this.ForeColor = System.Drawing.Color.Black;
            this.MaximizeBox = false;
            this.Name = "ServerForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Chess Board";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblServerStatus;
        private System.Windows.Forms.Label NameGame;
        private System.Windows.Forms.Label lblConnectClient;
        private System.Windows.Forms.Label lblMatchStatus;
        private System.Windows.Forms.Button btnStartServer;
        private System.Windows.Forms.Button btnStopMatch;
        private System.Windows.Forms.Button btnEndServer;
        private System.Windows.Forms.Button btnStartMatch;
        private System.Windows.Forms.ListBox msgServer;
    }
}

