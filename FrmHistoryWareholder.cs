using Store_X.BLL;
using System;
using System.Windows.Forms;

namespace Store_X
{
    // Đổi tên form cho đúng với giao diện của bạn
    public partial class FrmHistoryWareholder : Form
    {
        public FrmHistoryWareholder()
        {
            InitializeComponent();
        }

        private void FrmHistoryWareholder_Load(object sender, EventArgs e)
        {
            // Tải lịch sử kho ngay khi form được mở
            LoadWarehouseHistory();
        }

        /// <summary>
        /// Tải toàn bộ lịch sử giao dịch kho vào DataGridView.
        /// </summary>
        private void LoadWarehouseHistory()
        {
            try
            {
                dgvWarehouseHistory.DataSource = WarehouseHistoryBLL.Instance.GetAllWarehouseHistory();
                // Định dạng lại các cột để dễ nhìn
                dgvWarehouseHistory.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                dgvWarehouseHistory.Columns["HistoryID"].HeaderText = "Mã Giao Dịch";
                dgvWarehouseHistory.Columns["ProductName"].HeaderText = "Tên Sản Phẩm";
                dgvWarehouseHistory.Columns["EmployeeName"].HeaderText = "Nhân Viên Thực Hiện";
                dgvWarehouseHistory.Columns["TransactionType"].HeaderText = "Loại Giao Dịch";
                dgvWarehouseHistory.Columns["Quantity"].HeaderText = "Số Lượng";
                dgvWarehouseHistory.Columns["TransactionDate"].HeaderText = "Thời Gian";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi tải lịch sử kho: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Nút "refresh lịch sử kho": Tải lại danh sách lịch sử.
        /// </summary>
        private void btnRefreshHistory_Click(object sender, EventArgs e)
        {
            LoadWarehouseHistory();
            MessageBox.Show("Đã làm mới lịch sử kho.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        /// <summary>
        /// Sự kiện CellClick để xem chi tiết một giao dịch.
        /// </summary>
        private void dgvWarehouseHistory_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dgvWarehouseHistory.Rows[e.RowIndex];
                string message = $"--- Chi tiết giao dịch kho ---\n\n" +
                                 $"Mã giao dịch: {row.Cells["HistoryID"].Value}\n" +
                                 $"Sản phẩm: {row.Cells["ProductName"].Value}\n" +
                                 $"Loại giao dịch: {row.Cells["TransactionType"].Value}\n" +
                                 $"Số lượng: {row.Cells["Quantity"].Value}\n" +
                                 $"Người thực hiện: {row.Cells["EmployeeName"].Value}\n" +
                                 $"Thời gian: {Convert.ToDateTime(row.Cells["TransactionDate"].Value):dd/MM/yyyy HH:mm:ss}";

                MessageBox.Show(message, "Chi tiết giao dịch", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {

            // Gọi lại phương thức tải dữ liệu để cập nhật thông tin mới nhất
            LoadWarehouseHistory();

            // (Tùy chọn) Hiển thị thông báo để người dùng biết thao tác đã hoàn tất
            MessageBox.Show("Đã làm mới lịch sử kho.", "Thông báo",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}