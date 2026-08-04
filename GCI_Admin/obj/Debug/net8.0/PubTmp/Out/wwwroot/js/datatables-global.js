/**
 * GCI Portal - Global DataTables Initializer
 * ============================================
 * Single place for all DataTable initialization.
 * - Auto-initializes all <table class="datatable"> on DOM ready.
 * - Exposes window.GCIDataTable.init(selector) for AJAX-loaded partials.
 * - Wires up column-level filter rows (inputs & selects inside <tfoot> or
 *   the 2nd <thead> row) automatically.
 * - Includes Excel / PDF / Print export buttons on every table.
 * - Safe to call multiple times (destroy-and-reinit pattern).
 */

(function ($) {
    'use strict';

    // Suppress DataTables browser alert popups
    if ($.fn.dataTable) {
        $.fn.dataTable.ext.errMode = 'none';
    }

    /**
     * Core initializer.
     * @param {string|jQuery} selector - CSS selector or jQuery element.
     *        Pass null / undefined to init ALL .datatable tables on the page.
     */
    function initDataTable(selector, customOptions) {
        var $tables = selector ? $(selector) : $('table.datatable');

        if (!$tables.length) return;

        $tables.each(function () {
            var $table = $(this);
            var tableId = $table.attr('id') || '';

            // Destroy existing instance safely
            if ($.fn.DataTable.isDataTable($table)) {
                $table.DataTable().destroy();
            }

            // Detect if there is a 2nd thead row used as a column-filter row
            var hasFilterRow = $table.find('thead tr').length > 1;

            // Detect if there is a tfoot used as filter row (members table style)
            var hasTfootFilter = $table.find('tfoot input, tfoot select').length > 0;

            var dtOptions = {
                responsive: true,
                autoWidth: false,
                pageLength: 10,
                lengthMenu: [10, 25, 50, 100],
                language: {
                    search: '<i class="fa fa-search"></i>',
                    searchPlaceholder: 'Search...',
                    lengthMenu: 'Show _MENU_ entries',
                    info: 'Showing _START_ to _END_ of _TOTAL_ entries',
                    infoEmpty: 'No entries to show',
                    emptyTable: 'No data available',
                    zeroRecords: 'No matching records found',
                    paginate: {
                        first: '<i class="fa fa-angle-double-left"></i>',
                        previous: '<i class="fa fa-angle-left"></i>',
                        next: '<i class="fa fa-angle-right"></i>',
                        last: '<i class="fa fa-angle-double-right"></i>'
                    }
                },
                dom:
                    '<"dt-toolbar row align-items-center mb-2"' +
                        '<"col-sm-12 col-md-6"B>' +
                        '<"col-sm-12 col-md-6 d-flex justify-content-end"f>' +
                    '>' +
                    '<"row"<"col-sm-12"tr>>' +
                    '<"row dt-footer align-items-center mt-2"' +
                        '<"col-sm-12 col-md-5"i>' +
                        '<"col-sm-12 col-md-7 d-flex justify-content-end"lp>' +
                    '>',
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
                ],
                columnDefs: [
                    // Disable ordering & searching on the last column (typically Actions)
                    {
                        targets: -1,
                        orderable: false,
                        searchable: false
                    }
                ]
            };

            // If using column filter rows, hide the built-in search bar
            // because the user filters per-column instead.
            if (hasFilterRow) {
                // Remove last column from orderable off — already set
                dtOptions.orderCellsTop = true;
            }

            // Merge custom options
            var mergedOptions = $.extend(true, {}, dtOptions, customOptions);

            // Special array merging logic to avoid overwriting default array elements by index
            if (customOptions && customOptions.columnDefs) {
                mergedOptions.columnDefs = customOptions.columnDefs.concat(dtOptions.columnDefs);
            }
            if (customOptions && customOptions.buttons) {
                mergedOptions.buttons = customOptions.buttons;
            }

            var table = $table.DataTable(mergedOptions);

            // ── Wire up 2nd THEAD row filters (e.g. configTable, deaconsReportTable) ──
            if (hasFilterRow) {
                $table.find('thead tr:eq(1) th').each(function (i) {
                    var $th = $(this);

                    $th.find('input').on('keyup change', function () {
                        if (table.column(i).search() !== this.value) {
                            table.column(i).search(this.value).draw();
                        }
                    });

                    $th.find('select').on('change', function () {
                        if (table.column(i).search() !== this.value) {
                            table.column(i).search(this.value).draw();
                        }
                    });
                });
            }

            // ── Wire up TFOOT row filters (e.g. membersTable) ──
            if (hasTfootFilter) {
                $table.find('tfoot th').each(function (i) {
                    var $th = $(this);

                    $th.find('input').on('keyup change', function () {
                        if (table.column(i).search() !== this.value) {
                            table.column(i).search(this.value).draw();
                        }
                    });

                    $th.find('select').on('change', function () {
                        if (table.column(i).search() !== this.value) {
                            table.column(i).search(this.value).draw();
                        }
                    });
                });
            }
        });
    }

    // ── Public API ──────────────────────────────────────────────────────
    window.GCIDataTable = {
        /**
         * Initialize DataTable(s).
         * @param {string|jQuery} [selector] - Specific table selector.
         *        Omit to init all .datatable tables.
         * @param {object} [customOptions] - Custom options to merge.
         */
        init: function (selector, customOptions) {
            initDataTable(selector, customOptions);
        }
    };

    // ── Auto-init on DOM Ready ───────────────────────────────────────────
    $(document).ready(function () {
        if ($.fn.DataTable) {
            initDataTable();
        }
    });

})(jQuery);
