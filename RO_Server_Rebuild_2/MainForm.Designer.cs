namespace RO_Server_Rebuild_2
{
    partial class MainForm
    {
        /// <summary>
        /// 필수 디자이너 변수입니다.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 사용 중인 모든 리소스를 정리합니다.
        /// </summary>
        /// <param name="disposing">관리되는 리소스를 삭제해야 하면 true이고, 그렇지 않으면 false입니다.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form 디자이너에서 생성한 코드

        /// <summary>
        /// 디자이너 지원에 필요한 메서드입니다. 
        /// 이 메서드의 내용을 코드 편집기로 수정하지 마세요.
        /// </summary>
        private void InitializeComponent()
        {
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.menuAdmin = new System.Windows.Forms.ToolStripMenuItem();
            this.menuPlcRegister = new System.Windows.Forms.ToolStripMenuItem();
            this.menuLogViewer = new System.Windows.Forms.ToolStripMenuItem();
            this.menuAdminSetting = new System.Windows.Forms.ToolStripMenuItem();
            this.mainPanel = new System.Windows.Forms.Panel();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuAdmin});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(800, 24);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // menuAdmin
            // 
            this.menuAdmin.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuPlcRegister,
            this.menuLogViewer,
            this.menuAdminSetting});
            this.menuAdmin.Name = "menuAdmin";
            this.menuAdmin.Size = new System.Drawing.Size(55, 20);
            this.menuAdmin.Text = "관리자";
            // 
            // menuPlcRegister
            // 
            this.menuPlcRegister.Name = "menuPlcRegister";
            this.menuPlcRegister.Size = new System.Drawing.Size(122, 22);
            this.menuPlcRegister.Text = "설비등록";
            this.menuPlcRegister.Click += new System.EventHandler(this.menuPlcRegister_Click);
            // 
            // menuLogViewer
            // 
            this.menuLogViewer.Name = "menuLogViewer";
            this.menuLogViewer.Size = new System.Drawing.Size(122, 22);
            this.menuLogViewer.Text = "통합로그";
            this.menuLogViewer.Click += new System.EventHandler(this.menuLogViewer_Click);
            // 
            // menuAdminSetting
            // 
            this.menuAdminSetting.Name = "menuAdminSetting";
            this.menuAdminSetting.Size = new System.Drawing.Size(122, 22);
            this.menuAdminSetting.Text = "설정";
            this.menuAdminSetting.Click += new System.EventHandler(this.menuAdminSetting_Click);
            // 
            // mainPanel
            // 
            this.mainPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mainPanel.Location = new System.Drawing.Point(0, 24);
            this.mainPanel.Margin = new System.Windows.Forms.Padding(0);
            this.mainPanel.Name = "mainPanel";
            this.mainPanel.Size = new System.Drawing.Size(800, 426);
            this.mainPanel.TabIndex = 2;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.mainPanel);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "RO Sever System";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem menuAdmin;
        private System.Windows.Forms.ToolStripMenuItem menuPlcRegister;
        private System.Windows.Forms.ToolStripMenuItem menuLogViewer;
        private System.Windows.Forms.Panel mainPanel;
        private System.Windows.Forms.ToolStripMenuItem menuAdminSetting;
    }
}

