const VoucherMode = Object.freeze({ "NEW": 1, "EDIT": 2, "VIEW": 3 });

function TemplateVoucher(options, model) {
    let mode = options.mode;
    let $form = $("#" + options.form);
    let $table = $("#" + options.tableId);
    let $modal = $("#" + options.modalId);

    /** Details */

    [ItemPacking] = AutoNumeric.multiple(["input#ItemPacking"], develoverSettings.OptionAutoNumericPacking);
    [ItemQuantity] = AutoNumeric.multiple(["input#ItemQuantity"], develoverSettings.OptionAutoNumericQuantity);
    [ItemAmount] = AutoNumeric.multiple(["input#ItemAmount"], develoverSettings.OptionAutoNumericAmount);
    [ItemAmountVND] = AutoNumeric.multiple(["input#ItemAmountVND"], develoverSettings.OptionAutoNumericAmountVND);

    window.operateEvents = {
        "click .edit": function (e, value, row, index) {
            $modal.data("rowData", row);
            $modal.data("rowIndex", index);
            $modal.modal("show");
        },
        "click .delete": function (e, value, row, index) {
            swal({
                title: "Delete row?",
                text: "Do you want to delete this row now? This cannot be undone.",
                icon: "warning",
                buttons: true,
                dangerMode: true,
            }).then((res) => {
                if (res) {
                    $table.bootstrapTable("remove", {
                        field: "id",
                        values: [row.id]
                    });
                }
            });
        }
    }

    $modal.on("show.bs.modal", (e) => {
        let rowData = $modal.data("rowData"); // Extract info from data-* attributes

        if (rowData === undefined) {
            $modal.find("h5.modal-title").text("Add item");
            $modal.find("button.save").text("Add");
            $modal.find("button.save-close").removeClass("d-none");
        }
        else {
            $modal.find("h5.modal-title").text("Edit item");
            $modal.find("button.save").text("Save");
        }
    })

    $modal.on("shown.bs.modal", (e) => {
        let rowData = $modal.data("rowData"); // Extract info from data-* attributes

        if (rowData === undefined) {
            $modal.find("input#ItemId").val(uuidv4());
            $modal.find("input#ItemBaseVoucherId").val(model.id);
            $modal.find("select#ItemSelect").selectpicker("val", "");
            $modal.find("input#ItemText").val("");
            $modal.find("input#ItemDate").datepicker("update", "");
            ItemPacking.set(0);
            ItemQuantity.set(0);
            ItemAmount.set(0);
            ItemAmountVND.set(0);
            $modal.find("textarea#ItemNote").val("");
        }
        else {
            $modal.find("input#ItemId").val(rowData.id);
            $modal.find("input#ItemBaseVoucherId").val(rowData.baseVoucherId);
            $modal.find("select#ItemSelect").selectpicker("val", rowData.select);
            $modal.find("select#ItemSelect").trigger("change");
            $modal.find("input#ItemText").val(rowData.text);
            $modal.find("input#ItemDate").datepicker("update", rowData.date);
            ItemPacking.set(rowData.packing);
            ItemQuantity.set(rowData.quantity);
            ItemAmount.set(rowData.amount);
            ItemAmountVND.set(rowData.amountVND);
            $modal.find("textarea#ItemNote").val(rowData.note);

        }
    })

    $modal.on("hidden.bs.modal", (e) => {
        $modal.find("button.save-close").addClass("d-none");

        $modal.find("input#ItemId").val("");
        $modal.find("input#ItemBaseVoucherId").val("");
        $modal.find("select#ItemSelect").selectpicker("val", "");
        $modal.find("input#ItemText").val("");
        $modal.find("input#ItemDate").datepicker("update", "");
        ItemPacking.set(0);
        ItemQuantity.set(0);
        ItemAmount.set(0);
        ItemAmountVND.set(0);
        $modal.find("textarea#ItemNote").val("");

        $modal.find(".is-invalid").removeClass("is-invalid");
        $modal.find(".invalid-feedback").html("");

        $modal.removeData("rowIndex");
        $modal.removeData("rowData");
    })

    $("select#ItemSelect", $modal).on("change", (e) => {
        //let selectedValue = $(this).val();
    })

    this.addItem = (closeAfterAdd) => {
        if (!validateDataInput(["ItemSelect", "ItemText", "ItemDate"], ["Select", "Text", "Date"], $modal))
            return;

        let index = $modal.data("rowIndex");

        if (index === undefined) {

            index = $table.bootstrapTable("getData").length;

            $table.bootstrapTable("insertRow", {
                index: index,
                row: {
                    id: $("input#ItemId").val(),
                    baseVoucherId: $("input#ItemBaseVoucherId").val(),
                    select: $("select#ItemSelect").val(),
                    text: $("input#ItemText").val(),
                    date: $("input#ItemDate").val(),
                    packing: ItemPacking.getNumber(),
                    quantity: ItemQuantity.getNumber(),
                    amount: ItemAmount.getNumber(),
                    amountVND: ItemAmountVND.getNumber(),
                    note: $("textarea#ItemNote").val(),
                }
            });
        }
        else {
            $table.bootstrapTable("updateRow", {
                index: index,
                row: {
                    id: $("input#ItemId").val(),
                    baseVoucherId: $("input#ItemBaseVoucherId").val(),
                    select: $("select#ItemSelect").val(),
                    text: $("input#ItemText").val(),
                    date: $("input#ItemDate").val(),
                    packing: ItemPacking.getNumber(),
                    quantity: ItemQuantity.getNumber(),
                    amount: ItemAmount.getNumber(),
                    amountVND: ItemAmountVND.getNumber(),
                    note: $("textarea#ItemNote").val(),
                }
            });
            $modal.removeData("rowIndex");
            closeAfterAdd = true;
        }

        if (closeAfterAdd) {
            $modal.modal("hide");
        }
        else {
            $("input#ItemId", $modal).val(uuidv4());
        }
    }

    this.hideModal = () => {
        $modal.modal("hide");
    }

    /** Details */


    /** Master */

    [TotalPacking] = AutoNumeric.multiple(["input#TotalPacking"], develoverSettings.OptionAutoNumericPacking);
    [TotalQuantity] = AutoNumeric.multiple(["input#TotalQuantity"], develoverSettings.OptionAutoNumericQuantity);
    [TotalAmount] = AutoNumeric.multiple(["input#TotalAmount"], develoverSettings.OptionAutoNumericAmount);
    [TotalAmountVND] = AutoNumeric.multiple(["input#TotalAmountVND"], develoverSettings.OptionAutoNumericAmountVND);

    function CreateVoucherNo() {
        let voucherNo = $("input#Text").val()

        if (voucherNo === "") {
            let id = $("input#Id").val()

            $.ajax({
                url: "/basevoucher/getvoucherno",
                type: "get",
                data: { id: id }
            }).done((res) => {
                $("input#Text").val(res.voucherNo);
            }).fail((err) => {
                console.log(err)
            });
        }
    }

    function createModel() {
        let items = [];

        $table.bootstrapTable("getData").map((row, index) => {
            let item = {
                id: row.id,
                baseVoucherId: row.baseVoucherId,
                sequenceNo: index + 1,
                select: row.select,
                text: row.text,
                date: row.date,
                packing: row.packing,
                quantity: row.quantity,
                amount: row.amount,
                amountVND: row.amountVND,
                note: row.note
            }

            items.push(item);
        });

        model.select = $("select#Select", $form).val();
        model.text = $("input#Text", $form).val();
        model.date = $("input#Date", $form).val();
        model.totalPacking = TotalPacking.getNumber();
        model.totalQuantity = TotalQuantity.getNumber();
        model.totalAmount = TotalAmount.getNumber();
        model.totalAmountVND = TotalAmountVND.getNumber();
        model.note = $("textarea#Note", $form).val();
        model.items = items;
    }

    this.save = () => {
        if (!validateDataInput(["Select", "Text", "Date"], ["Select", "Text", "Date"], $form))
            return;

        createModel();

        $.ajax({
            url: "/basevoucher/save",
            type: "post",
            data: { mode, model },
        }).done((res) => {
            swal({
                title: "Success",
                text: "The voucher has been saved!",
                icon: "success",
                timer: develoverSettings.swal.timer,
                closeOnClickOutside: develoverSettings.swal.closeOnClickOutside,
                closeOnEsc: develoverSettings.swal.closeOnEsc
            }).then(() => {
                window.location.href = "/basevoucher/detail?id=" + res.id;
            })
        }).fail((err) => {
            if (err.status === 400) {
                showHideValidateResult(JSON.parse(err.responseText), $form);
            }
            else {
                swal("[" + err.status + "] " + err.responseText, {
                    icon: "error",
                });
            }
        });
    }

    this.delete = () => {
        swal({
            title: "Delete Base Voucher?",
            text: "Do you want to delete the base voucher now? This cannot be undone.",
            icon: "warning",
            buttons: true,
            dangerMode: true,
        }).then((res) => {
            if (res) {
                $.ajax({
                    url: "/basevoucher/delete",
                    type: "post",
                    data: { id: model.id }
                }).done((res) => {
                    swal({
                        title: "Success",
                        text: "The base voucher has been deleted!",
                        icon: "success",
                        timer: develoverSettings.swal.timer,
                        closeOnClickOutside: develoverSettings.swal.closeOnClickOutside,
                        closeOnEsc: develoverSettings.swal.closeOnEsc
                    }).then(() => {
                        window.location.href = "/basevoucher";
                    })
                }).fail((err) => {
                    swal("[" + err.status + "] " + err.responseText, {
                        icon: "error",
                    });
                });
            }
        });
    }


    /** Master */


    this.initialize = () => {
        let columns = model.detailColumns;

        if (mode === VoucherMode.VIEW) {
            columns.pop(); // remove operate column
        }

        initBootstrapTable(options.tableId, columns, [], model.detailDataUrl);

        if (mode === VoucherMode.VIEW) {
            $form.find("input:not(.datepicker), textarea").prop("readonly", true);
            $form.find("input.datepicker, select, button, a").prop("disabled", true);

            $form.find(".add-item").remove();
        }
    }
}