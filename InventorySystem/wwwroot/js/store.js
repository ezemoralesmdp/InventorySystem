let dataTable;

$(document).ready(function () {
    loadDataTable();
});

const loadDataTable = () => {
    dataTable = $("#tblData").DataTable({
        "ajax": {
            "url": "/Admin/Store/GetAll"
        },
        "columns": [
            { "data": "name", "width": "20%" },
            { "data": "description", "width": "40%" },
            {
                "data": "state",
                "render": function (data) {
                    return (data) ? "Active" : "Inactive";
                },
                "width": "20%"
            },
            {
                "data": "id",
                "render": function (data) {
                    return `
                        <div class="text-center">
                            <a href="/Admin/Store/Upsert/${data}" class="btn btn-success btn-sm text-white" style="cursor:pointer">
                                <i class="bi bi-pencil-square"></i>
                            </a>
                            <a onClick=Delete("/Admin/Store/Delete/${data}") class="btn btn-danger btn-sm text-white" style="cursor:pointer">
                                <i class="bi bi-trash3-fill"></i>
                            </a>
                        </div>
                    `;
                },
                "wifth": "20%"
            }
        ]
    });
}

const Delete = (url) => {
    swal({
        title: "Are you sure to Delete the store?",
        text: "This record cannot be recovered",
        icon: "warning",
        buttons: true,
        dangerMode: true
    })
    .then((confirmDelete) => {
        if (confirmDelete) {
            $.ajax({
                type: "POST",
                url: url,
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
    });

    $('.swal-text').css('color', 'red');
}