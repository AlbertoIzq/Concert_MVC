var dataTable

$(document).ready(function () {
    loadDataTable();
})

function loadDataTable() {
    dataTable = $('#tableData').DataTable({
        "ajax": { url: '/admin/company/getall' },
        "columns": [
            { data: 'name', "width": "10%" },
            { data: 'streetAddress', "width": "25%" },
            { data: 'city', "width": "10%" },
            { data: 'state', "width": "10%" },
            { data: 'country', "width": "5%" },
            { data: 'postalCode', "width":"5%" },
            { data: 'phoneNumber', "width": "10%" },
            {
                data: 'id',
                "render": function (data) {
                    return `<div class="w-75 btn-group" role="group">
                                <a href="/Admin/Company/Upsert?id=${data}" class="btn btn-primary mx-1">
                                    <i class="bi bi-pencil-square"></i> Edit
                                </a>
                                <a onClick=Delete('/Admin/Company/Delete/${data}') class="btn btn-danger mx-1">
                                    <i class="bi bi-trash3"></i> Delete
                                </a>
                            </div>`
                },
                "width": "25%"
            }
        ]
    });
}

function Delete (url) {
    Swal.fire({
        title: "Are you sure?",
        text: "You won't be able to revert this!",
        icon: "warning",
        showCancelButton: true,
        confirmButtonColor: "#3085d6",
        cancelButtonColor: "#d33",
        confirmButtonText: "Yes, delete it!"
    }).then((result) => {
        if (result.isConfirmed) {
            $.ajax({
                url: url,
                type: 'DELETE',
                success: function (data) {
                    toastr.success(data.message);
                    // Reload dataTable to not show already deleted company
                    dataTable.ajax.reload();
                }
            })
        }
    });
}