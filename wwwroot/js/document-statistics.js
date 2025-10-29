// File: wwwroot/js/document-statistics.js

const DocumentStatistics = (function () {
    // Private variables
    let totalGiaoTrinhEl = null;
    let totalTaiLieuEl = null;
    let availableGiaoTrinhEl = null;
    let availableTaiLieuEl = null;
    let categoryTableBody = null;
    let toggleDetailBtn = null;
    let categoryDetail = null;
    let toggleIcon = null;
    let toggleText = null;
    let isDetailVisible = false;

    // MỚI: Biến cho Modal
    let categoryDetailModalEl = null;
    let categoryDetailModal = null;
    let modalCategoryTitleEl = null;
    let modalCategoryBodyEl = null;

    // Private methods
    function showLoadingCard(element) {
        element.innerHTML = `
            <div class="spinner-border spinner-border-sm" role="status">
                <span class="visually-hidden">Loading...</span>
            </div>`;
    }

    function showError(element, message) {
        element.innerHTML = `<small class="text-danger">${message}</small>`;
    }

    function showLoadingTable() {
        categoryTableBody.innerHTML = `
            <tr>
                <td colspan="5" class="text-center">
                    <div class="spinner-border" role="status">
                        <span class="visually-hidden">Loading...</span>
                    </div>
                </td>
            </tr>`;
    }

    function showTableError(message) {
        categoryTableBody.innerHTML = `
            <tr>
                <td colspan="5" class="text-center text-danger">
                    <i class="bx bx-error-circle me-1"></i>
                    ${message}
                </td>
            </tr>`;
    }

    function showNoDataTable() {
        categoryTableBody.innerHTML = `
            <tr>
                <td colspan="5" class="text-center text-muted">
                    <i class="bx bx-info-circle me-1"></i>
                    Không có dữ liệu
                </td>
            </tr>`;
    }

    function animateNumber(element, targetNumber, duration = 1000) {
        const start = 0;
        const increment = targetNumber / (duration / 16);
        let current = start;

        const timer = setInterval(() => {
            current += increment;
            if (current >= targetNumber) {
                element.textContent = targetNumber.toLocaleString('vi-VN');
                clearInterval(timer);
            } else {
                element.textContent = Math.floor(current).toLocaleString('vi-VN');
            }
        }, 16);
    }

    function calculatePercentage(available, total) {
        if (total === 0) return 0;
        return ((available / total) * 100).toFixed(1);
    }

    function createProgressBar(percentage) {
        let colorClass = 'bg-success';
        if (percentage < 30) colorClass = 'bg-danger';
        else if (percentage < 60) colorClass = 'bg-warning';

        return `
            <div class="d-flex align-items-center gap-2">
                <div class="progress flex-grow-1" style="height: 8px;">
                    <div class="progress-bar ${colorClass}" 
                         role="progressbar" 
                         style="width: ${percentage}%"
                         aria-valuenow="${percentage}" 
                         aria-valuemin="0" 
                         aria-valuemax="100">
                    </div>
                </div>
                <small class="text-muted">${percentage}%</small>
            </div>`;
    }

    function renderSummaryCards(data) {
        // ... (Giữ nguyên code)
        if (data.tongGiaoTrinh !== undefined) {
            animateNumber(totalGiaoTrinhEl, data.tongGiaoTrinh);
        }
        if (data.tongTaiLieu !== undefined) {
            animateNumber(totalTaiLieuEl, data.tongTaiLieu);
        }
        if (data.giaoTrinhCoSan !== undefined) {
            animateNumber(availableGiaoTrinhEl, data.giaoTrinhCoSan);
        }
        if (data.taiLieuCoSan !== undefined) {
            animateNumber(availableTaiLieuEl, data.taiLieuCoSan);
        }
    }

    function renderCategoryTable(categories) {
        if (!categories || categories.length === 0) {
            showNoDataTable();
            return;
        }

        const allCategoriesDefinitions = [
            { code: 'GT', name: 'Sách giáo trình' },
            { code: 'TK', name: 'Sách tham khảo' },
            { code: 'LV', name: 'Luận văn' },
            { code: 'LA', name: 'Luận án' },
            { code: 'BA', name: 'Báo' },
            { code: 'TC', name: 'Tạp chí' },
            { code: 'DT', name: 'Đề tài nghiên cứu khoa học' },
            { code: 'KY', name: 'Kỷ yếu hội thảo' },
            { code: 'DTU', name: 'Tài liệu điện tử' },
            { code: 'NB', name: 'Tài liệu nội bộ' }
        ];

        const combinedCategories = allCategoriesDefinitions.map(catDef => {
            const cat = categories.find(c => c.tenDanhMuc === catDef.name);
            return {
                ...catDef,
                tongSoLuong: cat ? cat.tongSoLuong : 0,
                soLuongCoSan: cat ? cat.soLuongCoSan : 0,
                apiData: cat
            };
        });

        combinedCategories.sort((a, b) => b.tongSoLuong - a.tongSoLuong);

        // TẠO HÀNG (ROWS)
        const rows = combinedCategories.map(item => {
            const cat = item.apiData;
            const catDef = item;
            const categoryCode = item.code; // 'GT', 'TK', etc.
            const categoryName = item.name;

            // Thêm class "category-row-clickable" và data- attributes
            const rowClass = "class='category-row-clickable' style='cursor: pointer;'";
            const rowData = `data-category-code="${categoryCode}" data-category-name="${categoryName}"`;

            if (cat) {
                const percentage = calculatePercentage(cat.soLuongCoSan, cat.tongSoLuong);
                const dangMuon = cat.tongSoLuong - cat.soLuongCoSan;

                return `
                    <tr ${rowClass} ${rowData}>
                        <td>
                            <div class="d-flex align-items-center">
                                <i class="bx bx-folder me-2 text-primary"></i>
                                <strong>${cat.tenDanhMuc}</strong>
                            </div>
                        </td>
                        <td class="text-center">
                            <span class="badge bg-label-primary">${cat.tongSoLuong}</span>
                        </td>
                        <td class="text-center">
                            <span class="badge bg-label-success">${cat.soLuongCoSan}</span>
                        </td>
                        <td class="text-center">
                            <span class="badge bg-label-warning">${dangMuon}</span>
                        </td>
                        <td>${createProgressBar(percentage)}</td>
                    </tr>`;
            } else {
                return `
                    <tr ${rowClass} ${rowData}>
                        <td>
                            <div class="d-flex align-items-center">
                                <i class="bx bx-folder me-2 text-muted"></i>
                                <span class="text-muted">${catDef.name}</span>
                            </div>
                        </td>
                        <td class="text-center">
                            <span class="badge bg-label-secondary">0</span>
                        </td>
                        <td class="text-center">
                            <span class="badge bg-label-secondary">0</span>
                        </td>
                        <td class="text-center">
                            <span class="badge bg-label-secondary">0</span>
                        </td>
                        <td>${createProgressBar(0)}</td>
                    </tr>`;
            }
        }).join('');

        categoryTableBody.innerHTML = rows;

        // MỚI: Thêm event listeners cho các hàng vừa tạo
        addTableClickListeners();
    }

    // MỚI: Hàm thêm event listener vào các hàng
    function addTableClickListeners() {
        const clickableRows = categoryTableBody.querySelectorAll('.category-row-clickable');
        clickableRows.forEach(row => {
            row.addEventListener('click', handleCategoryRowClick);
        });
    }

    // MỚI: Hàm xử lý khi click vào hàng
    function handleCategoryRowClick(event) {
        const row = event.currentTarget;
        const code = row.dataset.categoryCode;
        const name = row.dataset.categoryName;

        if (!code || !categoryDetailModal) return;

        // Cập nhật modal và hiển thị
        modalCategoryTitleEl.textContent = `Chi tiết: ${name}`;
        modalCategoryBodyEl.innerHTML = `
            <div class="d-flex justify-content-center p-3">
                <div class="spinner-border" role="status">
                    <span class="visually-hidden">Loading...</span>
                </div>
            </div>`;
        categoryDetailModal.show();

        // Tải dữ liệu chi tiết
        fetchCategoryDetails(code);
    }

    // MỚI: Hàm gọi API lấy chi tiết
    async function fetchCategoryDetails(code) {
        try {
            const response = await fetch(`/ThongKe/GetChiTietTheLoai?maTheLoai=${code}`);
            if (!response.ok) {
                throw new Error(`Lỗi HTTP ${response.status}`);
            }
            const data = await response.json();
            renderCategoryDetailsTable(data);
        } catch (error) {
            console.error('Error fetching category details:', error);
            modalCategoryBodyEl.innerHTML = `<div class="alert alert-danger">Không thể tải chi tiết: ${error.message}</div>`;
        }
    }

    // MỚI: Hàm render bảng chi tiết trong Modal
    function renderCategoryDetailsTable(details) {
        if (!details || details.length === 0) {
            modalCategoryBodyEl.innerHTML = `<div class="alert alert-info">Không có dữ liệu chi tiết cho thể loại này.</div>`;
            return;
        }

        const tableRows = details.map(item => {
            const statusBadge = item.trangThai === "Đang mượn"
                ? `<span class="badge bg-label-warning">Đang mượn</span>`
                : `<span class="badge bg-label-success">Có sẵn</span>`;

            return `
                <tr>
                    <td>${item.maBanSao}</td>
                    <td>${item.tenTaiLieu}</td>
                    <td class="text-center">${statusBadge}</td>
                </tr>`;
        }).join('');

        modalCategoryBodyEl.innerHTML = `
            <div class="table-responsive" style="max-height: 400px; overflow-y: auto;">
                <table class="table table-hover">
                    <thead class="table-light" style="position: sticky; top: 0;">
                        <tr>
                            <th>Mã Bản Sao</th>
                            <th>Tên Tài Liệu</th>
                            <th class="text-center">Trạng Thái</th>
                        </tr>
                    </thead>
                    <tbody>
                        ${tableRows}
                    <tbody>
                </table>
            </div>
        `;
    }


    function toggleCategoryDetail() {
        // ... (Giữ nguyên code)
        isDetailVisible = !isDetailVisible;
        if (isDetailVisible) {
            categoryDetail.style.display = 'block';
            toggleIcon.className = 'bx bx-chevron-up';
            toggleText.textContent = 'Ẩn chi tiết';
        } else {
            categoryDetail.style.display = 'none';
            toggleIcon.className = 'bx bx-chevron-down';
            toggleText.textContent = 'Xem chi tiết';
        }
    }

    async function fetchStatistics() {
        // ... (Giữ nguyên code)
        try {
            const response = await fetch('/ThongKe/GetThongKeTaiLieu');
            if (!response.ok) {
                throw new Error(`Lỗi HTTP ${response.status}`);
            }
            return await response.json();
        } catch (error) {
            throw error;
        }
    }

    function loadStatistics() {
        // ... (Giữ nguyên code)
        showLoadingCard(totalGiaoTrinhEl);
        showLoadingCard(totalTaiLieuEl);
        showLoadingCard(availableGiaoTrinhEl);
        showLoadingCard(availableTaiLieuEl);
        showLoadingTable();

        fetchStatistics()
            .then(data => {
                renderSummaryCards(data);
                renderCategoryTable(data.chiTietTheoDanhMuc);
            })
            .catch(error => {
                console.error('Error loading statistics:', error);
                showError(totalGiaoTrinhEl, 'Lỗi');
                showError(totalTaiLieuEl, 'Lỗi');
                showError(availableGiaoTrinhEl, 'Lỗi');
                showError(availableTaiLieuEl, 'Lỗi');
                showTableError(error.message);
            });
    }

    // Public API
    return {
        init: function () {
            // Get DOM elements (cũ)
            totalGiaoTrinhEl = document.getElementById('totalGiaoTrinh');
            totalTaiLieuEl = document.getElementById('totalTaiLieu');
            availableGiaoTrinhEl = document.getElementById('availableGiaoTrinh');
            availableTaiLieuEl = document.getElementById('availableTaiLieu');
            categoryTableBody = document.querySelector('#categoryTable tbody');
            toggleDetailBtn = document.getElementById('toggleDetailBtn');
            categoryDetail = document.getElementById('categoryDetail');
            toggleIcon = document.getElementById('toggleIcon');
            toggleText = document.getElementById('toggleText');

            // MỚI: Get DOM elements (của Modal)
            categoryDetailModalEl = document.getElementById('categoryDetailModal');
            modalCategoryTitleEl = document.getElementById('modalCategoryTitle');
            modalCategoryBodyEl = document.getElementById('modalCategoryBody');

            if (!totalGiaoTrinhEl || !totalTaiLieuEl || !categoryTableBody || !toggleDetailBtn) {
                console.error('Required DOM elements not found!');
                return;
            }

            // MỚI: Khởi tạo đối tượng Modal của Bootstrap
            if (categoryDetailModalEl) {
                categoryDetailModal = new bootstrap.Modal(categoryDetailModalEl);
            } else {
                console.error('Category detail modal element not found!');
            }

            // Add event listener for toggle button
            toggleDetailBtn.addEventListener('click', toggleCategoryDetail);

            // Load statistics
            loadStatistics();
        },

        refresh: function () {
            loadStatistics();
        }
    };
})();

// Auto-init khi DOM ready
document.addEventListener('DOMContentLoaded', function () {
    DocumentStatistics.init();
});