namespace WinFormsApp1
{
    partial class Form1
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            lblAccount = new Label();
            txtLoginAccount = new TextBox();
            lblPassword = new Label();
            txtLoginPassword = new TextBox();
            btnLogin = new Button();
            lblMessage = new Label();
            lblId = new Label();
            txtId = new TextBox();
            lblOldPassword = new Label();
            txtOldPassword = new TextBox();
            lblNewPassword = new Label();
            txtNewPassword = new TextBox();
            btnChangePassword = new Button();
            dataGridView1 = new DataGridView();
            ((System.ComponentModel.ISupportInitialize)dataGridView1).BeginInit();
            SuspendLayout();

            // lblAccount
            lblAccount.AutoSize = true;
            lblAccount.Location = new Point(20, 20);
            lblAccount.Name = "lblAccount";
            lblAccount.Size = new Size(45, 17);
            lblAccount.Text = "账号：";

            // txtLoginAccount
            txtLoginAccount.Location = new Point(71, 16);
            txtLoginAccount.Name = "txtLoginAccount";
            txtLoginAccount.Size = new Size(150, 23);

            // lblPassword
            lblPassword.AutoSize = true;
            lblPassword.Location = new Point(20, 53);
            lblPassword.Name = "lblPassword";
            lblPassword.Size = new Size(45, 17);
            lblPassword.Text = "密码：";

            // txtLoginPassword
            txtLoginPassword.Location = new Point(71, 49);
            txtLoginPassword.Name = "txtLoginPassword";
            txtLoginPassword.PasswordChar = '*';
            txtLoginPassword.Size = new Size(150, 23);

            // btnLogin
            btnLogin.Location = new Point(227, 47);
            btnLogin.Name = "btnLogin";
            btnLogin.Size = new Size(75, 29);
            btnLogin.Text = "登录";
            btnLogin.UseVisualStyleBackColor = true;
            btnLogin.Click += btnLogin_Click;

            // lblMessage
            lblMessage.AutoSize = true;
            lblMessage.ForeColor = Color.Green;
            lblMessage.Location = new Point(20, 86);
            lblMessage.Name = "lblMessage";
            lblMessage.Size = new Size(0, 17);

            // lblId
            lblId.AutoSize = true;
            lblId.Location = new Point(20, 120);
            lblId.Name = "lblId";
            lblId.Size = new Size(28, 17);
            lblId.Text = "Id：";

            // txtId
            txtId.Location = new Point(54, 116);
            txtId.Name = "txtId";
            txtId.ReadOnly = true;
            txtId.Size = new Size(300, 23);

            // lblOldPassword
            lblOldPassword.AutoSize = true;
            lblOldPassword.Location = new Point(20, 153);
            lblOldPassword.Name = "lblOldPassword";
            lblOldPassword.Size = new Size(56, 17);
            lblOldPassword.Text = "旧密码：";

            // txtOldPassword
            txtOldPassword.Location = new Point(82, 149);
            txtOldPassword.Name = "txtOldPassword";
            txtOldPassword.PasswordChar = '*';
            txtOldPassword.Size = new Size(150, 23);

            // lblNewPassword
            lblNewPassword.AutoSize = true;
            lblNewPassword.Location = new Point(20, 186);
            lblNewPassword.Name = "lblNewPassword";
            lblNewPassword.Size = new Size(56, 17);
            lblNewPassword.Text = "新密码：";

            // txtNewPassword
            txtNewPassword.Location = new Point(82, 182);
            txtNewPassword.Name = "txtNewPassword";
            txtNewPassword.PasswordChar = '*';
            txtNewPassword.Size = new Size(150, 23);

            // btnChangePassword
            btnChangePassword.Location = new Point(238, 180);
            btnChangePassword.Name = "btnChangePassword";
            btnChangePassword.Size = new Size(100, 29);
            btnChangePassword.Text = "修改密码";
            btnChangePassword.UseVisualStyleBackColor = true;
            btnChangePassword.Click += btnChangePassword_Click;

            // dataGridView1
            dataGridView1.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridView1.Location = new Point(20, 220);
            dataGridView1.Name = "dataGridView1";
            dataGridView1.RowTemplate.Height = 25;
            dataGridView1.Size = new Size(560, 250);

            // Form1
            AutoScaleDimensions = new SizeF(7F, 17F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(600, 500);
            Controls.Add(dataGridView1);
            Controls.Add(btnChangePassword);
            Controls.Add(txtNewPassword);
            Controls.Add(lblNewPassword);
            Controls.Add(txtOldPassword);
            Controls.Add(lblOldPassword);
            Controls.Add(txtId);
            Controls.Add(lblId);
            Controls.Add(lblMessage);
            Controls.Add(btnLogin);
            Controls.Add(txtLoginPassword);
            Controls.Add(lblPassword);
            Controls.Add(txtLoginAccount);
            Controls.Add(lblAccount);
            Name = "Form1";
            Text = "Windows窗体布局";
            ((System.ComponentModel.ISupportInitialize)dataGridView1).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label lblAccount;
        private TextBox txtLoginAccount;
        private Label lblPassword;
        private TextBox txtLoginPassword;
        private Button btnLogin;
        private Label lblMessage;
        private Label lblId;
        private TextBox txtId;
        private Label lblOldPassword;
        private TextBox txtOldPassword;
        private Label lblNewPassword;
        private TextBox txtNewPassword;
        private Button btnChangePassword;
        private DataGridView dataGridView1;
    }
}
