$(document).ready(function () {
    loadDataTable();
})

function loadDataTable() {
    dataTable = $('#tableData').DataTable({
        "ajax": { url: '/admin/song/getall' },
        "columns": [
            { data: 'artist', "width": "20%" },
            { data: 'title', "width": "20%" },
            { data: 'length', "width": "15%" },
            { data: 'releaseYear', "width": "10%" },
            { data: 'genre.name', "width": "10%" },
            {
                data: 'id',
                "render": function (data) {
                    return `<div class="w-75 btn-group" role="group">
                                <a href="/admin/song/upsert?id=${data}" class="btn btn-primary mx-1">
                                    <i class="bi bi-pencil-square"></i> Edit
                                </a>
                                <a href="/admin/song/delete?id=${data}" class="btn btn-danger mx-1">
                                    <i class="bi bi-trash3"></i> Delete
                                </a>
                            </div>`
                },
                "width": "25%"
            }
        ]
    });
}