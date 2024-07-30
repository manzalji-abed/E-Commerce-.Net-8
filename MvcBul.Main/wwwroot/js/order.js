
// Include Toastr library
var toastrScript = document.createElement('script');
toastrScript.src = 'https://cdnjs.cloudflare.com/ajax/libs/toastr.js/latest/toastr.min.js';
document.head.appendChild(toastrScript);

var toastrCSS = document.createElement('link');
toastrCSS.rel = 'stylesheet';
toastrCSS.href = 'https://cdnjs.cloudflare.com/ajax/libs/toastr.js/latest/toastr.min.css';
document.head.appendChild(toastrCSS);

var dataTable;

$(document).ready(function () {
    var url = window.location.search;
    if (url.includes("inprocess")) {
        loadDataTable("inprocess")
    }
    else if (url.includes("completed")) {
        loadDataTable("completed")
    }
    else if (url.includes("approved")) {
        loadDataTable("approved")
    }
    else if (url.includes("pending")) {
        loadDataTable("pending")
    }
    else {
        loadDataTable("all");
    }
})

function loadDataTable(status) {
    dataTable = $('#tblData').DataTable({
        "ajax": { url: '/order/getall?status=' + status },
        columns: [
            { data: 'id', "width":"5%" },
            { data: 'name', "width": "25%" },
            { data: 'phoneNumber', "width": "15%" },
            { data: 'applicationUser.email', "width": "20%" },
            { data: 'orderStatus', "width": "10%" },
            { data: 'orderTotal', "width": "10%" },
            {
                data:'id',
                "render": function (data) {
                    return `<div class="w-75 btn-group" role="group">
                    <a href="/order/details?orderId=${data}" class="btn btn-primary mx-2"> <i class="bi bi-pencil-square"></i> Edit </a>
                    </div>
                    `
                }
                , "width": "10%"
            }
        ]
    });
}

