// =====================================================================
// === FrmLogin.cs (PHIÊN BẢN HOÀN CHỈNH ĐÃ SỬA LỖI VÀ TỐI ƯU HÓA) ===
// =====================================================================

using Store_X.BLL;
using System;
using System.Data;
using System.Windows.Forms;

namespace Store_X
{
    public partial class FrmLogin : Form
    {
        public FrmLogin()
        {
            InitializeComponent();
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {// --- Dán toàn bộ phương thức này vào FrmLogin.cs ---


            // --- BƯỚC 1: LẤY VÀ KIỂM TRA DỮ LIỆU ĐẦU VÀO ---
            string username = txtUsername.Text.Trim();
            string password = txtPassword.Text;

            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                MessageBox.Show("Vui lòng nhập đầy đủ tên đăng nhập và mật khẩu.", "Thiếu thông tin", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                // --- BƯỚC 2: GỌI BLL ĐỂ THỰC HIỆN LOGIC ĐĂNG NHẬP ---
                // AccountBLL sẽ xử lý việc tìm user, băm mật khẩu và so sánh
                DataRow loginResult = AccountBLL.Instance.Login(username, password);

                // --- BƯỚC 3: XỬ LÝ KẾT QUẢ ĐĂNG NHẬP ---
                if (loginResult != null)
                {
                    // --- ĐĂNG NHẬP THÀNH CÔNG ---

                    // 3.1. Lưu thông tin phiên đăng nhập vào lớp tĩnh
                    CurrentUserSession.AccountID = Convert.ToInt32(loginResult["AccountID"]);
                    CurrentUserSession.Username = loginResult["Username"].ToString();
                    CurrentUserSession.Role = loginResult["RoleName"].ToString();
                    CurrentUserSession.EmployeeID = loginResult["EmployeeID"] as int?; // Xử lý an toàn giá trị null
                    CurrentUserSession.CustomerID = loginResult["CustomerID"] as int?;   // Xử lý an toàn giá trị null
                    string employeeName = loginResult["EmployeeName"].ToString(); // Tên nhân viên để truyền đi

                    // 3.2. Ẩn form đăng nhập hiện tại
                    this.Hide();

                    // 3.3. Phân quyền và mở Form tương ứng dựa trên vai trò
                    switch (CurrentUserSession.Role)
                    {
                        case "Admin":
                            FrmAdmin fAdmin = new FrmAdmin();
                            fAdmin.ShowDialog();
                            break;

                        case "Sale":
                            if (CurrentUserSession.EmployeeID.HasValue)
                            {
                                FrmSale fSale = new FrmSale(CurrentUserSession.EmployeeID.Value);
                                fSale.ShowDialog();
                            }
                            else
                            {
                                MessageBox.Show("Lỗi: Tài khoản nhân viên bán hàng không có ID hợp lệ.", "Lỗi dữ liệu", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                this.Show(); // Hiển thị lại form đăng nhập
                                return; // Dừng lại để tránh đóng form
                            }
                            break;

                        case "Warehouse":
                            if (CurrentUserSession.EmployeeID.HasValue)
                            {
                                FrmWarehouse fWarehouse = new FrmWarehouse(CurrentUserSession.EmployeeID.Value, employeeName);
                                fWarehouse.ShowDialog();
                            }
                            else
                            {
                                MessageBox.Show("Lỗi: Tài khoản nhân viên kho không có ID hợp lệ.", "Lỗi dữ liệu", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                this.Show();
                                return;
                            }
                            break;

                        case "Customer":
                            if (CurrentUserSession.CustomerID.HasValue)
                            {
                                FrmCustomer fCustomer = new FrmCustomer(CurrentUserSession.CustomerID.Value);
                                fCustomer.ShowDialog();
                            }
                            else
                            {
                                MessageBox.Show("Lỗi: Tài khoản khách hàng không có ID hợp lệ.", "Lỗi dữ liệu", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                this.Show();
                                return;
                            }
                            break;

                        default:
                            MessageBox.Show($"Vai trò '{CurrentUserSession.Role}' không được hỗ trợ hoặc chưa được phê duyệt.", "Lỗi phân quyền", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            this.Show(); // Hiển thị lại form đăng nhập
                            CurrentUserSession.Clear(); // Xóa phiên đăng nhập lỗi
                            return;
                    }

                    // 3.4. Sau khi Form chính được đóng, đóng luôn Form đăng nhập để kết thúc ứng dụng
                    this.Close();
                }
                else
                {
                    // --- ĐĂNG NHẬP THẤT BẠI ---
                    MessageBox.Show("Tên đăng nhập hoặc mật khẩu không chính xác.", "Đăng nhập thất bại", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                // Bắt các lỗi không mong muốn khác (ví dụ: mất kết nối CSDL)
                MessageBox.Show("Đã xảy ra lỗi trong quá trình đăng nhập: " + ex.Message, "Lỗi hệ thống", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        

        private void btnCreateAccount_Click(object sender, EventArgs e)
        {
           
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            // Thoát ứng dụng
            Application.Exit();
        }

        private void btnCreate_Click(object sender, EventArgs e)
        {
            // Mở form tạo tài khoản
            FrmCreateAccount fCreate = new FrmCreateAccount();
            fCreate.ShowDialog();
        }

        private void FrmLogin_Load(object sender, EventArgs e)
        {

        }
    }
}