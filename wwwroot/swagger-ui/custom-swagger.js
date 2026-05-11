// 经典Swagger UI 交互增强
(function() {
    'use strict';
    
    // 页面加载完成后执行
    window.addEventListener('load', function() {
        // 添加自定义类名到body
        document.body.classList.add('classic-swagger-theme');
        
        // 创建右下角跳转按钮
        createHtmlTestButton();
    });
    
    function createHtmlTestButton() {
        // 创建按钮
        const button = document.createElement('button');
        button.className = 'html-test-button';
        button.innerHTML = '<i class="fas fa-flask"></i> 打开测试页面';
        button.onclick = toggleDropdown;
        
        // 创建下拉菜单
        const dropdown = document.createElement('div');
        dropdown.className = 'html-test-dropdown';
        dropdown.id = 'htmlTestDropdown';
        dropdown.innerHTML = `
            <a href="/test-all.html" target="_blank">
                <i class="fas fa-vial"></i>
                <span>一体化测试工具</span>
            </a>
            <a href="/Bookmarker.html" target="_blank">
                <i class="fas fa-bookmark"></i>
                <span>书签管理测试</span>
            </a>
            <a href="/bookmark_get_list.html" target="_blank">
                <i class="fas fa-list"></i>
                <span>获取书签列表</span>
            </a>
            <a href="/bookmark_get_id.html" target="_blank">
                <i class="fas fa-search"></i>
                <span>获取单个书签</span>
            </a>
            <a href="/bookmark_post.html" target="_blank">
                <i class="fas fa-plus"></i>
                <span>新增书签</span>
            </a>
            <a href="/bookmark_put.html" target="_blank">
                <i class="fas fa-edit"></i>
                <span>修改书签</span>
            </a>
            <a href="/bookmark_delete.html" target="_blank">
                <i class="fas fa-trash"></i>
                <span>删除书签</span>
            </a>
        `;
        
        // 添加到页面
        document.body.appendChild(button);
        document.body.appendChild(dropdown);
        
        // 点击其他地方关闭下拉菜单
        document.addEventListener('click', function(e) {
            if (!button.contains(e.target) && !dropdown.contains(e.target)) {
                dropdown.classList.remove('show');
            }
        });
    }
    
    function toggleDropdown() {
        const dropdown = document.getElementById('htmlTestDropdown');
        dropdown.classList.toggle('show');
    }
})();
