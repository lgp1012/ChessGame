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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ServerForm));
            this.lblServerStatus = new System.Windows.Forms.Label();
            this.NameGame = new System.Windows.Forms.Label();
            this.clientList = new System.Windows.Forms.ListBox();
            this.lblConnectClient = new System.Windows.Forms.Label();
            this.lblClientList = new System.Windows.Forms.Label();
            this.lblMatchStatus = new System.Windows.Forms.Label();
            this.btnStartServer = new System.Windows.Forms.Button();
            this.btnStopMatch = new System.Windows.Forms.Button();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.btnEndServer = new System.Windows.Forms.Button();
            this.btnStartMatch = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // lblServerStatus
            // 
            this.lblServerStatus.AutoSize = true;
            this.lblServerStatus.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(192)))), ((int)(((byte)(192)))));
            this.lblServerStatus.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblServerStatus.Font = new System.Drawing.Font("Microsoft Sans Serif", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblServerStatus.ForeColor = System.Drawing.Color.Black;
            this.lblServerStatus.Location = new System.Drawing.Point(12, 59);
            this.lblServerStatus.Name = "lblServerStatus";
            this.lblServerStatus.Size = new System.Drawing.Size(236, 27);
            this.lblServerStatus.TabIndex = 0;
            this.lblServerStatus.Text = "Server status: STOPPED";
            // 
            // NameGame
            // 
            this.NameGame.AutoSize = true;
            this.NameGame.BackColor = System.Drawing.Color.LavenderBlush;
            this.NameGame.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.NameGame.Font = new System.Drawing.Font("Microsoft Sans Serif", 20F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.NameGame.ForeColor = System.Drawing.Color.Maroon;
            this.NameGame.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
            this.NameGame.Location = new System.Drawing.Point(140, 9);
            this.NameGame.Name = "NameGame";
            this.NameGame.Size = new System.Drawing.Size(184, 33);
            this.NameGame.TabIndex = 1;
            this.NameGame.Text = "Chess Game";
            this.NameGame.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // clientList
            // 
            this.clientList.FormattingEnabled = true;
            this.clientList.Location = new System.Drawing.Point(114, 131);
            this.clientList.Name = "clientList";
            this.clientList.Size = new System.Drawing.Size(134, 43);
            this.clientList.TabIndex = 2;
            // 
            // lblConnectClient
            // 
            this.lblConnectClient.AutoSize = true;
            this.lblConnectClient.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(192)))), ((int)(((byte)(192)))));
            this.lblConnectClient.Font = new System.Drawing.Font("Microsoft Sans Serif", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblConnectClient.ForeColor = System.Drawing.Color.Black;
            this.lblConnectClient.Location = new System.Drawing.Point(12, 96);
            this.lblConnectClient.Name = "lblConnectClient";
            this.lblConnectClient.Size = new System.Drawing.Size(202, 25);
            this.lblConnectClient.TabIndex = 3;
            this.lblConnectClient.Text = "Connected Client: 0/2";
            // 
            // lblClientList
            // 
            this.lblClientList.AutoSize = true;
            this.lblClientList.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(192)))), ((int)(((byte)(192)))));
            this.lblClientList.Font = new System.Drawing.Font("Microsoft Sans Serif", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblClientList.Location = new System.Drawing.Point(12, 135);
            this.lblClientList.Name = "lblClientList";
            this.lblClientList.Size = new System.Drawing.Size(96, 25);
            this.lblClientList.TabIndex = 4;
            this.lblClientList.Text = "Client list:";
            // 
            // lblMatchStatus
            // 
            this.lblMatchStatus.AutoSize = true;
            this.lblMatchStatus.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblMatchStatus.ForeColor = System.Drawing.Color.Yellow;
            this.lblMatchStatus.Location = new System.Drawing.Point(95, 194);
            this.lblMatchStatus.Name = "lblMatchStatus";
            this.lblMatchStatus.Size = new System.Drawing.Size(229, 29);
            this.lblMatchStatus.TabIndex = 5;
            this.lblMatchStatus.Text = "Waiting for players...";
            // 
            // btnStartServer
            // 
            this.btnStartServer.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(192)))), ((int)(((byte)(192)))));
            this.btnStartServer.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnStartServer.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnStartServer.ForeColor = System.Drawing.Color.Maroon;
            this.btnStartServer.Location = new System.Drawing.Point(17, 226);
            this.btnStartServer.Name = "btnStartServer";
            this.btnStartServer.Size = new System.Drawing.Size(93, 45);
            this.btnStartServer.TabIndex = 6;
            this.btnStartServer.Text = "Start Server";
            this.btnStartServer.UseVisualStyleBackColor = false;
            this.btnStartServer.Click += new System.EventHandler(this.btnStartServer_Click);
            // 
            // btnStopMatch
            // 
            this.btnStopMatch.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(192)))), ((int)(((byte)(192)))));
            this.btnStopMatch.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnStopMatch.Enabled = false;
            this.btnStopMatch.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnStopMatch.ForeColor = System.Drawing.Color.Brown;
            this.btnStopMatch.Location = new System.Drawing.Point(352, 226);
            this.btnStopMatch.Name = "btnStopMatch";
            this.btnStopMatch.Size = new System.Drawing.Size(93, 45);
            this.btnStopMatch.TabIndex = 7;
            this.btnStopMatch.Text = "Stop Match";
            this.btnStopMatch.UseVisualStyleBackColor = false;
            this.btnStopMatch.Click += new System.EventHandler(this.btnStopMatch_Click);
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox1.Image")));
            this.pictureBox1.Location = new System.Drawing.Point(304, 51);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(141, 123);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox1.TabIndex = 8;
            this.pictureBox1.TabStop = false;
            // 
            // btnEndServer
            // 
            this.btnEndServer.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(192)))), ((int)(((byte)(192)))));
            this.btnEndServer.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnEndServer.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnEndServer.ForeColor = System.Drawing.Color.Maroon;
            this.btnEndServer.Location = new System.Drawing.Point(127, 226);
            this.btnEndServer.Name = "btnEndServer";
            this.btnEndServer.Size = new System.Drawing.Size(93, 45);
            this.btnEndServer.TabIndex = 9;
            this.btnEndServer.Text = "End Server";
            this.btnEndServer.UseVisualStyleBackColor = false;
            this.btnEndServer.Click += new System.EventHandler(this.btnEndServer_Click);
            // 
            // btnStartMatch
            // 
            this.btnStartMatch.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(192)))), ((int)(((byte)(192)))));
            this.btnStartMatch.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnStartMatch.Enabled = false;
            this.btnStartMatch.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnStartMatch.ForeColor = System.Drawing.Color.Brown;
            this.btnStartMatch.Location = new System.Drawing.Point(240, 226);
            this.btnStartMatch.Name = "btnStartMatch";
            this.btnStartMatch.Size = new System.Drawing.Size(93, 45);
            this.btnStartMatch.TabIndex = 10;
            this.btnStartMatch.Text = "Start Match";
            this.btnStartMatch.UseVisualStyleBackColor = false;
            this.btnStartMatch.Click += new System.EventHandler(this.btnStartMatch_Click);
            // 
            // ServerForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.ClientSize = new System.Drawing.Size(468, 286);
            this.Controls.Add(this.btnStartMatch);
            this.Controls.Add(this.btnEndServer);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.btnStopMatch);
            this.Controls.Add(this.btnStartServer);
            this.Controls.Add(this.lblMatchStatus);
            this.Controls.Add(this.lblClientList);
            this.Controls.Add(this.lblConnectClient);
            this.Controls.Add(this.clientList);
            this.Controls.Add(this.NameGame);
            this.Controls.Add(this.lblServerStatus);
            this.Cursor = System.Windows.Forms.Cursors.Default;
            this.ForeColor = System.Drawing.Color.Black;
            this.MaximizeBox = false;
            this.Name = "ServerForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Chess Board";
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblServerStatus;
        private System.Windows.Forms.Label NameGame;
        private System.Windows.Forms.ListBox clientList;
        private System.Windows.Forms.Label lblConnectClient;
        private System.Windows.Forms.Label lblClientList;
        private System.Windows.Forms.Label lblMatchStatus;
        private System.Windows.Forms.Button btnStartServer;
        private System.Windows.Forms.Button btnStopMatch;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Button btnEndServer;
        private System.Windows.Forms.Button btnStartMatch;
    }
}

