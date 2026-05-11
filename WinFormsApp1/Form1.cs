using System.Text;
using System.Text.Json;

namespace WinFormsApp1
{
    public partial class Form1 : Form
    {
        private static readonly HttpClient httpClient = new HttpClient();
        private string rootUrl = "http://localhost:5248";
        private DataModelLogin modelLogin = new DataModelLogin();

        public Form1()
        {
            InitializeComponent();
        }

        private async Task LoadList()
        {
            try
            {
                string url = $"{rootUrl}/api/Bookmark";
                httpClient.DefaultRequestHeaders.Remove("Authorization");
                httpClient.DefaultRequestHeaders.Add("Authorization", modelLogin.token);
                var result = await httpClient.GetStringAsync(url);
                var bookmarks = JsonSerializer.Deserialize<List<Bookmark>>(result);
                dataGridView1.DataSource = bookmarks;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"加载书签失败：{ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void btnLogin_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(txtLoginAccount.Text))
                {
                    MessageBox.Show("请输入账号", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (string.IsNullOrWhiteSpace(txtLoginPassword.Text))
                {
                    MessageBox.Show("请输入密码", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                string url = $"{rootUrl}/api/Login/login";

                var loginData = new { Account = txtLoginAccount.Text, Password = txtLoginPassword.Text };
                var jsonContent = new StringContent(JsonSerializer.Serialize(loginData), Encoding.UTF8, "application/json");

                var response = await httpClient.PostAsync(url, jsonContent);

                if (!response.IsSuccessStatusCode)
                {
                    var message = await response.Content.ReadAsStringAsync();
                    MessageBox.Show(message, "登录失败", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                var result = await response.Content.ReadAsStringAsync();
                var returnModel = JsonSerializer.Deserialize<DataModelLogin>(result);

                if (returnModel == null)
                {
                    MessageBox.Show("登录失败：返回数据异常", "操作提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                modelLogin.account = returnModel.account;
                modelLogin.role = returnModel.role;
                modelLogin.id = returnModel.id;
                modelLogin.token = returnModel.token;

                txtId.Text = modelLogin.id.ToString();
                txtOldPassword.Text = txtLoginPassword.Text;

                lblMessage.Text = $"用户信息：{returnModel.account} {returnModel.role}";

                await LoadList();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"登录请求失败：{ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void btnChangePassword_Click(object sender, EventArgs e)
        {
            try
            {
                if (!Guid.TryParse(txtId.Text, out Guid userId))
                {
                    MessageBox.Show("用户ID无效，请先登录", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (string.IsNullOrWhiteSpace(txtOldPassword.Text))
                {
                    MessageBox.Show("请输入旧密码", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (string.IsNullOrWhiteSpace(txtNewPassword.Text))
                {
                    MessageBox.Show("请输入新密码", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (txtNewPassword.Text.Length < 6)
                {
                    MessageBox.Show("新密码长度至少6位", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (txtOldPassword.Text == txtNewPassword.Text)
                {
                    MessageBox.Show("新旧密码不能相同", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                string url = $"{rootUrl}/api/Login/password";

                var changeData = new { Id = userId, OldPassword = txtOldPassword.Text, NewPassword = txtNewPassword.Text };
                var jsonContent = new StringContent(JsonSerializer.Serialize(changeData), Encoding.UTF8, "application/json");

                httpClient.DefaultRequestHeaders.Remove("Authorization");
                httpClient.DefaultRequestHeaders.Add("Authorization", modelLogin.token);

                var response = await httpClient.PatchAsync(url, jsonContent);

                if (!response.IsSuccessStatusCode)
                {
                    var message = await response.Content.ReadAsStringAsync();
                    MessageBox.Show(message, "修改密码失败", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                MessageBox.Show("修改密码成功", "操作提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                txtOldPassword.Text = txtNewPassword.Text;
                txtNewPassword.Clear();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"修改密码请求失败：{ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
