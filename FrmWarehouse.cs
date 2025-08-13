// =====================================================================
// === FrmWarehouse.cs (PHIÊN BẢN HOÀN CHỈNH ĐÃ SỬA LỖI VÀ HỢP NHẤT) ===
// =====================================================================

using Store_X.BLL;
using Store_X.DTO;
using System;
using System.Data;
using System.Windows.Forms;

namespace Store_X
{
    public partial class FrmWarehouse : Form
    {  // === BIẾN LƯU TRỮ TRẠNG THÁI CỦA FORM ===
       // Chỉ KHAI BÁO các biến ở đây
        private int _currentEmployeeId;
        private string _currentEmployeeName;
        private int? _currentProductId = null; // Dùng để lưu ID sản phẩm đã được tìm thấy và xác nhận

       

        /// <summary>
        /// Hàm khởi tạo, nên nhận thông tin nhân viên đăng nhập.
        /// </summary>
        public FrmWarehouse(int employeeId, string employeeName) // Tạm thời bỏ tham số để dễ chạy
        {
            InitializeComponent();
            _currentEmployeeId = employeeId;
            _currentEmployeeName = employeeName;
            // LƯU Ý: Khi hệ thống đăng nhập hoàn thiện, bạn nên truyền thông tin nhân viên vào đây.
            // _currentEmployeeId = employeeId;
            // _currentEmployeeName = employeeName;
        }

        private void FrmWarehouse_Load(object sender, EventArgs e)
        {
            // Tạm thời hard-code thông tin nhân viên để code chạy được
            _currentEmployeeId = 1;
            _currentEmployeeName = "Admin";

            // Hiển thị thông tin nhân viên (nếu có ô tương ứng)
            txtEmployeeName.Text = _currentEmployeeName;
            txtEmployeeID.Text = _currentEmployeeId.ToString();

            // Tải lịch sử kho khi form được mở
            LoadWarehouseHistory();
        }

        #region Helper Methods (Các phương thức hỗ trợ)

