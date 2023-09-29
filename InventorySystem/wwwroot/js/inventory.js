let dataTable;

$(document).ready(function () {
    loadDataTable();
});

const loadDataTable = () => {
    dataTable = $("#tblData").DataTable({
        "ajax": {
            "url": "/Inventory/Inventory/GetAll"
        },
        "columns": [
            { "data": "store.name" },
            { "data": "product.description" },
            {
                "data": "product.cost", "className": "text-end",
                "render": function (data) {
                    var d = data.toFixed(2).replace(/\d(?=(\d{3})+\.)/g, '$&,');
                    return d;
                }
            },
            { "data": "amount", "className": "text-end" }
        ]
    });
}