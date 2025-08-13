// =====================================================================
// === FrmSale.cs (PHIÊN BẢN ĐÃ SỬA LỖI KIỂU DỮ LIỆU) ===
// =====================================================================

using Store_X.BLL;
using Store_X.DTO;
using System;
using System.Collections.Generic;
using System.Data;
using System.Windows.Forms;

namespace Store_X
{
    public partial class FrmSale : Form
    {
        private int _currentEmployeeId;
        private int? _currentCustomerId = null;
        private DataTable _currentInvoiceDetails;

        public FrmSale(int employeeId)
        {
            InitializeComponent();
            _currentEmployeeId = employeeId;
        }

        private void FrmSale_Load(object sender, EventArgs e)
        {
            InitializeNewInvoice();
            LoadRecentInvoiceHistory();
        }

        private void InitializeNewInvoice()
        {
            _currentInvoiceDetails = new DataTable();
            _currentInvoiceDetails.Columns.Add("ProductID", typeof(int));
            _currentInvoiceDetails.Columns.Add("ProductName", typeof(string));
            _currentInvoiceDetails.Columns.Add("Quantity", typeof(int));
            _currentInvoiceDetails.Columns.Add("UnitPrice", typeof(decimal));
            _currentInvoiceDetails.Columns.Add("Total", typeof(decimal), "Quantity * UnitPrice");
            dgvInvoiceDetails.DataSource = _currentInvoiceDetails;
            ClearAllFields();
        }

        private void ClearAllFields()
        {
            txtProductID.Clear();
            txtProductName.Clear();
            txtPrice.Clear();
            numQuantity.Value = 1;
            txtCustomerID.Clear();
            txtCustomerName.Text = "Khách lẻ";
            _currentCustomerId = null;
            if (_currentInvoiceDetails != null) _currentInvoiceDetails.Clear();
            txtProductID.Focus();
        }

        private void LoadRecentInvoiceHistory()
        {
            try
            {
                // << SỬA LỖI: Đảm bảo biến nhận kết quả là DataTable >>
                // Phương thức GetRecentInvoices() trả về một bảng, không phải một dòng.
                dgvHistory.DataSource = InvoiceBLL.Instance.GetRecentInvoices();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tải lịch sử hóa đơn: " + ex.Message);
            }
        }

        private void txtProductID_Leave(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtProductID.Text)) return;
            try
            {
                int productId = Convert.ToInt32(txtProductID.Text);
                DataRow product = ProductBLL.Instance.GetProductById(productId);
                if (product != null)
                {
                    txtProductName.Text = product["ProductName"].ToString();
                    txtPrice.Text = product["Price"].ToString();
                    numQuantity.Focus();
                }
                else
                {
                    MessageBox.Show("Không tìm thấy sản phẩm với ID này.", "Không tìm thấy", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtProductName.Clear();
                    txtPrice.Clear();
                }
            }
            catch (FormatException) { /* Bỏ qua lỗi */ }
        }