        /// <summary>
        /// Tải toàn bộ lịch sử giao dịch kho vào DataGridView.
        /// </summary>
        void LoadWarehouseHistory()
        {
            try
            {
                // Giả sử DataGridView của bạn có tên là dgvHistory
                dgvHistory.DataSource = WarehouseHistoryBLL.Instance.GetAllWarehouseHistory();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi tải lịch sử kho: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Xóa trắng các ô nhập liệu thông tin sản phẩm.
        /// </summary>
        void ClearProductFields()
        {
            txtProductID.Clear();
            txtProductName.Clear();
            txtPrice.Clear();
            txtSupplier.Clear(); // Giả sử là ô nhà sản xuất
            txtCategory.Clear();
            numQuantity.Value = 1; // Giả sử NumericUpDown có tên là numQuantity
            _currentProductId = null; // Quan trọng: Reset ID sản phẩm đã chọn
            txtFindByID.Focus(); // Đưa con trỏ về ô tìm kiếm
        }

        #endregion

        #region Button and Control Events

        /// <summary>
        /// Nút "Find...": Tìm thông tin sản phẩm bằng ID và điền vào các ô.
        /// </summary>
        private void btnFind_Click(object sender, EventArgs e)
        {
        }

        /// <summary>
        /// Nút "Add": Thực hiện giao dịch Nhập hoặc Xuất kho sau khi đã tìm sản phẩm.
        /// </summary>
        private void btnAdd_Click(object sender, EventArgs e)
        {
        }

        /// <summary>
        /// Nút "refresh lịch sử kho": Tải lại danh sách lịch sử.
        /// </summary>
        private void btnRefreshHistory_Click(object sender, EventArgs e)
        {
         
        }

        // Nút "Log out"
        private void btnLogout_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("Bạn có chắc chắn muốn đăng xuất không?", "Xác nhận đăng xuất", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                Application.Restart();
            }
        }

        // Nút "btnViewR" (Xem báo cáo)
        private void btnViewR_Click(object sender, EventArgs e)
        {
           
        }

        #endregion

        private void btnAdd_Click_1(object sender, EventArgs e)
        {

            // Bước 1: Kiểm tra xem đã tìm và chọn được sản phẩm hợp lệ chưa
            if (_currentProductId == null)
            {
                MessageBox.Show("Vui lòng tìm một sản phẩm hợp lệ bằng ID trước khi thực hiện giao dịch.", "Chưa chọn sản phẩm", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int quantity = (int)numQuantity.Value;

            // Bước 2: Hỏi người dùng muốn Nhập hay Xuất
            DialogResult choice = MessageBox.Show("Bạn muốn thực hiện hành động nào cho sản phẩm này?\n\n- Chọn 'Yes' để NHẬP KHO.\n- Chọn 'No' để XUẤT KHO.",
                                                   "Xác nhận hành động",
                                                   MessageBoxButtons.YesNoCancel,
                                                   MessageBoxIcon.Question);

            string transactionType;
            if (choice == DialogResult.Yes)
            {
                transactionType = "Nhập";
            }
            else if (choice == DialogResult.No)
            {
                transactionType = "Xuất";
            }
            else // Nếu người dùng nhấn Cancel hoặc đóng hộp thoại
            {
                return; // Không làm gì cả
            }

            // Bước 3: Gọi BLL để thực hiện giao dịch
            if (WarehouseHistoryBLL.Instance.CreateWarehouseTransaction(_currentProductId.Value, _currentEmployeeId, transactionType, quantity, $"Giao dịch từ form Kho"))
            {
                MessageBox.Show($"{transactionType} kho thành công!", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                LoadWarehouseHistory(); // Tải lại lịch sử
                ClearProductFields();     // Xóa trắng các ô
                txtFindByID.Clear();
            }
            else
            {
                MessageBox.Show($"{transactionType} kho thất bại! Vui lòng kiểm tra lại (đặc biệt là số lượng tồn kho nếu bạn đang xuất hàng).", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnViewReports_Click(object sender, EventArgs e)
        {
            // Mở FrmAdmin, nó sẽ tự động cấu hình để chỉ hiển thị tab báo cáo (nếu bạn đã có logic đó)
            FrmAdmin fAdminReports = new FrmAdmin();
            fAdminReports.ShowDialog();
        }

        private void btnRefreshHistory_Click_1(object sender, EventArgs e)
        {
            LoadWarehouseHistory();
            MessageBox.Show("Đã làm mới lịch sử kho.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void btnFind_Click_1(object sender, EventArgs e)
        {

            if (string.IsNullOrWhiteSpace(txtFindByID.Text))
            {
                MessageBox.Show("Vui lòng nhập ID sản phẩm cần tìm.", "Thiếu thông tin", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                int productId = Convert.ToInt32(txtFindByID.Text);

                // Giả sử BLL trả về DataTable
                DataTable dt = ProductBLL.Instance.GetProductDetailsById(productId);

                if (dt != null && dt.Rows.Count > 0)
                {
                    DataRow row = dt.Rows[0];
                    _currentProductId = Convert.ToInt32(row["ProductID"]); // Lưu lại ID để thực hiện giao dịch
                    txtProductID.Text = row["ProductID"].ToString();
                    txtProductName.Text = row["ProductName"].ToString();
                    txtPrice.Text = row["Price"].ToString();
                    txtSupplier.Text = row["SupplierName"].ToString();
                    txtCategory.Text = row["CategoryName"].ToString();
                    numQuantity.Focus(); // Di chuyển con trỏ tới ô số lượng
                }
                else
                {
                    MessageBox.Show("Không tìm thấy sản phẩm với ID này.", "Không tìm thấy", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    ClearProductFields();
                }
            }
            catch (FormatException)
            {
                MessageBox.Show("ID sản phẩm phải là một con số.", "Lỗi định dạng", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {

            DialogResult result = MessageBox.Show("Bạn có chắc chắn muốn đăng xuất không?", "Xác nhận đăng xuất", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                Application.Restart();
            }
        }
    }
}