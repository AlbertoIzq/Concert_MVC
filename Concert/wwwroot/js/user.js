var dataTable

$(document).ready(function () {
    loadDataTable();
})

function loadDataTable() {
    dataTable = $('#tableData').DataTable({
        "ajax": { url: '/admin/user/getall' },
        "columns": [
            { data: 'name', "width": "10%" },
            { data: 'surname', "width": "10%" },
            { data: 'email', "width": "15%" },
            { data: 'phoneNumber', "width": "15%" },
            { data: 'company.name', "width": "15%" },
            { data: 'role', "width":"10%" },
            {
                data: { id: "id", lockoutEnd: "lockoutEnd"},
                "render": function (data) {
                    var today = new Date().getTime();
                    var lockout = new Date(data.lockoutEnd).getTime();

                    var btnTypeClass = "btn-danger";
                    var btnIconClass = "bi bi-lock-fill";
                    var btnText = "Lock";

                    if (lockout > today) {
                        var btnTypeClass = "btn-success";
                        var btnIconClass = "bi bi-unlock-fill";
                        var btnText = "Unlock";
                    }

                    return `
                        <div class="text center">
                            <a onClick=LockUnlock('${data.id}') class="btn ${btnTypeClass} text-white" style="cursor:pointer; width:100px;">
                                <i class="${btnIconClass}"></i>${btnText}
                            </a>
                            <a class="btn btn-danger text-white" style="cursor:pointer; width:120px;">
                                <i class="bi bi-pencil-square"></i>Permission
                            </a>
                        `
                },
                "width": "25%"
            }
        ]
    });
}

function LockUnlock(id) {
    $.ajax({
        type: "POST",
        url: '/Admin/User/LockUnlock',
        data: JSON.stringify(id),
        contentType: "application/json",
        success: function (data) {
            if (data.success) {
                toastr.success(data.message);
                dataTable.ajax.reload();
            }
        }
    });
}