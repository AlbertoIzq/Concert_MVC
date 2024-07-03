var dataTable

$(document).ready(function () {
    loadDataTable();
})

function loadDataTable() {
    dataTable = $('#tableData').DataTable({
        "ajax": { url: '/admin/order/getall' },
        "columns": [
            { data: 'id', "width": "5%" },
            { data: 'name', "width": "15%" },
            { data: 'surname', "width": "15%" },
            { data: 'phoneNumber', "width": "15%" },
            { data: 'applicationUser.email', "width": "20%" },
            { data: 'orderStatus', "width": "10%" },
            { data: 'orderTotal', "width": "5%" },
            {
                data: 'id',
                "render": function (data) {
                    return `<div class="w-75 btn-group" role="group">
                                <a href="/Admin/Order/Details?orderId=${data}" class="btn btn-primary mx-1">
                                    <i class="bi bi-pencil-square"></i>Details
                                </a>
                            </div>`
                },
                "width": "15%"
            }
        ]
    });
}