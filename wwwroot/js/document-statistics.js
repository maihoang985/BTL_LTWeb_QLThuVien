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
        // Tổng số giáo trình
        if (data.tongGiaoTrinh !== undefined) {
            animateNumber(totalGiaoTrinhEl, data.tongGiaoTrinh);
        }

        // Tổng số tài liệu
        if (data.tongTaiLieu !== undefined) {
            animateNumber(totalTaiLieuEl, data.tongTaiLieu);
        }

        // Giáo trình có sẵn
        if (data.giaoTrinhCoSan !== undefined) {
            animateNumber(availableGiaoTrinhEl, data.giaoTrinhCoSan);
        }

        // Tài liệu có sẵn
        if (data.taiLieuCoSan !== undefined) {
            animateNumber(availableTaiLieuEl, data.taiLieuCoSan);
        }
    }

    function renderCategoryTable(categories) {
        if (!categories || categories.length === 0) {
            showNoDataTable();
            return;
        }

        // Danh sách đầy đủ 10 thể loại
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

        // Kết hợp dữ liệu từ API và định nghĩa đầy đủ, đồng thời chuẩn bị cho việc sắp xếp
        const combinedCategories = allCategoriesDefinitions.map(catDef => {
            const cat = categories.find(c => c.tenDanhMuc === catDef.name);
            return {
                ...catDef,
                tongSoLuong: cat ? cat.tongSoLuong : 0, // Lấy tổng số lượng, mặc định là 0
                soLuongCoSan: cat ? cat.soLuongCoSan : 0,
                // Giữ lại đối tượng gốc từ API nếu có để dễ dàng render
                apiData: cat
            };
        });

        // Sắp xếp các thể loại theo tổng số lượng (tongSoLuong) từ cao đến thấp (giảm dần)
        combinedCategories.sort((a, b) => b.tongSoLuong - a.tongSoLuong);

        // Tạo các hàng (rows) cho bảng
        const rows = combinedCategories.map(item => {
            const cat = item.apiData;
            const catDef = item;

            if (cat) {
                const percentage = calculatePercentage(cat.soLuongCoSan, cat.tongSoLuong);
                const dangMuon = cat.tongSoLuong - cat.soLuongCoSan;

                return `
                    <tr>
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
                // Thể loại không có dữ liệu (tongSoLuong = 0)
                return `
                    <tr>
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
    }

    function toggleCategoryDetail() {
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
        // Show loading states
        showLoadingCard(totalGiaoTrinhEl);
        showLoadingCard(totalTaiLieuEl);
        showLoadingCard(availableGiaoTrinhEl);
        showLoadingCard(availableTaiLieuEl);
        showLoadingTable();

        fetchStatistics()
            .then(data => {
                // Render summary cards
                renderSummaryCards(data);

                // Render category table
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
            // Get DOM elements
            totalGiaoTrinhEl = document.getElementById('totalGiaoTrinh');
            totalTaiLieuEl = document.getElementById('totalTaiLieu');
            availableGiaoTrinhEl = document.getElementById('availableGiaoTrinh');
            availableTaiLieuEl = document.getElementById('availableTaiLieu');
            categoryTableBody = document.querySelector('#categoryTable tbody');
            toggleDetailBtn = document.getElementById('toggleDetailBtn');
            categoryDetail = document.getElementById('categoryDetail');
            toggleIcon = document.getElementById('toggleIcon');
            toggleText = document.getElementById('toggleText');

            if (!totalGiaoTrinhEl || !totalTaiLieuEl || !categoryTableBody || !toggleDetailBtn) {
                console.error('Required DOM elements not found!');
                return;
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