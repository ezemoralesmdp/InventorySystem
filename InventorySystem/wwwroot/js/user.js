let dataTable;

$(document).ready(function () {
    loadDataTable();
});

const loadDataTable = () => {
    dataTable = $("#tblData").DataTable({
        "ajax": {
            "url": "/Admin/User/GetAll"
        },
        "columns": [
            { "data": "email" },
            { "data": "names" },
            { "data": "lastName" },
            { "data": "phoneNumber" },
            { "data": "role" },
            {
                "data": {
                    id: "id", lockoutEnd: "lockoutEnd"
                },
                "render": function (data) {
                    let today = new Date().getTime();
                    let locked = new Date(data.lockoutEnd).getTime();
                    if (locked > today) {
                        //Usuario esta bloqueado
                        return `
                            <div class="text-center">
                                <a onClick=LockUnlock('${data.id}') class="btn btn-danger btn-sm text-white" style="cursor:pointer", width:150px >
                                    <i class="bi bi-unlock-fill"></i> Unlock
                                </a>
                            </div>
                        `;
                    }
                    else {
                        return `
                            <div class="text-center">
                                <a onClick=LockUnlock('${data.id}') class="btn btn-success btn-sm text-white" style="cursor:pointer" width:150px >
                                    <i class="bi bi-lock-fill"></i> Lock
                                </a>
                            </div>
                        `;
                    }
                },
                "wifth": "20%"
            }
        ]
    });
}

const LockUnlock = (id) => {
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
            else
                toastr.error(data.message);
        }
    });
}