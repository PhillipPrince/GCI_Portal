$(document).ready(function () {

    if ($.fn.DataTable) {

        $('.datatable').DataTable({
            responsive: true,
            autoWidth: false,
            pageLength: 10,
            lengthChange: true,
            dom: 'Bfrtip',
            buttons: [
                {
                    extend: 'excel',
                    className: 'btn btn-success btn-sm',
                    text: '<i class="fa fa-file-excel"></i> Excel'
                },
                {
                    extend: 'pdf',
                    className: 'btn btn-danger btn-sm',
                    text: '<i class="fa fa-file-pdf"></i> PDF'
                },
                {
                    extend: 'print',
                    className: 'btn btn-primary btn-sm',
                    text: '<i class="fa fa-print"></i> Print'
                }
            ]
        });

    }

});
