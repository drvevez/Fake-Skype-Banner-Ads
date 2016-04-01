namespace NoSkypeBannerAds
{
    partial class Form1
    {
        /// <summary>
        /// Variable nécessaire au concepteur.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Nettoyage des ressources utilisées.
        /// </summary>
        /// <param name="disposing">true si les ressources managées doivent être supprimées ; sinon, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Code généré par le Concepteur Windows Form

        /// <summary>
        /// Méthode requise pour la prise en charge du concepteur - ne modifiez pas
        /// le contenu de cette méthode avec l'éditeur de code.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.urlh = new System.Windows.Forms.TextBox();
            this.urlr = new System.Windows.Forms.TextBox();
            this.button1 = new System.Windows.Forms.Button();
            this.Message = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.timerConnection = new System.Windows.Forms.Timer(this.components);
            this.label3 = new System.Windows.Forms.Label();
            this.TimerWatch = new System.Windows.Forms.Timer(this.components);
            this.SuspendLayout();
            // 
            // urlh
            // 
            this.urlh.Location = new System.Drawing.Point(30, 31);
            this.urlh.Name = "urlh";
            this.urlh.Size = new System.Drawing.Size(390, 20);
            this.urlh.TabIndex = 0;
            this.urlh.Text = "http://hervemarchal.free.fr/skypenewbanner/blob.html";
            // 
            // urlr
            // 
            this.urlr.Location = new System.Drawing.Point(30, 108);
            this.urlr.Name = "urlr";
            this.urlr.Size = new System.Drawing.Size(390, 20);
            this.urlr.TabIndex = 1;
            this.urlr.Text = "http://hervemarchal.free.fr/skypenewbanner/blob.html";
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(30, 172);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(190, 23);
            this.button1.TabIndex = 2;
            this.button1.Text = "Send  Urls to Skype";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // Message
            // 
            this.Message.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.Message.Location = new System.Drawing.Point(30, 235);
            this.Message.Multiline = true;
            this.Message.Name = "Message";
            this.Message.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.Message.Size = new System.Drawing.Size(494, 206);
            this.Message.TabIndex = 3;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(27, 15);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(83, 13);
            this.label1.TabIndex = 4;
            this.label1.Text = "top banner URL";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(27, 92);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(93, 13);
            this.label2.TabIndex = 5;
            this.label2.Text = "Right banner URL";
            // 
            // timerConnection
            // 
            this.timerConnection.Enabled = true;
            this.timerConnection.Interval = 500;
            this.timerConnection.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(34, 219);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(113, 13);
            this.label3.TabIndex = 6;
            this.label3.Text = "Events log from Skype";
            // 
            // TimerWatch
            // 
            this.TimerWatch.Interval = 500;
            this.TimerWatch.Tick += new System.EventHandler(this.TimerWatch_Tick);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(550, 467);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.Message);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.urlr);
            this.Controls.Add(this.urlh);
            this.Name = "Form1";
            this.Text = "Skype Banner Pub Killer";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.Form1_FormClosed);
            this.Load += new System.EventHandler(this.Form1_Load);
            this.Shown += new System.EventHandler(this.Form1_Shown);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox urlh;
        private System.Windows.Forms.TextBox urlr;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.TextBox Message;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Timer timerConnection;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Timer TimerWatch;




    }
}

