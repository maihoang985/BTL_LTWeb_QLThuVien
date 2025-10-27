/**
 * Custom Theme Switcher Script for Sneat Template
 * ------------------------------------------------
 * This script handles theme switching (Light/Dark) using the toggle 
 * in the Navbar and persists the preference in Local Storage.
 * * Sửa lỗi: Đảm bảo thiết lập class và data-theme trên thẻ <html> nhất quán.
 */

'use strict';

document.addEventListener('DOMContentLoaded', function () {
    // 1. Khai báo các biến và tham chiếu DOM
    const themeToggle = document.getElementById('theme-toggle');
    const htmlElement = document.documentElement; // Tham chiếu đến thẻ <html>
    const localStorageKey = 'theme';
    const lightClass = 'light-style';
    const darkClass = 'dark-style';
    const lightTheme = 'theme-default';
    const darkTheme = 'theme-dark-custom';

    // ✨ Thêm: Tham chiếu DOM cho Bảng và Table Head
    const tableElement = document.getElementById('myTable');
    const tableHeadElement = document.getElementById('myTableHead');

    // ✨ Thêm: Các Class của Table
    const tableDarkClass = 'table-dark';
    const tableLightClass = 'table-light';

    if (!themeToggle || !tableElement || !tableHeadElement) {
        // Thoát nếu công tắc chuyển đổi theme hoặc bảng không tồn tại
        return;
    }

    // 2. Hàm áp dụng theme
    function applyTheme(theme) {
        if (theme === 'dark') {
            // Áp dụng theme tối cho toàn bộ trang
            htmlElement.classList.remove(lightClass);
            htmlElement.classList.add(darkClass);
            htmlElement.setAttribute('data-theme', darkTheme);
            themeToggle.checked = true;

            // ✨ CHỈNH SỬA: Theme Dark -> thead phải là Light (table-light)
            tableHeadElement.classList.remove(tableDarkClass); // Bỏ dark
            tableHeadElement.classList.add(tableLightClass);  // Thêm light

        } else {
            // Áp dụng theme sáng (mặc định) cho toàn bộ trang
            htmlElement.classList.remove(darkClass);
            htmlElement.classList.add(lightClass);
            htmlElement.setAttribute('data-theme', lightTheme);
            themeToggle.checked = false;

            // ✨ CHỈNH SỬA: Theme Light -> thead phải là Dark (table-dark)
            tableHeadElement.classList.remove(tableLightClass); // Bỏ light
            tableHeadElement.classList.add(tableDarkClass);   // Thêm dark
        }

        // Lưu trạng thái mới vào Local Storage
        localStorage.setItem(localStorageKey, theme);
        window.dispatchEvent(new Event('resize'));
    }

    // 3. Tải theme đã lưu (khi trang tải)
    let savedTheme = localStorage.getItem(localStorageKey);

    if (savedTheme) {
        applyTheme(savedTheme);
    } else {
        const currentTheme = htmlElement.classList.contains(darkClass) ? 'dark' : 'light';
        themeToggle.checked = (currentTheme === 'dark');
        // Gọi applyTheme để đảm bảo bảng được thiết lập đúng ngay cả khi không có savedTheme
        applyTheme(currentTheme);
    }

    // 4. Xử lý sự kiện chuyển đổi
    themeToggle.addEventListener('change', function () {
        const newTheme = themeToggle.checked ? 'dark' : 'light';
        applyTheme(newTheme);
    });
});