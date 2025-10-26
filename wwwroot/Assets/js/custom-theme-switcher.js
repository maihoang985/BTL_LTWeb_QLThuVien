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

    if (!themeToggle) {
        // Thoát nếu công tắc chuyển đổi theme không tồn tại trên NavBar
        return;
    }

    // 2. Hàm áp dụng theme
    function applyTheme(theme) {
        if (theme === 'dark') {
            // Áp dụng theme tối
            htmlElement.classList.remove(lightClass);
            htmlElement.classList.add(darkClass);

            htmlElement.setAttribute('data-theme', darkTheme);

            themeToggle.checked = true;
        } else {
            // Áp dụng theme sáng (mặc định)
            htmlElement.classList.remove(darkClass);
            htmlElement.classList.add(lightClass);

            htmlElement.setAttribute('data-theme', lightTheme);

            themeToggle.checked = false;
        }

        // Lưu trạng thái mới vào Local Storage
        localStorage.setItem(localStorageKey, theme);

        // Kích hoạt sự kiện resize để các thành phần JS (ví dụ: Charts) cập nhật
        window.dispatchEvent(new Event('resize'));
    }

    // 3. Tải theme đã lưu (khi trang tải)
    let savedTheme = localStorage.getItem(localStorageKey);

    if (savedTheme) {
        // Áp dụng theme đã lưu
        applyTheme(savedTheme);
    } else {
        // Nếu không có theme lưu, thiết lập trạng thái toggle dựa trên class hiện tại của <html> (mặc định là light-style)
        const currentTheme = htmlElement.classList.contains(darkClass) ? 'dark' : 'light';
        themeToggle.checked = (currentTheme === 'dark');
    }

    // 4. Xử lý sự kiện chuyển đổi
    themeToggle.addEventListener('change', function () {
        const newTheme = themeToggle.checked ? 'dark' : 'light';
        applyTheme(newTheme);
    });
});