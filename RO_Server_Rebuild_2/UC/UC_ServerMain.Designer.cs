namespace RO_Server_Rebuild_2.UC
{
    partial class UC_ServerMain
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            this.headerPanel = new System.Windows.Forms.Panel();
            this.lblTitle = new System.Windows.Forms.Label();
            this.lblStatus = new System.Windows.Forms.Label();
            this.commandPanel = new System.Windows.Forms.Panel();
            this.btnServerStart = new System.Windows.Forms.Button();
            this.btnCollectStart = new System.Windows.Forms.Button();
            this.lblApiState = new System.Windows.Forms.Label();
            this.lblRepeatState = new System.Windows.Forms.Label();
            this.repeatPanel = new System.Windows.Forms.TableLayoutPanel();
            this.lblRepeat = new System.Windows.Forms.Label();
            this.gridCollect = new System.Windows.Forms.DataGridView();
            this.headerPanel.SuspendLayout();
            this.commandPanel.SuspendLayout();
            this.repeatPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridCollect)).BeginInit();
            this.SuspendLayout();
            // 
            // headerPanel
            // 
            this.headerPanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(36)))), ((int)(((byte)(48)))), ((int)(((byte)(67)))));
            this.headerPanel.Controls.Add(this.lblTitle);
            this.headerPanel.Controls.Add(this.lblStatus);
            this.headerPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.headerPanel.Location = new System.Drawing.Point(0, 0);
            this.headerPanel.Margin = new System.Windows.Forms.Padding(0, 0, 0, 10);
            this.headerPanel.Name = "headerPanel";
            this.headerPanel.Size = new System.Drawing.Size(1560, 64);
            this.headerPanel.TabIndex = 1;
            // 
            // lblTitle
            // 
            this.lblTitle.AutoSize = true;
            this.lblTitle.Font = new System.Drawing.Font("맑은 고딕", 15F, System.Drawing.FontStyle.Bold);
            this.lblTitle.ForeColor = System.Drawing.Color.White;
            this.lblTitle.Location = new System.Drawing.Point(18, 9);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(241, 28);
            this.lblTitle.TabIndex = 0;
            this.lblTitle.Text = "HYUNDAI RB RO Server";
            // 
            // lblStatus
            // 
            this.lblStatus.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblStatus.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(66)))), ((int)(((byte)(100)))));
            this.lblStatus.Font = new System.Drawing.Font("맑은 고딕", 9.5F, System.Drawing.FontStyle.Bold);
            this.lblStatus.ForeColor = System.Drawing.Color.White;
            this.lblStatus.Location = new System.Drawing.Point(1089, 18);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(450, 28);
            this.lblStatus.TabIndex = 2;
            this.lblStatus.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // commandPanel
            // 
            this.commandPanel.BackColor = System.Drawing.Color.White;
            this.commandPanel.Controls.Add(this.btnServerStart);
            this.commandPanel.Controls.Add(this.btnCollectStart);
            this.commandPanel.Controls.Add(this.lblApiState);
            this.commandPanel.Controls.Add(this.lblRepeatState);
            this.commandPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.commandPanel.Location = new System.Drawing.Point(0, 64);
            this.commandPanel.Margin = new System.Windows.Forms.Padding(0, 0, 0, 10);
            this.commandPanel.Name = "commandPanel";
            this.commandPanel.Size = new System.Drawing.Size(1560, 62);
            this.commandPanel.TabIndex = 2;
            // 
            // btnServerStart
            // 
            this.btnServerStart.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnServerStart.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(31)))), ((int)(((byte)(78)))), ((int)(((byte)(121)))));
            this.btnServerStart.FlatAppearance.BorderColor = System.Drawing.Color.DimGray;
            this.btnServerStart.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnServerStart.Font = new System.Drawing.Font("맑은 고딕", 9.5F, System.Drawing.FontStyle.Bold);
            this.btnServerStart.ForeColor = System.Drawing.Color.White;
            this.btnServerStart.Location = new System.Drawing.Point(1261, 16);
            this.btnServerStart.Name = "btnServerStart";
            this.btnServerStart.Size = new System.Drawing.Size(120, 36);
            this.btnServerStart.TabIndex = 0;
            this.btnServerStart.Text = "서버 시작";
            this.btnServerStart.UseVisualStyleBackColor = false;
            this.btnServerStart.Click += new System.EventHandler(this.btnServerStart_Click);
            // 
            // btnCollectStart
            // 
            this.btnCollectStart.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCollectStart.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(31)))), ((int)(((byte)(78)))), ((int)(((byte)(121)))));
            this.btnCollectStart.FlatAppearance.BorderColor = System.Drawing.Color.DimGray;
            this.btnCollectStart.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCollectStart.Font = new System.Drawing.Font("맑은 고딕", 9.5F, System.Drawing.FontStyle.Bold);
            this.btnCollectStart.ForeColor = System.Drawing.Color.White;
            this.btnCollectStart.Location = new System.Drawing.Point(1401, 16);
            this.btnCollectStart.Name = "btnCollectStart";
            this.btnCollectStart.Size = new System.Drawing.Size(140, 36);
            this.btnCollectStart.TabIndex = 3;
            this.btnCollectStart.Text = "PLC 통신 시작";
            this.btnCollectStart.UseVisualStyleBackColor = false;
            this.btnCollectStart.Click += new System.EventHandler(this.btnCollectStart_Click);
            // 
            // lblApiState
            // 
            this.lblApiState.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(232)))), ((int)(((byte)(236)))), ((int)(((byte)(241)))));
            this.lblApiState.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Bold);
            this.lblApiState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(84)))), ((int)(((byte)(92)))), ((int)(((byte)(104)))));
            this.lblApiState.Location = new System.Drawing.Point(18, 16);
            this.lblApiState.Name = "lblApiState";
            this.lblApiState.Size = new System.Drawing.Size(112, 30);
            this.lblApiState.TabIndex = 5;
            this.lblApiState.Text = "정지";
            this.lblApiState.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblRepeatState
            // 
            this.lblRepeatState.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(232)))), ((int)(((byte)(236)))), ((int)(((byte)(241)))));
            this.lblRepeatState.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Bold);
            this.lblRepeatState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(84)))), ((int)(((byte)(92)))), ((int)(((byte)(104)))));
            this.lblRepeatState.Location = new System.Drawing.Point(152, 16);
            this.lblRepeatState.Name = "lblRepeatState";
            this.lblRepeatState.Size = new System.Drawing.Size(107, 30);
            this.lblRepeatState.TabIndex = 6;
            this.lblRepeatState.Text = "PLC 통신 정지";
            this.lblRepeatState.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // repeatPanel
            // 
            this.repeatPanel.BackColor = System.Drawing.Color.White;
            this.repeatPanel.ColumnCount = 1;
            this.repeatPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.repeatPanel.Controls.Add(this.lblRepeat, 0, 0);
            this.repeatPanel.Controls.Add(this.gridCollect, 0, 1);
            this.repeatPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.repeatPanel.Location = new System.Drawing.Point(0, 126);
            this.repeatPanel.Margin = new System.Windows.Forms.Padding(0);
            this.repeatPanel.Name = "repeatPanel";
            this.repeatPanel.Padding = new System.Windows.Forms.Padding(14);
            this.repeatPanel.RowCount = 2;
            this.repeatPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 36F));
            this.repeatPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.repeatPanel.Size = new System.Drawing.Size(1560, 382);
            this.repeatPanel.TabIndex = 3;
            // 
            // lblRepeat
            // 
            this.lblRepeat.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblRepeat.Font = new System.Drawing.Font("맑은 고딕", 10F, System.Drawing.FontStyle.Bold);
            this.lblRepeat.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(36)))), ((int)(((byte)(48)))), ((int)(((byte)(67)))));
            this.lblRepeat.Location = new System.Drawing.Point(14, 14);
            this.lblRepeat.Margin = new System.Windows.Forms.Padding(0);
            this.lblRepeat.Name = "lblRepeat";
            this.lblRepeat.Size = new System.Drawing.Size(1532, 36);
            this.lblRepeat.TabIndex = 0;
            this.lblRepeat.Text = "설비 PLC 통신 리스트";
            this.lblRepeat.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // gridCollect
            // 
            this.gridCollect.AllowUserToAddRows = false;
            this.gridCollect.AllowUserToDeleteRows = false;
            this.gridCollect.AllowUserToResizeRows = false;
            this.gridCollect.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.gridCollect.BackgroundColor = System.Drawing.Color.WhiteSmoke;
            this.gridCollect.BorderStyle = System.Windows.Forms.BorderStyle.None;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(37)))), ((int)(((byte)(49)))), ((int)(((byte)(68)))));
            dataGridViewCellStyle1.Font = new System.Drawing.Font("맑은 고딕", 9.5F, System.Drawing.FontStyle.Bold);
            dataGridViewCellStyle1.ForeColor = System.Drawing.Color.White;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(37)))), ((int)(((byte)(49)))), ((int)(((byte)(68)))));
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.Color.White;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.gridCollect.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.gridCollect.ColumnHeadersHeight = 44;
            this.gridCollect.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("맑은 고딕", 9.5F);
            dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.Color.White;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(40)))), ((int)(((byte)(54)))));
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.gridCollect.DefaultCellStyle = dataGridViewCellStyle2;
            this.gridCollect.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gridCollect.EnableHeadersVisualStyles = false;
            this.gridCollect.GridColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(229)))), ((int)(((byte)(235)))));
            this.gridCollect.Location = new System.Drawing.Point(14, 50);
            this.gridCollect.Margin = new System.Windows.Forms.Padding(0);
            this.gridCollect.MultiSelect = false;
            this.gridCollect.Name = "gridCollect";
            this.gridCollect.ReadOnly = true;
            this.gridCollect.RowHeadersVisible = false;
            this.gridCollect.RowTemplate.Height = 34;
            this.gridCollect.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.gridCollect.Size = new System.Drawing.Size(1532, 318);
            this.gridCollect.TabIndex = 1;
            // 
            // UC_ServerMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.repeatPanel);
            this.Controls.Add(this.commandPanel);
            this.Controls.Add(this.headerPanel);
            this.Name = "UC_ServerMain";
            this.Size = new System.Drawing.Size(1560, 508);
            this.headerPanel.ResumeLayout(false);
            this.headerPanel.PerformLayout();
            this.commandPanel.ResumeLayout(false);
            this.repeatPanel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.gridCollect)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel headerPanel;
        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.Label lblStatus;
        private System.Windows.Forms.Panel commandPanel;
        private System.Windows.Forms.Button btnServerStart;
        private System.Windows.Forms.Button btnCollectStart;
        private System.Windows.Forms.Label lblApiState;
        private System.Windows.Forms.Label lblRepeatState;
        private System.Windows.Forms.TableLayoutPanel repeatPanel;
        private System.Windows.Forms.Label lblRepeat;
        private System.Windows.Forms.DataGridView gridCollect;
    }
}
