var dataTable;

$(document).ready(function () {
    var url = window.location.search;
    if (url.includes("approved")) {
        loadDataTable("GetOrderList?state=approved");
    }
    else {
        if (url.includes("completed")) {
            loadDataTable("GetOrderList?state=completed");
        }
        else {
            loadDataTable("GetOrderList?state=all");
        }
    }
});

function loadDataTable(url) {
    dataTable = $('#tblData').DataTable({
        "ajax": {
            "url": "/Admin/Order/" + url
        },
        "columns": [
            { "data": "id" },
            { "data": "clientNames" },
            { "data": "telephone" },
            { "data": "user.email" },
            { "data": "orderState" },
            {
                "data": "totalOrder", "className": "text-end",
                "render": function (data) {
                    var d = data.toFixed(2).replace(/\d(?=(\d{3})+\.)/g, '$&,');
                    return d;
                }
            },
            {
                "data": "id",
                "render": function (data) {
                    return `
                        <div class="text-center">
                            <a href="/Admin/Order/Detail/${data}" class="btn btn-success text-white" style="cursor:pointer">
                                <i class="bi bi-ticket-detailed"></i>
                            </a>                           
                        </div>
                        `;
                }
            }
        ]
    });
}