        private void btnFindCustomer_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtCustomerID.Text))
            {
                txtCustomerName.Text = "Khách lẻ";
                _currentCustomerId = null;
                return;
            }
            try
            {
                int customerId = Convert.ToInt32(txtCustomerID.Text);
                DataTable customerTable = CustomerBLL.Instance.GetCustomerById(customerId);
                if (customerTable != null && customerTable.Rows.Count > 0)
                {
                    DataRow customer = customerTable.Rows[0];
                    txtCustomerName.Text = customer["CustomerName"].ToString();
                    _currentCustomerId = customerId;
                }
                else
                {
                    MessageBox.Show("Không tìm thấy khách hàng với ID này.", "Không tìm thấy", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtCustomerName.Clear();
                    _currentCustomerId = null;
                }
            }
            catch (FormatException) { MessageBox.Show("ID khách hàng phải là một con số.", "Lỗi định dạng", MessageBoxButtons.OK, MessageBoxIcon.Error); }
        }

        private void btnCreateInvoice_Click(object sender, EventArgs e)
        {

            if (_currentInvoiceDetails.Rows.Count == 0)
            {
                MessageBox.Show("Hóa đơn phải có ít nhất một sản phẩm.", "Hóa đơn trống", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                // Tạo đối tượng Invoice (thông tin chính của hóa đơn)
                Invoice invoiceHeader = new Invoice
                {
                    EmployeeID = _currentEmployeeId,
                    CustomerID = _currentCustomerId, // Có thể là null nếu là khách lẻ
                    InvoiceDate = DateTime.Now,
                    TotalAmount = (decimal)_currentInvoiceDetails.Compute("SUM(Total)", "") // Tự động tính tổng tiền
                };

                // Tạo danh sách các chi tiết hóa đơn từ DataTable
                List<InvoiceDetail> invoiceDetailsList = new List<InvoiceDetail>();
                foreach (DataRow row in _currentInvoiceDetails.Rows)
                {
                    invoiceDetailsList.Add(new InvoiceDetail
                    {
                        ProductID = Convert.ToInt32(row["ProductID"]),
                        Quantity = Convert.ToInt32(row["Quantity"]),
                        UnitPrice = Convert.ToDecimal(row["UnitPrice"])
                    });
                }

                // Gọi BLL để thực hiện lưu vào CSDL (trong một transaction)
                if (InvoiceBLL.Instance.CreateInvoice(invoiceHeader, invoiceDetailsList))
                {
                    MessageBox.Show("Tạo hóa đơn thành công!", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    InitializeNewInvoice();     // Khởi tạo lại form cho hóa đơn mới
                    LoadRecentInvoiceHistory(); // Cập nhật lại danh sách lịch sử
                }
                else
                {
                    MessageBox.Show("Tạo hóa đơn thất bại! Nguyên nhân có thể do số lượng tồn kho không đủ.", "Lỗi nghiệp vụ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Đã xảy ra lỗi hệ thống khi tạo hóa đơn: " + ex.Message, "Lỗi hệ thống", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnFindCustomer_Click_1(object sender, EventArgs e)
        {

            if (string.IsNullOrWhiteSpace(txtCustomerID.Text))
            {
                // Nếu người dùng không nhập ID, mặc định là khách lẻ
                txtCustomerName.Text = "Khách lẻ";
                _currentCustomerId = null;
                return;
            }
            try
            {
                int customerId = Convert.ToInt32(txtCustomerID.Text);
                DataTable customerTable = CustomerBLL.Instance.GetCustomerById(customerId);
                if (customerTable != null && customerTable.Rows.Count > 0)
                {
                    DataRow customer = customerTable.Rows[0];
                    txtCustomerName.Text = customer["CustomerName"].ToString();
                    _currentCustomerId = customerId; // Lưu lại ID của khách hàng
                }
                else
                {
                    MessageBox.Show("Không tìm thấy khách hàng với ID này.", "Không tìm thấy", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtCustomerName.Clear();
                    _currentCustomerId = null;
                }
            }
            catch (FormatException)
            {
                MessageBox.Show("ID khách hàng phải là một con số.", "Lỗi định dạng", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnAddToList_Click(object sender, EventArgs e)
        {

            if (string.IsNullOrWhiteSpace(txtProductID.Text) || string.IsNullOrWhiteSpace(txtProductName.Text))
            {
                MessageBox.Show("Vui lòng tìm một sản phẩm hợp lệ để thêm vào hóa đơn.", "Thiếu thông tin sản phẩm", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int productId = Convert.ToInt32(txtProductID.Text);
            string productName = txtProductName.Text;
            int quantity = (int)numQuantity.Value;
            decimal unitPrice = Convert.ToDecimal(txtPrice.Text);

            // Kiểm tra xem sản phẩm đã có trong hóa đơn chưa
            foreach (DataRow row in _currentInvoiceDetails.Rows)
            {
                if ((int)row["ProductID"] == productId)
                {
                    // Nếu đã có, chỉ cần cập nhật lại số lượng
                    row["Quantity"] = (int)row["Quantity"] + quantity;
                    goto ClearProductInput; // Nhảy đến phần xóa trắng ô nhập liệu
                }
            }

            // Nếu sản phẩm chưa có, thêm một dòng mới vào DataTable
            _currentInvoiceDetails.Rows.Add(productId, productName, quantity, unitPrice);

        ClearProductInput:
            // Xóa các ô thông tin sản phẩm để chuẩn bị cho lần nhập tiếp theo
            txtProductID.Clear();
            txtProductName.Clear();
            txtPrice.Clear();
            numQuantity.Value = 1;
            txtProductID.Focus();
        }

        private void btnFindInvoice_Click(object sender, EventArgs e)
        {

            // Chức năng này sẽ mở một Form khác hoặc một Dialog để người dùng nhập ID
            // và hiển thị chi tiết hóa đơn đó. Hiện tại ta tạm để trống.
            MessageBox.Show("Chức năng tìm kiếm hóa đơn chi tiết đang được phát triển.", "Thông báo");
        }

        private void btnRefreshHistory_Click(object sender, EventArgs e)
        {

            LoadRecentInvoiceHistory();
            MessageBox.Show("Đã làm mới lịch sử hóa đơn.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void btnLogout_Click(object sender, EventArgs e)
        {

            DialogResult result = MessageBox.Show("Bạn có chắc chắn muốn đăng xuất không?", "Xác nhận đăng xuất", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                Application.Restart();
            }
        }

        // ... (Dán các phương thức btnAddProduct_Click, btnCreateInvoice_Click, và các nút khác vào đây) ...
        // Các phương thức này đã đúng và không cần thay đổi.
    }
}