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
            { data: 'releaseYear', "width": "15%" },
            { data: 'genre.name', "width": "15%" },
        ]
    });
}