var dataTable

$(document).ready(function () {
    loadDataTable();
})

function loadDataTable() {
    dataTable = $('#tableData').DataTable({
        "ajax": { url: '/admin/song/getall' },
        "columns": [
            { data: 'artist', "width": "15%" },
            { data: 'title', "width": "20%" },
            { data: 'length', "width": "10%" },
            { data: 'releaseYear', "width": "10%" },
            { data: 'genre.name', "width": "10%" },
            { data: 'language.name', "width": "10%" },
            {
                data: 'id',
                "render": function (data) {
                    return `<div class="w-75 btn-group" role="group">
                                <a href="/Admin/Song/Upsert?id=${data}" class="btn btn-primary mx-1">
                                    <i class="bi bi-pencil-square"></i> Edit
                                </a>
                                <a onClick=Delete('/Admin/Song/Delete/${data}') class="btn btn-danger mx-1">
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
                    // Reload dataTable to not show already deleted song
                    dataTable.ajax.reload();
                }
            })
        }
    });
}