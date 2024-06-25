var dataTable

$(document).ready(function () {
    loadDataTable();
})

function loadDataTable() {
    dataTable = $('#tableData').DataTable({
        "ajax": { url: '/admin/service/getall' },
        "columns": [
            { data: 'name', "width": "35%" },
            { data: 'priceFixed', "width": "20%" },
            { data: 'pricePerSong', "width": "20%" },
            {
                data: 'id',
                "render": function (data) {
                    return `<div class="w-75 btn-group" role="group">
                                <a href="/Admin/Service/Upsert?id=${data}" class="btn btn-primary mx-1">
                                    <i class="bi bi-pencil-square"></i> Edit
                                </a>
                                <a onClick=Delete('/Admin/Service/Delete/${data}') class="btn btn-danger mx-1">
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
                    if (data.success) {
                        toastr.success(data.message);
                    }
                    else {
                        toastr.warning(data.message);
                    }
                    // Reload dataTable to not show already deleted service
                    dataTable.ajax.reload();
                }
            })
        }
    });
}