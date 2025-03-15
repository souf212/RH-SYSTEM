document.addEventListener('DOMContentLoaded', function () {
    // Toggle between grid and list views
    const viewButtons = document.querySelectorAll('.view-button');
    const cardContainer = document.querySelector('.card');

    viewButtons.forEach(button => {
        button.addEventListener('click', function () {
            viewButtons.forEach(btn => btn.classList.remove('active'));
            this.classList.add('active');

            const viewType = this.getAttribute('data-view');
            if (viewType === 'list') {
                cardContainer.classList.add('list-view');
            } else {
                cardContainer.classList.remove('list-view');
            }
        });
    });

    // Search functionality
    const searchInput = document.getElementById('searchInput');
    const statusFilter = document.getElementById('statusFilter');
    const assigneeFilter = document.getElementById('assigneeFilter');

    const filterTasks = () => {
        const searchTerm = searchInput.value.toLowerCase();
        const statusTerm = statusFilter.value;
        const assigneeTerm = assigneeFilter.value;

        const cards = document.querySelectorAll('.task-card');
        const rows = document.querySelectorAll('.task-table tbody tr');

        // Filter grid items
        cards.forEach(card => {
            const title = card.querySelector('.task-title').textContent.toLowerCase();
            const status = card.querySelector('.task-status').textContent.trim();
            const assignee = card.querySelector('.task-assignee span').textContent;

            const matchesSearch = title.includes(searchTerm);
            const matchesStatus = statusTerm === '' || status.includes(statusTerm);
            const matchesAssignee = assigneeTerm === '' || assignee === assigneeTerm;

            if (matchesSearch && matchesStatus && matchesAssignee) {
                card.style.display = '';
            } else {
                card.style.display = 'none';
            }
        });

        // Filter table rows
        rows.forEach(row => {
            const title = row.cells[0].textContent.toLowerCase();
            const assignee = row.cells[1].textContent.trim();
            const status = row.cells[3].textContent.trim();

            const matchesSearch = title.includes(searchTerm);
            const matchesStatus = statusTerm === '' || status.includes(statusTerm);
            const matchesAssignee = assigneeTerm === '' || assignee.includes(assigneeTerm);

            if (matchesSearch && matchesStatus && matchesAssignee) {
                row.style.display = '';
            } else {
                row.style.display = 'none';
            }
        });
    };

    searchInput.addEventListener('input', filterTasks);
    statusFilter.addEventListener('change', filterTasks);
    assigneeFilter.addEventListener('change', filterTasks);
});