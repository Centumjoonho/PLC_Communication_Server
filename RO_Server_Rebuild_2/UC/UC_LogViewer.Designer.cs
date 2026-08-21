namespace RO_Server_Rebuild_2.UC
{
    partial class UC_LogViewer
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

        #region 구성 요소 디자이너에서 생성한 코드

        /// <summary> 
        /// 디자이너 지원에 필요한 메서드입니다. 
        /// 이 메서드의 내용을 코드 편집기로 수정하지 마세요.
        /// </summary>
        private void InitializeComponent()
        {
            this.headerPanel = new System.Windows.Forms.Panel();
            this.lblTitle = new System.Windows.Forms.Label();
            this.btnBack = new System.Windows.Forms.Button();
            this.lblLog = new System.Windows.Forms.Label();
            this.txtLog = new System.Windows.Forms.RichTextBox();
            this.lblError = new System.Windows.Forms.Label();
            this.txtError = new System.Windows.Forms.RichTextBox();
            this.logSplitContainer = new System.Windows.Forms.SplitContainer();
            this.headerPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.logSplitContainer)).BeginInit();
            this.logSplitContainer.Panel1.SuspendLayout();
            this.logSplitContainer.Panel2.SuspendLayout();
            this.logSplitContainer.SuspendLayout();
            this.SuspendLayout();
            // 
            // headerPanel
            // 
            this.headerPanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(36)))), ((int)(((byte)(48)))), ((int)(((byte)(67)))));
            this.headerPanel.Controls.Add(this.lblTitle);
            this.headerPanel.Controls.Add(this.btnBack);
            this.headerPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.headerPanel.Location = new System.Drawing.Point(0, 0);
            this.headerPanel.Margin = new System.Windows.Forms.Padding(0, 0, 0, 10);
            this.headerPanel.Name = "headerPanel";
            this.headerPanel.Size = new System.Drawing.Size(1443, 64);
            this.headerPanel.TabIndex = 2;
            // 
            // lblTitle
            // 
            this.lblTitle.AutoSize = true;
            this.lblTitle.Font = new System.Drawing.Font("맑은 고딕", 15F, System.Drawing.FontStyle.Bold);
            this.lblTitle.ForeColor = System.Drawing.Color.White;
            this.lblTitle.Location = new System.Drawing.Point(18, 17);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(99, 28);
            this.lblTitle.TabIndex = 0;
            this.lblTitle.Text = "통합 로그";
            // 
            // btnBack
            // 
            this.btnBack.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnBack.BackColor = System.Drawing.Color.LightSlateGray;
            this.btnBack.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(95)))), ((int)(((byte)(112)))), ((int)(((byte)(140)))));
            this.btnBack.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnBack.Font = new System.Drawing.Font("맑은 고딕", 9.5F, System.Drawing.FontStyle.Bold);
            this.btnBack.ForeColor = System.Drawing.Color.White;
            this.btnBack.Location = new System.Drawing.Point(1304, 20);
            this.btnBack.Name = "btnBack";
            this.btnBack.Size = new System.Drawing.Size(109, 28);
            this.btnBack.TabIndex = 1;
            this.btnBack.Text = "메인 화면";
            this.btnBack.UseVisualStyleBackColor = false;
            this.btnBack.Click += new System.EventHandler(this.btnBack_Click);
            // 
            // lblLog
            // 
            this.lblLog.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblLog.Font = new System.Drawing.Font("맑은 고딕", 9.5F, System.Drawing.FontStyle.Bold);
            this.lblLog.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(36)))), ((int)(((byte)(48)))), ((int)(((byte)(67)))));
            this.lblLog.Location = new System.Drawing.Point(0, 0);
            this.lblLog.Margin = new System.Windows.Forms.Padding(0);
            this.lblLog.Name = "lblLog";
            this.lblLog.Size = new System.Drawing.Size(1443, 24);
            this.lblLog.TabIndex = 3;
            this.lblLog.Text = "LOG";
            this.lblLog.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // txtLog
            // 
            this.txtLog.BackColor = System.Drawing.Color.White;
            this.txtLog.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtLog.Font = new System.Drawing.Font("Consolas", 9.5F);
            this.txtLog.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(36)))), ((int)(((byte)(48)))), ((int)(((byte)(67)))));
            this.txtLog.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtLog.Location = new System.Drawing.Point(0, 24);
            this.txtLog.Margin = new System.Windows.Forms.Padding(0);
            this.txtLog.Name = "txtLog";
            this.txtLog.ReadOnly = true;
            this.txtLog.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Both;
            this.txtLog.Size = new System.Drawing.Size(1443, 261);
            this.txtLog.TabIndex = 4;
            this.txtLog.Text = "";
            this.txtLog.WordWrap = false;
            // 
            // lblError
            // 
            this.lblError.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblError.Font = new System.Drawing.Font("맑은 고딕", 9.5F, System.Drawing.FontStyle.Bold);
            this.lblError.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(145)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            this.lblError.Location = new System.Drawing.Point(0, 0);
            this.lblError.Margin = new System.Windows.Forms.Padding(0);
            this.lblError.Name = "lblError";
            this.lblError.Size = new System.Drawing.Size(1443, 24);
            this.lblError.TabIndex = 5;
            this.lblError.Text = "ERROR";
            this.lblError.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // txtError
            // 
            this.txtError.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(246)))), ((int)(((byte)(246)))));
            this.txtError.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtError.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtError.Font = new System.Drawing.Font("Consolas", 9.5F);
            this.txtError.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(145)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            this.txtError.Location = new System.Drawing.Point(0, 24);
            this.txtError.Margin = new System.Windows.Forms.Padding(0);
            this.txtError.Name = "txtError";
            this.txtError.ReadOnly = true;
            this.txtError.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Both;
            this.txtError.Size = new System.Drawing.Size(1443, 221);
            this.txtError.TabIndex = 6;
            this.txtError.Text = "";
            this.txtError.WordWrap = false;
            // 
            // logSplitContainer
            // 
            this.logSplitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.logSplitContainer.Location = new System.Drawing.Point(0, 64);
            this.logSplitContainer.Name = "logSplitContainer";
            this.logSplitContainer.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // logSplitContainer.Panel1
            // 
            this.logSplitContainer.Panel1.Controls.Add(this.txtLog);
            this.logSplitContainer.Panel1.Controls.Add(this.lblLog);
            this.logSplitContainer.Panel1MinSize = 140;
            // 
            // logSplitContainer.Panel2
            // 
            this.logSplitContainer.Panel2.Controls.Add(this.txtError);
            this.logSplitContainer.Panel2.Controls.Add(this.lblError);
            this.logSplitContainer.Panel2MinSize = 140;
            this.logSplitContainer.Size = new System.Drawing.Size(1443, 536);
            this.logSplitContainer.SplitterDistance = 285;
            this.logSplitContainer.SplitterWidth = 6;
            this.logSplitContainer.TabIndex = 7;
            // 
            // UC_LogViewer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.logSplitContainer);
            this.Controls.Add(this.headerPanel);
            this.Name = "UC_LogViewer";
            this.Size = new System.Drawing.Size(1443, 600);
            this.headerPanel.ResumeLayout(false);
            this.headerPanel.PerformLayout();
            this.logSplitContainer.Panel1.ResumeLayout(false);
            this.logSplitContainer.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.logSplitContainer)).EndInit();
            this.logSplitContainer.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel headerPanel;
        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.Button btnBack;
        private System.Windows.Forms.Label lblLog;
        private System.Windows.Forms.RichTextBox txtLog;
        private System.Windows.Forms.Label lblError;
        private System.Windows.Forms.RichTextBox txtError;
        private System.Windows.Forms.SplitContainer logSplitContainer;
    }
}
