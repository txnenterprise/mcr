$(document).ready(function () {
    var table = $('#table-grid').DataTable({
        lengthChange: false,
        dom: 'Bfrtip',
        buttons: [
            {
                extend: 'copyHtml5',
                text: 'Copiar',
            },
            'excelHtml5',
            'csvHtml5',
            'pdfHtml5',
            {
                extend: 'colvis',
                text: 'Colunas',
            }
        ],
        responsive: true,
        "oLanguage": {
            "sSearch": "Pesquisar:",
            "copyHtml5": "Copiar",
            emptyTable: "Nenhum registro localizado",
            info: "Exibindo _START_ a _END_ de _TOTAL_ registros",
            infoEmpty: "Nenhum registro a ser exibido",
            loadingRecords: "Carregando...",
            zeroRecords: "Nenhum registro localizado",
            "oPaginate": {
                "sFirst": "",
                "sPrevious": "<",
                "sNext": ">",
                "sLast": ""
            }
        }

    });

    var tableGridWithoutButtons = $('#table-grid-without-buttons').DataTable({
        lengthChange: false,
        dom: 'Bfrtip',
        responsive: true,
        "oLanguage": {
            "sSearch": "Pesquisar:",
            "copyHtml5": "Copiar",
            emptyTable: "Nenhum registro localizado",
            info: "Exibindo _START_ a _END_ de _TOTAL_ registros",
            infoEmpty: "Nenhum registro a ser exibido",
            loadingRecords: "Carregando...",
            zeroRecords: "Nenhum registro localizado",
            "oPaginate": {
                "sFirst": "",
                "sPrevious": "<",
                "sNext": ">",
                "sLast": ""
            }
        }

    });

    new DataTable('#table-grid-group-index-0', {
        order: [[0, 'asc']],
        rowGroup: {
            dataSrc: 2
        }
    });

    var table = $('#table-group-0').DataTable({
        "columnDefs": [
            { "targets": [0], "visible": false, "searchable": false }
        ],
        lengthChange: false,
        dom: 'Bfrtip',
        buttons: [
            {
                extend: 'copyHtml5',
                text: 'Copiar',
            },
            'excelHtml5',
            'csvHtml5',
            'pdfHtml5',
            {
                extend: 'colvis',
                text: 'Colunas',
            }
        ],
        responsive: true,
        "oLanguage": {
            "sSearch": "Pesquisar:",
            "copyHtml5": "Copiar",
            emptyTable: "Nenhum registro localizado",
            info: "Exibindo _START_ a _END_ de _TOTAL_ registros",
            infoEmpty: "Nenhum registro a ser exibido",
            loadingRecords: "Carregando...",
            zeroRecords: "Nenhum registro localizado",
            "oPaginate": {
                "sFirst": "",
                "sPrevious": "<",
                "sNext": ">",
                "sLast": ""
            }
        },
        "order": [[5, 'desc']],
        "displayLength": 25,
        "drawCallback": function (settings) {
            var api = this.api();
            var rows = api.rows({ page: 'current' }).nodes();
            var last = null;

            api.column(0, { page: 'current' }).data().each(function (group, i) {
                if (last !== group) {
                    $(rows).eq(i).before(
                        '<tr class="group"><td colspan="9" style="background-color: #89b996; color: #fff;">' + group + '</td></tr>'
                    );

                    last = group;
                }
            });
        }
    });
});