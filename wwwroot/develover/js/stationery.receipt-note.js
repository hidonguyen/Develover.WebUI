var receiptNote = (function ReceiptNote(self) {
    let baseUrl = '/stationery/receipt-note';
    let mode = VoucherMode.VIEW;
    let modelId = urlParams.get('id');
    let context = $("#panel-receipt-note");
    let table = $("#dt-receipt-notes");
    let detailContext = $("#modal-add-item");
    let detailTable = $("#dt-receipt-note-items");

    let isInDetailPage = false;
    if (context.length > 0) {
        isInDetailPage = true;
    }

    let isSaved = false;

    let model = {};

    let initializeIndex = () => {
        table = $('#dt-receipt-notes').dataTable({
            responsive: true,
            ajax: baseUrl + '/getlist',
            columns: [
                { data: "no", title: "Số phiếu" },
                { data: "date", title: "Ngày phiếu" },
                { data: "status", title: "Tình trạng" }
            ],
            columnDefs: [
                {
                    targets: 0,
                    render: goodsReceiptNoteDrillDownFormatter
                },
                {
                    targets: 1,
                    render: dateFormatter
                },
                {
                    targets: -1,
                    width: 65,
                    render: statusFormatter
                }
            ],
            dom:
                "<'row mb-3'<'col-sm-12 col-md-6 d-flex flex-wrap align-items-center justify-content-start'<'custom-button mb-1 mr-3'>f><'col-sm-12 col-md-6 d-flex align-items-center justify-content-end'<'mr-3'l>B>>" +
                "<'row'<'col-sm-12'tr>>" +
                "<'row'<'col-sm-12 col-md-5'i><'col-sm-12 col-md-7'p>>",
            buttons: [
                {
                    extend: "excelHtml5",
                    text: '<i class="fal fa-file-excel"></i>',
                    titleAttr: "Export to Excel",
                    className: "btn-outline-primary btn-sm mr-1"
                },
                //{
                //    extend: "copyHtml5",
                //    text: '<i class="fal fa-copy"></i>',
                //    titleAttr: "Copy to clipboard",
                //    className: "btn-outline-primary btn-sm mr-1"
                //},
                {
                    extend: "print",
                    text: '<i class="fal fa-print"></i>',
                    titleAttr: "Print table",
                    className: "btn-outline-primary btn-sm"
                }
            ]
        });

        $("div.custom-button").html('<a class="btn btn-new-index btn-outline-success" title="Thêm mới (Z+N)" href="javascript:void(0);" data-hotkey="Z+N"><i class="fal fa-file-plus"></i><span> Thêm mới</span></a>');
        $('.btn-new-index').on('click', self.new);
    }

    let initializeDetailTable = () => {
        detailTable = $('#dt-receipt-note-items').DataTable({
            scrollX: true,
            scrollCollapse: true,
            fixedColumns: {
                rightColumns: 1
            },
            columns: [
                { data: "id", title: "id" },
                { data: null, title: "#" },
                { data: "stockItemId", title: "stockItemId" },
                { data: "stockItem", title: "Hàng hoá" },
                { data: "locationId", title: "locationId" },
                { data: "location", title: "Kho" },
                { data: "quantity", title: "Số lượng" },
                { data: "price", title: "Đơn giá" },
                { data: "amount", title: "Thành tiền" },
                { data: "note", title: "Ghi chú" },
                { data: "operate", title: "Thao tác" },
            ],
            columnDefs: [
                {
                    targets: 0,
                    visible: false
                },
                {
                    targets: 1,
                    render: seqNoFormatter
                },
                {
                    targets: 2,
                    visible: false
                },
                {
                    targets: 4,
                    visible: false
                },
                {
                    targets: 6,
                    visible: false
                },
                {
                    targets: -1,
                    width: 65,
                    render: operateFormatter
                }
            ],
            dom:
                "<'row mb-3'<'col-sm-12 col-md-6 d-flex flex-wrap align-items-center justify-content-start'<'custom-button mb-1 mr-3'>>>" +
                "<'row'<'col-sm-12'tr>>"
        });

        $("div.custom-button").html('<a class="btn btn-add-item btn-outline-success" title="Thêm dòng mới (Z+N)" href="javascript:void(0);" data-hotkey="Z+N"><i class="fal fa-file-plus"></i><span> Thêm dòng mới</span></a>');
        $('.btn-add-item', context).on('click', self.showModal);
    }

    self.addButtonEvents = () => {
        $('.btn-new', context).on('click', self.new);
        $('.btn-edit', context).on('click', self.edit);
        $('.btn-save', context).on('click', self.save);
        $('.btn-cancel', context).on('click', self.cancel);
        $('.btn-delete', context).on('click', self.delete);
        $('.btn-close', context).on('click', self.hideModal);
        $('.btn-save', detailContext).on('click', self.addItem);
        $('.btn-save-close', detailContext).on('click', self.addItemClose);
    }

    let getModel = (id) => {
        $.ajax({
            url: baseUrl + '/getmodel?id=' + id,
            type: "get",
            async: false
        }).done((res) => {
            model = res.model;
        }).fail((err) => {
            console.log(err);
            showErrorMessage();
        });
    }

    let loadData = () => {
        getModel(modelId);
        modelId = model.id;
        if (isEmptyUUID(model.id)) {
            mode = VoucherMode.INVALID;
            setFunctionButtonState(mode, context);
            disableInputData(context);
            Swal.fire({
                title: "Dữ liệu không tồn tại",
                text: "Không tìm thấy thông tin, vui lòng kiểm tra lại!",
                icon: "error",
            });
        }
        clearInputData(context);
        //binding to control
        $("#No").val(model.no);
    }

    let createModel = () => {
        let items = [];

        var table = $('#example').DataTable();

        table.data().each(function (row, index) {
            let item = {
                id: row.id,
                goodsReceiptNoteId: model.id,
                sequenceNo: index + 1,
                stockItemId: row.stockItemId,
                locationId: row.locationId,
                quantity: row.quantity,
                price: row.price,
                amount: row.amount,
                note: row.note
            }

            items.push(item);
        });

        model.date = $("input#Date", context).val();
        model.no = $("input#No", context).val();
        model.supplierId = $("select#SupplierId", context).val();
        model.note = $("textarea#Note", context).val();
        model.items = items;
    }



    /** Details */
    if (isInDetailPage) {
        [Quantity] = AutoNumeric.multiple(["#modal-add-item input#Quantity"], develoverSettings.OptionAutoNumericQuantity);
        [Price] = AutoNumeric.multiple(["#modal-add-item input#Price"], develoverSettings.OptionAutoNumericAmount);
        [Amount] = AutoNumeric.multiple(["#modal-add-item input#Amount"], develoverSettings.OptionAutoNumericAmountVND);
    }
    //window.operateEvents = {
    //    "click .edit": function (e, value, row, index) {
    //        detailContext.data("rowData", row);
    //        detailContext.data("rowIndex", index);
    //        detailContext.modal("show");
    //    },
    //    "click .delete": function (e, value, row, index) {
    //        swal({
    //            title: "Delete row?",
    //            text: "Do you want to delete this row now? This cannot be undone.",
    //            icon: "warning",
    //            buttons: true,
    //            dangerMode: true,
    //        }).then((res) => {
    //            if (res) {
    //                $table.bootstrapTable("remove", {
    //                    field: "id",
    //                    values: [row.id]
    //                });
    //            }
    //        });
    //    }
    //}

    detailContext.on("show.bs.modal", (e) => {
        let rowData = detailContext.data("rowData"); // Extract info from data-* attributes

        if (rowData === undefined) {
            $("h5.modal-title", detailContext).text("Thêm dòng mới");
            $("button.btn-save", detailContext).show();
        }
        else {
            detailContext.find("h5.modal-title").text("Sửa dữ liệu");
            $("button.btn-save", detailContext).hide();
        }
    })

    detailContext.on("shown.bs.modal", (e) => {
        let rowData = detailContext.data("rowData"); // Extract info from data-* attributes

        if (rowData === undefined) {
            $("input#Id", detailContext).val(uuidv4());
        }
        else {
            $("input#Id", detailContext).val(rowData.id);
            $("select#StockItemId", detailContext).val(rowData.stockItemId).trigger("change");
            $("select#LocationId", detailContext).val(rowData.locationId).trigger("change");
            Quantity.set(rowData.quantity);
            Price.set(rowData.price);
            Amount.set(rowData.amount);
            $("textarea#Note", detailContext).val(rowData.note);

        }
    })

    detailContext.on("hidden.bs.modal", (e) => {
        detailContext.removeData("rowIndex");
        detailContext.removeData("rowData");

        clearInputData(detailContext);
        Quantity.set(0);
        Price.set(0);
        Amount.set(0);
    })

    //calcAmount
    $("input#Price ,input#Quantity", detailContext).on("change", (e) => {
        calcAmount();
    })
    function calcAmount() {
        Amount.set(Quantity.getNumber() * Price.getNumber());
    }

    //END calcAmount

    self.addItem = (closeAfterAdd) => {
        if (mode !== VoucherMode.NEW && mode !== VoucherMode.EDIT)
            return;

        if (!$("form", detailContext)[0].checkValidity())
            return;

        let index = detailContext.data("rowIndex");

        let data = {
            id: detailContext.find("input#Id").val(),
            stockItemId: detailContext.find("select#StockItemId").val(),
            stockItem: detailContext.find("select#StockItemId option:selected").text(),
            unitOfMeasure: detailContext.find("input#StockItem_UnitOfMeasure_Name").val(),
            locationId: detailContext.find("select#LocationId").val(),
            location: detailContext.find("select#LocationId option:selected").text(),
            quantity: Quantity.getNumber(),
            price: Price.getNumber(),
            amount: Amount.getNumber(),
            note: detailContext.find("textarea#Note").val(),
        }
        if (index === undefined) {
            detailTable.row.add(data).draw();
        }
        else {
            detailTable.row(index).data(data).draw();
            detailContext.removeData("rowIndex");
            closeAfterAdd = true;
        }

        if (closeAfterAdd === true) {
            self.hideModal();
        }
        else {
            $("input#Id", detailContext).val(uuidv4());
        }
    }

    self.addItemClose = () => {
        self.addItem(true);
    }

    self.showModal = () => {
        detailContext.modal("show");
    }

    self.hideModal = () => {
        detailContext.modal("hide");
    }

    self.initialize = () => {
        self.addButtonEvents();
        if (isInDetailPage) {
            initializeDetailTable();
            if (modelId && !isEmptyUUID(modelId)) {
                mode = VoucherMode.VIEW;
                loadData();
                disableInputData(context);
            }
            else {
                mode = VoucherMode.NEW;
                getModel();
            }
            setFunctionButtonState(mode, context);
        }
        else {
            initializeIndex();
        }
    }

    self.new = () => {
        window.location.href = baseUrl + '/new';
    }

    self.edit = () => {
        if (mode === VoucherMode.INVALID)
            return;

        mode = VoucherMode.EDIT;
        setFunctionButtonState(mode, context);
        enableInputData(context);
    }

    self.save = () => {
        if (mode === VoucherMode.INVALID)
            return;

        if (!$("form", context)[0].checkValidity())
            return;

        createModel();

        $.ajax({
            url: baseUrl + "/save",
            type: "post",
            data: { mode, model }
        }).done((res) => {
            isSaved = true;
            Swal.fire({
                title: "Success",
                text: "The branch has been saved!",
                icon: "success"
            }).then(() => {
                window.location.href = baseUrl + "/detail?id=" + res.id;
            })
        }).fail((err) => {
            console.log(err);
            if (err.status === 400) {
                showHideValidateResult(JSON.parse(err.responseText), context);
            }
            else {
                showErrorMessage();
            }
        });
    }

    self.cancel = () => {
        if (mode === VoucherMode.INVALID)
            return;

        if (mode === VoucherMode.NEW && (!modelId || isEmptyUUID(modelId))) {
            history.back();
        }
        else {
            Swal.fire({
                title: "Huỷ phiếu?",
                text: "Dữ liệu bạn đã nhập sẽ bị mất, bạn có chắc chắn muốn huỷ phiếu không?",
                icon: "question",
                showCancelButton: true,
                confirmButtonText: "Có",
                cancelButtonText: "Không"
            }).then(function (result) {
                if (result.value) {
                    self.initialize();
                }
            });
        }
    }

    self.delete = () => {
        if (mode === VoucherMode.INVALID)
            return;

        Swal.fire({
            title: "Xoá phiếu?",
            text: "Dữ liệu bạn đã nhập sẽ bị xoá, bạn có chắc chắn muốn xoá phiếu không?",
            icon: "question",
            showCancelButton: true,
            confirmButtonText: "Có",
            cancelButtonText: "Không"
        }).then((res) => {
            if (res.value) {
                $.ajax({
                    url: baseUrl + "/delete",
                    type: "post",
                    data: { id: model.id }
                }).done((res) => {
                    window.location.href = baseUrl;
                }).fail((err) => {
                    console.log(err);
                    showErrorMessage();
                });
            }
        });
    }

    //window.addEventListener('beforeunload', function (e) {
    //    if (!isSaved && mode === VoucherMode.NEW || mode === VoucherMode.EDIT)
    //        e.returnValue = "";
    //});

    return self;

    /** Details */

})({});

document.addEventListener('DOMContentLoaded', function () {
    receiptNote.initialize();
}); 